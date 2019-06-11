// -------------------------------------------------------------------------------------------------
// <copyright file="TickBus.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Bus
{
    using System.Collections.Generic;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging.Interfaces;

    /// <summary>
    /// Provides a <see cref="Tick"/> data bus.
    /// </summary>
    public sealed class TickBus : Component
    {
        private readonly IEndpoint tickPublisher;
        private readonly List<IEndpoint> subscriptions;
        private bool hasSubscribers;

        /// <summary>
        /// Initializes a new instance of the <see cref="TickBus"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="tickPublisher">The tick publisher endpoint..</param>
        public TickBus(IComponentryContainer container, IEndpoint tickPublisher)
        : base(container)
        {
            this.tickPublisher = tickPublisher;
            this.subscriptions = new List<IEndpoint>();

            this.RegisterHandler<Subscribe<Tick>>(this.OnMessage);
            this.RegisterHandler<Unsubscribe<Tick>>(this.OnMessage);
            this.RegisterHandler<Tick>(this.Publish);
        }

        private void OnMessage(Subscribe<Tick> message)
        {
            var type = message.SubscriptionType;
            var subscriber = message.Subscriber;

            if (this.subscriptions.Contains(subscriber))
            {
                this.Log.Warning($"{subscriber} is already subscribed to {type} data.");
                return; // Design time error
            }

            this.subscriptions.Add(subscriber);
            this.SetHasSubscribers();
        }

        private void OnMessage(Unsubscribe<Tick> message)
        {
            var type = message.SubscriptionType;
            var subscriber = message.Subscriber;

            if (!this.subscriptions.Contains(subscriber))
            {
                this.Log.Warning($"{subscriber} is already unsubscribed from {type} data.");
                return;
            }

            this.subscriptions.Remove(subscriber);
            this.SetHasSubscribers();
        }

        private void Publish(Tick tick)
        {
            this.tickPublisher.Send(tick);

            if (!this.hasSubscribers)
            {
                return; // No subscribers to send tick to
            }

            for (var i = 0; i < this.subscriptions.Count; i++)
            {
                this.subscriptions[i].Send(tick);
            }
        }

        private void SetHasSubscribers()
        {
            this.hasSubscribers = this.subscriptions.Count > 0;
        }
    }
}
