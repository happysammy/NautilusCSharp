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
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages;
    using Nautilus.Common.Messaging;
    using Nautilus.Database.Messages.Commands;
    using Nautilus.Database.Messages.Events;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    using Resolution = Nautilus.DomainModel.Enums.Resolution;
    using QuoteType = Nautilus.DomainModel.Enums.QuoteType;
    using Tick = Nautilus.DomainModel.ValueObjects.Tick;
    using Bar = Nautilus.DomainModel.ValueObjects.BarSpecification;
    using BarSpecification = Nautilus.DomainModel.ValueObjects.BarSpecification;

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
        /// <param name="symbol">The symbol.</param>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public BarAggregator(
            IComponentryContainer container,
            Symbol symbol)
            : base(
            ServiceContext.Database,
            LabelFactory.Component(
                nameof(BarAggregator),
                symbol),
            container)
        {
            Validate.NotNull(container, nameof(container));
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
            this.Receive<Subscribe<BarType>>(msg => this.OnMessage(msg));
            this.Receive<Unsubscribe<BarType>>(msg => this.OnMessage(msg));
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
                    case QuoteType.Bid:
                        builder.Value.Update(tick.Bid);
                        break;

                    case QuoteType.Ask:
                        builder.Value.Update(tick.Ask);
                        break;

                    case QuoteType.Mid:
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
        private void OnMessage(Subscribe<BarType> message)
        {
            Debug.NotNull(message, nameof(message));

            var barSpec = message.DataType.Specification;

            if (barSpec.Resolution == Resolution.Tick)
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

        /// <summary>
        /// Handles the message by removing all bar builders for the relevant bar specifications.
        /// </summary>
        /// <param name="message">The received message.</param>
        private void OnMessage(Unsubscribe<BarType> message)
        {
            Debug.NotNull(message, nameof(message));

            var barType = message.DataType.Specification;

            if (this.barBuilders.ContainsKey(barType))
            {
                this.barBuilders.Remove(barType);

                Log.Debug($"Removed {barType} bars.");
            }
        }
    }
}
