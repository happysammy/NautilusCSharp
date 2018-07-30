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
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Collections;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Represents a collection of <see cref="AtomicOrder"/>s to be managed together.
    /// </summary>
    [Immutable]
    [PerformanceOptimized]
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
            OrderPacketId orderPacketId,
            ZonedDateTime timestamp)
            : base(orderPacketId, timestamp)
        {
            Debug.NotNull(symbol, nameof(symbol));
            Debug.NotNull(tradeType, nameof(tradeType));
            Debug.NotNull(orders, nameof(orders));
            Debug.NotNull(orderPacketId, nameof(orderPacketId));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.Symbol = symbol;
            this.TradeType = tradeType;
            this.Orders = new ReadOnlyList<AtomicOrder>(orders);

            var tempList = new List<OrderId>();

            foreach (var order in this.Orders)
            {
                tempList.Add(order.Entry.Id);
                tempList.Add(order.StopLoss.Id);

                if (order.ProfitTarget.HasValue)
                {
                    tempList.Add(order.ProfitTarget.Value.Id);
                }
            }

            this.OrderIdList = new ReadOnlyList<OrderId>(tempList);
        }

        /// <summary>
        /// Gets the atomic order packets symbol.
        /// </summary>
        public Symbol Symbol { get; }

        /// <summary>
        /// Gets the atomic order packets trade type.
        /// </summary>
        public TradeType TradeType { get; }

        /// <summary>
        /// Gets the atomic order packets list of orders.
        /// </summary>
        public ReadOnlyList<AtomicOrder> Orders { get; }

        /// <summary>
        /// Gets the atomic order packets list of order identifiers.
        /// </summary>
        public ReadOnlyList<OrderId> OrderIdList { get; }
    }
}
