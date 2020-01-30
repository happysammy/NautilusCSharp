//--------------------------------------------------------------------------------------------------
// <copyright file="BarAggregator.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Aggregation
{
    using System;
    using System.Collections.Generic;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Common.Messages.Events;
    using Nautilus.Core.Correctness;
    using Nautilus.Data.Messages.Commands;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging.Interfaces;

    /// <summary>
    /// Provides a bar aggregator for a certain symbol.
    /// </summary>
    public sealed class BarAggregator : Component
    {
        private readonly IEndpoint parent;
        private readonly Symbol symbol;
        private readonly List<BarSpecification> specifications;
        private readonly Dictionary<BarSpecification, BarBuilder?> barBuilders;
        private readonly Dictionary<BarSpecification, BarBuilder> pendingBuilders;

        /// <summary>
        /// Initializes a new instance of the <see cref="BarAggregator"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="parent">The parent endpoint.</param>
        /// <param name="symbol">The symbol.</param>
        public BarAggregator(
            IComponentryContainer container,
            IEndpoint parent,
            Symbol symbol)
            : base(container)
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
        public IReadOnlyCollection<BarSpecification> Specifications => this.specifications.AsReadOnly();

        private static Price GetUpdateQuote(PriceType priceType, Tick tick)
        {
            switch (priceType)
            {
                case PriceType.Bid:
                    return tick.Bid;
                case PriceType.Ask:
                    return tick.Ask;
                case PriceType.Mid:
                    var decimalsPlusOne = tick.Bid.DecimalPrecision + 1;
                    return Price.Create(
                        Math.Round((tick.Bid + tick.Ask) / 2, decimalsPlusOne),
                        decimalsPlusOne);
                case PriceType.Last:
                    goto default;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(priceType, nameof(priceType));
            }
        }

        private void OnMessage(Tick tick)
        {
            Debug.EqualTo(tick.Symbol, this.symbol, nameof(tick.Symbol));  // Design time error

            foreach (var barSpec in this.barBuilders.Keys)
            {
                var quote = GetUpdateQuote(barSpec.PriceType, tick);

                if (this.barBuilders.TryGetValue(barSpec, out var builder))
                {
                    if (builder is null)
                    {
                        this.pendingBuilders.Add(barSpec, new BarBuilder(quote));
                        continue;
                    }

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
            // Add all pending builders to builders
            foreach (var barSpec in this.pendingBuilders.Keys)
            {
                this.barBuilders[barSpec] = this.pendingBuilders[barSpec];
            }

            this.pendingBuilders.Clear();
        }

        private void OnMessage(CloseBar message)
        {
            var barSpec = message.BarSpecification;
            if (this.barBuilders.TryGetValue(barSpec, out var builder))
            {
                if (builder is null)
                {
                    return;  // No bar to build
                }

                // Close the bar
                var bar = builder.Build(message.ScheduledTime);
                var barType = new BarType(this.symbol, barSpec);
                var barData = new BarData(barType, bar);

                // Send to bar aggregation controller (parent)
                this.parent.Send(barData);

                // Refresh bar builder
                this.barBuilders[barSpec] = new BarBuilder(bar.Close);
            }
            else
            {
                this.Log.Warning($"Does not contain the {nameof(BarSpecification)}({message.BarSpecification}).");
            }
        }

        private void OnMessage(Subscribe<BarType> message)
        {
            var barSpec = message.Subscription.Specification;
            if (this.specifications.Contains(barSpec))
            {
                this.Log.Warning($"Already subscribed to {message.Subscription} bars.");
                return;
            }

            this.specifications.Add(barSpec);

            if (this.barBuilders.ContainsKey(barSpec))
            {
                return; // Already contains builder
            }

            this.barBuilders.Add(barSpec, null);
        }

        private void OnMessage(Unsubscribe<BarType> message)
        {
            var barSpec = message.Subscription.Specification;
            if (!this.specifications.Contains(barSpec))
            {
                this.Log.Warning($"Already unsubscribed from {message.Subscription} bars.");
                return;
            }

            this.specifications.Remove(barSpec);

            if (this.barBuilders.ContainsKey(barSpec))
            {
                this.barBuilders.Remove(barSpec);
            }
        }

        private void OnMessage(MarketClosed message)
        {
            // Purge bar builders
            foreach (var barSpec in this.barBuilders.Keys)
            {
                this.barBuilders[barSpec] = null;
            }
        }
    }
}
