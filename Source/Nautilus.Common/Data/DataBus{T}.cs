// -------------------------------------------------------------------------------------------------
// <copyright file="DataBus{T}.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks.Dataflow;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Messaging;

    /// <summary>
    /// Provides a generic data bus.
    /// </summary>
    /// <typeparam name="T">The bus data type.</typeparam>
    public sealed class DataBus<T> : Component
    {
        // The BroadcastBlock<T> ensures that the current element is broadcast to any linked targets
        // before allowing the element to be overwritten.
        private readonly BroadcastBlock<object> pipeline;
        private readonly List<Mailbox> subscriptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataBus{T}"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        public DataBus(IComponentryContainer container)
        : base(container)
        {
            Condition.NotNull(typeof(T), nameof(T));

            // TODO: Add Condition method for the below
            var attributes = typeof(T).GetCustomAttributes(typeof(ImmutableAttribute), true);
            Condition.True(attributes.Length > 0, "The data type <T> is not defined as immutable.");

            this.pipeline = new BroadcastBlock<object>(
                CloneData,
                new ExecutionDataflowBlockOptions
            {
                BoundedCapacity = (int)Math.Pow(2, 22),
            });

            this.subscriptions = new List<Mailbox>();

            this.RegisterHandler<Subscribe<Type>>(this.OnMessage);
            this.RegisterHandler<Unsubscribe<Type>>(this.OnMessage);
        }

        /// <summary>
        /// Gets the bus data type.
        /// </summary>
        public Type BusType { get; } = typeof(T);

        /// <summary>
        /// Gets the data bus subscriptions.
        /// </summary>
        public IReadOnlyCollection<Address> Subscriptions => this.subscriptions.Select(m => m.Address).ToList().AsReadOnly();

        /// <summary>
        /// Posts the given data onto the data bus.
        /// </summary>
        /// <param name="data">The data to post.</param>
        public void PostData(T data)
        {
            this.pipeline.Post(data!); // data cannot be null (checked in constructor)
        }

        /// <inheritdoc />
        protected override void OnStart(Start start)
        {
        }

        /// <inheritdoc />
        protected override void OnStop(Stop stop)
        {
        }

        private static object CloneData(object data)
        {
            return data;
        }

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
                return;  // Design time error
            }

            this.pipeline.LinkTo(subscriber.Endpoint.GetLink());
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

            // TODO: Currently can't easily unlink a subscriber from data
            this.subscriptions.Remove(subscriber);
            this.Log.Information($"Unsubscribed {subscriber} from {this.BusType.Name} data.");
        }
    }
}
