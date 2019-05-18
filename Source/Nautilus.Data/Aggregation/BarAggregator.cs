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
    using Nautilus.Core.Extensions;
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
        private readonly List<BarSpecification> specifications;
        private readonly Dictionary<BarSpecification, BarBuilder?> barBuilders;
        private readonly Dictionary<BarSpecification, BarBuilder> pendingBuilders;

        /// <summary>
        /// Initializes a new instance of the <see cref="BarAggregator"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="parent">The parent endpoint.</param>
        /// <param name="symbol">The symbol.</param>
        public BarAggregator(
            IComponentryContainer container,
            IEndpoint parent,
            Symbol symbol)
            : base(NautilusService.Data, container)
        {
            this.parent = parent;
            this.symbol = symbol;
            this.specifications = new List<BarSpecification>();
            this.barBuilders = new Dictionary<BarSpecification, BarBuilder?>();
            this.pendingBuilders = new Dictionary<BarSpecification, BarBuilder>();

            this.RegisterHandler<Tick>(this.OnMessage);
            this.RegisterHandler<CloseBar>(this.OnMessage);
            this.RegisterHandler<Subscribe<BarType>>(this.OnMessage);
            this.RegisterHandler<Unsubscribe<BarType>>(this.OnMessage);
            this.RegisterHandler<MarketClosed>(this.OnMessage);
        }

        /// <summary>
        /// Gets the bar aggregators current subscriptions.
        /// </summary>
        public IEnumerable<BarSpecification> Specifications => this.specifications;

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
                    goto default;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(quoteType, nameof(quoteType));
            }
        }

        private void OnMessage(Tick tick)
        {
            Debug.EqualTo(tick.Symbol, this.symbol, nameof(tick.Symbol));

            foreach (var (barSpec, builder) in this.barBuilders)
            {
                var quote = GetUpdateQuote(barSpec.QuoteType, tick);

                if (builder is null)
                {
                    this.pendingBuilders.Add(barSpec, new BarBuilder(quote));
                }
                else
                {
                    builder.Update(quote);
                }
            }

            if (this.pendingBuilders.Count > 0)
            {
                this.CreateBuilders();
            }
        }

        private void CreateBuilders()
        {
            foreach (var (barSpec, builder) in this.pendingBuilders)
            {
                this.barBuilders[barSpec] = builder;
                this.Log.Debug($"Created new BarBuilder for {barSpec}.");
            }

            this.pendingBuilders.Clear();
        }

        private void OnMessage(CloseBar message)
        {
            var barSpec = message.BarSpecification;

            if (!this.barBuilders.ContainsKey(barSpec))
            {
                this.Log.Warning($"Does not contain the bar specification {message.BarSpecification}");
                return;
            }

            var builder = this.barBuilders[barSpec];
            if (builder is null)
            {
                return; // No builder to build bar.
            }

            // Close the bar.
            var bar = builder.Build(this.TimeNow().Floor(barSpec.Duration));

            // Send to bar aggregation controller (parent).
            this.parent.Send((new BarType(this.symbol, barSpec), bar));

            // Refresh bar builder.
            this.barBuilders[barSpec] = new BarBuilder(bar.Close);
        }

        private void OnMessage(Subscribe<BarType> message)
        {
            var barSpec = message.DataType.Specification;

            if (this.specifications.Contains(barSpec))
            {
                this.Log.Warning($"Already subscribed to {message.DataType} bars.");
                return;
            }

            this.specifications.Add(barSpec);

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

            if (!this.specifications.Contains(barSpec))
            {
                this.Log.Warning($"Already unsubscribed from {message.DataType} bars.");
                return;
            }

            this.specifications.Remove(barSpec);

            if (this.barBuilders.ContainsKey(barSpec))
            {
                this.barBuilders.Remove(barSpec);
            }

            this.Log.Debug($"Unsubscribed from {message.DataType} bars.");
        }

        private void OnMessage(MarketClosed message)
        {
            // Purge bar builders.
            foreach (var barSpec in this.barBuilders.Keys)
            {
                this.barBuilders[barSpec] = null;
            }
        }
    }
}
