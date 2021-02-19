﻿//--------------------------------------------------------------------------------------------------
// <copyright file="Position.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
//--------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Nautilus.Core.Collections;
using Nautilus.Core.Correctness;
using Nautilus.DomainModel.Aggregates.Base;
using Nautilus.DomainModel.Enums;
using Nautilus.DomainModel.Events.Base;
using Nautilus.DomainModel.Identifiers;
using Nautilus.DomainModel.ValueObjects;
using NodaTime;

namespace Nautilus.DomainModel.Aggregates
{
    /// <summary>
    /// Represents a financial market position.
    /// </summary>
    public sealed class Position : Aggregate<PositionId, OrderFillEvent, Position>
    {
        private readonly UniqueList<OrderId> orderIds;
        private readonly UniqueList<ExecutionId> executionIds;
        private readonly Dictionary<OrderId, Quantity> buyQuantities;
        private readonly Dictionary<OrderId, Quantity> sellQuantities;
        private readonly Dictionary<OrderId, Price> fillPrices;

        private Quantity buyQuantity;
        private Quantity sellQuantity;
        private decimal relativeQuantity;
        private int precision;

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
            this.buyQuantities = new Dictionary<OrderId, Quantity>();
            this.sellQuantities = new Dictionary<OrderId, Quantity>();
            this.fillPrices = new Dictionary<OrderId, Price>();

            this.IdBroker = initial.PositionIdBroker;
            this.AccountId = initial.AccountId;
            this.FromOrderId = initial.OrderId;
            this.Symbol = initial.Symbol;
            this.BaseCurrency = initial.Currency;
            this.EntryDirection = initial.OrderSide;
            this.OpenedTime = initial.ExecutionTime;
            this.AverageOpenPrice = initial.AveragePrice.Value;
            this.Commission = decimal.Zero;
            this.Interest = decimal.Zero;
            this.RealizedPoints = decimal.Zero;
            this.RealizedReturn = 0;
            this.RealizedPnl = Money.Zero(this.BaseCurrency);

            this.buyQuantity = Quantity.Zero();         // Initialized in this.Update()
            this.sellQuantity = Quantity.Zero();        // Initialized in this.Update()
            this.relativeQuantity = decimal.Zero;       // Initialized in this.Update()
            this.precision = initial.FilledQuantity.Precision;
            this.Quantity = Quantity.Zero();            // Initialized in this.Update()
            this.PeakQuantity = Quantity.Zero();        // Initialized in this.Update()
            this.MarketPosition = MarketPosition.Flat;  // Initialized in this.Update()

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
        /// Gets the positions base currency.
        /// </summary>
        public Currency BaseCurrency { get; }

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
        /// Gets the positions opened duration.
        /// </summary>
        public Duration? OpenDuration { get; private set; }

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
        /// Gets the positions total commission.
        /// </summary>
        public decimal Commission { get; private set; }

        /// <summary>
        /// Gets the positions total interest.
        /// </summary>
        public decimal Interest { get; private set; }

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
        public Money RealizedPnl { get; private set; }

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
        /// Set the positions commission to the given value.
        /// </summary>
        /// <param name="value">The commission value to set.</param>
        public void SetCommission(decimal value)
        {
            this.Commission = value;
        }

        /// <summary>
        /// Set the positions interest to the given value.
        /// </summary>
        /// <param name="value">The interest value to set.</param>
        public void SetInterest(decimal value)
        {
            this.Interest = value;
        }

        /// <summary>
        /// Return the positions unrealized points.
        /// </summary>
        /// <param name="last">The position symbols last tick.</param>
        /// <returns>The points as a decimal.</returns>
        public decimal UnrealizedPoints(QuoteTick last)
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
        public double UnrealizedReturn(QuoteTick last)
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
        public Money UnrealizedPnl(QuoteTick last)
        {
            Debug.EqualTo(this.Symbol, last.Symbol, nameof(last.Symbol));

            switch (this.MarketPosition)
            {
                case MarketPosition.Long:
                    return this.CalculatePnl(this.AverageOpenPrice, last.Bid.Value, this.Quantity);
                case MarketPosition.Short:
                    return this.CalculatePnl(this.AverageOpenPrice, last.Ask.Value, this.Quantity);
                case MarketPosition.Flat:
                    return Money.Zero(this.BaseCurrency);
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(this.MarketPosition, nameof(this.MarketPosition));
            }
        }

        /// <summary>
        /// Return the positions total points (sum of realized and unrealized points).
        /// </summary>
        /// <param name="last">The position symbols last tick.</param>
        /// <returns>The total points as a <see cref="decimal"/>.</returns>
        public decimal TotalPoints(QuoteTick last)
        {
            Debug.EqualTo(this.Symbol, last.Symbol, nameof(last.Symbol));

            return this.RealizedPoints + this.UnrealizedPoints(last);
        }

