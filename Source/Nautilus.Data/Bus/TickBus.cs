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
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging.Interfaces;

    /// <summary>
    /// Provides a <see cref="Tick"/> data bus.
    /// </summary>
    public sealed class TickBus : Component
    {
        private readonly List<IEndpoint> subscriptions;
        private bool hasSubscribers;

        /// <summary>
        /// Initializes a new instance of the <see cref="TickBus"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        public TickBus(IComponentryContainer container)
        : base(container)
        {
            this.subscriptions = new List<IEndpoint>();

            this.RegisterHandler<ISubscribe>(this.OnMessage);
            this.RegisterHandler<IUnsubscribe>(this.OnMessage);
            this.RegisterHandler<Tick>(this.Publish);
        }

        private void OnMessage(ISubscribe message)
        {
            var type = message.SubscriptionType;
            var subscriber = message.Subscriber;

            if (type != typeof(Tick))
            {
                this.Log.Error($"Cannot subscribe to {type} data (only {typeof(Tick)} data).");
            }

            if (this.subscriptions.Contains(subscriber))
            {
                this.Log.Warning($"{subscriber} is already subscribed to {type} data.");
                return; // Design time error
            }

            this.subscriptions.Add(subscriber);
            this.SetHasSubscribers();
        }

        private void OnMessage(IUnsubscribe message)
        {
            var type = message.SubscriptionType;
            var subscriber = message.Subscriber;

            if (type != typeof(Tick))
            {
                this.Log.Error($"Cannot unsubscribe from {type} data (only {typeof(Tick)} data).");
                return; // Design time error
            }

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
