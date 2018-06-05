//--------------------------------------------------------------------------------------------------
// <copyright file="AtomicOrderPacket.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Entities
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The immutable sealed <see cref="AtomicOrderPacket"/> class. Represents a collection of
    /// <see cref="AtomicOrder"/>(s) to be managed together.
    /// </summary>
    [Immutable]
    public sealed class AtomicOrderPacket : Entity<AtomicOrderPacket>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AtomicOrderPacket" /> class.
        /// </summary>
        /// <param name="symbol">The order packet symbol.</param>
        /// <param name="tradeType">The order packet trade Type.</param>
        /// <param name="orders">The order packet orders.</param>
        /// <param name="orderPacketId">The order packet label.</param>
        /// <param name="timestamp">The order packet timestamp.</param>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if any
        /// struct argument is the default value.</exception>
        public AtomicOrderPacket(
            Symbol symbol,
            TradeType tradeType,
            List<AtomicOrder> orders,
            EntityId orderPacketId,
            ZonedDateTime timestamp)
            : base(orderPacketId, timestamp)
        {
            Validate.NotNull(symbol, nameof(symbol));
            Validate.NotNull(tradeType, nameof(tradeType));
            Validate.NotNull(orders, nameof(orders));
            Validate.NotNull(orderPacketId, nameof(orderPacketId));
            Validate.NotDefault(timestamp, nameof(timestamp));

            this.Symbol = symbol;
            this.TradeType = tradeType;
            this.Orders = orders.ToImmutableList();

            var tempList = new List<EntityId>();

            foreach (var order in this.Orders)
            {
                tempList.Add(order.EntryOrder.OrderId);
                tempList.Add(order.StopLossOrder.OrderId);

                if (order.ProfitTargetOrder.HasValue)
                {
                    tempList.Add(order.ProfitTargetOrder.Value.OrderId);
                }
            }

            this.OrderIdList = tempList.ToImmutableList();
        }

        /// <summary>
        /// Gets the atomic order packets symbol.
        /// </summary>
        public Symbol Symbol { get; }

        /// <summary>
        /// Gets the atomic order packets identifier.
        /// </summary>
        public EntityId OrderPacketId => this.EntityId;

        /// <summary>
        /// Gets the atomic order packets trade type.
        /// </summary>
        public TradeType TradeType { get; }

        /// <summary>
        /// Gets the atomic order packets list of orders.
        /// </summary>
        public IImmutableList<AtomicOrder> Orders { get; }

        /// <summary>
        /// Gets the atomic order packets list of order identifiers.
        /// </summary>
        public IReadOnlyList<EntityId> OrderIdList { get; }

        /// <summary>
        /// Gets the atomic order packets timestamp.
        /// </summary>
        public ZonedDateTime Timestamp => this.EntityTimestamp;
    }
}