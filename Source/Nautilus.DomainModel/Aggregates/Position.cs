//--------------------------------------------------------------------------------------------------
// <copyright file="Position.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Aggregates
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.CQS;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Aggregates.Base;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.Interfaces;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Represents a financial market position.
    /// </summary>
    [PerformanceOptimized]
    public sealed class Position : Aggregate<Position>, IPosition
    {
        private int relativeQuantity;

        /// <summary>
        /// Initializes a new instance of the <see cref="Position"/> class.
        /// </summary>
        /// <param name="symbol">The position symbol.</param>
        /// <param name="fromEntryOrderId">The position entry order identifier.</param>
        /// <param name="positionId">The position identifier.</param>
        /// <param name="timestamp">The position timestamp.</param>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if any
        /// struct argument is the default value.</exception>
        public Position(
            Symbol symbol,
            OrderId fromEntryOrderId,
            PositionId positionId,
            ZonedDateTime timestamp)
            : base(
                  positionId,
                  timestamp)
        {
            Validate.NotNull(symbol, nameof(symbol));
            Validate.NotNull(fromEntryOrderId, nameof(fromEntryOrderId));
            Validate.NotNull(positionId, nameof(positionId));
            Validate.NotEqualTo(timestamp, nameof(timestamp), default(ZonedDateTime));

            this.Symbol = symbol;
            this.FromEntryOrderId = fromEntryOrderId;
            this.EntryTime = Option<ZonedDateTime?>.None();
            this.AverageEntryPrice = Option<Price>.None();
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
        public Option<ZonedDateTime?> EntryTime { get; private set; }

        /// <summary>
        /// Gets the positions average entry price (optional).
        /// </summary>
        public Option<Price> AverageEntryPrice { get; private set; }

        /// <summary>
        /// Applies the given <see cref="Event"/> to this position.
        /// </summary>
        /// <param name="event">The position event.</param>
        /// <returns>A <see cref="CommandResult"/> result.</returns>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed.")]
        public override CommandResult Apply(Event @event)
        {
            Debug.NotNull(@event, nameof(@event));

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
            Debug.NotNull(@event, nameof(@event));

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
            Debug.NotNull(@event, nameof(@event));

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
            Debug.NotNull(averagePrice, nameof(averagePrice));
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
                this.EntryTime = Option<ZonedDateTime?>.Some(eventTime);
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
