//--------------------------------------------------------------------------------------------------
// <copyright file="MarketDataProcessor.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The sealed <see cref="BarAggregationController"/> class.
    /// </summary>
    public sealed class BarAggregationController : ActorComponentBusConnectedBase
    {
        private readonly IComponentryContainer storedContainer;
        private readonly IImmutableList<Enum> barDataReceivers;
        private readonly IDictionary<Symbol, IActorRef> barAggregators;

        /// <summary>
        /// Initializes a new instance of the <see cref="BarAggregationController"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="serviceContext">The service context.</param>
        /// <param name="barReceivers">The bar data receivers.</param>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public BarAggregationController(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            Enum serviceContext,
            IImmutableList<Enum> barReceivers)
            : base(
            serviceContext,
            LabelFactory.Component(nameof(BarAggregationController)),
            container,
            messagingAdapter)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(serviceContext, nameof(serviceContext));
            Validate.NotNull(barReceivers, nameof(barReceivers));

            this.storedContainer = container;
            this.barDataReceivers = barReceivers.ToImmutableList();
            this.barAggregators = new Dictionary<Symbol, IActorRef>();

            this.SetupCommandMessageHandling();
            this.SetupEventMessageHandling();
        }

        /// <summary>
        /// Sets up all <see cref="CommandMessage"/> handling methods.
        /// </summary>
        private void SetupCommandMessageHandling()
        {
            this.Receive<SubscribeBarData>(msg => this.OnMessage(msg));
            this.Receive<UnsubscribeBarData>(msg => this.OnMessage(msg));
        }

        /// <summary>
        /// Sets up all <see cref="EventMessage"/> handling methods.
        /// </summary>
        private void SetupEventMessageHandling()
        {
            this.Receive<Tick>(msg => this.OnMessage(msg));
            this.Receive<BarClosed>(msg => this.OnMessage(msg));
        }

        private void OnMessage(Tick tick)
        {
            Debug.NotNull(tick, nameof(tick));

            if (this.barAggregators.ContainsKey(tick.Symbol))
            {
                this.barAggregators[tick.Symbol].Tell(tick);
            }
        }

        private void OnMessage(SubscribeBarData message)
        {
            Debug.NotNull(message, nameof(message));

            if (!this.barAggregators.ContainsKey(message.Symbol))
            {
                var barAggregatorRef = Context.ActorOf(Props.Create(() => new BarAggregator(
                    this.storedContainer,
                    this.Service,
                    message.Symbol)));

                this.barAggregators.Add(message.Symbol, barAggregatorRef);
            }

            this.barAggregators[message.Symbol].Tell(message);
        }

        private void OnMessage(UnsubscribeBarData message)
        {
            Debug.NotNull(message, nameof(message));
            Validate.DictionaryContainsKey(message.Symbol, nameof(message.Symbol), this.barAggregators);

            this.barAggregators[message.Symbol].Tell(message);
        }

        private void OnMessage(BarClosed message)
        {
            var @event = new EventMessage(
                message,
                this.NewGuid(),
                this.TimeNow());

            this.Send(this.barDataReceivers, @event);
        }
    }
}
