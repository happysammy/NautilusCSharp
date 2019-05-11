//--------------------------------------------------------------------------------------------------
// <copyright file="Position.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.DomainModel.Aggregates.Base;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Represents a financial market position.
    /// </summary>
    [PerformanceOptimized]
    public sealed class Position : Aggregate<Position>
    {
        private readonly List<OrderId> orderIds = new List<OrderId>();
        private readonly List<ExecutionId> executionIds = new List<ExecutionId>();
        private readonly List<ExecutionTicket> executionTickets = new List<ExecutionTicket>();

        private int relativeQuantity;

        /// <summary>
        /// Initializes a new instance of the <see cref="Position"/> class.
        /// </summary>
        /// <param name="symbol">The position symbol.</param>
        /// <param name="positionId">The position identifier.</param>
        /// <param name="timestamp">The position timestamp.</param>
        public Position(
            Symbol symbol,
            PositionId positionId,
            ZonedDateTime timestamp)
            : base(
                  positionId,
                  timestamp)
        {
            this.Symbol = symbol;
            this.Quantity = Quantity.Zero();
            this.PeakQuantity = Quantity.Zero();
            this.MarketPosition = MarketPosition.Flat;
            this.FromOrderId = OptionRef<OrderId>.None();
            this.LastOrderId = OptionRef<OrderId>.None();
            this.EntryTime = OptionVal<ZonedDateTime>.None();
            this.ExitTime = OptionVal<ZonedDateTime>.None();
            this.AverageEntryPrice = OptionRef<Price>.None();
            this.AverageExitPrice = OptionRef<Price>.None();
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
        public OptionRef<OrderId> FromOrderId { get; private set; }

        /// <summary>
        /// Gets the positions last order identifier.
        /// </summary>
        public OptionRef<OrderId> LastOrderId { get; private set; }

        /// <summary>
        /// Gets the positions entry direction.
        /// </summary>
        public OptionVal<OrderSide> EntryDirection { get; private set; }

        /// <summary>
        /// Gets the positions entry time.
        /// </summary>
        public OptionVal<ZonedDateTime> EntryTime { get; private set; }

        /// <summary>
        /// Gets the positions exit time.
        /// </summary>
        public OptionVal<ZonedDateTime> ExitTime { get; private set; }

        /// <summary>
        /// Gets the positions average entry price.
        /// </summary>
        public OptionRef<Price> AverageEntryPrice { get; private set; }

        /// <summary>
        /// Gets the positions average exit price.
        /// </summary>
        public OptionRef<Price> AverageExitPrice { get; private set; }

        /// <summary>
        /// Gets the last event applied to the position.
        /// </summary>
        public Event LastEvent => this.Events.Last();

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
        public override void Apply(Event @event)
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
                default: throw new InvalidOperationException(
                    $"The event {@event} is not recognized by the position {this}");
            }

            this.Events.Add(@event);
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
                    this.relativeQuantity += quantity;
                    break;
                case OrderSide.SELL:
                    this.relativeQuantity -= quantity;
                    break;
                case OrderSide.UNKNOWN:
                    throw new ArgumentOutOfRangeException(nameof(orderSide), orderSide, "The order side was UNKNOWN.");
                default:
                    throw new ArgumentOutOfRangeException(nameof(orderSide), orderSide, "The order side was undefined.");
            }

            this.Quantity = Quantity.Create(Math.Abs(this.relativeQuantity));

            // Update peak quantity
            if (this.Quantity > this.PeakQuantity)
            {
                this.PeakQuantity = this.Quantity;
            }

            // Exit logic
            if (this.relativeQuantity == 0)
            {
                this.ExitTime = eventTime;
                this.AverageExitPrice = averagePrice;
                this.IsExited = true;
            }

            this.SetMarketPosition();
        }

        private void SetMarketPosition()
        {
            if (this.relativeQuantity > 0)
            {
                this.MarketPosition = MarketPosition.Long;
            }
            else if (this.relativeQuantity < 0)
            {
                this.MarketPosition = MarketPosition.Short;
            }
            else
            {
                this.MarketPosition = MarketPosition.Flat;
            }
        }
    }
}
