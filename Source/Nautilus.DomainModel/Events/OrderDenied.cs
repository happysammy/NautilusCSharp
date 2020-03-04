//--------------------------------------------------------------------------------------------------
// <copyright file="OrderDenied.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Events
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Events.Base;
    using Nautilus.DomainModel.Identifiers;
    using NodaTime;

    /// <summary>
    /// Represents an event where an order has been denied by the system (due risk controls).
    /// </summary>
    [Immutable]
    public sealed class OrderDenied : OrderEvent
    {
        private static readonly Type EventType = typeof(OrderDenied);

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderDenied"/> class.
        /// </summary>
        /// <param name="orderId">The event order identifier.</param>
        /// <param name="deniedReason">The event denied reason.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="eventTimestamp">The event timestamp.</param>
        public OrderDenied(
            OrderId orderId,
            string deniedReason,
            Guid eventId,
            ZonedDateTime eventTimestamp)
            : base(
                orderId,
                EventType,
                eventId,
                eventTimestamp)
        {
            Debug.NotDefault(eventId, nameof(eventId));
            Debug.NotDefault(eventTimestamp, nameof(eventTimestamp));

            this.DeniedReason = deniedReason;
        }

        /// <summary>
        /// Gets the events message.
        /// </summary>
        public string DeniedReason { get; }

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{this.Type.Name}(" +
                                             $"OrderId={this.OrderId.Value}, " +
                                             $"DeniedReason={this.DeniedReason})";
    }
}
