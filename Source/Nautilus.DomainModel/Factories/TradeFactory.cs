// -------------------------------------------------------------------------------------------------
// <copyright file="TradeFactory.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Factories
{
    using System.Collections.Generic;
    using NautechSystems.CSharp.Annotations;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;

    /// <summary>
    /// A factory which creates valid <see cref="Trade"/>(s) for the system.
    /// </summary>
    [Immutable]
    public static class TradeFactory
    {
        /// <summary>
        /// Creates and returns a new <see cref="Trade"/> from the given <see cref="AtomicOrderPacket"/>.
        /// </summary>
        /// <param name="orderPacket">The order packet.</param>
        /// <returns>A <see cref="Trade"/>.</returns>
        public static Trade Create(AtomicOrderPacket orderPacket)
        {
            var tradeUnits = CreateTradeUnits(orderPacket);

            return new Trade(
                orderPacket.Symbol,
                orderPacket.OrderPacketId,
                orderPacket.TradeType,
                tradeUnits,
                orderPacket.OrderIdList,
                orderPacket.Timestamp);
        }

        private static List<TradeUnit> CreateTradeUnits(AtomicOrderPacket orderPacket)
        {
            var tradeId = orderPacket.OrderPacketId;
            var tradeUnits = new List<TradeUnit>();

            foreach (var atomicOrder in orderPacket.Orders)
            {
                tradeUnits.Add(new TradeUnit(
                    EntityIdFactory.TradeUnit(tradeId, tradeUnits.Count),
                    LabelFactory.TradeUnit(tradeUnits.Count),
                    atomicOrder.EntryOrder,
                    atomicOrder.StopLossOrder,
                    atomicOrder.ProfitTargetOrder,
                    orderPacket.Timestamp));
            }

            return tradeUnits;
        }
    }
}