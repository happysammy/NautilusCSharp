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
    using Nautilus.DomainModel.Identifiers;
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
        /// <param name="label">The trade unit label.</param>
        /// <param name="entry">The entry order.</param>
        /// <param name="stopLoss">The stop-loss order.</param>
        /// <param name="profitTarget">The profit target order.</param>
        /// <param name="timestamp">The initialization timestamp.</param>
        public TradeUnit(
            TradeUnitId tradeUnitId,
            Label label,
            Order entry,
            Order stopLoss,
            Option<Order> profitTarget,
            ZonedDateTime timestamp)
            : base(tradeUnitId, timestamp)
        {
            Debug.NotNull(tradeUnitId, nameof(tradeUnitId));
            Debug.NotNull(label, nameof(label));
            Debug.NotNull(entry, nameof(entry));
            Debug.True(entry.Price.HasValue, nameof(entry.Price));
            Debug.True(stopLoss.Type == OrderType.STOP_MARKET, nameof(stopLoss.Type));
            Debug.NotNull(stopLoss, nameof(stopLoss));
            Debug.NotNull(profitTarget, nameof(profitTarget));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.Symbol = entry.Symbol;
            this.Label = label;
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

            this.OrderIds = new ReadOnlyList<OrderId>(this.orders.Select(o => o.Id).ToList());
        }

        /// <summary>
        /// Gets the trade units symbol.
        /// </summary>
        public Symbol Symbol { get; }

        /// <summary>
        /// Gets the trade units label.
        /// </summary>
        public Label Label { get; }

        /// <summary>
        /// Gets the trade units entry order.
        /// </summary>
        public Order Entry { get; }

        /// <summary>
        /// Gets the trade units profit target (optional).
        /// </summary>
        public Option<Order> ProfitTarget { get; }

        /// <summary>
        /// Gets the trade units stop-loss order.
        /// </summary>
        public Order StopLoss { get; }

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
        public ReadOnlyList<OrderId> OrderIds { get; }

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
        public bool IsOrderContained(OrderId orderId)
        {
            Debug.NotNull(orderId, nameof(orderId));

            return this.OrderIds.Contains(orderId);
        }

        /// <summary>
        /// Returns the <see cref="Order"/> matching the given order identifier (optional).
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns>A <see cref="Option{Order}" />.</returns>
        public Option<Order> GetOrderById(OrderId orderId)
        {
            Debug.NotNull(orderId, nameof(orderId));

            return this.orders.FirstOrDefault(o => o.Id == orderId);
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
               .FirstOrDefault(order => order.Id.Equals(orderEvent?.OrderId))
              ?.Apply(orderEvent)
               .OnSuccess(() => this.Events.Add(orderEvent));
        }

        /// <summary>
        /// Returns a string representation of the <see cref="TradeUnit"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{nameof(TradeUnit)}({this.Id})";

        private Position CreatePosition(Order entry, ZonedDateTime timestamp)
        {
            Debug.NotNull(entry, nameof(entry));

            return new Position(
                this.Symbol,
                entry.Id,
                new PositionId($"{this.Id}_{nameof(Position)}"),
                timestamp);
        }
    }
}
