//--------------------------------------------------------------------------------------------------
// <copyright file="StopOrder.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Orders
{
    using Nautilus.Core;
    using Nautilus.Core.CQS;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.FiniteStateMachine;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The base class for all Stop order types.
    /// </summary>
    public abstract class StopOrder : Order
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StopOrder" /> class.
        /// </summary>
        /// <param name="symbol">The order symbol.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="orderLabel">The order label.</param>
        /// <param name="orderSide">The order side.</param>
        /// <param name="orderType">The order Type.</param>
        /// <param name="quantity">The order quantity.</param>
        /// <param name="price">The order entry price.</param>
        /// <param name="timeInForce">The order time in force.</param>
        /// <param name="expireTime">The order expire time (optional).</param>
        /// <param name="timestamp">The order timestamp.</param>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if any
        /// struct argument is the default value.</exception>
        protected StopOrder(
            Symbol symbol,
            EntityId orderId,
            Label orderLabel,
            OrderSide orderSide,
            OrderType orderType,
            Quantity quantity,
            Price price,
            TimeInForce timeInForce,
            Option<ZonedDateTime?> expireTime,
            ZonedDateTime timestamp)
            : base(
                  symbol,
                  orderId,
                  orderLabel,
                  orderSide,
                  orderType,
                  quantity,
                  timestamp)
        {
            Debug.NotNull(symbol, nameof(symbol));
            Debug.NotNull(orderId, nameof(orderId));
            Debug.NotNull(orderLabel, nameof(orderLabel));
            Debug.NotDefault(orderSide, nameof(orderSide));
            Debug.NotDefault(orderType, nameof(orderType));
            Debug.NotNull(quantity, nameof(quantity));
            Debug.NotNull(price, nameof(price));
            Debug.NotDefault(timeInForce, nameof(timeInForce));
            Debug.NotNull(expireTime, nameof(expireTime));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.Price = price;
            this.TimeInForce = timeInForce;
            this.ExpireTime = this.ValidateExpireTime(expireTime);
        }

        /// <summary>
        /// Gets the orders price.
        /// </summary>
        public Price Price { get; private set; }

        /// <summary>
        /// The orders slippage.
        /// </summary>
        public decimal Slippage => this.CalculateSlippage();

        /// <summary>
        /// Gets the orders time in force.
        /// </summary>
        public TimeInForce TimeInForce { get; }

        /// <summary>
        /// Gets the orders expire time (optional).
        /// </summary>
        public Option<ZonedDateTime?> ExpireTime { get; }

        /// <summary>
        /// Applies the order event to this order.
        /// </summary>
        /// <param name="event">The order event.</param>
        /// <returns>A <see cref="CommandResult"/> result.</returns>
        public override CommandResult Apply(Event @event)
        {
            Debug.NotNull(@event, nameof(@event));

            switch (@event)
            {
                case OrderRejected orderRejected:
                    return base.Apply(orderRejected);

                case OrderCancelled orderCancelled:
                    return base.Apply(orderCancelled);

                case OrderWorking orderWorking:
                    return base.Apply(orderWorking);

                case OrderPartiallyFilled orderPartiallyFilled:
                    return base.Apply(orderPartiallyFilled);

                case OrderFilled orderFilled:
                    return base.Apply(orderFilled);

                case OrderExpired orderExpired:
                    return this.When(orderExpired);

                case OrderModified orderModified:
                    return this.When(orderModified);

                default: return CommandResult.Fail($"Command Failure (Event not recognized by Order {this}.)");
            }
        }

        private CommandResult When(OrderExpired orderEvent)
        {
            Debug.NotNull(orderEvent, nameof(orderEvent));

            return this.Process(new Trigger(nameof(OrderExpired)))
                .OnSuccess(() => this.Events.Add(orderEvent));
        }

        private CommandResult When(OrderModified orderEvent)
        {
            Debug.NotNull(orderEvent, nameof(orderEvent));

            return this.Process(new Trigger(nameof(OrderModified)))
                .OnSuccess(() => this.Events.Add(orderEvent))
                .OnSuccess(() => this.UpdateBrokerOrderId(orderEvent.BrokerOrderId))
                .OnSuccess(() => { this.Price = orderEvent.ModifiedPrice; });
        }

        private Option<ZonedDateTime?> ValidateExpireTime(Option<ZonedDateTime?> expireTime)
        {
            Debug.NotNull(expireTime, nameof(expireTime));

            if (expireTime.HasValue)
            {
                Validate.True(this.TimeInForce == TimeInForce.GTD, nameof(this.TimeInForce));
                Validate.True(this.IsExpireTimeGreaterThanTimestamp(expireTime), nameof(expireTime));
            }

            return expireTime;
        }

        private bool IsExpireTimeGreaterThanTimestamp(Option<ZonedDateTime?> expireTime)
        {
            Debug.NotNull(expireTime, nameof(expireTime));
            // ReSharper disable once PossibleInvalidOperationException
            return ZonedDateTime.Comparer.Instant.Compare((ZonedDateTime)expireTime.Value, this.Timestamp) >= 0;
        }

        private decimal CalculateSlippage()
        {
            if (this.AveragePrice.Value == decimal.Zero)
            {
                return decimal.Zero;
            }

            return this.OrderSide == OrderSide.BUY
                       ? this.AveragePrice.Value - this.Price
                       : this.Price - this.AveragePrice.Value;
        }
    }
}
