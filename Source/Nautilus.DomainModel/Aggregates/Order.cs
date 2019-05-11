//--------------------------------------------------------------------------------------------------
// <copyright file="Order.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Aggregates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel.Aggregates.Base;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.FiniteStateMachine;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Represents a financial market order.
    /// </summary>
    [PerformanceOptimized]
    public sealed class Order : Aggregate<Order>
    {
        private readonly FiniteStateMachine orderState = OrderStateMachine.Create();
        private readonly List<OrderId> orderIds = new List<OrderId>();
        private readonly List<OrderId> orderIdsBroker = new List<OrderId>();
        private readonly List<ExecutionId> executionIds = new List<ExecutionId>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Order" /> class.
        /// </summary>
        /// <param name="symbol">The order symbol.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="orderLabel">The order label.</param>
        /// <param name="orderSide">The order side.</param>
        /// <param name="orderType">The order type.</param>
        /// <param name="quantity">The order quantity.</param>
        /// <param name="price">The order price (optional).</param>
        /// <param name="timeInForce">The order time in force.</param>
        /// <param name="expireTime">The order expire time (optional).</param>
        /// <param name="timestamp">The order timestamp.</param>
        public Order(
            Symbol symbol,
            OrderId orderId,
            Label orderLabel,
            OrderSide orderSide,
            OrderType orderType,
            Quantity quantity,
            OptionRef<Price> price,
            TimeInForce timeInForce,
            OptionVal<ZonedDateTime> expireTime,
            ZonedDateTime timestamp)
            : base(orderId, timestamp)
        {
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.Symbol = symbol;
            this.Label = orderLabel;
            this.Side = orderSide;
            this.Type = orderType;
            this.Quantity = quantity;
            this.FilledQuantity = Quantity.Zero();
            this.Price = price;
            this.AveragePrice = OptionRef<Price>.None();
            this.Slippage = OptionVal<decimal>.None();
            this.TimeInForce = timeInForce;
            this.ExpireTime = expireTime;
            this.orderIds.Add(this.Id);

            this.ValidateExpireTime(expireTime);
        }

        /// <summary>
        /// Gets the orders identifier.
        /// </summary>
        public new OrderId Id => (OrderId)base.Id;

        /// <summary>
        /// Gets the orders symbol.
        /// </summary>
        public Symbol Symbol { get; }

        /// <summary>
        /// Gets the orders identifier count.
        /// </summary>
        public int IdCount => this.orderIds.Count;

        /// <summary>
        /// Gets the orders current identifier.
        /// </summary>
        public OrderId IdCurrent => this.orderIds[this.orderIds.Count - 1];

        /// <summary>
        /// Gets the orders current identifier for the broker.
        /// </summary>
        public OptionRef<OrderId> IdBroker => this.orderIdsBroker.Count > 0
            ? this.orderIdsBroker.Last()
            : OptionRef<OrderId>.None();

        /// <summary>
        /// Gets the orders current execution identifier.
        /// </summary>
        public OptionRef<ExecutionId> ExecutionId => this.executionIds.Count > 0
            ? this.executionIds.Last()
            : OptionRef<ExecutionId>.None();

        /// <summary>
        /// Gets the orders label.
        /// </summary>
        public Label Label { get; }

        /// <summary>
        /// Gets the orders type.
        /// </summary>
        public OrderType Type { get; }

        /// <summary>
        /// Gets the orders side.
        /// </summary>
        public OrderSide Side { get; }

        /// <summary>
        /// Gets the orders quantity.
        /// </summary>
        public Quantity Quantity { get; }

        /// <summary>
        /// Gets the orders filled quantity.
        /// </summary>
        public Quantity FilledQuantity { get; private set; }

        /// <summary>
        /// Gets the orders price.
        /// </summary>
        public OptionRef<Price> Price { get; private set; }

        /// <summary>
        /// Gets the orders average fill price (optional, may be unfilled).
        /// </summary>
        public OptionRef<Price> AveragePrice { get; private set; }

        /// <summary>
        /// Gets the orders slippage.
        /// </summary>
        public OptionVal<decimal> Slippage { get; private set; }

        /// <summary>
        /// Gets the orders time in force.
        /// </summary>
        public TimeInForce TimeInForce { get; }

        /// <summary>
        /// Gets the orders expire time (optional).
        /// </summary>
        public OptionVal<ZonedDateTime> ExpireTime { get; }

        /// <summary>
        /// Gets the orders last event time.
        /// </summary>
        public ZonedDateTime LastEventTime => this.Events.Count > 0
            ? this.Events.Last().Timestamp
            : this.Timestamp;

        /// <summary>
        /// Gets the current order status.
        /// </summary>
        public OrderStatus Status => (OrderStatus)this.orderState.State.Value;

        /// <summary>
        /// Gets a value indicating whether the order status is complete.
        /// </summary>
        public bool IsComplete { get; private set; }

        /// <summary>
        /// Adds the modified order identifier to the order.
        /// </summary>
        /// <param name="modifiedOrderId">The modified order identifier.</param>
        public void AddModifiedOrderId(OrderId modifiedOrderId)
        {
            this.orderIds.Add(modifiedOrderId);
        }

        /// <summary>
        /// Returns a read-only list of the orders.
        /// </summary>
        /// <returns>A read only collection.</returns>
        public IEnumerable<OrderId> GetOrderIdList() => this.orderIds;

        /// <summary>
        /// Returns a read-only list of broker order identifiers.
        /// </summary>
        /// <returns>A read only collection.</returns>
        public IEnumerable<OrderId> GetBrokerOrderIdList() => this.orderIdsBroker;

        /// <summary>
        /// Returns a read-only list of execution identifiers.
        /// </summary>
        /// <returns>A read only collection.</returns>
        public IEnumerable<ExecutionId> GetExecutionIdList() => this.executionIds;

        /// <summary>
        /// Returns an immutable collection of the order events.
        /// </summary>
        /// <returns>A read only collection.</returns>
        public IEnumerable<Event> GetEvents() => this.Events;

        /// <summary>
        /// Applies the given <see cref="Event"/> to the <see cref="Order"/>.
        /// </summary>
        /// <param name="orderEvent">The order event.</param>
        public override void Apply(Event orderEvent)
        {
            switch (orderEvent)
            {
                case OrderRejected @event:
                    this.When(@event);
                    break;
                case OrderCancelled @event:
                    this.When(@event);
                    break;
                case OrderWorking @event:
                    this.When(@event);
                    break;
                case OrderPartiallyFilled @event:
                    this.When(@event);
                    break;
                case OrderFilled @event:
                    this.When(@event);
                    break;
                case OrderExpired @event:
                    this.When(@event);
                    break;
                case OrderModified @event:
                    this.When(@event);
                    break;
                default: throw new InvalidOperationException($"The {orderEvent} is not recognized by the order {this}");
            }

            this.Events.Add(orderEvent);
        }

        /// <summary>
        /// Returns a string representation of the <see cref="Order"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{nameof(Order)}-{this.Symbol}-{this.Id}";

        /// <summary>
        /// Updates the broker order identifier list with the given <see cref="OrderId"/>
        /// (if not already present).
        /// </summary>
        /// <param name="orderId">The broker order identifier.</param>
        private void UpdateBrokerOrderIds(OrderId orderId)
        {
            if (!this.orderIdsBroker.Contains(orderId))
            {
                this.orderIdsBroker.Add(orderId);
            }
        }

        private void When(OrderRejected orderEvent)
        {
            this.orderState.Process(new Trigger(nameof(OrderRejected)));
            this.IsComplete = true;
        }

        private void When(OrderCancelled orderEvent)
        {
            this.orderState.Process(new Trigger(nameof(OrderCancelled)));
            this.IsComplete = true;
        }

        private void When(OrderWorking orderEvent)
        {
            this.orderState.Process(new Trigger(nameof(OrderWorking)));
            this.UpdateBrokerOrderIds(orderEvent.OrderIdBroker);
        }

        private void When(OrderPartiallyFilled orderEvent)
        {
            this.orderState.Process(new Trigger(nameof(OrderPartiallyFilled)));
            this.executionIds.Add(orderEvent.ExecutionId);
            this.FilledQuantity = orderEvent.FilledQuantity;
            this.AveragePrice = orderEvent.AveragePrice;
            this.Slippage = OptionVal<decimal>.Some(this.CalculateSlippage());
        }

        private void When(OrderFilled orderEvent)
        {
            this.orderState.Process(new Trigger(nameof(OrderFilled)));
            this.executionIds.Add(orderEvent.ExecutionId);
            this.FilledQuantity = orderEvent.FilledQuantity;
            this.AveragePrice = orderEvent.AveragePrice;
            this.Slippage = OptionVal<decimal>.Some(this.CalculateSlippage());
            this.IsComplete = true;
        }

        private void When(OrderExpired orderEvent)
        {
            this.orderState.Process(new Trigger(nameof(OrderExpired)));
            this.IsComplete = true;
        }

        private void When(OrderModified orderEvent)
        {
            this.orderState.Process(new Trigger(nameof(OrderModified)));
            this.UpdateBrokerOrderIds(orderEvent.BrokerOrderId);
            this.Price = orderEvent.ModifiedPrice;
        }

        private void ValidateExpireTime(OptionVal<ZonedDateTime> expireTime)
        {
            if (expireTime.HasNoValue)
            {
                Precondition.True(this.TimeInForce != TimeInForce.GTD, nameof(this.TimeInForce));
            }
            else
            {
                var expireTimeValue = expireTime.Value;
                Precondition.True(this.TimeInForce == TimeInForce.GTD, nameof(this.TimeInForce));
                Precondition.True(expireTimeValue.IsGreaterThan(this.Timestamp), nameof(expireTime));
            }
        }

        private decimal CalculateSlippage()
        {
            if (this.Price.HasNoValue || this.AveragePrice.HasNoValue)
            {
                return decimal.Zero;
            }

            return this.Side == OrderSide.BUY
                ? this.AveragePrice.Value - this.Price.Value
                : this.Price.Value - this.AveragePrice.Value;
        }

        private static class OrderStateMachine
        {
            internal static FiniteStateMachine Create()
            {
                var stateTransitionTable = new Dictionary<StateTransition, State>
                {
                    { new StateTransition(new State(OrderStatus.Initialized), new Trigger(nameof(OrderSubmitted))), new State(OrderStatus.Submitted) },
                    { new StateTransition(new State(OrderStatus.Initialized), new Trigger(nameof(OrderRejected))), new State(OrderStatus.Rejected) },
                    { new StateTransition(new State(OrderStatus.Initialized), new Trigger(nameof(OrderCancelled))), new State(OrderStatus.Cancelled) },
                    { new StateTransition(new State(OrderStatus.Initialized), new Trigger(nameof(OrderWorking))), new State(OrderStatus.Working) },
                    { new StateTransition(new State(OrderStatus.Working), new Trigger(nameof(OrderCancelled))), new State(OrderStatus.Cancelled) },
                    { new StateTransition(new State(OrderStatus.Working), new Trigger(nameof(OrderModified))), new State(OrderStatus.Working) },
                    { new StateTransition(new State(OrderStatus.Working), new Trigger(nameof(OrderExpired))), new State(OrderStatus.Expired) },
                    { new StateTransition(new State(OrderStatus.Working), new Trigger(nameof(OrderFilled))), new State(OrderStatus.Filled) },
                    { new StateTransition(new State(OrderStatus.Working), new Trigger(nameof(OrderPartiallyFilled))), new State(OrderStatus.PartiallyFilled) },
                    { new StateTransition(new State(OrderStatus.PartiallyFilled), new Trigger(nameof(OrderPartiallyFilled))), new State(OrderStatus.PartiallyFilled) },
                    { new StateTransition(new State(OrderStatus.PartiallyFilled), new Trigger(nameof(OrderFilled))), new State(OrderStatus.Filled) },
                };

                return new FiniteStateMachine(stateTransitionTable, new State(OrderStatus.Initialized));
            }
        }
    }
}
