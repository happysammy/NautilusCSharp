//--------------------------------------------------------------------------------------------------
// <copyright file="TradeLogic.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel
{
    using System.Linq;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Collections;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Enums;

    /// <summary>
    /// Provides generic trade logic.
    /// </summary>
    [Immutable]
    public static class TradeLogic
    {
        /// <summary>
        /// Returns the <see cref="TradeStatus"/> of the given <see cref="TradeUnit"/>.
        /// </summary>
        /// <param name="tradeUnit">The trade unit.</param>
        /// <returns>The current trade status.</returns>
        public static TradeStatus CalculateTradeStatus(TradeUnit tradeUnit)
        {
            Debug.NotNull(tradeUnit, nameof(tradeUnit));

            if (tradeUnit.Position.MarketPosition == MarketPosition.Flat
             && tradeUnit.Entry.Status == OrderStatus.Initialized)
            {
                return TradeStatus.Initialized;
            }

            if (tradeUnit.Position.MarketPosition == MarketPosition.Flat
             && tradeUnit.Entry.Status == OrderStatus.Working)
            {
                return TradeStatus.Pending;
            }

            if (tradeUnit.Position.MarketPosition != MarketPosition.Flat
            || !tradeUnit.Entry.IsComplete
            || !tradeUnit.StopLoss.IsComplete
            || (tradeUnit.ProfitTarget.HasValue && !ProfitTargetHasValueAndIsComplete(tradeUnit.ProfitTarget)))
            {
                return TradeStatus.Active;
            }

            if (tradeUnit.Position.MarketPosition == MarketPosition.Flat
             && tradeUnit.Entry.IsComplete
             && tradeUnit.StopLoss.IsComplete
             && (tradeUnit.ProfitTarget.HasNoValue || ProfitTargetHasValueAndIsComplete(tradeUnit.ProfitTarget)))
            {
                return TradeStatus.Completed;
            }

            return TradeStatus.Unknown;
        }

        /// <summary>
        /// Returns the <see cref="TradeStatus"/> of the given collection of <see cref="TradeUnit"/>(s).
        /// </summary>
        /// <param name="tradeUnits">The trade units.</param>
        /// <returns>The current trade status.</returns>
        public static TradeStatus CalculateTradeStatus(ReadOnlyList<TradeUnit> tradeUnits)
        {
            Debug.NotNull(tradeUnits, nameof(tradeUnits));

            if (tradeUnits.Any(tradeUnit => tradeUnit.TradeStatus == TradeStatus.Pending)
             && tradeUnits.All(tradeUnit => tradeUnit.Position.MarketPosition == MarketPosition.Flat))
            {
                return TradeStatus.Pending;
            }

            if (tradeUnits.Any(tradeUnit => tradeUnit.TradeStatus == TradeStatus.Active))
            {
                return TradeStatus.Active;
            }

            if (tradeUnits.All(tradeUnit => tradeUnit.TradeStatus == TradeStatus.Completed))
            {
                return TradeStatus.Completed;
            }

            return TradeStatus.Initialized;
        }

        /// <summary>
        /// Returns the <see cref="MarketPosition"/> of the given collection of <see cref="TradeUnit"/>(s).
        /// </summary>
        /// <param name="tradeUnits">The trade units.</param>
        /// <returns>The current market condition.</returns>
        public static MarketPosition CalculateMarketPosition(ReadOnlyList<TradeUnit> tradeUnits)
        {
            Debug.NotNull(tradeUnits, nameof(tradeUnits));

            if (tradeUnits.All(tradeUnit => tradeUnit.Position.MarketPosition == MarketPosition.Flat))
            {
                return MarketPosition.Flat;
            }

            if (tradeUnits.Any(tradeUnit => tradeUnit.Position.MarketPosition == MarketPosition.Long)
             && tradeUnits.All(tradeUnit => tradeUnit.Position.MarketPosition != MarketPosition.Short))
            {
                return MarketPosition.Long;
            }

            if (tradeUnits.Any(tradeUnit => tradeUnit.Position.MarketPosition == MarketPosition.Short)
             && tradeUnits.All(tradeUnit => tradeUnit.Position.MarketPosition != MarketPosition.Long))
            {
                return MarketPosition.Short;
            }

            return MarketPosition.Unknown;
        }

        private static bool ProfitTargetHasValueAndIsComplete(Option<Order> order)
        {
            return order.HasValue && order.Value.IsComplete;
        }
    }
}
