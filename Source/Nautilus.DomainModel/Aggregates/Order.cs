//--------------------------------------------------------------
// <copyright file="Order.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.DomainModel.Aggregates
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using NautechSystems.CSharp.CQS;
    using NautechSystems.CSharp.Extensions;
    using NautechSystems.CSharp.Validation;
    using Nautilus.Core;
    using Nautilus.Core.FiniteStateMachine;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The base class for all order types.
    /// </summary>
    public abstract class Order : Aggregate<Order>
    {
        /// <summary>
        /// Returns the orders protected finite state machine.
        /// </summary>
        protected readonly FiniteStateMachine OrderState = OrderStateMachine.Create();

        private readonly IList<EntityId> orderIdList = new List<EntityId>();
        private readonly IList<EntityId> brokerOrderIdList = new List<EntityId>();
        private readonly IList<EntityId> executionIdList = new List<EntityId>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Order" /> class.
        /// </summary>
        /// <param name="symbol">The order symbol.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="orderLabel">The order label.</param>
        /// <param name="orderSide">The order side.</param>
        /// <param name="orderType">The order type.</param>
        /// <param name="quantity">The order quantity.</param>
        /// <param name="timestamp">The order timestamp.</param>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if any
        /// struct argument is the default value.</exception>
        protected Order(
            Symbol symbol,
            EntityId orderId,
            Label orderLabel,
            OrderSide orderSide,
            OrderType orderType,
            Quantity quantity,
            ZonedDateTime timestamp)
            : base(orderId, timestamp)
        {
            Debug.NotNull(symbol, nameof(symbol));
            Debug.NotNull(orderId, nameof(orderId));
            Debug.NotNull(orderLabel, nameof(orderLabel));
            Debug.NotNull(quantity, nameof(quantity));

            this.Symbol = symbol;
            this.OrderLabel = orderLabel;
            this.OrderSide = orderSide;
            this.OrderType = orderType;
            this.Quantity = quantity;

            this.orderIdList.Add(this.OrderId);
            this.brokerOrderIdList.Add(new EntityId("None"));
            this.executionIdList.Add(new EntityId("None"));
        }

        /// <summary>
        /// Gets the orders symbol.
        /// </summary>
        public Symbol Symbol { get; }

        /// <summary>
        /// Gets the orders identifier.
        /// </summary>
        public EntityId OrderId => this.EntityId;

        /// <summary>
        /// Gets the orders identifiers count.
        /// </summary>
        public int OrderIdCount => this.orderIdList.Count;

        /// <summary>
        /// Gets the orders current identifier.
        /// </summary>
        public EntityId CurrentOrderId => this.orderIdList[this.orderIdList.LastIndex()];

        /// <summary>
        /// Gets the order current broker identifier.
        /// </summary>
        public EntityId BrokerOrderId => this.brokerOrderIdList[this.brokerOrderIdList.LastIndex()];

        /// <summary>
        /// Gets the orders current execution identifier.
        /// </summary>
        public EntityId ExecutionId => this.executionIdList[this.executionIdList.LastIndex()];

        /// <summary>
        /// Gets the orders label.
        /// </summary>
        public Label OrderLabel { get; }

        /// <summary>
        /// Gets the orders type.
        /// </summary>
        public OrderType OrderType { get; }

        /// <summary>
        /// Gets the orders side.
        /// </summary>
        public OrderSide OrderSide { get; }

        /// <summary>
        /// Gets the orders quantity.
        /// </summary>
        public Quantity Quantity { get; }

        /// <summary>
        /// Gets the orders filled quantity.
        /// </summary>
        public Quantity FilledQuantity { get; private set; } = Quantity.Zero();

        /// <summary>
        /// Gets the orders average fill price.
        /// </summary>
        public Price AveragePrice { get; private set; } = Price.Zero();

        /// <summary>
        /// Gets the current order status.
        /// </summary>
        public OrderStatus OrderStatus => (OrderStatus)this.OrderState.CurrentState.Value;

        /// <summary>
        /// Gets the orders last event time.
        /// </summary>
        public ZonedDateTime LastEventTime => this.Events.Count > 0
            ? this.Events[this.Events.LastIndex()].Timestamp
            : this.OrderTimestamp;

        /// <summary>
        /// Gets the orders timestamp.
        /// </summary>
        public ZonedDateTime OrderTimestamp => this.EntityTimestamp;

        /// <summary>
        /// Gets a result indicating whether the order status is complete.
        /// </summary>
        public bool IsComplete => this.IsCompleteResult();

        /// <summary>
        /// Adds the modified order identifier to the order.
        /// </summary>
        /// <param name="modifiedOrderId">The modified order identifier.</param>
        /// <exception cref="ValidationException">Throws if the argument is null.</exception>
        public void AddModifiedOrderId(EntityId modifiedOrderId)
        {
            Validate.NotNull(modifiedOrderId, nameof(modifiedOrderId));

            this.orderIdList.Add(modifiedOrderId);
        }

        /// <summary>
        /// Returns an immutable collection of the orders.
        /// </summary>
        /// <returns>A read only collection.</returns>
        public IReadOnlyCollection<EntityId> GetOrderIdList() => this.orderIdList.ToImmutableList();

        /// <summary>
        /// Returns an immutable collection of the broker order identifiers.
        /// </summary>
        /// <returns>A read only collection.</returns>
        public IReadOnlyCollection<EntityId> GetBrokerOrderIdList() => this.brokerOrderIdList.ToImmutableList();

        /// <summary>
        /// Returns an immutable collection of the order events.
        /// </summary>
        /// <returns>A read only collection.</returns>
        public IReadOnlyCollection<Event> GetEvents() => this.Events.ToImmutableList();

        /// <summary>
        /// Applies the given <see cref="Event"/> to the <see cref="Order"/>.
        /// </summary>
        /// <param name="event">The event.</param>
        /// <returns>A <see cref="CommandResult"/> result.</returns>
        /// <exception cref="ValidationException">Throws if the event argument is null.</exception>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed.")]
        public override CommandResult Apply(Event @event)
        {
            Validate.NotNull(@event, nameof(@event));

            switch (@event)
            {
                case OrderRejected orderRejected:
                    return this.When(orderRejected);

                case OrderCancelled orderCancelled:
                    return this.When(orderCancelled);

                case OrderWorking orderWorking:
                    return this.When(orderWorking);

                case OrderPartiallyFilled orderPartiallyFilled:
                    return this.When(orderPartiallyFilled);

                case OrderFilled orderFilled:
                    return this.When(orderFilled);

                default: return CommandResult.Fail($"The event is not recognized by the order {this}");
            }
        }

        /// <summary>
        /// Returns a string representation of the <see cref="Order"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{nameof(Order)}-{this.Symbol}-{this.OrderId}";

        /// <summary>
        /// Updates the broker order identifier list with the given <see cref="EntityId"/>
        /// (if not already present).
        /// </summary>
        /// <param name="orderId">The broker order identifier.</param>
        protected void UpdateBrokerOrderId(EntityId orderId)
        {
            Debug.NotNull(orderId, nameof(orderId));

            if (!this.brokerOrderIdList.Contains(orderId))
            {
                this.brokerOrderIdList.Add(orderId);
            }
        }

        /// <summary>
        /// Updates the execution identifier list with the given <see cref="EntityId"/>
        /// (if not already present).
        /// </summary>
        /// <param name="executionId">The execution identifier.</param>
        protected void UpdateExecutionId(EntityId executionId)
        {
            Debug.NotNull(executionId, nameof(executionId));

            if (!this.executionIdList.Contains(executionId))
            {
                this.executionIdList.Add(executionId);
            }
        }

        private CommandResult When(OrderRejected orderEvent)
        {
            Debug.NotNull(orderEvent, nameof(orderEvent));

            return this.OrderState
                .Process(new Trigger(nameof(OrderRejected)))
                .OnSuccess(() => this.Events.Add(orderEvent));
        }

        private CommandResult When(OrderCancelled orderEvent)
        {
            Debug.NotNull(orderEvent, nameof(orderEvent));

            return this.OrderState
                .Process(new Trigger(nameof(OrderCancelled)))
                .OnSuccess(() => this.Events.Add(orderEvent));
        }

        private CommandResult When(OrderWorking orderEvent)
        {
            Debug.NotNull(orderEvent, nameof(orderEvent));

            return this.OrderState
                .Process(new Trigger(nameof(OrderWorking)))
                .OnSuccess(() => this.Events.Add(orderEvent))
                .OnSuccess(() => this.UpdateBrokerOrderId(orderEvent.BrokerOrderId));
        }

        private CommandResult When(OrderPartiallyFilled orderEvent)
        {
            Debug.NotNull(orderEvent, nameof(orderEvent));

            return this.OrderState
                .Process(new Trigger(nameof(OrderPartiallyFilled)))
                .OnSuccess(() => this.Events.Add(orderEvent))
                .OnSuccess(() => this.UpdateExecutionId(orderEvent.ExecutionId))
                .OnSuccess(() => { this.FilledQuantity = orderEvent.FilledQuantity; })
                .OnSuccess(() => { this.AveragePrice = orderEvent.AveragePrice; });
        }

        private CommandResult When(OrderFilled orderEvent)
        {
            Debug.NotNull(orderEvent, nameof(orderEvent));

            return this.OrderState
                .Process(new Trigger(nameof(OrderFilled)))
                .OnSuccess(() => this.Events.Add(orderEvent))
                .OnSuccess(() => { this.FilledQuantity = orderEvent.FilledQuantity; })
                .OnSuccess(() => { this.AveragePrice = orderEvent.AveragePrice; });
        }

        private bool IsCompleteResult()
        {
            return this.OrderStatus == OrderStatus.Cancelled
                || this.OrderStatus == OrderStatus.Expired
                || this.OrderStatus == OrderStatus.Filled
                || this.OrderStatus == OrderStatus.Rejected;
        }

        private static class OrderStateMachine
        {
            internal static FiniteStateMachine Create()
            {
                var stateTransitionTable = new Dictionary<StateTransition, State>
                {
                    { new StateTransition(new State(OrderStatus.Initialized), new Trigger(nameof(OrderRejected))), new State(OrderStatus.Rejected) },
                    { new StateTransition(new State(OrderStatus.Initialized), new Trigger(nameof(OrderCancelled))), new State(OrderStatus.Cancelled) },
                    { new StateTransition(new State(OrderStatus.Initialized), new Trigger(nameof(OrderWorking))), new State(OrderStatus.Working) },
                    { new StateTransition(new State(OrderStatus.Working), new Trigger(nameof(OrderCancelled))), new State(OrderStatus.Cancelled) },
                    { new StateTransition(new State(OrderStatus.Working), new Trigger(nameof(OrderModified))), new State(OrderStatus.Working) },
                    { new StateTransition(new State(OrderStatus.Working), new Trigger(nameof(OrderExpired))), new State(OrderStatus.Expired) },
                    { new StateTransition(new State(OrderStatus.Working), new Trigger(nameof(OrderFilled))), new State(OrderStatus.Filled) },
                    { new StateTransition(new State(OrderStatus.Working), new Trigger(nameof(OrderPartiallyFilled))), new State(OrderStatus.PartiallyFilled) },
                    { new StateTransition(new State(OrderStatus.PartiallyFilled), new Trigger(nameof(OrderPartiallyFilled))), new State(OrderStatus.PartiallyFilled) },
                    { new StateTransition(new State(OrderStatus.PartiallyFilled), new Trigger(nameof(OrderFilled))), new State(OrderStatus.Filled) }
                };

                return new FiniteStateMachine(stateTransitionTable, new State(OrderStatus.Initialized));
            }
        }
    }
}
