//--------------------------------------------------------------------------------------------------
// <copyright file="AlphaModelService.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.AlphaModel
{
    using System.Collections.Generic;
    using Akka.Actor;
    using Nautilus.Core.Validation;
    using Nautilus.BlackBox.AlphaModel.Strategy;
    using Nautilus.BlackBox.Core.Build;
    using Nautilus.BlackBox.Core.Enums;
    using Nautilus.BlackBox.Core.Messages.SystemCommands;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.Data.Messages;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Provides a message end point into the <see cref="BlackBoxService.AlphaModel"/> service.
    /// </summary>
    public sealed class AlphaModelService : ActorComponentBusConnectedBase
    {
        private readonly BlackBoxContainer storedContainer;
        private readonly AlphaStrategyModuleStore alphaStrategyModuleStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlphaModelService"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="alphaStrategyModuleStore">The alpha strategy module store.</param>
        /// <exception cref="ValidationException">Throws any argument is null.</exception>
        public AlphaModelService(
            BlackBoxContainer container,
            IMessagingAdapter messagingAdapter,
            AlphaStrategyModuleStore alphaStrategyModuleStore)
            : base(
            BlackBoxService.AlphaModel,
            LabelFactory.Service(BlackBoxService.AlphaModel),
            container,
            messagingAdapter)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(alphaStrategyModuleStore, nameof(alphaStrategyModuleStore));

            this.storedContainer = container;
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
            this.Receive<BarDataEvent>(msg => this.OnMessage(msg));
        }

        private void OnMessage(BarDataEvent message)
        {
            Debug.NotNull(message, nameof(message));

            this.Execute(() =>
            {
                var symbolBarSpec = new SymbolBarSpec(message.Symbol, message.BarSpecification);

                this.alphaStrategyModuleStore.Tell(symbolBarSpec, message);
            });
        }

        private void OnMessage(CreateAlphaStrategyModule message)
        {
            Debug.NotNull(message, nameof(message));

            this.Execute(() =>
            {
                var strategyLabel = LabelFactory.StrategyLabel(message.Symbol, message.TradeType);

                var alphasStrategyModuleRef = AlphaStrategyModuleFactory.Create(
                    this.storedContainer,
                    this.GetMessagingAdapter(),
                    message.Strategy,
                    Context);

                // TODO: Refactor below.
                var symbolBarSpec = new SymbolBarSpec(message.Symbol, message.Strategy.TradeProfile.BarSpecification);
                this.alphaStrategyModuleStore.AddStrategy(strategyLabel, symbolBarSpec, alphasStrategyModuleRef);

                var createPortfolio = new CreatePortfolio(
                    message.Strategy.Instrument,
                    this.NewGuid(),
                    this.TimeNow());

                var registerSymbolDataType = new SubscribeBarData(
                    message.Strategy.Instrument.Symbol,
                    new List<BarSpecification>{message.Strategy.TradeProfile.BarSpecification},
                    this.NewGuid(),
                    this.TimeNow());

                this.Send(BlackBoxService.Portfolio, createPortfolio);
                this.Send(BlackBoxService.Data, registerSymbolDataType);

                this.Log.Debug($"{strategyLabel} created");
            });
        }

        private void OnMessage(RemoveAlphaStrategyModule message)
        {
            Debug.NotNull(message, nameof(message));

            this.Execute(() =>
            {
                var strategyLabel = LabelFactory.StrategyLabel(message.Symbol, message.TradeType);

                this.alphaStrategyModuleStore.RemoveStrategy(strategyLabel);

                this.Log.Information($"{strategyLabel} removed");
            });
        }
    }
}
