//--------------------------------------------------------------------------------------------------
// <copyright file="BarAggregator.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Aggregators
{
    using System;
    using System.Collections.Generic;
    using Akka.Actor;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Common.Messages.Events;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Data.Messages.Commands;
    using Nautilus.Data.Messages.Events;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Ingests ticks and produces <see cref="Bar"/>s based on the given list of <see cref="BarSpecification"/>s.
    /// </summary>
    [PerformanceOptimized]
    public sealed class BarAggregator : ActorComponentBase
    {
        private static readonly Duration OneMinuteDuration = Duration.FromMinutes(1);
        private readonly Symbol symbol;
        private readonly SpreadAnalyzer spreadAnalyzer;
        private readonly Dictionary<BarSpecification, BarBuilder> barBuilders;

        private Tick? lastTick;
        private bool isMarketOpen;

        /// <summary>
        /// Initializes a new instance of the <see cref="BarAggregator"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="symbol">The symbol.</param>
        /// <param name="isMarketOpen">The is market open flag.</param>
        public BarAggregator(
            IComponentryContainer container,
            Symbol symbol,
            bool isMarketOpen)
            : base(
            NautilusService.Data,
            LabelFactory.Create(
                nameof(BarAggregator),
                symbol),
            container)
        {
            this.symbol = symbol;
            this.spreadAnalyzer = new SpreadAnalyzer();
            this.barBuilders = new Dictionary<BarSpecification, BarBuilder>();

            this.isMarketOpen = isMarketOpen;

            // Command messages.
            this.Receive<CloseBar>(this.OnMessage);
            this.Receive<Subscribe<BarType>>(this.OnMessage);
            this.Receive<Unsubscribe<BarType>>(this.OnMessage);

            // Event messages.
            this.Receive<MarketOpened>(this.OnMessage);
            this.Receive<MarketClosed>(this.OnMessage);
            this.Receive<Tick>(this.OnMessage);
        }

        /// <summary>
        /// On receiving the tick; updates all bar builders and records as last tick.
        /// </summary>
        /// <param name="tick">The received tick.</param>
        /// <exception cref="InvalidOperationException">The quote type is not recognized.</exception>
        private void OnMessage(Tick tick)
        {
            Debug.EqualTo(tick.Symbol, this.symbol, nameof(tick.Symbol));

            foreach (var builder in this.barBuilders)
            {
                switch (builder.Key.QuoteType)
                {
                    case QuoteType.Bid:
                        builder.Value.Update(tick.Bid);
                        break;

                    case QuoteType.Ask:
                        builder.Value.Update(tick.Ask);
                        break;

                    case QuoteType.Mid:
                        var decimalsPlusOne = tick.Bid.DecimalPrecision + 1;
                        builder.Value.Update(
                            Price.Create(
                                Math.Round((tick.Bid + tick.Ask) / 2, decimalsPlusOne),
                                decimalsPlusOne));
                        break;
                    default: throw new InvalidOperationException("QuoteType not recognized.");
                }
            }

            this.spreadAnalyzer.Update(tick);
            this.lastTick = tick;
        }

        /// <summary>
        /// Handles the message by checking if the relevant bar builder is contained, it will close
        /// the bar sending a closed bar event to the parent. A new bar builder is then created.
        /// </summary>
        /// <param name="message">The received message.</param>
        private void OnMessage(CloseBar message)
        {
            var barSpec = message.BarSpecification;

            if (!this.isMarketOpen)
            {
                // Won't close a bar outside market hours.
                return;
            }

            if (barSpec.Duration == OneMinuteDuration)
            {
                this.spreadAnalyzer.OnBarUpdate(message.CloseTime);
            }

            if (this.barBuilders.ContainsKey(message.BarSpecification))
            {
                var builder = this.barBuilders[barSpec];

                // No ticks have been received by the builder.
                if (builder.IsNotInitialized || this.lastTick is null)
                {
                    return;
                }

                // Close the bar and send to parent.
                var barType = new BarType(this.symbol, barSpec);
                var bar = builder.Build(message.CloseTime);

                var barClosed = new BarClosed(
                    barType,
                    bar,
                    this.lastTick,
                    this.spreadAnalyzer.AverageSpread,
                    this.NewGuid());
                Context.Parent.Tell(barClosed);

                // Create and initialize new builder.
                this.barBuilders[barSpec] = new BarBuilder(bar.Close);

                return;
            }

            this.Log.Warning($"Does not contain the bar specification {message.BarSpecification}");
        }

        /// <summary>
        /// Handles the message by adding a new bar builder for each contained bar specifications.
        /// </summary>
        /// <param name="message">The received message.</param>
        /// <exception cref="InvalidOperationException">If the resolution is for tick bars.</exception>
        private void OnMessage(Subscribe<BarType> message)
        {
            var barSpec = message.DataType.Specification;

            if (barSpec.Resolution == Resolution.Tick)
            {
                // TODO
                throw new InvalidOperationException("Tick bars not yet supported.");
            }

            if (!this.barBuilders.ContainsKey(barSpec))
            {
                this.barBuilders.Add(barSpec, new BarBuilder());

                this.Log.Debug($"Added {barSpec} bars.");
            }
        }

        /// <summary>
        /// Handles the message by removing all bar builders for the relevant bar specifications.
        /// </summary>
        /// <param name="message">The received message.</param>
        private void OnMessage(Unsubscribe<BarType> message)
        {
            var barType = message.DataType.Specification;

            if (this.barBuilders.ContainsKey(barType))
            {
                this.barBuilders.Remove(barType);

                this.Log.Debug($"Removed {barType} bars.");
            }
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
                this.barBuilders[barSpec] = new BarBuilder();
            }
        }
    }
}
