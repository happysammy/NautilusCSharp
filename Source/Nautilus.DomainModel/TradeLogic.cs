//--------------------------------------------------------------
// <copyright file="TradeLogic.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.DomainModel
{
    using System.Collections.Generic;
    using System.Linq;
    using NautechSystems.CSharp;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Orders;

    /// <summary>
    /// The immutable static <see cref="TradeLogic"/> class. Represents generic trade logic as a
    /// finite state machine.
    /// </summary>
    [Immutable]
    public static class TradeLogic
    {
        /// <summary>
        /// Returns the <see cref="TradeStatus"/> of the given <see cref="TradeUnit"/>.
        /// </summary>
        /// <param name="tradeUnit">The trade unit.</param>
        /// <returns>A <see cref="TradeStatus"/>.</returns>
        /// <exception cref="ValidationException">Throws if the argument is null.</exception>
        public static TradeStatus CalculateTradeStatus(TradeUnit tradeUnit)
        {
            Validate.NotNull(tradeUnit, nameof(tradeUnit));

            if (tradeUnit.Position.MarketPosition == MarketPosition.Flat
             && tradeUnit.Entry.OrderStatus == OrderStatus.Initialized)
            {
                return TradeStatus.Initialized;
            }

            if (tradeUnit.Position.MarketPosition == MarketPosition.Flat
             && tradeUnit.Entry.OrderStatus == OrderStatus.Working)
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
        /// <returns>The <see cref="TradeStatus" />.</returns>
        /// <exception cref="ValidationException">Throws if the argument is null.</exception>
        public static TradeStatus CalculateTradeStatus(IReadOnlyCollection<TradeUnit> tradeUnits)
        {
            Validate.NotNull(tradeUnits, nameof(tradeUnits));

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
        /// <returns>A <see cref="MarketPosition" />.</returns>
        /// <exception cref="ValidationException">Throws if the argument is null.</exception>
        public static MarketPosition CalculateMarketPosition(IReadOnlyCollection<TradeUnit> tradeUnits)
        {
            Validate.NotNull(tradeUnits, nameof(tradeUnits));

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

        private static bool ProfitTargetHasValueAndIsComplete(Option<StopOrder> order)
        {
            return order.HasValue && order.Value.IsComplete;
        }
    }
}