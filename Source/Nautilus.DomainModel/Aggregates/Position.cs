//--------------------------------------------------------------------------------------------------
// <copyright file="Position.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Aggregates
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Message;
    using Nautilus.DomainModel.Aggregates.Base;
    using Nautilus.DomainModel.Enums;
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
        private readonly HashSet<OrderId> orderIds;
        private readonly HashSet<ExecutionId> executionIds;
        private readonly HashSet<ExecutionTicket> executionTickets;

        private int relativeQuantity;

        /// <summary>
        /// Initializes a new instance of the <see cref="Position"/> class.
        /// </summary>
        /// <param name="positionId">The position identifier.</param>
        /// <param name="event">The fill event which opened the position.</param>
        public Position(PositionId positionId, OrderFillEvent @event)
            : base(positionId, @event.ExecutionTime)
        {
            this.AppendEvent(@event);

            this.orderIds = new HashSet<OrderId> { @event.OrderId };
            this.executionIds = new HashSet<ExecutionId> { @event.ExecutionId };
            this.executionTickets = new HashSet<ExecutionTicket> { @event.ExecutionTicket };

            this.Symbol = @event.Symbol;
            this.FromOrderId = @event.OrderId;
            this.EntryDirection = @event.OrderSide;
            this.EntryTime = @event.ExecutionTime;
            this.ExitTime = null;
            this.AverageEntryPrice = @event.AveragePrice;
            this.AverageExitPrice = null;

            this.relativeQuantity = 0;                  // Initialized in FillLogic
            this.Quantity = Quantity.Zero();            // Initialized in FillLogic
            this.PeakQuantity = Quantity.Zero();        // Initialized in FillLogic
            this.MarketPosition = MarketPosition.Flat;  // Initialized in FillLogic

            this.FillLogic(@event);
            this.CheckClassInvariants();
        }

        /// <summary>
        /// Gets the positions symbol.
        /// </summary>
        public Symbol Symbol { get; }

        /// <summary>
        /// Gets the positions entry order identifier.
        /// </summary>
        public OrderId FromOrderId { get; }

        /// <summary>
        /// Gets the positions last order identifier.
        /// </summary>
        public OrderId LastOrderId => this.orderIds.Last();

        /// <summary>
        /// Gets the positions last execution identifier.
        /// </summary>
        public ExecutionId LastExecutionId => this.executionIds.Last();

        /// <summary>
        /// Gets the positions last execution ticket.
        /// </summary>
        public ExecutionTicket LastExecutionTicket => this.executionTickets.Last();

        /// <summary>
        /// Gets the positions entry direction.
        /// </summary>
        public OrderSide EntryDirection { get; }

        /// <summary>
        /// Gets the positions entry time.
        /// </summary>
        public ZonedDateTime EntryTime { get; }

        /// <summary>
        /// Gets the positions exit time.
        /// </summary>
        public ZonedDateTime? ExitTime { get; private set; }

        /// <summary>
        /// Gets the positions average entry price.
        /// </summary>
        public Price AverageEntryPrice { get; }

        /// <summary>
        /// Gets the positions average exit price.
        /// </summary>
        public Price? AverageExitPrice { get; private set; }

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
        /// Gets a value indicating whether the position is entered.
        /// </summary>
        public bool IsOpen => this.MarketPosition != MarketPosition.Flat;

        /// <summary>
        /// Gets a value indicating whether the position is exited.
        /// </summary>
        public bool IsClosed => this.MarketPosition == MarketPosition.Flat;

        /// <summary>
        /// Gets a value indicating whether the market position is flat.
        /// </summary>
        public bool IsFlat => this.MarketPosition == MarketPosition.Flat;

        /// <summary>
        /// Gets a value indicating whether the market position is long.
        /// </summary>
        public bool IsLong => this.MarketPosition == MarketPosition.Long;

        /// <summary>
        /// Gets a value indicating whether the market position is short.
        /// </summary>
        public bool IsShort => this.MarketPosition == MarketPosition.Short;

        /// <summary>
        /// Returns a collection of the positions order identifiers.
        /// </summary>
        /// <returns>The events collection.</returns>
        public ImmutableSortedSet<OrderId> GetOrderIds() => this.orderIds.ToImmutableSortedSet();

        /// <summary>
        /// Returns a collection of the positions execution identifiers.
        /// </summary>
        /// <returns>The events collection.</returns>
        public ImmutableSortedSet<ExecutionId> GetExecutionIds() => this.executionIds.ToImmutableSortedSet();

        /// <summary>
        /// Returns a collection of the positions execution tickets.
        /// </summary>
        /// <returns>The events collection.</returns>
        public ImmutableSortedSet<ExecutionTicket> GetExecutionTickets() => this.executionTickets.ToImmutableSortedSet();

        /// <summary>
        /// Applies the given <see cref="Event"/> to this position.
        /// </summary>
        /// <param name="event">The position event.</param>
        public void Apply(OrderFillEvent @event)
        {
            this.AppendEvent(@event);

            this.orderIds.Add(@event.OrderId);
            this.executionIds.Add(@event.ExecutionId);
            this.executionTickets.Add(@event.ExecutionTicket);

            this.FillLogic(@event);
        }

        private static int CalculateRelativeQuantity(OrderFillEvent @event)
        {
            switch (@event.OrderSide)
            {
                case OrderSide.BUY:
                    return @event.FilledQuantity.Value;
                case OrderSide.SELL:
                    return -@event.FilledQuantity.Value;
                case OrderSide.UNKNOWN:
                    goto default;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(@event.OrderSide, nameof(@event.OrderSide));
            }
        }

        private void FillLogic(OrderFillEvent @event)
        {
            // Set quantities
            this.relativeQuantity += CalculateRelativeQuantity(@event);
            this.Quantity = Quantity.Create(Math.Abs(this.relativeQuantity));
            if (this.Quantity > this.PeakQuantity)
            {
                this.PeakQuantity = this.Quantity;
            }

            // Set market position
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
                this.ExitTime = @event.ExecutionTime;
                this.AverageExitPrice = @event.AveragePrice;
            }
        }

        [System.Diagnostics.Conditional("DEBUG")]
        private void CheckClassInvariants()
        {
            Debug.True(this.orderIds.Count == 1, "this.orderIds.Count == 1");
            Debug.True(this.executionIds.Count == 1, "this.executionIds.Count == 1");
            Debug.True(this.executionTickets.Count == 1, "this.executionTickets.Count == 1");
            Debug.True(this.EventCount == 1, "this.Events.Count == 1");
        }
    }
}
