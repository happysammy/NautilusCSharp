//--------------------------------------------------------------------------------------------------
// <copyright file="BarAggregator.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Aggregators
{
    using System;
    using System.Collections.Generic;
    using Akka.Actor;
    using Nautilus.Core.Validation;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.Database.Messages.Commands;
    using Nautilus.Database.Messages.Events;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Ingests ticks and produces <see cref="Bar"/>s based on the given list of <see cref="BarSpecification"/>s.
    /// </summary>
    public sealed class BarAggregator : ActorComponentBase
    {
        private static readonly Duration OneMinuteDuration = Duration.FromMinutes(1);
        private readonly Symbol symbol;
        private readonly SpreadAnalyzer spreadAnalyzer;
        private readonly IDictionary<BarSpecification, BarBuilder> barBuilders;

        private Tick lastTick;

        /// <summary>
        /// Initializes a new instance of the <see cref="BarAggregator"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="serviceContext">The service context.</param>
        /// <param name="symbol">The symbol.</param>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public BarAggregator(
            IComponentryContainer container,
            Enum serviceContext,
            Symbol symbol)
            : base(
            serviceContext,
            LabelFactory.Component(
                nameof(BarAggregator),
                symbol),
            container)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(serviceContext, nameof(serviceContext));
            Validate.NotNull(symbol, nameof(symbol));

            this.symbol = symbol;
            this.spreadAnalyzer = new SpreadAnalyzer(0.00001m);  // TODO: Hardcoded ticksize.
            this.barBuilders = new Dictionary<BarSpecification, BarBuilder>();

            this.SetupCommandMessageHandling();
            this.SetupEventMessageHandling();
        }

        /// <summary>
        /// Sets up all <see cref="CommandMessage"/> handling methods.
        /// </summary>
        private void SetupCommandMessageHandling()
        {
            this.Receive<CloseBar>(msg => this.OnMessage(msg));
            this.Receive<SubscribeBarData>(msg => this.OnMessage(msg));
            this.Receive<UnsubscribeBarData>(msg => this.OnMessage(msg));
        }

        /// <summary>
        /// Sets up all <see cref="EventMessage"/> handling methods.
        /// </summary>
        private void SetupEventMessageHandling()
        {
            this.Receive<Tick>(msg => this.OnMessage(msg));
        }

        /// <summary>
        /// On receiving the tick; updates all bar builders and records as last tick.
        /// </summary>
        /// <param name="tick">The received tick.</param>
        /// <exception cref="InvalidOperationException">The quote type is not recognized.</exception>
        private void OnMessage(Tick tick)
        {
            Debug.NotNull(tick, nameof(tick));
            Debug.EqualTo(tick.Symbol, nameof(tick.Symbol), this.symbol);

            foreach (var builder in this.barBuilders)
            {
                switch (builder.Key.QuoteType)
                {
                    case BarQuoteType.Bid:
                        builder.Value.Update(tick.Bid);
                        break;

                    case BarQuoteType.Ask:
                        builder.Value.Update(tick.Ask);
                        break;

                    case BarQuoteType.Mid:
                        builder.Value.Update(
                            Price.Create(Math.Round((tick.Bid + tick.Ask) / 2, 10), 10));
                        break;
                    default:
                        throw new InvalidOperationException();
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
            Debug.NotNull(message, nameof(message));

            var barSpec = message.BarSpecification;

            if (barSpec.Duration == OneMinuteDuration)
            {
                this.spreadAnalyzer.OnBarUpdate(message.CloseTime);
            }

            if (this.barBuilders.ContainsKey(message.BarSpecification))
            {

                var builder = this.barBuilders[barSpec];

                // No ticks have been received by the builder.
                if (builder.IsNotInitialized)
                {
                    return;
                }

                // Close the bar and send to parent.
                var bar = builder.Build(message.CloseTime);
                var barClosed = new BarClosed(
                    this.symbol,
                    barSpec,
                    bar,
                    this.lastTick,
                    this.spreadAnalyzer.AverageSpread,
                    this.NewGuid());
                Context.Parent.Tell(barClosed);

                // Create and initialize new builder.
                builder = new BarBuilder();
                builder.Update(bar.Close);

                return;
            }

            Log.Warning($"Does not contain the bar specification {message.BarSpecification}");
        }

        /// <summary>
        /// Handles the message by adding a new bar builder for each contained bar specifications.
        /// </summary>
        /// <param name="message">The received message.</param>
        /// <exception cref="InvalidOperationException">If the resolution is for tick bars.</exception>
        private void OnMessage(SubscribeBarData message)
        {
            Debug.NotNull(message, nameof(message));

            foreach (var barSpec in message.BarSpecifications)
            {
                if (barSpec.Resolution == BarResolution.Tick)
                {
                    // TODO
                    throw new InvalidOperationException("Tick bars not yet supported.");
                }

                if (!this.barBuilders.ContainsKey(barSpec))
                {
                    this.barBuilders.Add(barSpec, new BarBuilder());

                    Log.Debug($"Added {barSpec} bars.");
                }
            }
        }

        /// <summary>
        /// Handles the message by removing all bar builders for the relevant bar specifications.
        /// </summary>
        /// <param name="message">The received message.</param>
        private void OnMessage(UnsubscribeBarData message)
        {
            Debug.NotNull(message, nameof(message));

            foreach (var barSpec in message.BarSpecifications)
            {
                if (this.barBuilders.ContainsKey(barSpec))
                {
                    this.barBuilders.Remove(barSpec);

                    Log.Debug($"Removed {barSpec} bars.");
                }
            }
        }
    }
}
