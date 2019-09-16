//--------------------------------------------------------------------------------------------------
// <copyright file="TradeEvent.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Events
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Message;
    using Nautilus.DomainModel.Events.Base;
    using Nautilus.DomainModel.Identifiers;

    /// <summary>
    /// Represents an event where an order had been accepted by the broker.
    /// </summary>
    [Immutable]
    public sealed class TradeEvent : Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TradeEvent"/> class.
        /// </summary>
        /// <param name="traderId">The event trader identifier.</param>
        /// <param name="orderEvent">The event order event.</param>
        public TradeEvent(TraderId traderId, OrderEvent orderEvent)
            : base(
                typeof(TradeEvent),
                orderEvent.Id,
                orderEvent.Timestamp)
        {
            this.TraderId = traderId;
            this.Event = orderEvent;
        }

        /// <summary>
        /// Gets the events trader identifier.
        /// </summary>
        public TraderId TraderId { get; }

        /// <summary>
        /// Gets the trade event.
        /// </summary>
        public OrderEvent Event { get; }

        /// <summary>
        /// Returns a string representation of this <see cref="TradeEvent"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{nameof(TradeEvent)}({this.Event.Type.Name}, OrderId=({this.Event.OrderId.Value})";
    }
}
