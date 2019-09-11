//--------------------------------------------------------------------------------------------------
// <copyright file="Order.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Aggregates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Nautilus.Core.Collections;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Types;
    using Nautilus.DomainModel.Aggregates.Base;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Events.Base;
    using Nautilus.DomainModel.FiniteStateMachine;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Represents a financial market order.
    /// </summary>
    public sealed class Order : Aggregate<OrderId, OrderEvent, Order>
    {
        private readonly FiniteStateMachine<OrderState> orderFiniteStateMachine;
        private readonly UniqueList<OrderId> orderIds;
        private readonly UniqueList<ExecutionId> executionIds;

        /// <summary>
        /// Initializes a new instance of the <see cref="Order"/> class.
        /// </summary>
        /// <param name="initial">The initial order event.</param>
        public Order(OrderInitialized initial)
            : base(initial.OrderId, initial)
        {
            this.orderFiniteStateMachine = CreateOrderFiniteStateMachine();
            this.orderIds = new UniqueList<OrderId>(this.Id);
            this.executionIds = new UniqueList<ExecutionId>();

            this.Symbol = initial.Symbol;
            this.Label = initial.Label;
            this.OrderSide = initial.OrderSide;
            this.OrderType = initial.OrderType;
            this.Quantity = initial.Quantity;
            this.FilledQuantity = Quantity.Zero();
            this.Price = initial.Price;
            this.TimeInForce = initial.TimeInForce;
            this.ExpireTime = this.ValidateExpireTime(initial.ExpireTime);
            this.IsBuy = this.OrderSide == OrderSide.BUY;
            this.IsSell = this.OrderSide == OrderSide.SELL;
            this.IsWorking = false;
            this.IsCompleted = false;

            this.CheckInitialization();
        }

        /// <summary>
        /// Gets the order initialized event identifier.
        /// </summary>
        public Guid InitId => this.InitialEvent.Id;

        /// <summary>
        /// Gets the orders last identifier.
        /// </summary>
        public OrderId IdLast => this.orderIds.Last();

        /// <summary>
        /// Gets the orders last identifier for the broker.
        /// </summary>
        public OrderIdBroker? IdBroker { get; private set; }

        /// <summary>
        /// Gets the orders account identifier.
        /// </summary>
        public AccountId? AccountId { get; private set; }

        /// <summary>
        /// Gets the orders last execution identifier.
        /// </summary>
        public ExecutionId? ExecutionId => this.executionIds.LastOrNull();

        /// <summary>
        /// Gets the orders identifier count.
        /// </summary>
        public int IdCount => this.orderIds.Count;

        /// <summary>
        /// Gets the orders symbol.
        /// </summary>
        public Symbol Symbol { get; }

        /// <summary>
        /// Gets the orders label.
        /// </summary>
        public Label Label { get; }

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
        public Quantity FilledQuantity { get; private set; }

        /// <summary>
        /// Gets the orders price.
        /// </summary>
        public Price? Price { get; private set; }

        /// <summary>
        /// Gets the orders average fill price.
        /// </summary>
        public Price? AveragePrice { get; private set; }

        /// <summary>
        /// Gets the orders slippage.
        /// </summary>
        public decimal? Slippage { get; private set; }

        /// <summary>
        /// Gets the orders time in force.
        /// </summary>
        public TimeInForce TimeInForce { get; }

        /// <summary>
        /// Gets the orders expire time.
        /// </summary>
        public ZonedDateTime? ExpireTime { get; }

        /// <summary>
        /// Gets the current order state.
        /// </summary>
        public OrderState State => this.orderFiniteStateMachine.State;

        /// <summary>
        /// Gets a value indicating whether the order side is BUY.
        /// </summary>
        public bool IsBuy { get; }

        /// <summary>
        /// Gets a value indicating whether the order side is SELL.
        /// </summary>
        public bool IsSell { get; }

        /// <summary>
        /// Gets a value indicating whether the order is working.
        /// </summary>
        public bool IsWorking { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the order is completed.
        /// </summary>
        public bool IsCompleted { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Order" /> class.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="symbol">The order symbol.</param>
        /// <param name="label">The order label.</param>
        /// <param name="side">The order side.</param>
        /// <param name="type">The order type.</param>
        /// <param name="quantity">The order quantity.</param>
        /// <param name="price">The order price (optional).</param>
        /// <param name="timeInForce">The order time in force.</param>
        /// <param name="expireTime">The order expire time (optional).</param>
        /// <param name="timestamp">The order timestamp.</param>
        /// <param name="initId">The order initialization event identifier.</param>
        /// <returns>A new order.</returns>
        public static Order Create(
            OrderId orderId,
            Symbol symbol,
            Label label,
            OrderSide side,
            OrderType type,
            Quantity quantity,
            Price? price,
            TimeInForce timeInForce,
            ZonedDateTime? expireTime,
            ZonedDateTime timestamp,
            Guid initId)
        {
            Debug.NotDefault(side, nameof(side));
            Debug.NotDefault(type, nameof(type));
            Debug.NotDefault(timeInForce, nameof(timeInForce));
            Debug.NotDefault(timestamp, nameof(timestamp));

            var initial = new OrderInitialized(
                orderId,
                symbol,
                label,
                side,
                type,
                quantity,
                price,
                timeInForce,
                expireTime,
                initId,
                timestamp);

            return new Order(initial);
        }

        /// <summary>
        /// Adds the modified order identifier to the order identifiers.
        /// </summary>
        /// <param name="modified">The modified order identifier.</param>
        public void AddModifiedOrderId(OrderId modified)
        {
            this.orderIds.Add(modified);
        }

        /// <summary>
        /// Returns the order identifiers.
        /// </summary>
        /// <returns>A read only collection.</returns>
        public UniqueList<OrderId> GetOrderIds() => this.orderIds.Copy();

        /// <summary>
        /// Returns the execution identifiers.
        /// </summary>
        /// <returns>A read only collection.</returns>
        public UniqueList<ExecutionId> GetExecutionIds() => this.executionIds.Copy();

        /// <summary>
        /// Returns a new order finite state machine.
        /// </summary>
        /// <returns>The finite state machine.</returns>
        internal static FiniteStateMachine<OrderState> CreateOrderFiniteStateMachine()
        {
            var stateTransitionTable = new Dictionary<StateTransition<OrderState>, OrderState>
            {
                { new StateTransition<OrderState>(OrderState.Initialized, Trigger.Event(typeof(OrderSubmitted))), OrderState.Submitted },
                { new StateTransition<OrderState>(OrderState.Initialized, Trigger.Event(typeof(OrderCancelled))), OrderState.Cancelled },
                { new StateTransition<OrderState>(OrderState.Initialized, Trigger.Event(typeof(OrderWorking))), OrderState.Working },  // TODO: Find out correct states
                { new StateTransition<OrderState>(OrderState.Submitted, Trigger.Event(typeof(OrderRejected))), OrderState.Rejected },
                { new StateTransition<OrderState>(OrderState.Submitted, Trigger.Event(typeof(OrderWorking))), OrderState.Working },  // TODO: Find out correct states
                { new StateTransition<OrderState>(OrderState.Submitted, Trigger.Event(typeof(OrderAccepted))), OrderState.Accepted },
                { new StateTransition<OrderState>(OrderState.Submitted, Trigger.Event(typeof(OrderCancelled))), OrderState.Cancelled },
                { new StateTransition<OrderState>(OrderState.Accepted, Trigger.Event(typeof(OrderCancelReject))), OrderState.Accepted }, // OrderCancelReject (state should remain unchanged).
                { new StateTransition<OrderState>(OrderState.Accepted, Trigger.Event(typeof(OrderWorking))), OrderState.Working },
                { new StateTransition<OrderState>(OrderState.Accepted, Trigger.Event(typeof(OrderFilled))), OrderState.Filled },
                { new StateTransition<OrderState>(OrderState.Accepted, Trigger.Event(typeof(OrderPartiallyFilled))), OrderState.PartiallyFilled },
                { new StateTransition<OrderState>(OrderState.Working, Trigger.Event(typeof(OrderCancelReject))), OrderState.Working }, // OrderCancelReject (state should remain unchanged).
                { new StateTransition<OrderState>(OrderState.Working, Trigger.Event(typeof(OrderWorking))), OrderState.Working }, // TODO: Find out correct states
                { new StateTransition<OrderState>(OrderState.Working, Trigger.Event(typeof(OrderCancelled))), OrderState.Cancelled },
                { new StateTransition<OrderState>(OrderState.Working, Trigger.Event(typeof(OrderModified))), OrderState.Working },
                { new StateTransition<OrderState>(OrderState.Working, Trigger.Event(typeof(OrderExpired))), OrderState.Expired },
                { new StateTransition<OrderState>(OrderState.Working, Trigger.Event(typeof(OrderFilled))), OrderState.Filled },
                { new StateTransition<OrderState>(OrderState.Working, Trigger.Event(typeof(OrderPartiallyFilled))), OrderState.PartiallyFilled },
                { new StateTransition<OrderState>(OrderState.PartiallyFilled, Trigger.Event(typeof(OrderCancelReject))), OrderState.PartiallyFilled }, // OrderCancelReject (state should remain unchanged).
                { new StateTransition<OrderState>(OrderState.PartiallyFilled, Trigger.Event(typeof(OrderPartiallyFilled))), OrderState.PartiallyFilled },
                { new StateTransition<OrderState>(OrderState.PartiallyFilled, Trigger.Event(typeof(OrderCancelled))), OrderState.Cancelled },
                { new StateTransition<OrderState>(OrderState.PartiallyFilled, Trigger.Event(typeof(OrderFilled))), OrderState.Filled },
            };

            // Check that all OrderCancelReject events leave the state unchanged
            Debug.True(
                stateTransitionTable
                .Where(kvp => kvp.Key.Trigger.ToString().Equals(nameof(OrderCancelReject)))
                .All(kvp => kvp.Key.CurrentState.ToString().Equals(kvp.Value.ToString())), nameof(stateTransitionTable));

            return new FiniteStateMachine<OrderState>(stateTransitionTable, OrderState.Initialized);
        }

        /// <inheritdoc />
        protected override void OnEvent(OrderEvent orderEvent)
        {
            this.orderFiniteStateMachine.Process(Trigger.Event(orderEvent));

            switch (orderEvent)
            {
                case OrderInitialized @event:
                    this.When(@event);
                    break;
                case OrderSubmitted @event:
                    this.When(@event);
                    break;
                case OrderRejected @event:
                    this.When(@event);
                    break;
                case OrderAccepted @event:
                    this.When(@event);
                    break;
                case OrderWorking @event:
                    this.When(@event);
                    break;
                case OrderCancelReject @event:
                    this.When(@event);
                    break;
                case OrderCancelled @event:
                    this.When(@event);
                    break;
                case OrderExpired @event:
                    this.When(@event);
                    break;
                case OrderModified @event:
                    this.When(@event);
                    break;
                case OrderPartiallyFilled @event:
                    this.When(@event);
                    break;
                case OrderFilled @event:
                    this.When(@event);
                    break;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(orderEvent, nameof(orderEvent));
            }
        }

        private void When(OrderInitialized @event)
        {
            // Do nothing
        }

        private void When(OrderSubmitted @event)
        {
            this.AccountId = @event.AccountId;
        }

        private void When(OrderRejected @event)
        {
            this.SetStateToCompleted();
        }

        private void When(OrderAccepted @event)
        {
            // Do nothing
        }

        private void When(OrderWorking @event)
        {
            Debug.True(!(this.Price is null), "An order with a null price can never be working.");
            Debug.EqualTo(@event.Symbol, this.Symbol, nameof(@event.Symbol));
            Debug.EqualTo(@event.Label, this.Label, nameof(@event.Label));
            Debug.EqualTo(@event.OrderSide, this.OrderSide, nameof(@event.OrderSide));
            Debug.EqualTo(@event.OrderType, this.OrderType, nameof(@event.OrderSide));
            Debug.EqualTo(@event.Quantity, this.Quantity, nameof(@event.OrderSide));
            Debug.EqualTo(@event.TimeInForce, this.TimeInForce, nameof(@event.TimeInForce));

            this.IdBroker = @event.OrderIdBroker;
            this.SetStateToWorking();
        }

        private void When(OrderCancelReject @event)
        {
            // Do nothing
        }

        private void When(OrderCancelled @event)
        {
            this.SetStateToCompleted();
        }

        private void When(OrderExpired @event)
        {
            this.SetStateToCompleted();
        }

        private void When(OrderModified @event)
        {
            this.Price = @event.ModifiedPrice;
        }

        private void When(OrderPartiallyFilled @event)
        {
            this.executionIds.Add(@event.ExecutionId);
            this.FilledQuantity = @event.FilledQuantity;
            this.AveragePrice = @event.AveragePrice;
            this.Slippage = this.CalculateSlippage();
        }

        private void When(OrderFilled @event)
        {
            this.executionIds.Add(@event.ExecutionId);
            this.FilledQuantity = @event.FilledQuantity;
            this.AveragePrice = @event.AveragePrice;
            this.Slippage = this.CalculateSlippage();
            this.SetStateToCompleted();
        }

        private ZonedDateTime? ValidateExpireTime(ZonedDateTime? expireTime)
        {
            if (expireTime.HasValue)
            {
                var expireTimeValue = expireTime.Value;
                Condition.True(this.TimeInForce == TimeInForce.GTD, nameof(this.TimeInForce));
                Condition.True(expireTimeValue.IsGreaterThanOrEqualTo(this.Timestamp), nameof(expireTime));
            }
            else
            {
                Condition.True(this.TimeInForce != TimeInForce.GTD, nameof(this.TimeInForce));
            }

            return expireTime;
        }

        private void SetStateToWorking()
        {
            this.IsWorking = true;
            this.IsCompleted = false;
        }

        private void SetStateToCompleted()
        {
            this.IsWorking = false;
            this.IsCompleted = true;
        }

        private decimal CalculateSlippage()
        {
            if (this.Price is null || this.AveragePrice is null)
            {
                return decimal.Zero;
            }

            return this.OrderSide == OrderSide.BUY
                ? this.AveragePrice.Value - this.Price.Value
                : this.Price.Value - this.AveragePrice.Value;
        }

        [System.Diagnostics.Conditional("DEBUG")]
        private void CheckInitialization()
        {
            Debug.True(this.orderIds.First() == this.Id, "this.orderIds[0] == this.Id");
            Debug.True(this.InitialEvent is OrderInitialized, "this.Events[0] is OrderInitialized");
        }
    }
}
