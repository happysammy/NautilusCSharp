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
        private int filledQuantityBuys;
        private int filledQuantitySells;

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

            this.IdBroker = initial.PositionIdBroker;
            this.AccountId = initial.AccountId;
            this.FromOrderId = initial.OrderId;
            this.Symbol = initial.Symbol;
            this.EntryDirection = initial.OrderSide;
            this.OpenedTime = initial.ExecutionTime;
            this.AverageOpenPrice = initial.AveragePrice.Value;
            this.RealizedPoints = decimal.Zero;
            this.RealizedReturn = 0;

            this.relativeQuantity = 0;                   // Initialized in this.Update()
            this.filledQuantityBuys = 0;                 // Initialized in this.Update()
            this.filledQuantitySells = 0;                // Initialized in this.Update()
            this.Quantity = Quantity.Zero();             // Initialized in this.Update()
            this.PeakOpenQuantity = Quantity.Zero();     // Initialized in this.Update()
            this.MarketPosition = MarketPosition.Flat;   // Initialized in this.Update()

            this.Update(initial);
            this.CheckInitialization();
        }

        /// <summary>
        /// Gets the positions broker position identifier.
        /// </summary>
        public PositionIdBroker IdBroker { get; }

        /// <summary>
        /// Gets the positions account identifier.
        /// </summary>
        public AccountId AccountId { get; }

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
        /// Gets the positions initial entry direction.
        /// </summary>
        public OrderSide EntryDirection { get; }

        /// <summary>
        /// Gets the positions opened time.
        /// </summary>
        public ZonedDateTime OpenedTime { get; }

        /// <summary>
        /// Gets the positions closed time.
        /// </summary>
        public ZonedDateTime? ClosedTime { get; private set; }

        /// <summary>
        /// Gets the positions current quantity.
        /// </summary>
        public Quantity Quantity { get; private set; }

        /// <summary>
        /// Gets the positions filled quantity from buy side orders.
        /// </summary>
        public Quantity FilledQuantityBuys => Quantity.Create(this.filledQuantityBuys);

        /// <summary>
        /// Gets the positions filled quantity from sell side orders.
        /// </summary>
        public Quantity FilledQuantitySells => Quantity.Create(this.filledQuantitySells);

        /// <summary>
        /// Gets the positions peak quantity.
        /// </summary>
        public Quantity PeakOpenQuantity { get; private set; }

        /// <summary>
        /// Gets the positions market position.
        /// </summary>
        public MarketPosition MarketPosition { get; private set; }

        /// <summary>
        /// Gets the positions average open price.
        /// </summary>
        public decimal AverageOpenPrice { get; private set; }

        /// <summary>
        /// Gets the positions average close price.
        /// </summary>
        public decimal? AverageClosePrice { get; private set; }

        /// <summary>
        /// Gets the positions points realized.
        /// </summary>
        public decimal RealizedPoints { get; private set; }

        /// <summary>
        /// Gets the positions return realized.
        /// </summary>
        public double RealizedReturn { get; private set; }

        /// <summary>
        /// Gets the positions PnL realized (in the symbols quote currency).
        /// </summary>
        public decimal RealizedPnl { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the position is open.
        /// </summary>
        public bool IsOpen => this.MarketPosition != MarketPosition.Flat;

        /// <summary>
        /// Gets a value indicating whether the position is closed.
        /// </summary>
        public bool IsClosed => this.MarketPosition == MarketPosition.Flat;

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

        /// <summary>
        /// Return the positions unrealized points.
        /// </summary>
        /// <param name="last">The position symbols last tick.</param>
        /// <returns>The points as a decimal.</returns>
        public decimal UnrealizedPoints(Tick last)
        {
            Debug.EqualTo(this.Symbol, last.Symbol, nameof(last.Symbol));

            switch (this.MarketPosition)
            {
                case MarketPosition.Long:
                    return this.CalculatePoints(this.AverageOpenPrice, last.Bid.Value);
                case MarketPosition.Short:
                    return this.CalculatePoints(this.AverageOpenPrice, last.Ask.Value);
                case MarketPosition.Flat:
                    return decimal.Zero;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(this.MarketPosition, nameof(this.MarketPosition));
            }
        }

        /// <summary>
        /// Return the positions unrealized return.
        /// </summary>
        /// <param name="last">The position symbols last tick.</param>
        /// <returns>The return as a double.</returns>
        public double UnrealizedReturn(Tick last)
        {
            Debug.EqualTo(this.Symbol, last.Symbol, nameof(last.Symbol));

            switch (this.MarketPosition)
            {
                case MarketPosition.Long:
                    return this.CalculateReturn(this.AverageOpenPrice, last.Bid.Value);
                case MarketPosition.Short:
                    return this.CalculateReturn(this.AverageOpenPrice, last.Ask.Value);
                case MarketPosition.Flat:
                    return 0;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(this.MarketPosition, nameof(this.MarketPosition));
            }
        }

        /// <summary>
        /// Return the positions unrealized PnL (not including costs, in the position symbols quote currency).
        /// </summary>
        /// <param name="last">The position symbols last tick.</param>
        /// <returns>The PnL as a decimal.</returns>
        public decimal UnrealizedPnl(Tick last)
        {
            Debug.EqualTo(this.Symbol, last.Symbol, nameof(last.Symbol));

            switch (this.MarketPosition)
            {
                case MarketPosition.Long:
                    return this.CalculatePnl(this.AverageOpenPrice, last.Bid.Value, this.Quantity.Value);
                case MarketPosition.Short:
                    return this.CalculatePnl(this.AverageOpenPrice, last.Ask.Value, this.Quantity.Value);
                case MarketPosition.Flat:
                    return 0;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(this.MarketPosition, nameof(this.MarketPosition));
            }
        }

        /// <summary>
        /// Return the positions total points (sum of realized and unrealized points).
        /// </summary>
        /// <param name="last">The position symbols last tick.</param>
        /// <returns>The total points as a <see cref="decimal"/>.</returns>
        public decimal TotalPoints(Tick last)
        {
            Debug.EqualTo(this.Symbol, last.Symbol, nameof(last.Symbol));

            return this.RealizedPoints + this.UnrealizedPoints(last);
        }

        /// <summary>
        /// Return the positions total return (sum of realized and unrealized returns).
        /// </summary>
        /// <param name="last">The position symbols last tick.</param>
        /// <returns>The total return as a <see cref="double"/>.</returns>
        public double TotalReturn(Tick last)
        {
            Debug.EqualTo(this.Symbol, last.Symbol, nameof(last.Symbol));

            return this.RealizedReturn + this.UnrealizedReturn(last);
        }

        /// <summary>
        /// Return the positions total PnL (sum of realized and unrealized PnL, not including costs,
        /// in the position symbols quote currency).
        /// </summary>
        /// <param name="last">The position symbols last tick.</param>
        /// <returns>The total PnL as a <see cref="decimal"/>.</returns>
        public decimal TotalPnl(Tick last)
        {
            Debug.EqualTo(this.Symbol, last.Symbol, nameof(last.Symbol));

            return this.RealizedPnl + this.UnrealizedPnl(last);
        }

        /// <inheritdoc />
        protected override void OnEvent(OrderFillEvent @event)
        {
            Debug.EqualTo(this.IdBroker, @event.PositionIdBroker, nameof(@event.PositionIdBroker));

            this.orderIds.Add(@event.OrderId);
            this.executionIds.Add(@event.ExecutionId);

            this.Update(@event);
        }

        private void Update(OrderFillEvent @event)
        {
            switch (@event.OrderSide)
            {
                case OrderSide.BUY:
                    this.HandleBuyOrderFill(@event);
                    break;
                case OrderSide.SELL:
                    this.HandleSellOrderFill(@event);
                    break;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(@event.OrderSide, nameof(@event.OrderSide));
            }

            this.Quantity = Quantity.Create(Math.Abs(this.relativeQuantity));
            if (this.Quantity > this.PeakOpenQuantity)
            {
                this.PeakOpenQuantity = this.Quantity;
            }

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
                this.ClosedTime = @event.ExecutionTime;
            }
        }

        private void HandleBuyOrderFill(OrderFillEvent @event)
        {
            this.filledQuantityBuys += @event.FilledQuantity;

            // LONG POSITION
            if (this.relativeQuantity > 0)
            {
                this.AverageOpenPrice = this.CalculateAveragePrice(@event, this.AverageOpenPrice, this.filledQuantityBuys);
            }

            // SHORT POSITION
            else if (this.relativeQuantity < 0)
            {
                this.AverageClosePrice = !this.AverageClosePrice.HasValue
                    ? @event.AveragePrice.Value
                    : this.CalculateAveragePrice(@event, this.AverageClosePrice.Value, this.filledQuantityBuys);

                this.RealizedPoints = this.CalculatePoints(this.AverageOpenPrice, @event.AveragePrice.Value);
                this.RealizedReturn = this.CalculateReturn(this.AverageOpenPrice, @event.AveragePrice.Value);
                this.RealizedPnl = this.CalculatePnl(this.AverageOpenPrice, @event.AveragePrice.Value, @event.FilledQuantity.Value);
            }

            this.relativeQuantity += @event.FilledQuantity;
        }

        private void HandleSellOrderFill(OrderFillEvent @event)
        {
            this.filledQuantitySells += @event.FilledQuantity;

            // SHORT POSITION
            if (this.relativeQuantity < 0)
            {
                this.AverageOpenPrice = this.CalculateAveragePrice(@event, this.AverageOpenPrice, this.filledQuantitySells);
            }

            // LONG POSITION
            else if (this.relativeQuantity > 0)
            {
                this.AverageClosePrice = !this.AverageClosePrice.HasValue
                    ? @event.AveragePrice.Value
                    : this.CalculateAveragePrice(@event, this.AverageClosePrice.Value, this.filledQuantitySells);

                this.RealizedPoints = this.CalculatePoints(this.AverageOpenPrice, @event.AveragePrice.Value);
                this.RealizedReturn = this.CalculateReturn(this.AverageOpenPrice, @event.AveragePrice.Value);
                this.RealizedPnl = this.CalculatePnl(this.AverageOpenPrice, @event.AveragePrice.Value, @event.FilledQuantity.Value);
            }

            this.relativeQuantity -= @event.FilledQuantity;
        }

        private decimal CalculateAveragePrice(OrderFillEvent @event, decimal currentAveragePrice, int totalFills)
        {
            return ((this.Quantity.Value * currentAveragePrice) + (@event.FilledQuantity.Value * @event.AveragePrice.Value))
                   / totalFills;
        }

        private decimal CalculatePoints(decimal openedPrice, decimal closedPrice)
        {
            switch (this.MarketPosition)
            {
                case MarketPosition.Long:
                    return closedPrice - openedPrice;
                case MarketPosition.Short:
                    return openedPrice - closedPrice;
                case MarketPosition.Flat:
                    return decimal.Zero;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(this.MarketPosition, nameof(this.MarketPosition));
            }
        }

        private double CalculateReturn(decimal openedPrice, decimal closedPrice)
        {
            switch (this.MarketPosition)
            {
                case MarketPosition.Long:
                    return ((double)closedPrice - (double)openedPrice) / (double)openedPrice;
                case MarketPosition.Short:
                    return ((double)openedPrice - (double)closedPrice) / (double)openedPrice;
                case MarketPosition.Flat:
                    return 0;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(this.MarketPosition, nameof(this.MarketPosition));
            }
        }

        private decimal CalculatePnl(decimal openedPrice, decimal closedPrice, int filledQuantity)
        {
            return Math.Round(this.CalculatePoints(openedPrice, closedPrice) * filledQuantity, 2);
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
