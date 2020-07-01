//--------------------------------------------------------------------------------------------------
// <copyright file="BarAggregator.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
//--------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
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

namespace Nautilus.Data.Aggregation
{
    /// <summary>
    /// Provides a bar aggregator for a certain symbol.
    /// </summary>
    public sealed class BarAggregator : MessagingComponent
    {
        private readonly IEndpoint controller;
        private readonly Symbol symbol;
        private readonly List<BarSpecification> specifications;
        private readonly Dictionary<BarSpecification, BarBuilder?> barBuilders;
        private readonly Dictionary<BarSpecification, BarBuilder> pendingBuilders;

        /// <summary>
        /// Initializes a new instance of the <see cref="BarAggregator"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="controller">The aggregation controller endpoint.</param>
        /// <param name="symbol">The symbol.</param>
        public BarAggregator(
            IComponentryContainer container,
            IEndpoint controller,
            Symbol symbol)
            : base(container)
        {
            this.controller = controller;
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
        /// Gets the bar aggregators current specifications.
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
                    var decimalsPlusOne = tick.Bid.Precision + 1;
                    var midPrice = Math.Round((tick.Bid + tick.Ask) / 2, decimalsPlusOne);
                    return Price.Create(midPrice, decimalsPlusOne);
                case PriceType.Last:
                case PriceType.Undefined:
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
            var barSpec = message.Specification;
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
                this.controller.Send(barData);

                // Refresh bar builder
                this.barBuilders[barSpec] = new BarBuilder(bar.Close);
            }
            else
            {
                this.Logger.LogWarning($"Does not contain the {nameof(BarSpecification)}({message.Specification}).");
            }
        }

        private void OnMessage(Subscribe<BarType> message)
        {
            var barSpec = message.Subscription.Specification;
            if (this.specifications.Contains(barSpec))
            {
                this.Logger.LogWarning($"Already subscribed to {message.Subscription} bars.");
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
                this.Logger.LogWarning($"Already unsubscribed from {message.Subscription} bars.");
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
