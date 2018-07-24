//--------------------------------------------------------------------------------------------------
// <copyright file="TradeUnit.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Aggregates
{
    using System.Collections.Generic;
    using System.Linq;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Collections;
    using Nautilus.Core.CQS;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Orders;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Represents a trade unit as part of a larger trade aggregate.
    /// </summary>
    [PerformanceOptimized]
    public sealed class TradeUnit : Aggregate<TradeUnit>
    {
        private readonly ReadOnlyList<Order> orders;

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeUnit" /> class.
        /// </summary>
        /// <param name="tradeUnitId">The trade unit identifier.</param>
        /// <param name="tradeUnitLabel">The trade unit label.</param>
        /// <param name="entry">The entry order.</param>
        /// <param name="stopLoss">The stop-loss order.</param>
        /// <param name="profitTarget">The profit target order.</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if any
        /// struct argument is the default value.</exception>
        public TradeUnit(
            EntityId tradeUnitId,
            Label tradeUnitLabel,
            PricedOrder entry,
            StopMarketOrder stopLoss,
            Option<PricedOrder> profitTarget,
            ZonedDateTime timestamp)
            : base(tradeUnitId, timestamp)
        {
            Debug.NotNull(tradeUnitId, nameof(tradeUnitId));
            Debug.NotNull(tradeUnitLabel, nameof(tradeUnitLabel));
            Debug.NotNull(entry, nameof(entry));
            Debug.NotNull(stopLoss, nameof(stopLoss));
            Debug.NotNull(profitTarget, nameof(profitTarget));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.Symbol = entry.Symbol;
            this.TradeUnitLabel = tradeUnitLabel;
            this.Entry = entry;
            this.StopLoss = stopLoss;
            this.ProfitTarget = profitTarget;
            this.Position = this.CreatePosition(entry, timestamp);
            this.UnitSize = entry.Quantity;

            var orderList = new List<Order> {this.Entry, this.StopLoss};
            if (this.ProfitTarget.HasValue)
            {
                orderList.Add(this.ProfitTarget.Value);
            }
            this.orders = new ReadOnlyList<Order>(orderList);

            this.OrderIds = new ReadOnlyList<EntityId>(
                this.orders.Select(o => o.OrderId)
                    .ToList());
        }

        /// <summary>
        /// Gets the trade units identifier.
        /// </summary>
        public EntityId TradeUnitId => this.Id;

        /// <summary>
        /// Gets the trade units symbol.
        /// </summary>
        public Symbol Symbol { get; }

        /// <summary>
        /// Gets the trade units label.
        /// </summary>
        public Label TradeUnitLabel { get; }

        /// <summary>
        /// Gets the trade units entry order.
        /// </summary>
        public PricedOrder Entry { get; }

        /// <summary>
        /// Gets the trade units profit target (optional).
        /// </summary>
        public Option<PricedOrder> ProfitTarget { get; }

        /// <summary>
        /// Gets the trade units stop-loss order.
        /// </summary>
        public StopMarketOrder StopLoss { get; }

        /// <summary>
        /// Gets the trade units position.
        /// </summary>
        public Position Position { get; }

        /// <summary>
        /// Gets the trade units size.
        /// </summary>
        public Quantity UnitSize { get; }

        /// <summary>
        /// Gets the trade units order identifiers.
        /// </summary>
        /// <returns>A read only collection of orders.</returns>
        public ReadOnlyList<EntityId> OrderIds { get; }

        /// <summary>
        /// Returns the trade units trade status.
        /// </summary>
        public TradeStatus TradeStatus => TradeLogic.CalculateTradeStatus(this);

        /// <summary>
        /// Returns all of the trade units orders.
        /// </summary>
        /// <returns>A read only collection of orders.</returns>
        public ReadOnlyList<Order> GetAllOrders() => this.orders;

        /// <summary>
        /// Returns a value indicating whether the trade unit contains the given order identifier.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public bool IsOrderContained(EntityId orderId)
        {
            Debug.NotNull(orderId, nameof(orderId));

            return this.OrderIds.Contains(orderId);
        }

        /// <summary>
        /// Returns the <see cref="Order"/> matching the given order identifier (optional).
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns>A <see cref="Option{Order}" />.</returns>
        public Option<Order> GetOrderById(EntityId orderId)
        {
            Debug.NotNull(orderId, nameof(orderId));

            return this.orders.FirstOrDefault(o => o.OrderId == orderId);
        }

        /// <summary>
        /// Applies the given <see cref="Event"/> to this <see cref="TradeUnit"/>.
        /// </summary>
        /// <param name="event">The order event.</param>
        /// <returns>A <see cref="CommandResult"/> result.</returns>
        public override CommandResult Apply(Event @event)
        {
            Debug.NotNull(@event, nameof(@event));
            Debug.True(@event is OrderEvent, nameof(@event));

            var orderEvent = @event as OrderEvent;

            if (orderEvent is OrderFilled || orderEvent is OrderPartiallyFilled)
            {
                this.Position.Apply(@event);
            }

            if (orderEvent is OrderRejected)
            {
                this.StopLoss.Apply(orderEvent);

                if (this.ProfitTarget.HasValue)
                {
                    this.ProfitTarget.Value.Apply(orderEvent);
                }
            }

            return this.orders
               .FirstOrDefault(order => order.OrderId.Equals(orderEvent?.OrderId))
              ?.Apply(orderEvent)
               .OnSuccess(() => this.Events.Add(orderEvent));
        }

        /// <summary>
        /// Returns a string representation of the <see cref="TradeUnit"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{nameof(TradeUnit)}({this.TradeUnitId})";

        private Position CreatePosition(Order entry, ZonedDateTime timestamp)
        {
            Debug.NotNull(entry, nameof(entry));

            return new Position(
                this.Symbol,
                entry.OrderId,
                new EntityId($"{this.TradeUnitId}_{nameof(Position)}"),
                timestamp);
        }
    }
}
