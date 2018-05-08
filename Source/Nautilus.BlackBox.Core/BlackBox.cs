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
    using NautechSystems.CSharp.CQS;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core.Build;
    using Nautilus.BlackBox.Core.Enums;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.BlackBox.Core.Messages.SystemCommands;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Messaging;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;
    using NodaTime.Extensions;

    /// <summary>
    /// The <see cref="BlackBox"/> class. The object with contains all other <see cref="BlackBox"/>
    /// components.
    /// </summary>
    public class BlackBox : ComponentBusConnectedBase, IDisposable
    {
        private readonly ActorSystem actorSystem;
        private readonly IInstrumentRepository instrumentRepository;
        private readonly IBrokerageGateway brokerageGateway;
        private readonly IList<IAlphaStrategy> alphaStrategyList = new List<IAlphaStrategy>();
        private readonly IList<IAlphaStrategy> startedStrategies = new List<IAlphaStrategy>();

        private readonly Stopwatch stopwatch = new Stopwatch();

        /// <summary>
        /// Initializes a new instance of the <see cref="BlackBox"/> class.
        /// </summary>
        /// <param name="actorSystem">The actor system label.</param>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter"></param>
        /// <param name="switchboard">The service factory.</param>
        /// <param name="brokerageGateway">The brokerage gateway.</param>
        /// <param name="brokerageClient">The brokerage client.</param>
        /// <param name="account">The brokerage account.</param>
        /// <param name="riskModel">The risk model.</param>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public BlackBox(
            ActorSystem actorSystem,
            BlackBoxContainer container,
            MessagingAdapter messagingAdapter,
            Switchboard switchboard,
            IBrokerageGateway brokerageGateway,
            IBrokerageClient brokerageClient,
            IBrokerageAccount account,
            IRiskModel riskModel)
            : base(
                BlackBoxService.Core,
                new Label(nameof(BlackBox)),
                container,
                messagingAdapter)
        {
            Validate.NotNull(actorSystem, nameof(actorSystem));
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(switchboard, nameof(switchboard));
            Validate.NotNull(brokerageGateway, nameof(brokerageGateway));
            Validate.NotNull(brokerageClient, nameof(brokerageClient));
            Validate.NotNull(account, nameof(account));
            Validate.NotNull(riskModel, nameof(riskModel));

            this.actorSystem = actorSystem;
            this.instrumentRepository = container.InstrumentRepository;
            this.brokerageGateway = brokerageGateway;

            this.StartTime = this.TimeNow();
            this.stopwatch.Start();

            messagingAdapter.Send(new InitializeMessageSwitchboard(
                switchboard,
                this.NewGuid(),
                this.TimeNow()));

            this.Send(
                new List<Enum> { BlackBoxService.Data, BlackBoxService.Execution },
                new InitializeBrokerageGateway(
                    this.brokerageGateway,
                    this.NewGuid(),
                    this.TimeNow()));

            this.Send(
                BlackBoxService.Risk,
                new InitializeRiskModel(
                    account,
                    riskModel,
                    this.NewGuid(),
                    this.TimeNow()));

            brokerageClient.InitializeBrokerageGateway(this.brokerageGateway);

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
        /// Returns a value indicating whether the black box is connected to the broker.
        /// </summary>
        public void ConnectToBrokerage()
        {
            this.Execute(() =>
            {
                this.brokerageGateway.Connect();
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

            return this.instrumentRepository.GetInstrument(symbol);
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
                    BlackBoxService.AlphaModel,
                    new CreateAlphaStrategyModule(
                        strategy,
                        this.NewGuid(),
                        this.TimeNow()));

                this.Log.Information($"AlphaStrategyModule starting... ({strategy.ToString()})");
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
                    BlackBoxService.AlphaModel,
                    new RemoveAlphaStrategyModule(
                        strategy.Instrument.Symbol,
                        strategy.TradeProfile.TradeType,
                        this.NewGuid(),
                        this.TimeNow()));

                this.startedStrategies.Remove(strategy);

                this.Log.Information($"AlphaStrategyModule stopped ({strategy.ToString()})");
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

                this.Log.Information($"AlphaStrategyModule removed ({strategy.ToString()})");
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
                //this.MessagingAdapter.Send(new ShutdownSystem(this.NewGuid(), this.TimeNow()), this.Component.Context);

                this.brokerageGateway.Disconnect();

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

        private static void WaitUntilTrueOrTimeout(bool condition, int timeoutMilliseconds, int pollIntervalMilliseconds)
        {
            if (timeoutMilliseconds <= 0)
            {
                throw new ArgumentException("timeoutMilliseconds must be > 0 milliseconds");
            }

            if (pollIntervalMilliseconds < 0)
            {
                throw new ArgumentException("pollIntervalMilliseconds must be >= 0 milliseconds");
            }

            var stopwatch = Stopwatch.StartNew();

            do
            {
                Task.Delay(pollIntervalMilliseconds).Wait();
            }
            while (!condition && stopwatch.Elapsed < TimeSpan.FromMilliseconds(timeoutMilliseconds));
        }
    }
}
