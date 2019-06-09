//--------------------------------------------------------------------------------------------------
// <copyright file="Unsubscribe{T}.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messages.Commands
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Messaging.Interfaces;
    using NodaTime;

    /// <summary>
    /// Represents a command to unsubscribe from type T.
    /// </summary>
    /// <typeparam name="T">The data type.</typeparam>
    [Immutable]
    public sealed class Unsubscribe<T> : Command, IUnsubscribe
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Unsubscribe{T}"/> class.
        /// </summary>
        /// <param name="subscription">The subscription type.</param>
        /// <param name="subscriber">The subscriber endpoint.</param>
        /// <param name="id">The commands identifier.</param>
        /// <param name="timestamp">The commands timestamp.</param>
        public Unsubscribe(
            T subscription,
            IEndpoint subscriber,
            Guid id,
            ZonedDateTime timestamp)
            : base(
                typeof(Unsubscribe<T>),
                id,
                timestamp)
        {
            Debug.NotDefault(id, nameof(id));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.Subscription = subscription;
            this.Subscriber = subscriber;
        }

        /// <summary>
        /// Gets the type to unsubscribe from.
        /// </summary>
        public T Subscription { get; }

        /// <summary>
        /// Gets the subscriptions name.
        /// </summary>
        public string SubscriptionName => this.Subscription is null ? string.Empty : this.Subscription.ToString();

        /// <summary>
        /// Gets the subscribe messages subscription type.
        /// </summary>
        public Type SubscriptionType => typeof(T);

        /// <inheritdoc />
        public IEndpoint Subscriber { get; }
    }
}
