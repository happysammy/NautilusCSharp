//--------------------------------------------------------------------------------------------------
// <copyright file="TickBarAggregator.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Data.Market
{
    using System;
    using System.Collections.Generic;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core.Messages.SystemCommands;
    using Nautilus.BlackBox.Core.Build;
    using Nautilus.BlackBox.Core.Enums;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The sealed <see cref="TickBarAggregator"/> class.
    /// </summary>
    public sealed class TickBarAggregator : ActorComponentBusConnectedBase
    {
        private readonly Symbol symbol;
        private readonly BarSpecification BarSpecification;
        private readonly TradeType tradeType;
        private readonly SpreadAnalyzer spreadAnalyzer;

        private BarBuilder barBuilder;
        private int tickCounter;

        /// <summary>
        /// Initializes a new instance of the <see cref="TickBarAggregator"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public TickBarAggregator(
            BlackBoxContainer container,
            IMessagingAdapter messagingAdapter,
            SubscribeSymbolDataType message)
            : base(
            BlackBoxService.Data,
            LabelFactory.ComponentByTradeType(
                nameof(TickBarAggregator),
                message.Symbol,
                message.TradeType),
            container,
            messagingAdapter)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(message, nameof(message));

            this.symbol = message.Symbol;
            this.BarSpecification = message.BarSpecification;
            this.tradeType = message.TradeType;
            this.spreadAnalyzer = new SpreadAnalyzer(message.TickSize);

            this.SetupEventMessageHandling();
        }

        /// <summary>
        /// Sets up all <see cref="EventMessage"/> handling methods.
        /// </summary>
        private void SetupEventMessageHandling()
        {
            this.Receive<Tick>(msg => this.OnReceive(msg));
        }

        private void OnReceive(Tick quote)
        {
            Debug.NotNull(quote, nameof(quote));

            if (this.barBuilder == null)
            {
                this.OnFirstTick(quote);

                return;
            }

            this.tickCounter++;

            this.barBuilder.OnQuote(quote.Bid, quote.Timestamp);
            this.spreadAnalyzer.OnQuote(quote);

            if (this.tickCounter >= this.BarSpecification.Period)
            {
                this.CreateNewMarketDataEvent(quote);
                this.CreateBarBuilder(quote);
                this.spreadAnalyzer.OnBarUpdate(quote.Timestamp);
            }
        }

        private void OnFirstTick(Tick quote)
        {
            Debug.NotNull(quote, nameof(quote));

            this.CreateBarBuilder(quote);

            this.Log.Debug($"Registered for {this.BarSpecification} bars");
            this.Log.Debug($"Receiving quotes ({quote.Symbol.Code}) from {quote.Symbol.Exchange}...");
        }

        private void CreateBarBuilder(Tick quote)
        {
            Debug.NotNull(quote, nameof(quote));

            this.tickCounter = 1;
            this.barBuilder = new BarBuilder(quote.Bid, quote.Timestamp);
        }

        private void CreateNewMarketDataEvent(Tick quote)
        {
            Debug.NotNull(quote, nameof(quote));

            var newBar = this.barBuilder.Build(this.barBuilder.Timestamp);

            var marketData = new MarketDataEvent(
                this.symbol,
                this.tradeType,
                this.BarSpecification,
                newBar,
                quote,
                this.spreadAnalyzer.AverageSpread,
                false,
                this.NewGuid(),
                this.TimeNow());

            this.Send(
                new List<Enum>
                {
                    BlackBoxService.AlphaModel,
                    BlackBoxService.Portfolio,
                    BlackBoxService.Risk
                },
                new EventMessage(
                    marketData,
                    this.NewGuid(),
                    this.TimeNow()));
        }
    }
}
