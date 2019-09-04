//--------------------------------------------------------------------------------------------------
// <copyright file="AccountEvent.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Events.Base
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Message;
    using Nautilus.DomainModel.Identifiers;
    using NodaTime;

    /// <summary>
    /// The base class for all order events.
    /// </summary>
    [Immutable]
    public abstract class AccountEvent : Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountEvent"/> class.
        /// </summary>
        /// <param name="accountId">The event order identifier.</param>
        /// <param name="eventType">The event type.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="eventTimestamp">The event timestamp.</param>
        protected AccountEvent(
            AccountId accountId,
            Type eventType,
            Guid eventId,
            ZonedDateTime eventTimestamp)
            : base(
                eventType,
                eventId,
                eventTimestamp)
        {
            Debug.NotDefault(eventId, nameof(eventId));
            Debug.NotDefault(eventTimestamp, nameof(eventTimestamp));

            this.AccountId = accountId;
        }

        /// <summary>
        /// Gets the events order identifier.
        /// </summary>
        public AccountId AccountId { get; }

        /// <summary>
        /// Returns a string representation of this <see cref="OrderEvent"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{this.Type.Name}({this.AccountId.Value})";
    }
}
