//--------------------------------------------------------------------------------------------------
// <copyright file="BarAggregator.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Aggregation
{
    using System;
    using System.Collections.Generic;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Events;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Data.Messages.Commands;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging.Interfaces;

    /// <summary>
    /// Provides a bar aggregator for a certain symbol.
    /// </summary>
    [PerformanceOptimized]
    public sealed class BarAggregator : ComponentBase
    {
        private readonly IEndpoint parent;
        private readonly Symbol symbol;
        private readonly List<BarSpecification> subscriptions;
        private readonly Dictionary<BarSpecification, BarBuilder?> barBuilders;

        private Tick? lastTick;
        private bool isMarketOpen;

        /// <summary>
        /// Initializes a new instance of the <see cref="BarAggregator"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="parent">The parent endpoint.</param>
        /// <param name="symbol">The symbol.</param>
        /// <param name="isMarketOpen">The is market open flag.</param>
        public BarAggregator(
            IComponentryContainer container,
            IEndpoint parent,
            Symbol symbol,
            bool isMarketOpen)
            : base(NautilusService.Data, container)
        {
            this.parent = parent;
            this.symbol = symbol;
            this.subscriptions = new List<BarSpecification>();
            this.barBuilders = new Dictionary<BarSpecification, BarBuilder?>();

            this.isMarketOpen = isMarketOpen;

            this.RegisterHandler<Tick>(this.OnMessage);
            this.RegisterHandler<CloseBar>(this.OnMessage);
            this.RegisterHandler<Subscribe<BarType>>(this.OnMessage);
            this.RegisterHandler<Unsubscribe<BarType>>(this.OnMessage);
            this.RegisterHandler<MarketOpened>(this.OnMessage);
            this.RegisterHandler<MarketClosed>(this.OnMessage);
        }

        /// <summary>
        /// Gets the bar aggregators current subscriptions.
        /// </summary>
        public IEnumerable<BarSpecification> Subscriptions => this.subscriptions;

        private static Price GetUpdateQuote(QuoteType quoteType, Tick tick)
        {
            switch (quoteType)
            {
                case QuoteType.BID:
                    return tick.Bid;
                case QuoteType.ASK:
                    return tick.Ask;
                case QuoteType.MID:
                    var decimalsPlusOne = tick.Bid.DecimalPrecision + 1;
                    return Price.Create(
                        Math.Round((tick.Bid + tick.Ask) / 2, decimalsPlusOne),
                        decimalsPlusOne);
                case QuoteType.LAST:
                    throw new InvalidOperationException("Cannot update with QuoteType.Last.");
                default:
                    throw new InvalidOperationException($"QuoteType {quoteType} not recognized.");
            }
        }

        [PerformanceOptimized]
        private void OnMessage(Tick tick)
        {
            Debug.EqualTo(tick.Symbol, this.symbol, nameof(tick.Symbol));

            foreach (var (barSpec, builder) in this.barBuilders)
            {
                var quote = GetUpdateQuote(barSpec.QuoteType, tick);

                if (builder is null)
                {
                    this.barBuilders[barSpec] = new BarBuilder(quote);
                }
                else
                {
                    builder.Update(quote);
                }
            }

            this.lastTick = tick;
        }

        private void OnMessage(CloseBar message)
        {
            var barSpec = message.BarSpecification;

            if (!this.barBuilders.ContainsKey(barSpec))
            {
                this.Log.Warning($"Does not contain the bar specification {message.BarSpecification}");
                return;
            }

            if (!this.isMarketOpen)
            {
                return; // Won't close a bar outside market hours.
            }

            if (this.lastTick is null)
            {
                return; // No ticks have been received for the builders.
            }

            var builder = this.barBuilders[barSpec];
            if (builder is null)
            {
                return; // No builder to build bar.
            }

            // Close the bar.
            var bar = builder.Build(message.CloseTime);

            // Send to bar aggregation controller.
            this.parent.Send((new BarType(this.symbol, barSpec), bar));

            // Refresh bar builder.
            this.barBuilders[barSpec] = new BarBuilder(bar.Close);
        }

        private void OnMessage(Subscribe<BarType> message)
        {
            var barSpec = message.DataType.Specification;

            if (barSpec.Resolution == Resolution.TICK)
            {
                // TODO
                throw new InvalidOperationException("Tick bars not yet supported.");
            }

            if (this.subscriptions.Contains(barSpec))
            {
                this.Log.Warning($"Already subscribed to {message.DataType} bars.");
                return;
            }

            this.subscriptions.Add(barSpec);

            if (this.barBuilders.ContainsKey(barSpec))
            {
                return; // Already contains builder.
            }

            this.barBuilders.Add(barSpec, null);
            this.Log.Debug($"Subscribed to {message.DataType} bars.");
        }

        private void OnMessage(Unsubscribe<BarType> message)
        {
            var barSpec = message.DataType.Specification;

            if (!this.subscriptions.Contains(barSpec))
            {
                this.Log.Warning($"Already unsubscribed from {message.DataType} bars.");
                return;
            }

            this.subscriptions.Remove(barSpec);

            if (this.barBuilders.ContainsKey(barSpec))
            {
                this.barBuilders.Remove(barSpec);
            }

            this.Log.Debug($"Unsubscribed from {message.DataType} bars.");
        }

        private void OnMessage(MarketOpened message)
        {
            this.isMarketOpen = true;
        }

        private void OnMessage(MarketClosed message)
        {
            this.isMarketOpen = false;

            // Purge bar builders.
            foreach (var barSpec in this.barBuilders.Keys)
            {
                this.barBuilders[barSpec] = null;
            }
        }
    }
}
