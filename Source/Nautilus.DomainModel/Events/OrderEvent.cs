//--------------------------------------------------------------------------------------------------
// <copyright file="OrderEvent.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Events
{
    using System;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.Core;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The immutable abstract <see cref="OrderEvent"/> class. The base class for all order events.
    /// </summary>
    [Immutable]
    public abstract class OrderEvent : Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderEvent"/> class.
        /// </summary>
        /// <param name="symbol">The event order symbol.</param>
        /// <param name="orderId">The event order identifier.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="eventTimestamp">The event timestamp.</param>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if any
        /// struct argument is the default value.</exception>
        protected OrderEvent(
            Symbol symbol,
            EntityId orderId,
            Guid eventId,
            ZonedDateTime eventTimestamp)
            : base(eventId, eventTimestamp)
        {
            Validate.NotNull(symbol, nameof(symbol));
            Validate.NotNull(orderId, nameof(orderId));
            Validate.NotDefault(eventId, nameof(eventId));
            Validate.NotDefault(eventTimestamp, nameof(eventTimestamp));

            this.Symbol = symbol;
            this.OrderId = orderId;
        }

        /// <summary>
        /// Gets the events order symbol.
        /// </summary>
        public Symbol Symbol { get; }

        /// <summary>
        /// Gets the events order identifier.
        /// </summary>
        public EntityId OrderId { get; }

        /// <summary>
        /// Returns a string representation of the <see cref="OrderEvent"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => nameof(OrderEvent);
    }
}