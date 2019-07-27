// -------------------------------------------------------------------------------------------------
// <copyright file="DataBus{T}.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Core.Annotations;
    using Nautilus.Messaging;

    /// <summary>
    /// Provides a generic data bus.
    /// </summary>
    /// <typeparam name="T">The bus data type.</typeparam>
    public sealed class DataBus<T> : Component
    {
        private readonly List<Mailbox> subscriptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataBus{T}"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        public DataBus(IComponentryContainer container)
        : base(container, State.Running)
        {
            this.subscriptions = new List<Mailbox>();

            this.BusType = typeof(T);

            this.RegisterHandler<T>(this.Publish);
            this.RegisterHandler<Subscribe<Type>>(this.OnMessage);
            this.RegisterHandler<Unsubscribe<Type>>(this.OnMessage);
        }

        /// <summary>
        /// Gets the bus data type.
        /// </summary>
        public Type BusType { get; }

        /// <summary>
        /// Gets the data bus subscriptions.
        /// </summary>
        public IReadOnlyCollection<Address> Subscriptions => this.subscriptions.Select(m => m.Address).ToList().AsReadOnly();

        private void OnMessage(Subscribe<Type> message)
        {
            var type = message.Subscription;
            if (message.Subscription != this.BusType)
            {
                this.Log.Error($"Cannot subscribe to {type.Name} data (only {this.BusType.Name} data).");
                return;
            }

            var subscriber = message.Subscriber;

            if (this.subscriptions.Contains(subscriber))
            {
                this.Log.Warning($"The {subscriber} is already subscribed to {this.BusType.Name} data.");
                return; // Design time error
            }

            this.subscriptions.Add(subscriber);
            this.Log.Information($"Subscribed {subscriber} to {this.BusType.Name} data.");
        }

        private void OnMessage(Unsubscribe<Type> message)
        {
            var type = message.Subscription;
            if (message.Subscription != this.BusType)
            {
                this.Log.Error($"Cannot unsubscribe from {type.Name} data (only {this.BusType.Name} data).");
                return;
            }

            var subscriber = message.Subscriber;

            if (!this.subscriptions.Contains(subscriber))
            {
                this.Log.Warning($"The {subscriber} is not subscribed to {this.BusType.Name} data.");
                return;
            }

            this.subscriptions.Remove(subscriber);
            this.Log.Information($"Unsubscribed {subscriber} from {this.BusType.Name} data.");
        }

        [PerformanceOptimized]
        private void Publish(T data)
        {
            if (data is null)
            {
                return; // This should never happen.
            }

            if (this.subscriptions.Count == 0)
            {
                return; // No subscribers.
            }

            for (var i = 0; i < this.subscriptions.Count; i++)
            {
                this.subscriptions[i].Endpoint.Send(data);
            }
        }
    }
}
