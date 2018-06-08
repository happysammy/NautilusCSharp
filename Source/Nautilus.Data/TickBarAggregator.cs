//--------------------------------------------------------------------------------------------------
// <copyright file="TickBarAggregator.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data
{
    using System;
    using Akka.Actor;
    using Nautilus.Core.Validation;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.Data.Builders;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The sealed <see cref="TickBarAggregator"/> class.
    /// </summary>
    public sealed class TickBarAggregator : ActorComponentBase
    {
        private readonly Symbol symbol;
        private readonly BarSpecification barSpecification;
        private readonly SpreadAnalyzer spreadAnalyzer;

        private BarBuilder barBuilder;
        private int tickCounter;

        /// <summary>
        /// Initializes a new instance of the <see cref="TickBarAggregator"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="serviceContext">The service context.</param>
        /// <param name="symbolBarSpec">The symbol bar specification.</param>
        /// <param name="tickSize">The tick size.</param>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public TickBarAggregator(
            IComponentryContainer container,
            Enum serviceContext,
            SymbolBarSpec symbolBarSpec,
            decimal tickSize)
            : base(
            serviceContext,
            LabelFactory.Component(
                nameof(TickBarAggregator),
                symbolBarSpec),
            container)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(serviceContext, nameof(serviceContext));
            Validate.NotNull(symbolBarSpec, nameof(symbolBarSpec));

            this.symbol = symbolBarSpec.Symbol;
            this.barSpecification = symbolBarSpec.BarSpecification;
            this.spreadAnalyzer = new SpreadAnalyzer(tickSize);

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

            if (this.tickCounter >= this.barSpecification.Period)
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

            this.Log.Debug($"Registered for {this.barSpecification} bars");
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

            Context.Parent.Tell(new BarDataEvent(
                this.symbol,
                this.barSpecification,
                newBar,
                quote,
                this.spreadAnalyzer.AverageSpread,
                false,
                this.NewGuid(),
                this.TimeNow()),
                this.Self);
        }
    }
}
