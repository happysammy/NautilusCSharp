//--------------------------------------------------------------------------------------------------
// <copyright file="Subscribe{T}.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Messages.Commands
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using NodaTime;

    /// <summary>
    /// Represents a command to subscribe to type T.
    /// </summary>
    /// <typeparam name="T">The subscription type.</typeparam>
    [Immutable]
    public sealed class Subscribe<T> : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Subscribe{T}"/> class.
        /// </summary>
        /// <param name="subscriptionType">The subscription type.</param>
        /// <param name="id">The commands identifier.</param>
        /// <param name="timestamp">The commands timestamp.</param>
        public Subscribe(
            T subscriptionType,
            Guid id,
            ZonedDateTime timestamp)
            : base(
                typeof(Subscribe<T>),
                id,
                timestamp)
        {
            Debug.NotDefault(id, nameof(id));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.SubscriptionType = subscriptionType;
        }

        /// <summary>
        /// Gets the type to subscribe to.
        /// </summary>
        public T SubscriptionType { get; }

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"Subscribe<{this.SubscriptionType}>({this.Id})";
    }
}
