//--------------------------------------------------------------
// <copyright file="TradeUnit.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.DomainModel.Aggregates
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using NautechSystems.CSharp;
    using NautechSystems.CSharp.CQS;
    using NautechSystems.CSharp.Extensions;
    using NautechSystems.CSharp.Validation;
    using Nautilus.Core;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Orders;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The sealed <see cref="TradeUnit"/> class. Represents a trade unit as part of a larger trade
    /// aggregate.
    /// </summary>
    public sealed class TradeUnit : Aggregate<TradeUnit>
    {
        private readonly IList<Order> orders = new List<Order>();

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
            StopOrder entry,
            StopMarketOrder stopLoss,
            Option<StopOrder> profitTarget,
            ZonedDateTime timestamp)
            : base(tradeUnitId, timestamp)
        {
            Validate.NotNull(tradeUnitId, nameof(tradeUnitId));
            Validate.NotNull(tradeUnitLabel, nameof(tradeUnitLabel));
            Validate.NotNull(entry, nameof(entry));
            Validate.NotNull(stopLoss, nameof(stopLoss));
            Validate.NotNull(profitTarget, nameof(profitTarget));
            Validate.NotDefault(timestamp, nameof(timestamp));

            this.Symbol = entry.Symbol;
            this.TradeUnitLabel = tradeUnitLabel;
            this.Entry = entry;
            this.StopLoss = stopLoss;
            this.ProfitTarget = profitTarget;
            this.Position = this.CreatePosition(entry, timestamp);
            this.UnitSize = entry.Quantity;

            this.orders.Add(this.Entry);
            this.orders.Add(this.StopLoss);

            if (this.ProfitTarget.HasValue)
            {
                this.orders.Add(this.ProfitTarget.Value);
            }

            this.orders.ToImmutableList();

            this.OrderIds = this.orders.Select(o => o.OrderId).ToImmutableList();
        }

        /// <summary>
        /// Gets the trade units identifier.
        /// </summary>
        public EntityId TradeUnitId => this.EntityId;

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
        public StopOrder Entry { get; }

        /// <summary>
        /// Gets the trade units profit target (optional).
        /// </summary>
        public Option<StopOrder> ProfitTarget { get; }

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
        public IReadOnlyList<EntityId> OrderIds { get; }

        /// <summary>
        /// Returns the trade units trade status.
        /// </summary>
        public TradeStatus TradeStatus => TradeLogic.CalculateTradeStatus(this);

        /// <summary>
        /// Returns all of the trade units orders.
        /// </summary>
        /// <returns>A read only collection of orders.</returns>
        public IReadOnlyCollection<Order> GetAllOrders() => this.orders.ToList();

        /// <summary>
        /// Returns a value indicating whether the trade unit contains the given order identifier.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public bool IsOrderContained(EntityId orderId)
        {
            Validate.NotNull(orderId, nameof(orderId));

            return this.OrderIds.Contains(orderId);
        }

        /// <summary>
        /// Returns the <see cref="Order"/> matching the given order identifier (optional).
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns>A <see cref="Option{Order}" />.</returns>
        public Option<Order> GetOrderById(EntityId orderId)
        {
            Validate.NotNull(orderId, nameof(orderId));

            return this.orders.FirstOrDefault(o => o.OrderId == orderId);
        }

        /// <summary>
        /// Applies the given <see cref="Event"/> to this <see cref="TradeUnit"/>.
        /// </summary>
        /// <param name="event">The order event.</param>
        /// <returns>A <see cref="CommandResult"/> result.</returns>
        public override CommandResult Apply(Event @event)
        {
            Validate.NotNull(@event, nameof(@event));
            Validate.True(@event is OrderEvent, nameof(@event));

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
               .Apply(orderEvent)
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