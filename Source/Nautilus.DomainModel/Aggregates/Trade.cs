//--------------------------------------------------------------------------------------------------
// <copyright file="Trade.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Aggregates
{
    using System.Linq;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Collections;
    using Nautilus.Core.CQS;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Represents a financial market trade comprising of a collection of <see cref="TradeUnit"/>s
    /// to be managed together.
    /// </summary>
    [PerformanceOptimized]
    public sealed class Trade : Aggregate<Trade>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Trade" /> class.
        /// </summary>
        /// <param name="symbol">The trade symbol.</param>
        /// <param name="tradeId">The trade identifier.</param>
        /// <param name="tradeType">The trade type.</param>
        /// <param name="tradeUnits">The trade units.</param>
        /// <param name="orderIdList">The trade order identifier list.</param>
        /// <param name="timestamp">The trade timestamp.</param>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if any
        /// struct argument is the default value.</exception>
        public Trade(
            Symbol symbol,
            EntityId tradeId,
            TradeType tradeType,
            ReadOnlyList<TradeUnit> tradeUnits,
            ReadOnlyList<EntityId> orderIdList,
            ZonedDateTime timestamp)
            : base(tradeId, timestamp)
        {
            Debug.NotNull(symbol, nameof(symbol));
            Debug.NotNull(tradeId, nameof(symbol));
            Debug.NotNull(tradeType, nameof(tradeType));
            Debug.NotNull(tradeUnits, nameof(tradeUnits));
            Debug.NotNull(orderIdList, nameof(orderIdList));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.Symbol = symbol;
            this.TradeType = tradeType;
            this.TradeUnits = tradeUnits;
            this.OrderIdList = orderIdList;
            this.TotalQuantity = Quantity.Create(this.TradeUnits.Sum(unit => unit.Entry.Quantity.Value));
        }

        /// <summary>
        /// Gets the trades symbol.
        /// </summary>
        public Symbol Symbol { get; }

        /// <summary>
        /// Gets the trades identifier.
        /// </summary>
        public EntityId TradeId => this.Id;

        /// <summary>
        /// Gets the trades type.
        /// </summary>
        public TradeType TradeType { get; }

        /// <summary>
        /// Gets the trades units.
        /// </summary>
        public ReadOnlyList<TradeUnit> TradeUnits { get; }

        /// <summary>
        /// Gets the trades order identifier list.
        /// </summary>
        public ReadOnlyList<EntityId> OrderIdList { get; }

        /// <summary>
        /// Gets the trades total quantity.
        /// </summary>
        public Quantity TotalQuantity { get; }

        /// <summary>
        /// Gets the trades status.
        /// </summary>
        public TradeStatus TradeStatus => TradeLogic.CalculateTradeStatus(this.TradeUnits);

        /// <summary>
        /// Gets the trades market position.
        /// </summary>
        public MarketPosition MarketPosition => TradeLogic.CalculateMarketPosition(this.TradeUnits);

        /// <summary>
        /// Gets the trades timestamp.
        /// </summary>
        public ZonedDateTime TradeTimestamp => this.Timestamp;

        /// <summary>
        /// Returns an <see cref="Option{Order}"/> containing the <see cref="Order"/> (or no value
        /// if the order is not found).
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns>A <see cref="Option{Order}"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the given argument is null.</exception>
        public Option<Order> GetOrderById(EntityId orderId)
        {
            Debug.NotNull(orderId, nameof(orderId));

            return this.TradeUnits
               .Select(tradeUnits => tradeUnits.GetOrderById(orderId))
               .FirstOrDefault(order => order.HasValue);
        }

        /// <summary>
        /// Applies the given <see cref="Event"/> to this <see cref="Trade"/>.
        /// </summary>
        /// <param name="event">The event.</param>
        /// <returns>A <see cref="CommandResult"/> result.</returns>
        public override CommandResult Apply(Event @event) // TODO: refactor
        {
            Debug.NotNull(@event, nameof(@event));
            Debug.True(@event is OrderEvent, nameof(@event));

            if (!(@event is OrderEvent orderEvent))
            {
                return CommandResult.Fail($"The event is not of type {nameof(OrderEvent)}");
            }

            var tradeUnit = this.TradeUnits
               .FirstOrDefault(unit => unit.IsOrderContained(orderEvent.OrderId));

            return tradeUnit != null
                 ? tradeUnit.Apply(@event).OnSuccess(() => this.Events.Add(@event))
                 : CommandResult.Fail("Cannot find trade unit for order event");
        }
    }
}