        /// <summary>
        /// Return the positions total return (sum of realized and unrealized returns).
        /// </summary>
        /// <param name="last">The position symbols last tick.</param>
        /// <returns>The total return as a <see cref="double"/>.</returns>
        public double TotalReturn(QuoteTick last)
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
        public Money TotalPnl(QuoteTick last)
        {
            Debug.EqualTo(this.Symbol, last.Symbol, nameof(last.Symbol));

            return this.RealizedPnl.Add(this.UnrealizedPnl(last));
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
            this.fillPrices[@event.OrderId] = @event.AveragePrice;
            this.precision = Math.Max(this.precision, @event.AveragePrice.Precision);

            switch (@event.OrderSide)
            {
                case OrderSide.Buy:
                    this.HandleBuyOrderFill(@event);
                    break;
                case OrderSide.Sell:
                    this.HandleSellOrderFill(@event);
                    break;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(@event.OrderSide, nameof(@event.OrderSide));
            }

            this.relativeQuantity = this.buyQuantity - this.sellQuantity;
            this.Quantity = Quantity.Create(Math.Abs(this.relativeQuantity));
            if (this.Quantity > this.PeakQuantity)
            {
                this.PeakQuantity = this.Quantity;
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
                this.OpenDuration = this.ClosedTime - this.OpenedTime;
            }
        }

        private void HandleBuyOrderFill(OrderFillEvent @event)
        {
            this.buyQuantities[@event.OrderId] = @event.FilledQuantity;
            this.buyQuantity = this.CalculateTotalQuantity(this.buyQuantities);

            // LONG POSITION
            if (this.relativeQuantity > 0)
            {
                this.AverageOpenPrice = this.CalculateAveragePrice(this.buyQuantities, this.buyQuantity);
            }

            // SHORT POSITION
            else if (this.relativeQuantity < 0)
            {
                this.AverageClosePrice = this.CalculateAveragePrice(this.buyQuantities, this.buyQuantity);
                this.RealizedPoints = this.CalculatePoints(this.AverageOpenPrice, this.AverageClosePrice);
                this.RealizedReturn = this.CalculateReturn(this.AverageOpenPrice, this.AverageClosePrice);
                this.RealizedPnl = this.CalculatePnl(this.AverageOpenPrice, this.AverageClosePrice, this.buyQuantity);
            }
        }

        private void HandleSellOrderFill(OrderFillEvent @event)
        {
            this.sellQuantities[@event.OrderId] = @event.FilledQuantity;
            this.sellQuantity = this.CalculateTotalQuantity(this.sellQuantities);

            // SHORT POSITION
            if (this.relativeQuantity < 0)
            {
                this.AverageOpenPrice = this.CalculateAveragePrice(this.sellQuantities, this.sellQuantity);
            }

            // LONG POSITION
            else if (this.relativeQuantity > 0)
            {
                this.AverageClosePrice = this.CalculateAveragePrice(this.sellQuantities, this.sellQuantity);
                this.RealizedPoints = this.CalculatePoints(this.AverageOpenPrice, this.AverageClosePrice);
                this.RealizedReturn = this.CalculateReturn(this.AverageOpenPrice, this.AverageClosePrice);
                this.RealizedPnl = this.CalculatePnl(this.AverageOpenPrice, this.AverageClosePrice, this.sellQuantity);
            }
        }

        private Quantity CalculateTotalQuantity(Dictionary<OrderId, Quantity> quantities)
        {
            var total = decimal.Zero;
            foreach (var (_, quantity) in quantities)
            {
                total += quantity;
            }

            return Quantity.Create(total, this.precision);
        }

        private decimal CalculateAveragePrice(Dictionary<OrderId, Quantity> fills, Quantity totalQuantity)
        {
            var cumulativePrice = 0m;
            foreach (var (orderId, quantity) in fills)
            {
                cumulativePrice += this.fillPrices[orderId] * quantity;
            }

            return cumulativePrice / totalQuantity;
        }

        private decimal CalculatePoints(decimal openPrice, decimal? closePrice)
        {
            if (closePrice is null)
            {
                return 0;
            }

            switch (this.MarketPosition)
            {
                case MarketPosition.Long:
                    return (decimal)closePrice - openPrice;
                case MarketPosition.Short:
                    return openPrice - (decimal)closePrice;
                case MarketPosition.Flat:
                    return decimal.Zero;
                case MarketPosition.Undefined:
                    goto default;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(this.MarketPosition, nameof(this.MarketPosition));
            }
        }

        private double CalculateReturn(decimal openPrice, decimal? closePrice)
        {
            if (closePrice is null)
            {
                return 0;
            }

            switch (this.MarketPosition)
            {
                case MarketPosition.Long:
                    return ((double)closePrice - (double)openPrice) / (double)openPrice;
                case MarketPosition.Short:
                    return ((double)openPrice - (double)closePrice) / (double)openPrice;
                case MarketPosition.Flat:
                    return 0;
                case MarketPosition.Undefined:
                    goto default;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(this.MarketPosition, nameof(this.MarketPosition));
            }
        }

        private Money CalculatePnl(decimal openPrice, decimal? closePrice, Quantity filledQuantity)
        {
            return closePrice is null
                ? Money.Zero(this.BaseCurrency)
                : Money.Create(this.CalculatePoints(openPrice, closePrice) * filledQuantity, this.BaseCurrency);
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
