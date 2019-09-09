//--------------------------------------------------------------------------------------------------
// <copyright file="Unsubscribe{T}.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messages.Commands
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Message;
    using Nautilus.Messaging;
    using NodaTime;

    /// <summary>
    /// Represents a command to unsubscribe a component from type T.
    /// </summary>
    /// <typeparam name="T">The data type.</typeparam>
    [Immutable]
    public sealed class Unsubscribe<T> : Command
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
            Mailbox subscriber,
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
        /// Gets the commands type to unsubscribe from.
        /// </summary>
        public T Subscription { get; }

        /// <summary>
        /// Gets the commands subscription type.
        /// </summary>
        public Type SubscriptionType => typeof(T);

        /// <summary>
        /// Gets the commands subscriber mailbox.
        /// </summary>
        public Mailbox Subscriber { get; }
    }
}
