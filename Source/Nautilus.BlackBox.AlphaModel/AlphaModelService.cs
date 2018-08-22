//--------------------------------------------------------------------------------------------------
// <copyright file="AlphaModelService.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.AlphaModel
{
    using Nautilus.BlackBox.AlphaModel.Strategy;
    using Nautilus.BlackBox.Core.Build;
    using Nautilus.BlackBox.Core.Messages.Commands;
    using Nautilus.Common.Commands;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Provides a message end point into the <see cref="NautilusService.AlphaModel"/> service.
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
            NautilusService.AlphaModel,
            LabelFactory.Component(nameof(AlphaModelService)),
            container,
            messagingAdapter)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(alphaStrategyModuleStore, nameof(alphaStrategyModuleStore));

            this.storedContainer = container;
            this.alphaStrategyModuleStore = alphaStrategyModuleStore;

            // Setup message handling
            this.Receive<CreateAlphaStrategyModule>(msg => this.OnMessage(msg));
            this.Receive<RemoveAlphaStrategyModule>(msg => this.OnMessage(msg));
            this.Receive<BarDataEvent>(msg => this.OnMessage(msg));
        }

        private void OnMessage(BarDataEvent message)
        {
            Debug.NotNull(message, nameof(message));

            this.Execute(() =>
            {
                this.alphaStrategyModuleStore.Tell(message);
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
                var barType = new BarType(message.Symbol, message.Strategy.TradeProfile.BarSpecification);
                this.alphaStrategyModuleStore.AddStrategy(strategyLabel, barType, alphasStrategyModuleRef);

                var createPortfolio = new CreatePortfolio(
                    message.Strategy.Instrument,
                    this.NewGuid(),
                    this.TimeNow());

                var registerSymbolDataType = new Subscribe<BarType>(
                    barType,
                    this.NewGuid(),
                    this.TimeNow());

                this.Send(NautilusService.Portfolio, createPortfolio);
                this.Send(NautilusService.Data, registerSymbolDataType);

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
