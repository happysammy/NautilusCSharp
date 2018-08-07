//--------------------------------------------------------------------------------------------------
// <copyright file="BlackBox.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Akka.Actor;
    using Nautilus.BlackBox.Core.Build;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.BlackBox.Core.Messages.Commands;
    using Nautilus.Common.Commands;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Collections;
    using Nautilus.Core.CQS;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;
    using NodaTime.Extensions;

    /// <summary>
    /// The object with contains all <see cref="BlackBox"/> components.
    /// </summary>
    public class BlackBox : ComponentBusConnectedBase, IDisposable
    {
        private readonly ActorSystem actorSystem;
        private readonly IInstrumentRepository instrumentRepository;
        private readonly IFixClient fixClient;
        private readonly List<IAlphaStrategy> alphaStrategyList;
        private readonly List<IAlphaStrategy> startedStrategies;

        private readonly Stopwatch stopwatch = new Stopwatch();

        /// <summary>
        /// Initializes a new instance of the <see cref="BlackBox"/> class.
        /// </summary>
        /// <param name="actorSystem">The actor system label.</param>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="switchboard">The service factory.</param>
        /// <param name="executionGateway">The execution gateway.</param>
        /// <param name="fixClient">The FIX client.</param>
        /// <param name="account">The brokerage account.</param>
        /// <param name="riskModel">The risk model.</param>
        public BlackBox(
            ActorSystem actorSystem,
            BlackBoxContainer container,
            MessagingAdapter messagingAdapter,
            Switchboard switchboard,
            IExecutionGateway executionGateway,
            IFixClient fixClient,
            Account account,
            RiskModel riskModel)
            : base(
                NautilusService.BlackBox,
                LabelFactory.Component(nameof(BlackBox)),
                container,
                messagingAdapter)
        {
            Validate.NotNull(actorSystem, nameof(actorSystem));
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(switchboard, nameof(switchboard));
            Validate.NotNull(executionGateway, nameof(executionGateway));
            Validate.NotNull(fixClient, nameof(fixClient));
            Validate.NotNull(account, nameof(account));
            Validate.NotNull(riskModel, nameof(riskModel));

            this.actorSystem = actorSystem;
            this.instrumentRepository = container.InstrumentRepository;
            this.fixClient = fixClient;
            this.alphaStrategyList = new List<IAlphaStrategy>();
            this.startedStrategies = new List<IAlphaStrategy>();

            this.StartTime = this.TimeNow();
            this.stopwatch.Start();

            messagingAdapter.Send(new InitializeSwitchboard(
                switchboard,
                this.NewGuid(),
                this.TimeNow()));

            this.Send(
                new ReadOnlyList<NautilusService>(new List<NautilusService>
                {
                    NautilusService.Data,
                    NautilusService.Execution,
                }),
                new InitializeGateway(
                    executionGateway,
                    this.NewGuid(),
                    this.TimeNow()));

            this.Send(
                NautilusService.Risk,
                new InitializeRiskModel(
                    account,
                    riskModel,
                    this.NewGuid(),
                    this.TimeNow()));

            fixClient.InitializeGateway(executionGateway);

            this.stopwatch.Stop();
            this.Log.Information($"BlackBox instance created in {Math.Round(this.stopwatch.ElapsedDuration().TotalMilliseconds)}ms");
            this.Log.Information($"Environment={container.Environment}, Broker={container.Account.Broker}");
            this.stopwatch.Reset();
        }

        /// <summary>
        /// Gets the black box start time.
        /// </summary>
        public ZonedDateTime StartTime { get; }

        /// <summary>
        /// Connects to the brokerage.
        /// </summary>
        public void ConnectToBrokerage()
        {
            this.Execute(() =>
            {
                this.fixClient.Connect();
            });
        }

        /// <summary>
        /// Returns the instrument associated with the given security <see cref="Symbol"/>.
        /// </summary>
        /// <param name="symbol">The security symbol.</param>
        /// <returns>A <see cref="Instrument"/>.</returns>
        /// <exception cref="ValidationException">Throws if symbol is null.</exception>
        public QueryResult<Instrument> GetInstrument(Symbol symbol)
        {
            Validate.NotNull(symbol, nameof(symbol));

            return this.instrumentRepository.FindInCache(symbol);
        }

        /// <summary>
        /// Adds the given strategy to this black box (if not already contained).
        /// </summary>
        /// <param name="strategy">The strategy.</param>
        public void AddAlphaStrategyModule(IAlphaStrategy strategy)
        {
            this.Execute(() =>
            {
                Validate.NotNull(strategy, nameof(strategy));

                this.alphaStrategyList.Add(strategy);

                this.Log.Information($"AlphaStrategyModule added ({LogFormatter.ToOutput(strategy)})");
            });
        }

        /// <summary>
        /// Starts the given strategy held within this black box (if contained).
        /// </summary>
        /// <param name="strategy">The strategy.</param>
        public void StartAlphaStrategyModule(IAlphaStrategy strategy)
        {
            this.Execute(() =>
            {
                Validate.NotNull(strategy, nameof(strategy));

                this.startedStrategies.Add(strategy);

                this.Send(
                    NautilusService.AlphaModel,
                    new CreateAlphaStrategyModule(
                        strategy,
                        this.NewGuid(),
                        this.TimeNow()));

                this.Log.Information($"AlphaStrategyModule starting... ({strategy})");
            });
        }

        /// <summary>
        /// Starts all strategies held within this black box.
        /// </summary>
        public void StartAlphaStrategyModulesAll()
        {
            this.Execute(() =>
            {
                foreach (var strategy in this.alphaStrategyList.ToList())
                {
                    this.StartAlphaStrategyModule(strategy);
                }
            });
        }

        /// <summary>
        /// Stops all strategies held within this black box.
        /// </summary>
        public void StopAlphaStrategyModulesAll()
        {
            this.Execute(() =>
            {
                foreach (var strategy in this.startedStrategies.ToList())
                {
                    this.StopAlphaStrategyModule(strategy);
                }
            });
        }

        /// <summary>
        /// Stops the given strategy held within this black box (if contained).
        /// </summary>
        /// <param name="strategy">The strategy.</param>
        public void StopAlphaStrategyModule(IAlphaStrategy strategy)
        {
            this.Execute(() =>
            {
                Validate.NotNull(strategy, nameof(strategy));

                this.Send(
                    NautilusService.AlphaModel,
                    new RemoveAlphaStrategyModule(
                        strategy.Instrument.Symbol,
                        strategy.TradeProfile.TradeType,
                        this.NewGuid(),
                        this.TimeNow()));

                this.startedStrategies.Remove(strategy);

                this.Log.Information($"AlphaStrategyModule stopped ({strategy})");
            });
        }

        /// <summary>
        /// Removes the given strategy held within this black box (if contained).
        /// </summary>
        /// <param name="strategy">
        /// The strategy.
        /// </param>
        public void RemoveAlphaStrategyModule(IAlphaStrategy strategy)
        {
            this.Execute(() =>
            {
                Validate.NotNull(strategy, nameof(strategy));

                this.alphaStrategyList.Remove(strategy);

                this.Log.Information($"AlphaStrategyModule removed ({strategy})");
            });
        }

        /// <summary>
        /// Removes all strategies from this black box.
        /// </summary>
        public void RemoveAlphaStrategyModulesAll()
        {
            this.Execute(() =>
            {
                foreach (var strategy in this.alphaStrategyList.ToList())
                {
                    this.RemoveAlphaStrategyModule(strategy);
                }
            });
        }

        /// <summary>
        /// Gracefully terminates the black box.
        /// </summary>
        public void Terminate()
        {
            this.Execute(() =>
            {
                this.fixClient.Disconnect();

                this.StopAlphaStrategyModulesAll();
                this.RemoveAlphaStrategyModulesAll();

                // Placeholder for the log events (do not refactor away).
                var actorSystemName = this.actorSystem.Name;

                Task.Delay(100).Wait();

                this.Log.Information($"{actorSystemName} shutting down...");

                this.actorSystem.Terminate();

                this.Log.Information($"{actorSystemName} terminated");

                this.Dispose();
            });
        }

        /// <summary>
        /// Disposes this black box instance.
        /// </summary>
        public void Dispose()
        {
            this.Execute(() =>
            {
                 this.actorSystem.Dispose();
                 GC.SuppressFinalize(this);
            });
        }
    }
}
