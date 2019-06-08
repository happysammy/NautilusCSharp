// -------------------------------------------------------------------------------------------------
// <copyright file="DataBus{T}.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messaging
{
    using System.Collections.Generic;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Messaging.Interfaces;

    /// <summary>
    /// Provides a generic data bus.
    /// </summary>
    /// <typeparam name="T">The data bus type.</typeparam>
    public sealed class DataBus<T> : Component
    {
        private readonly List<IEndpoint> subscriptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataBus{T}"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        public DataBus(IComponentryContainer container)
        : base(container)
        {
            this.subscriptions = new List<IEndpoint>();

            this.RegisterHandler<ISubscribe>(this.OnMessage);
            this.RegisterHandler<IUnsubscribe>(this.OnMessage);
            this.RegisterHandler<T>(this.Publish);
        }

        private void OnMessage(ISubscribe message)
        {
            var type = message.SubscriptionType;
            var subscriber = message.Subscriber;

            if (type != typeof(T))
            {
                this.Log.Error($"Cannot subscribe to {type} data (only {typeof(T)} data).");
            }

            if (this.subscriptions.Contains(subscriber))
            {
                this.Log.Warning($"{subscriber} is already subscribed to {type} data.");
                return; // Design time error
            }

            this.subscriptions.Add(subscriber);
        }

        private void OnMessage(IUnsubscribe message)
        {
            var type = message.SubscriptionType;
            var subscriber = message.Subscriber;

            if (type != typeof(T))
            {
                this.Log.Error($"Cannot unsubscribe from {type} data (only {typeof(T)} data).");
                return; // Design time error
            }

            if (!this.subscriptions.Contains(subscriber))
            {
                this.Log.Warning($"{subscriber} is already unsubscribed from {type} data.");
                return;
            }

            this.subscriptions.Remove(subscriber);
        }

        private void Publish(T data)
        {
            for (var i = 0; i < this.subscriptions.Count; i++)
            {
                this.subscriptions[i].Send(data);

                this.Log.Verbose(
                    $"[{this.ProcessedCount}] {typeof(T)} -> {this.subscriptions[i]}");
            }
        }
    }
}
