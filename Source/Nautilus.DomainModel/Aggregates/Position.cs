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
    using Nautilus.Core.Collections;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Aggregates.Base;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events.Base;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Represents a financial market position.
    /// </summary>
    public sealed class Position : Aggregate<PositionId, OrderFillEvent, Position>
    {
        private readonly UniqueList<OrderId> orderIds;
        private readonly UniqueList<ExecutionId> executionIds;

        private int relativeQuantity;

        /// <summary>
        /// Initializes a new instance of the <see cref="Position"/> class.
        /// </summary>
        /// <param name="positionId">The position identifier.</param>
        /// <param name="initial">The initial fill event which opened the position.</param>
        public Position(PositionId positionId, OrderFillEvent initial)
            : base(positionId, initial)
        {
            this.orderIds = new UniqueList<OrderId>(initial.OrderId);
            this.executionIds = new UniqueList<ExecutionId>(initial.ExecutionId);

            this.AccountId = initial.AccountId;
            this.IdBroker = initial.PositionIdBroker;
            this.FromOrderId = initial.OrderId;
            this.Symbol = initial.Symbol;
            this.EntryDirection = initial.OrderSide;
            this.EntryTime = initial.ExecutionTime;
            this.AverageEntryPrice = initial.AveragePrice;

            this.relativeQuantity = 0;                  // Initialized in SetState
            this.Quantity = Quantity.Zero();            // Initialized in SetState
            this.PeakQuantity = Quantity.Zero();        // Initialized in SetState
            this.MarketPosition = MarketPosition.Flat;  // Initialized in SetState

            this.SetState(initial);
            this.CheckInitialization();
        }

        /// <summary>
        /// Gets the positions account identifier.
        /// </summary>
        public AccountId AccountId { get; }

        /// <summary>
        /// Gets the positions broker position identifier.
        /// </summary>
        public PositionIdBroker IdBroker { get; }

        /// <summary>
        /// Gets the positions initial entry order identifier.
        /// </summary>
        public OrderId FromOrderId { get; }

        /// <summary>
        /// Gets the positions last order identifier.
        /// </summary>
        public OrderId LastOrderId => this.LastEvent.OrderId;

        /// <summary>
        /// Gets the positions last execution identifier.
        /// </summary>
        public ExecutionId LastExecutionId => this.LastEvent.ExecutionId;

        /// <summary>
        /// Gets the positions symbol.
        /// </summary>
        public Symbol Symbol { get; }

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
        /// Gets a value indicating whether the position is open.
        /// </summary>
        public bool IsOpen => this.MarketPosition != MarketPosition.Flat;

        /// <summary>
        /// Gets a value indicating whether the position is closed.
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
        public UniqueList<OrderId> GetOrderIds() => this.orderIds.Copy();

        /// <summary>
        /// Returns a collection of the positions execution identifiers.
        /// </summary>
        /// <returns>The events collection.</returns>
        public UniqueList<ExecutionId> GetExecutionIds() => this.executionIds.Copy();

        /// <inheritdoc />
        protected override void OnEvent(OrderFillEvent @event)
        {
            Debug.EqualTo(this.IdBroker, @event.PositionIdBroker, nameof(@event.PositionIdBroker));

            this.orderIds.Add(@event.OrderId);
            this.executionIds.Add(@event.ExecutionId);

            this.SetState(@event);
        }

        private static int CalculateFilledRelativeQuantity(OrderFillEvent @event)
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

        private void SetState(OrderFillEvent @event)
        {
            // Set quantities
            this.relativeQuantity += CalculateFilledRelativeQuantity(@event);
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
        private void CheckInitialization()
        {
            Debug.True(this.orderIds.Count == 1, "this.orderIds.Count == 1");
            Debug.True(this.executionIds.Count == 1, "this.executionIds.Count == 1");
            Debug.True(this.EventCount == 1, "this.Events.Count == 1");
        }
    }
}
