//--------------------------------------------------------------------------------------------------
// <copyright file="Trade.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Aggregates
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Nautilus.Core;
    using Nautilus.Core.CQS;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The sealed <see cref="Trade"/> class. Represents a financial market trade comprising of
    /// <see cref="TradeUnit"/>(s) to be managed together.
    /// </summary>
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
            IReadOnlyCollection<TradeUnit> tradeUnits,
            IReadOnlyCollection<EntityId> orderIdList,
            ZonedDateTime timestamp)
            : base(tradeId, timestamp)
        {
            Validate.NotNull(symbol, nameof(symbol));
            Validate.NotNull(tradeId, nameof(symbol));
            Validate.NotNull(tradeType, nameof(tradeType));
            Validate.NotNull(tradeUnits, nameof(tradeUnits));
            Validate.NotNull(orderIdList, nameof(orderIdList));
            Validate.NotDefault(timestamp, nameof(timestamp));

            this.Symbol = symbol;
            this.TradeType = tradeType;
            this.TradeUnits = tradeUnits.ToImmutableList();
            this.OrderIdList = orderIdList.ToImmutableList();
            this.TotalQuantity = Quantity.Create(this.TradeUnits.Sum(unit => unit.Entry.Quantity.Value));
        }

        /// <summary>
        /// Gets the trades symbol.
        /// </summary>
        public Symbol Symbol { get; }

        /// <summary>
        /// Gets the trades identifier.
        /// </summary>
        public EntityId TradeId => this.EntityId;

        /// <summary>
        /// Gets the trades type.
        /// </summary>
        public TradeType TradeType { get; }

        /// <summary>
        /// Gets the trades units.
        /// </summary>
        public IReadOnlyList<TradeUnit> TradeUnits { get; }

        /// <summary>
        /// Gets the trades order identifier list.
        /// </summary>
        public IReadOnlyList<EntityId> OrderIdList { get; }

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
        public ZonedDateTime TradeTimestamp => this.EntityTimestamp;

        /// <summary>
        /// Returns an <see cref="Option{Order}"/> containing the <see cref="Order"/> (or no value
        /// if the order is not found).
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns>A <see cref="Option{Order}"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the given argument is null.</exception>
        public Option<Order> GetOrderById(EntityId orderId)
        {
            Validate.NotNull(orderId, nameof(orderId));

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
            Validate.NotNull(@event, nameof(@event));
            Validate.True(@event is OrderEvent, nameof(@event));

            var orderEvent = @event as OrderEvent;

            if (orderEvent is null)
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
