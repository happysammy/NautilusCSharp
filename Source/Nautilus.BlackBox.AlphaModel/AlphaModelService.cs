//--------------------------------------------------------------------------------------------------
// <copyright file="AlphaModelService.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.AlphaModel
{
    using Akka.Actor;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.AlphaModel.Strategy;
    using Nautilus.BlackBox.Core;
    using Nautilus.BlackBox.Core.Messages.SystemCommands;
    using Nautilus.BlackBox.Core.Setup;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Factories;

    /// <summary>
    /// Provides a message end point into the <see cref="BlackBoxService.AlphaModel"/> service.
    /// </summary>
    public sealed class AlphaModelService : ActorComponentBusConnectedBase
    {
        private readonly BlackBoxSetupContainer storedSetupContainer;
        private readonly AlphaStrategyModuleStore alphaStrategyModuleStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlphaModelService"/> class.
        /// </summary>
        /// <param name="setupContainer">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="alphaStrategyModuleStore">The alpha strategy module store.</param>
        /// <exception cref="ValidationException">Throws any argument is null.</exception>
        public AlphaModelService(
            BlackBoxSetupContainer setupContainer,
            IMessagingAdapter messagingAdapter,
            AlphaStrategyModuleStore alphaStrategyModuleStore)
            : base(
            BlackBoxService.AlphaModel,
            LabelFactory.Service(BlackBoxService.AlphaModel),
            setupContainer,
            messagingAdapter)
        {
            Validate.NotNull(setupContainer, nameof(setupContainer));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(alphaStrategyModuleStore, nameof(alphaStrategyModuleStore));

            this.storedSetupContainer = setupContainer;
            this.alphaStrategyModuleStore = alphaStrategyModuleStore;

            this.SetupCommandMessageHandling();
            this.SetupEventMessageHandling();
        }

        /// <summary>
        /// Set up all <see cref="CommandMessage"/> handling methods.
        /// </summary>
        private void SetupCommandMessageHandling()
        {
            this.Receive<CreateAlphaStrategyModule>(msg => this.OnMessage(msg));
            this.Receive<RemoveAlphaStrategyModule>(msg => this.OnMessage(msg));
        }

        /// <summary>
        /// Set up all <see cref="EventMessage"/> handling methods.
        /// </summary>
        private void SetupEventMessageHandling()
        {
            this.Receive<EventMessage>(msg => this.Self.Tell(msg.Event));
            this.Receive<MarketDataEvent>(msg => this.OnMessage(msg));
        }

        private void OnMessage(MarketDataEvent message)
        {
            Debug.NotNull(message, nameof(message));

            this.CommandHandler.Execute(() =>
            {
                var forStrategy = LabelFactory.StrategyLabel(message.Symbol, message.TradeType);

                this.alphaStrategyModuleStore.Tell(forStrategy, message);
            });
        }

        private void OnMessage(CreateAlphaStrategyModule message)
        {
            Debug.NotNull(message, nameof(message));

            this.CommandHandler.Execute(() =>
            {
                var strategyLabel = LabelFactory.StrategyLabel(message.Symbol, message.TradeType);

                var alphasStrategyModuleRef = AlphaStrategyModuleFactory.Create(
                    this.storedSetupContainer,
                    this.GetMessagingAdapter(),
                    message.Strategy,
                    Context);

                this.alphaStrategyModuleStore.AddStrategy(strategyLabel, alphasStrategyModuleRef);

                var createPortfolio = new CreatePortfolio(
                    message.Strategy.Instrument,
                    this.NewGuid(),
                    this.TimeNow());

                var registerSymbolDataType = new SubscribeSymbolDataType(
                    message.Strategy.Instrument.Symbol,
                    message.Strategy.TradeProfile.BarSpecification,
                    message.Strategy.TradeProfile.TradeType,
                    message.Strategy.Instrument.TickSize,
                    this.NewGuid(),
                    this.TimeNow());

                this.Send(BlackBoxService.Portfolio, createPortfolio);
                this.Send(BlackBoxService.Data, registerSymbolDataType);

                this.Log(LogLevel.Debug, $"{strategyLabel} created");
            });
        }

        private void OnMessage(RemoveAlphaStrategyModule message)
        {
            Debug.NotNull(message, nameof(message));

            this.CommandHandler.Execute(() =>
            {
                var strategyLabel = LabelFactory.StrategyLabel(message.Symbol, message.TradeType);

                this.alphaStrategyModuleStore.RemoveStrategy(strategyLabel);

                this.Log(LogLevel.Information, $"{strategyLabel} removed");
            });
        }
    }
}
