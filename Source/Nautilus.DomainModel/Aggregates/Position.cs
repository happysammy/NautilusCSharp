//--------------------------------------------------------------------------------------------------
// <copyright file="Position.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Aggregates
{
    using System;
    using System.Collections.Generic;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Aggregates.Base;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Events.Base;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Represents a financial market position.
    /// </summary>
    [PerformanceOptimized]
    public sealed class Position : Aggregate<Position>
    {
        private readonly List<OrderId> orderIds;
        private readonly List<ExecutionId> executionIds;
        private readonly List<ExecutionTicket> executionTickets;

        private int internalQuantity;

        /// <summary>
        /// Initializes a new instance of the <see cref="Position"/> class.
        /// </summary>
        /// <param name="positionId">The position identifier.</param>
        /// <param name="symbol">The position symbol.</param>
        /// <param name="timestamp">The position timestamp.</param>
        public Position(
            PositionId positionId,
            Symbol symbol,
            ZonedDateTime timestamp)
            : base(positionId, timestamp)
        {
            this.orderIds = new List<OrderId>();
            this.executionIds = new List<ExecutionId>();
            this.executionTickets = new List<ExecutionTicket>();

            this.Symbol = symbol;
            this.FromOrderId = null;
            this.LastOrderId = null;
            this.LastExecutionId = null;
            this.LastExecutionTicket = null;
            this.Quantity = Quantity.Zero();
            this.PeakQuantity = Quantity.Zero();
            this.MarketPosition = MarketPosition.Flat;
            this.EntryDirection = OrderSide.UNKNOWN;
            this.EntryTime = null;
            this.ExitTime = null;
            this.AverageEntryPrice = null;
            this.AverageExitPrice = null;
            this.IsEntered = false;
            this.IsExited = false;
        }

        /// <summary>
        /// Gets the positions symbol.
        /// </summary>
        public Symbol Symbol { get; }

        /// <summary>
        /// Gets the positions current quantity.
        /// </summary>
        public Quantity Quantity { get; private set; }

        /// <summary>
        /// Gets the positions peak quantity.
        /// </summary>
        public Quantity PeakQuantity { get; private set; }

        /// <summary>
        /// Gets the positions market position.
        /// </summary>
        public MarketPosition MarketPosition { get; private set; }

        /// <summary>
        /// Gets the positions entry order identifier.
        /// </summary>
        public OrderId? FromOrderId { get; private set; }

        /// <summary>
        /// Gets the positions last order identifier.
        /// </summary>
        public OrderId? LastOrderId { get; private set; }

        /// <summary>
        /// Gets the positions last execution identifier.
        /// </summary>
        public ExecutionId? LastExecutionId { get; private set; }

        /// <summary>
        /// Gets the positions last execution ticket.
        /// </summary>
        public ExecutionTicket? LastExecutionTicket { get; private set; }

        /// <summary>
        /// Gets the positions entry direction.
        /// </summary>
        public OrderSide EntryDirection { get; private set; }

        /// <summary>
        /// Gets the positions entry time.
        /// </summary>
        public ZonedDateTime? EntryTime { get; private set; }

        /// <summary>
        /// Gets the positions exit time.
        /// </summary>
        public ZonedDateTime? ExitTime { get; private set; }

        /// <summary>
        /// Gets the positions average entry price.
        /// </summary>
        public Price? AverageEntryPrice { get; private set; }

        /// <summary>
        /// Gets the positions average exit price.
        /// </summary>
        public Price? AverageExitPrice { get; private set; }

        /// <summary>
        /// Gets the last event applied to the position.
        /// </summary>
        public Event? LastEvent { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the position is entered.
        /// </summary>
        public bool IsEntered { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the position is exited.
        /// </summary>
        public bool IsExited { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the market position is flat.
        /// </summary>
        public bool IsFlat => this.MarketPosition is MarketPosition.Flat;

        /// <summary>
        /// Gets a value indicating whether the market position is long.
        /// </summary>
        public bool IsLong => this.MarketPosition is MarketPosition.Long;

        /// <summary>
        /// Gets a value indicating whether the market position is short.
        /// </summary>
        public bool IsShort => this.MarketPosition is MarketPosition.Short;

        /// <summary>
        /// Returns a collection of the positions order identifiers.
        /// </summary>
        /// <returns>The events collection.</returns>
        public IEnumerable<OrderId> GetOrderIds()
        {
            return this.orderIds;
        }

        /// <summary>
        /// Returns a collection of the positions execution identifiers.
        /// </summary>
        /// <returns>The events collection.</returns>
        public IEnumerable<ExecutionId> GetExecutionIds()
        {
            return this.executionIds;
        }

        /// <summary>
        /// Returns a collection of the positions execution tickets.
        /// </summary>
        /// <returns>The events collection.</returns>
        public IEnumerable<ExecutionTicket> GetExecutionTickets()
        {
            return this.executionTickets;
        }

        /// <summary>
        /// Returns a collection of the positions events.
        /// </summary>
        /// <returns>The events collection.</returns>
        public IEnumerable<Event> GetEvents()
        {
            return this.Events;
        }

        /// <summary>
        /// Applies the given <see cref="Event"/> to this position.
        /// </summary>
        /// <param name="event">The position event.</param>
        public void Apply(OrderEvent @event)
        {
            switch (@event)
            {
                case OrderPartiallyFilled partiallyFilled:
                    this.UpdatePosition(
                        partiallyFilled.OrderId,
                        partiallyFilled.ExecutionId,
                        partiallyFilled.ExecutionTicket,
                        partiallyFilled.OrderSide,
                        partiallyFilled.FilledQuantity.Value,
                        partiallyFilled.AveragePrice,
                        partiallyFilled.ExecutionTime);
                    break;
                case OrderFilled filled:
                    this.UpdatePosition(
                        filled.OrderId,
                        filled.ExecutionId,
                        filled.ExecutionTicket,
                        filled.OrderSide,
                        filled.FilledQuantity.Value,
                        filled.AveragePrice,
                        filled.ExecutionTime);
                    break;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(@event, nameof(@event));
            }

            this.Events.Add(@event);
            this.LastEvent = @event;
        }

        private void UpdatePosition(
            OrderId orderId,
            ExecutionId executionId,
            ExecutionTicket executionTicket,
            OrderSide orderSide,
            int quantity,
            Price averagePrice,
            ZonedDateTime eventTime)
        {
            Debug.PositiveInt32(quantity, nameof(quantity));
            Debug.NotDefault(eventTime, nameof(eventTime));

            this.orderIds.Add(orderId);
            this.executionIds.Add(executionId);
            this.executionTickets.Add(executionTicket);
            this.LastOrderId = orderId;
            this.LastExecutionId = executionId;
            this.LastExecutionTicket = executionTicket;

            // Entry logic
            if (this.IsEntered is false)
            {
                this.FromOrderId = orderId;
                this.EntryDirection = orderSide;
                this.EntryTime = eventTime;
                this.AverageEntryPrice = averagePrice;
                this.IsEntered = true;
            }

            // Fill logic
            switch (orderSide)
            {
                case OrderSide.BUY:
                    this.internalQuantity += quantity;
                    break;
                case OrderSide.SELL:
                    this.internalQuantity -= quantity;
                    break;
                case OrderSide.UNKNOWN:
                    goto default;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(orderSide, nameof(orderSide));
            }

            this.Quantity = Quantity.Create(Math.Abs(this.internalQuantity));

            // Update peak quantity
            if (this.Quantity > this.PeakQuantity)
            {
                this.PeakQuantity = this.Quantity;
            }

            // Market position logic
            if (this.internalQuantity > 0)
            {
                this.MarketPosition = MarketPosition.Long;
                this.IsExited = false;
            }
            else if (this.internalQuantity < 0)
            {
                this.MarketPosition = MarketPosition.Short;
                this.IsExited = false;
            }
            else
            {
                this.MarketPosition = MarketPosition.Flat;
                this.ExitTime = eventTime;
                this.AverageExitPrice = averagePrice;
                this.IsExited = true;
            }
        }
    }
}
