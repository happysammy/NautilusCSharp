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
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.CQS;
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
        private int relativeQuantity;

        /// <summary>
        /// Initializes a new instance of the <see cref="Position"/> class.
        /// </summary>
        /// <param name="symbol">The position symbol.</param>
        /// <param name="fromEntryOrderId">The position entry order identifier.</param>
        /// <param name="positionId">The position identifier.</param>
        /// <param name="timestamp">The position timestamp.</param>
        public Position(
            Symbol symbol,
            OrderId fromEntryOrderId,
            PositionId positionId,
            ZonedDateTime timestamp)
            : base(
                  positionId,
                  timestamp)
        {
            this.Symbol = symbol;
            this.FromEntryOrderId = fromEntryOrderId;
            this.EntryTime = OptionVal<ZonedDateTime>.None();
            this.AverageEntryPrice = OptionRef<Price>.None();
        }

        /// <summary>
        /// Gets the positions symbol.
        /// </summary>
        public Symbol Symbol { get; }

        /// <summary>
        /// Gets the positions entry order identifier.
        /// </summary>
        public OrderId FromEntryOrderId { get; }

        /// <summary>
        /// Gets the positions quantity.
        /// </summary>
        public Quantity Quantity => Quantity.Create(Math.Abs(this.relativeQuantity));

        /// <summary>
        /// Gets the positions market position.
        /// </summary>
        public MarketPosition MarketPosition => this.CalculateMarketPosition();

        /// <summary>
        /// Gets the positions entry time (optional).
        /// </summary>
        public OptionVal<ZonedDateTime> EntryTime { get; private set; }

        /// <summary>
        /// Gets the positions average entry price (optional).
        /// </summary>
        public OptionRef<Price> AverageEntryPrice { get; private set; }

        /// <summary>
        /// Applies the given <see cref="Event"/> to this position.
        /// </summary>
        /// <param name="event">The position event.</param>
        /// <returns>A <see cref="CommandResult"/> result.</returns>
        public override CommandResult Apply(Event @event)
        {
            switch (@event)
            {
                case OrderFilled orderFilled:
                    return this.When(orderFilled);

                case OrderPartiallyFilled orderPartiallyFilled:
                    return this.When(orderPartiallyFilled);

                default: return CommandResult.Fail(
                    $"The event is not recognized by the position {this}");
            }
        }

        private CommandResult When(OrderFilled @event)
        {
            this.UpdatePosition(
                @event.OrderSide,
                @event.FilledQuantity.Value,
                @event.AveragePrice,
                @event.ExecutionTime);
            this.Events.Add(@event);

            return CommandResult.Ok();
        }

        private CommandResult When(OrderPartiallyFilled @event)
        {
            this.UpdatePosition(
                @event.OrderSide,
                @event.FilledQuantity.Value,
                @event.AveragePrice,
                @event.ExecutionTime);
            this.Events.Add(@event);

            return CommandResult.Ok();
        }

        private void UpdatePosition(
            OrderSide orderSide,
            int quantity,
            Price averagePrice,
            ZonedDateTime eventTime)
        {
            Debug.PositiveInt32(quantity, nameof(quantity));
            Debug.NotDefault(eventTime, nameof(eventTime));

            if (orderSide == OrderSide.BUY)
            {
                this.relativeQuantity += quantity;
            }
            else if (orderSide == OrderSide.SELL)
            {
                this.relativeQuantity -= quantity;
            }

            if (this.EntryTime.HasNoValue)
            {
                this.EntryTime = OptionVal<ZonedDateTime>.Some(eventTime);
            }

            this.AverageEntryPrice = averagePrice;
        }

        private MarketPosition CalculateMarketPosition()
        {
            if (this.relativeQuantity > 0)
            {
                return MarketPosition.Long;
            }

            if (this.relativeQuantity < 0)
            {
                return MarketPosition.Short;
            }

            return MarketPosition.Flat;
        }
    }
}
