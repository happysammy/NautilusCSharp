//--------------------------------------------------------------------------------------------------
// <copyright file="BarAggregator.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Akka.Actor;
    using Nautilus.Core.Validation;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.Data.Messages;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Ingests ticks and produces <see cref="Bar"/>s based on the given list of <see cref="BarSpecification"/>s.
    /// </summary>
    public sealed class BarAggregator : ActorComponentBase
    {
        private readonly Symbol symbol;
        private readonly IDictionary<BarSpecification, BarBuilder> barBuilders;

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

        private void OnMessage(Tick tick)
        {
            Debug.NotNull(tick, nameof(tick));
            Debug.EqualTo(tick.Symbol, nameof(tick.Symbol), this.symbol);

            foreach (var builder in this.barBuilders)
            {
                switch (builder.Key.QuoteType)
                {
                    case BarQuoteType.Bid:
                        builder.Value.OnQuote(tick.Bid);
                        break;

                    case BarQuoteType.Ask:
                        builder.Value.OnQuote(tick.Ask);
                        break;

                    case BarQuoteType.Mid:
                        builder.Value.OnQuote(
                            Price.Create(Math.Round(tick.Bid + tick.Ask / 2, 10), 10));
                        break;
                    default:
                        throw new InvalidOperationException("The quote type is not recognized.");
                }
            }
        }

        private void OnMessage(CloseBar message)
        {
            Debug.NotNull(message, nameof(message));

            if (this.barBuilders.ContainsKey(message.BarSpecification))
            {
                var builder = this.barBuilders[message.BarSpecification];

                // No ticks have been received by the builder.
                if (!builder.IsInitialized)
                {
                    return;
                }

                // Close the bar and send to parent.
                var bar = builder.Build(message.CloseTime);
                Context.Parent.Tell(bar);

                // Create and initialize new builder.
                builder = new BarBuilder();
                builder.OnQuote(bar.Close);

                return;
            }

            Log.Warning($"Does not contain the bar specification {message.BarSpecification}");
        }

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
                }
            }
        }

        private void OnMessage(UnsubscribeBarData message)
        {
            Debug.NotNull(message, nameof(message));

            foreach (var barSpec in message.BarSpecifications)
            {
                if (this.barBuilders.ContainsKey(barSpec))
                {
                    this.barBuilders.Remove(barSpec);
                }
            }
        }
    }
}
