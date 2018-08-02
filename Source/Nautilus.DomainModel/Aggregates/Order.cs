//--------------------------------------------------------------------------------------------------
// <copyright file="Order.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.Core.Model;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.FiniteStateMachine;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.Interfaces;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The base class for all order types.
    /// </summary>
    [PerformanceOptimized]
    public sealed class Order : Aggregate<Order>, IOrder
    {
        private readonly FiniteStateMachine orderState = OrderStateMachine.Create();

        // Concrete lists for performance reasons.
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
            Option<Price> price,
            TimeInForce timeInForce,
            Option<ZonedDateTime?> expireTime,
            ZonedDateTime timestamp)
            : base(orderId, timestamp)
        {
            Debug.NotNull(symbol, nameof(symbol));
            Debug.NotNull(orderId, nameof(orderId));
            Debug.NotNull(orderLabel, nameof(orderLabel));
            Debug.NotNull(quantity, nameof(quantity));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.Symbol = symbol;
            this.Label = orderLabel;
            this.Side = orderSide;
            this.Type = orderType;
            this.Quantity = quantity;
            this.FilledQuantity = Quantity.Zero();
            this.Price = price;
            this.TimeInForce = timeInForce;
            this.ExpireTime = expireTime;
            this.orderIds.Add(this.Id);

            this.ValidateExpireTime(expireTime);
        }

        /// <summary>
        /// Gets the orders identifier.
        /// </summary>
        public new OrderId Id => base.Id as OrderId;

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
        public OrderId IdCurrent => this.orderIds.Last();

        /// <summary>
        /// Gets the orders current identifier for the broker.
        /// </summary>
        public Option<OrderId> IdBroker => this.orderIdsBroker.Count > 0
            ? this.orderIdsBroker.Last()
            : Option<OrderId>.None();

        /// <summary>
        /// Gets the orders current execution identifier.
        /// </summary>
        public Option<ExecutionId> ExecutionId => this.executionIds.Count > 0
            ? this.executionIds.Last()
            : Option<ExecutionId>.None();

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
        public Option<Price> Price { get; private set; }

        /// <summary>
        /// Gets the orders average fill price (optional, may be unfilled).
        /// </summary>
        public Option<Price> AveragePrice { get; private set; }

        /// <summary>
        /// Gets the orders slippage.
        /// </summary>
        public decimal Slippage => this.CalculateSlippage();

        /// <summary>
        /// Gets the orders time in force.
        /// </summary>
        public TimeInForce TimeInForce { get; }

        /// <summary>
        /// Gets the orders expire time (optional).
        /// </summary>
        public Option<ZonedDateTime?> ExpireTime { get; }

        /// <summary>
        /// Gets the orders last event time.
        /// </summary>
        public ZonedDateTime LastEventTime => this.Events.Count > 0
            ? this.Events[this.Events.LastIndex()].Timestamp
            : this.Timestamp;

        /// <summary>
        /// Gets the current order status.
        /// </summary>
        public OrderStatus Status => (OrderStatus)this.orderState.CurrentState.Value;

        /// <summary>
        /// Gets a value indicating whether the order status is complete.
        /// </summary>
        public bool IsComplete => this.IsCompleteResult();

        /// <summary>
        /// Adds the modified order identifier to the order.
        /// </summary>
        /// <param name="modifiedOrderId">The modified order identifier.</param>
        /// <exception cref="ValidationException">Throws if the argument is null.</exception>
        public void AddModifiedOrderId(OrderId modifiedOrderId)
        {
            Debug.NotNull(modifiedOrderId, nameof(modifiedOrderId));

            this.orderIds.Add(modifiedOrderId);
        }

        /// <summary>
        /// Returns a read-only list of the orders.
        /// </summary>
        /// <returns>A read only collection.</returns>
        public ReadOnlyList<OrderId> GetOrderIdList() => new ReadOnlyList<OrderId>(this.orderIds);

        /// <summary>
        /// Returns a read-only list of broker order identifiers.
        /// </summary>
        /// <returns>A read only collection.</returns>
        public ReadOnlyList<OrderId> GetBrokerOrderIdList() => new ReadOnlyList<OrderId>(this.orderIdsBroker);

        /// <summary>
        /// Returns a read-only list of execution identifiers.
        /// </summary>
        /// <returns>A read only collection.</returns>
        public ReadOnlyList<ExecutionId> GetExecutionIdList() => new ReadOnlyList<ExecutionId>(this.executionIds);

        /// <summary>
        /// Returns an immutable collection of the order events.
        /// </summary>
        /// <returns>A read only collection.</returns>
        public ReadOnlyList<Event> GetEvents() => new ReadOnlyList<Event>(this.Events);

        /// <summary>
        /// Applies the given <see cref="Event"/> to the <see cref="Order"/>.
        /// </summary>
        /// <param name="orderEvent">The order event.</param>
        /// <returns>The result of the operation.</returns>
        public override CommandResult Apply(Event orderEvent)
        {
            Debug.NotNull(orderEvent, nameof(orderEvent));

            switch (orderEvent)
            {
                case OrderRejected @event:
                    return this.When(@event);

                case OrderCancelled @event:
                    return this.When(@event);

                case OrderWorking @event:
                    return this.When(@event);

                case OrderPartiallyFilled @event:
                    return this.When(@event);

                case OrderFilled @event:
                    return this.When(@event);

                case OrderExpired @event:
                    return this.When(@event);

                case OrderModified @event:
                    return this.When(@event);

                default: return CommandResult.Fail($"The event is not recognized by the order {this}");
            }
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
            Debug.NotNull(orderId, nameof(orderId));

            if (!this.orderIdsBroker.Contains(orderId))
            {
                this.orderIdsBroker.Add(orderId);
            }
        }

        /// <summary>
        /// Updates the execution identifier list with the given <see cref="ExecutionId"/>
        /// (if not already present).
        /// </summary>
        /// <param name="executionId">The execution identifier.</param>
        private void UpdateExecutionIds(ExecutionId executionId)
        {
            Debug.NotNull(executionId, nameof(executionId));

            if (!this.executionIds.Contains(executionId))
            {
                this.executionIds.Add(executionId);
            }
        }

        /// <summary>
        /// Processes the trigger with the orders finite state machine.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        /// <returns>The result of the trigger process.</returns>
        private CommandResult Process(Trigger trigger)
        {
            return this.orderState.Process(trigger);
        }

        private CommandResult When(OrderRejected orderEvent)
        {
            Debug.NotNull(orderEvent, nameof(orderEvent));

            return this.orderState
                .Process(new Trigger(nameof(OrderRejected)))
                .OnSuccess(() => this.Events.Add(orderEvent));
        }

        private CommandResult When(OrderCancelled orderEvent)
        {
            Debug.NotNull(orderEvent, nameof(orderEvent));

            return this.orderState
                .Process(new Trigger(nameof(OrderCancelled)))
                .OnSuccess(() => this.Events.Add(orderEvent));
        }

        private CommandResult When(OrderWorking orderEvent)
        {
            Debug.NotNull(orderEvent, nameof(orderEvent));

            return this.orderState
                .Process(new Trigger(nameof(OrderWorking)))
                .OnSuccess(() => this.Events.Add(orderEvent))
                .OnSuccess(() => this.UpdateBrokerOrderIds(orderEvent.OrderIdBroker));
        }

        private CommandResult When(OrderPartiallyFilled orderEvent)
        {
            Debug.NotNull(orderEvent, nameof(orderEvent));

            return this.orderState
                .Process(new Trigger(nameof(OrderPartiallyFilled)))
                .OnSuccess(() => this.Events.Add(orderEvent))
                .OnSuccess(() => this.UpdateExecutionIds(orderEvent.ExecutionId))
                .OnSuccess(() => { this.FilledQuantity = orderEvent.FilledQuantity; })
                .OnSuccess(() => { this.AveragePrice = orderEvent.AveragePrice; });
        }

        private CommandResult When(OrderFilled orderEvent)
        {
            Debug.NotNull(orderEvent, nameof(orderEvent));

            return this.orderState
                .Process(new Trigger(nameof(OrderFilled)))
                .OnSuccess(() => this.Events.Add(orderEvent))
                .OnSuccess(() => { this.FilledQuantity = orderEvent.FilledQuantity; })
                .OnSuccess(() => { this.AveragePrice = orderEvent.AveragePrice; });
        }

        private CommandResult When(OrderExpired orderEvent)
        {
            Debug.NotNull(orderEvent, nameof(orderEvent));

            return this.Process(new Trigger(nameof(OrderExpired)))
                .OnSuccess(() => this.Events.Add(orderEvent));
        }

        private CommandResult When(OrderModified orderEvent)
        {
            Debug.NotNull(orderEvent, nameof(orderEvent));

            return this.Process(new Trigger(nameof(OrderModified)))
                .OnSuccess(() => this.Events.Add(orderEvent))
                .OnSuccess(() => this.UpdateBrokerOrderIds(orderEvent.BrokerOrderId))
                .OnSuccess(() => { this.Price = orderEvent.ModifiedPrice; });
        }

        private void ValidateExpireTime(Option<ZonedDateTime?> expireTime)
        {
            Debug.NotNull(expireTime, nameof(expireTime));

            if (expireTime.HasNoValue)
            {
                Validate.True(this.TimeInForce != TimeInForce.GTD, nameof(this.TimeInForce));
            }
            else
            {
                // ReSharper disable once PossibleInvalidOperationException
                var expireTimeValue = (ZonedDateTime)expireTime.Value;
                Validate.True(this.TimeInForce == TimeInForce.GTD, nameof(this.TimeInForce));
                Validate.True(expireTimeValue.IsGreaterThan(this.Timestamp), nameof(expireTime));
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

        private bool IsCompleteResult()
        {
            return this.Status == OrderStatus.Cancelled
                || this.Status == OrderStatus.Expired
                || this.Status == OrderStatus.Filled
                || this.Status == OrderStatus.Rejected;
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
