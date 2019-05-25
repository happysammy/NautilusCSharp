// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackObjectConverter.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization
{
    using System;
    using MsgPack;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Execution.Identifiers;
    using NodaTime;

    /// <summary>
    /// Provides methods for converting between <see cref="MessagePackObject"/>s and domain objects.
    /// </summary>
    internal static class MsgPackObjectConverter
    {
        /// <summary>
        /// Parses and returns the symbol from the given string.
        /// </summary>
        /// <param name="guid">The guid object.</param>
        /// <returns>The guid.</returns>
        internal static Guid ToGuid(MessagePackObject guid)
        {
            return Guid.Parse(guid.ToString());
        }

        /// <summary>
        /// Parses and returns the symbol from the given string.
        /// </summary>
        /// <param name="symbol">The symbol object.</param>
        /// <returns>The parsed symbol <see cref="string"/>.</returns>
        internal static Symbol ToSymbol(MessagePackObject symbol)
        {
            var splitSymbol = symbol.ToString().Split('.');
            return new Symbol(splitSymbol[0], splitSymbol[1].ToEnum<Venue>());
        }

        /// <summary>
        /// Parses and returns the symbol from the given string.
        /// </summary>
        /// <param name="traderId">The trader identifier object.</param>
        /// <returns>The parsed symbol <see cref="string"/>.</returns>
        internal static TraderId ToTraderId(MessagePackObject traderId)
        {
            return new TraderId(traderId.ToString());
        }

        /// <summary>
        /// Parses and returns the symbol from the given string.
        /// </summary>
        /// <param name="strategyId">The strategy identifier object.</param>
        /// <returns>The parsed symbol <see cref="string"/>.</returns>
        internal static StrategyId ToStrategyId(MessagePackObject strategyId)
        {
            return new StrategyId(strategyId.ToString());
        }

        /// <summary>
        /// Parses and returns the symbol from the given string.
        /// </summary>
        /// <param name="positionId">The position identifier object.</param>
        /// <returns>The parsed symbol <see cref="string"/>.</returns>
        internal static PositionId ToPositionId(MessagePackObject positionId)
        {
            return new PositionId(positionId.ToString());
        }

        /// <summary>
        /// Parses and returns the symbol from the given string.
        /// </summary>
        /// <param name="orderId">The order identifier object.</param>
        /// <returns>The parsed symbol <see cref="string"/>.</returns>
        internal static OrderId ToOrderId(MessagePackObject orderId)
        {
            return new OrderId(orderId.ToString());
        }

        /// <summary>
        /// Parses and returns the symbol from the given string.
        /// </summary>
        /// <param name="executionId">The execution identifier object.</param>
        /// <returns>The execution identifier.</returns>
        internal static ExecutionId ToExecutionId(MessagePackObject executionId)
        {
            return new ExecutionId(executionId.ToString());
        }

        /// <summary>
        /// Parses and returns the symbol from the given string.
        /// </summary>
        /// <param name="executionTicket">The execution ticket object.</param>
        /// <returns>The parsed execution ticket.</returns>
        internal static ExecutionTicket ToExecutionTicket(MessagePackObject executionTicket)
        {
            return new ExecutionTicket(executionTicket.ToString());
        }

        /// <summary>
        /// Parses and returns the symbol from the given string.
        /// </summary>
        /// <param name="label">The label object.</param>
        /// <returns>The parsed symbol <see cref="string"/>.</returns>
        internal static Label ToLabel(MessagePackObject label)
        {
            return new Label(label.ToString());
        }

        /// <summary>
        /// Parses and returns the symbol from the given string.
        /// </summary>
        /// <param name="brokerage">The brokerage object.</param>
        /// <returns>The brokerage.</returns>
        internal static Brokerage ToBrokerage(MessagePackObject brokerage)
        {
            return brokerage.ToString().ToEnum<Brokerage>();
        }

        /// <summary>
        /// Parses and returns the symbol from the given string.
        /// </summary>
        /// <param name="currency">The currency object.</param>
        /// <returns>The currency.</returns>
        internal static Currency ToCurrency(MessagePackObject currency)
        {
            return currency.ToString().ToEnum<Currency>();
        }

        /// <summary>
        /// Parses and returns the symbol from the given string.
        /// </summary>
        /// <param name="orderSide">The order side object.</param>
        /// <returns>The order side.</returns>
        internal static OrderSide ToOrderSide(MessagePackObject orderSide)
        {
            return orderSide.ToString().ToEnum<OrderSide>();
        }

        /// <summary>
        /// Parses and returns the symbol from the given string.
        /// </summary>
        /// <param name="orderType">The order type object.</param>
        /// <returns>The parsed symbol <see cref="string"/>.</returns>
        internal static OrderType ToOrderType(MessagePackObject orderType)
        {
            return orderType.ToString().ToEnum<OrderType>();
        }

        /// <summary>
        /// Parses and returns the symbol from the given string.
        /// </summary>
        /// <param name="timeInForce">The time in force object.</param>
        /// <returns>The time in force.</returns>
        internal static TimeInForce ToTimeInForce(MessagePackObject timeInForce)
        {
            return timeInForce.ToString().ToEnum<TimeInForce>();
        }

        /// <summary>
        /// Return a <see cref="Price"/> from the given string.
        /// </summary>
        /// <param name="quantity">The quantity object.</param>
        /// <returns>The quantity.</returns>
        internal static Quantity ToQuantity(MessagePackObject quantity)
        {
            return Quantity.Create(quantity.AsInt32());
        }

        /// <summary>
        /// Return a <see cref="Price"/> from the given string.
        /// </summary>
        /// <param name="money">The money object.</param>
        /// <param name="currency">The currency.</param>
        /// <returns>The money.</returns>
        internal static Money ToMoney(MessagePackObject money, Currency currency)
        {
            return Money.Create(Convert.ToDecimal(money.ToString()), currency);
        }

        /// <summary>
        /// Return a <see cref="Price"/> from the given string.
        /// </summary>
        /// <param name="price">The price string.</param>
        /// <returns>The optional price.</returns>
        internal static Price ToPrice(MessagePackObject price)
        {
            Debug.NotEmptyOrWhiteSpace(price.ToString(), nameof(price));

            return Price.Create(Convert.ToDecimal(price.ToString()));
        }

        /// <summary>
        /// Return a <see cref="Price"/> from the given string.
        /// </summary>
        /// <param name="price">The price string.</param>
        /// <returns>The optional price.</returns>
        internal static Price? ToNullablePrice(MessagePackObject price)
        {
            return price.Equals(MessagePackObject.Nil)
                ? null
                : Price.Create(Convert.ToDecimal(price.ToString()));
        }

        /// <summary>
        /// Return a <see cref="MessagePackObject"/> from the given price.
        /// </summary>
        /// <param name="price">The price.</param>
        /// <returns>The <see cref="MessagePackObject"/>.</returns>
        internal static MessagePackObject FromNullablePrice(Price? price)
        {
            return price is null
                ? MessagePackObject.Nil
                : price.ToString();
        }

        /// <summary>
        /// Parses and returns the expire time from the given string.
        /// </summary>
        /// <param name="timestamp">The timestamp object.</param>
        /// <returns>The parsed expire time <see cref="string"/>.</returns>
        internal static ZonedDateTime ToZonedDateTime(MessagePackObject timestamp)
        {
            return timestamp.ToString().ToZonedDateTimeFromIso();
        }

        /// <summary>
        /// Parses and returns the expire time from the given string.
        /// </summary>
        /// <param name="expireTimeString">The expire time string.</param>
        /// <returns>The parsed expire time <see cref="string"/>.</returns>
        internal static ZonedDateTime? ToExpireTime(MessagePackObject expireTimeString)
        {
            return expireTimeString == MessagePackObject.Nil
                ? null
                : expireTimeString.ToString().ToNullableZonedDateTimeFromIso();
        }

        /// <summary>
        /// Returns a <see cref="MessagePackObject"/> from the given nullable expire time.
        /// </summary>
        /// <param name="expireTime">The nullable expire time.</param>
        /// <returns>The <see cref="MessagePackObject"/>.</returns>
        internal static MessagePackObject ToExpireTime(ZonedDateTime? expireTime)
        {
            return expireTime is null
                ? MessagePackObject.Nil
                : expireTime.Value.ToIsoString();
        }
    }
}
