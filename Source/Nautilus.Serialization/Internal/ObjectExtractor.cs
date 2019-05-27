// -------------------------------------------------------------------------------------------------
// <copyright file="ObjectExtractor.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization.Internal
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
    /// Provides methods for extracting objects from <see cref="MessagePackObjectDictionary"/>s.
    /// </summary>
    internal static class ObjectExtractor
    {
        /// <summary>
        /// Parses and returns a decimal from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="decimalObject">The decimal object.</param>
        /// <returns>The parsed decimal.</returns>
        internal static decimal Decimal(MessagePackObject decimalObject)
        {
            return Convert.ToDecimal(decimalObject.ToString());
        }

        /// <summary>
        /// Parses and returns the symbol from the given string.
        /// </summary>
        /// <param name="guid">The guid object.</param>
        /// <returns>The guid.</returns>
        internal static Guid Guid(MessagePackObject guid)
        {
            return System.Guid.Parse(guid.ToString());
        }

        /// <summary>
        /// Parses and returns the symbol from the given string.
        /// </summary>
        /// <param name="symbol">The symbol object.</param>
        /// <returns>The parsed symbol <see cref="string"/>.</returns>
        internal static Symbol Symbol(MessagePackObject symbol)
        {
            return DomainModel.ValueObjects.Symbol.Create(symbol.ToString());
        }

        /// <summary>
        /// Parses and returns the symbol from the given string.
        /// </summary>
        /// <param name="brokerSymbol">The broker symbol object.</param>
        /// <returns>The parsed symbol <see cref="string"/>.</returns>
        internal static BrokerSymbol BrokerSymbol(MessagePackObject brokerSymbol)
        {
            return new BrokerSymbol(brokerSymbol.ToString());
        }

        /// <summary>
        /// Parses and returns the symbol from the given string.
        /// </summary>
        /// <param name="traderId">The trader identifier object.</param>
        /// <returns>The parsed symbol <see cref="string"/>.</returns>
        internal static TraderId TraderId(MessagePackObject traderId)
        {
            return new TraderId(traderId.ToString());
        }

        /// <summary>
        /// Parses and returns the symbol from the given string.
        /// </summary>
        /// <param name="strategyId">The strategy identifier object.</param>
        /// <returns>The parsed symbol <see cref="string"/>.</returns>
        internal static StrategyId StrategyId(MessagePackObject strategyId)
        {
            return new StrategyId(strategyId.ToString());
        }

        /// <summary>
        /// Parses and returns the symbol from the given string.
        /// </summary>
        /// <param name="positionId">The position identifier object.</param>
        /// <returns>The parsed symbol <see cref="string"/>.</returns>
        internal static PositionId PositionId(MessagePackObject positionId)
        {
            return new PositionId(positionId.ToString());
        }

        /// <summary>
        /// Parses and returns the symbol from the given string.
        /// </summary>
        /// <param name="orderId">The order identifier object.</param>
        /// <returns>The parsed symbol <see cref="string"/>.</returns>
        internal static OrderId OrderId(MessagePackObject orderId)
        {
            return new OrderId(orderId.ToString());
        }

        /// <summary>
        /// Parses and returns the symbol from the given string.
        /// </summary>
        /// <param name="executionId">The execution identifier object.</param>
        /// <returns>The execution identifier.</returns>
        internal static ExecutionId ExecutionId(MessagePackObject executionId)
        {
            return new ExecutionId(executionId.ToString());
        }

        /// <summary>
        /// Parses and returns the symbol from the given string.
        /// </summary>
        /// <param name="instrumentId">The instrument identifier object.</param>
        /// <returns>The execution identifier.</returns>
        internal static InstrumentId InstrumentId(MessagePackObject instrumentId)
        {
            return new InstrumentId(instrumentId.ToString());
        }

        /// <summary>
        /// Parses and returns the symbol from the given string.
        /// </summary>
        /// <param name="executionTicket">The execution ticket object.</param>
        /// <returns>The parsed execution ticket.</returns>
        internal static ExecutionTicket ExecutionTicket(MessagePackObject executionTicket)
        {
            return new ExecutionTicket(executionTicket.ToString());
        }

        /// <summary>
        /// Parses and returns the symbol from the given string.
        /// </summary>
        /// <param name="label">The label object.</param>
        /// <returns>The parsed symbol <see cref="string"/>.</returns>
        internal static Label Label(MessagePackObject label)
        {
            return new Label(label.ToString());
        }

        /// <summary>
        /// Parses and returns the symbol from the given string.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="toEnum">The enumerator object.</param>
        /// <returns>The brokerage.</returns>
        internal static TEnum Enum<TEnum>(MessagePackObject toEnum)
            where TEnum : struct
        {
            return toEnum.ToString().ToEnum<TEnum>();
        }

        /// <summary>
        /// Return a <see cref="DomainModel.ValueObjects.Price"/> from the given string.
        /// </summary>
        /// <param name="quantity">The quantity object.</param>
        /// <returns>The quantity.</returns>
        internal static Quantity Quantity(MessagePackObject quantity)
        {
            return DomainModel.ValueObjects.Quantity.Create(quantity.AsInt32());
        }

        /// <summary>
        /// Return a <see cref="DomainModel.ValueObjects.Price"/> from the given string.
        /// </summary>
        /// <param name="money">The money object.</param>
        /// <param name="currency">The currency.</param>
        /// <returns>The money.</returns>
        internal static Money Money(MessagePackObject money, Currency currency)
        {
            return DomainModel.ValueObjects.Money.Create(Convert.ToDecimal(money.ToString()), currency);
        }

        /// <summary>
        /// Return a <see cref="DomainModel.ValueObjects.Price"/> from the given string.
        /// </summary>
        /// <param name="price">The price string.</param>
        /// <returns>The optional price.</returns>
        internal static Price Price(MessagePackObject price)
        {
            Debug.NotEmptyOrWhiteSpace(price.ToString(), nameof(price));

            return DomainModel.ValueObjects.Price.Create(Convert.ToDecimal(price.ToString()));
        }

        /// <summary>
        /// Return a <see cref="DomainModel.ValueObjects.Price"/> from the given string.
        /// </summary>
        /// <param name="price">The price string.</param>
        /// <returns>The optional price.</returns>
        internal static Price? NullablePrice(MessagePackObject price)
        {
            return price.Equals(MessagePackObject.Nil)
                ? null
                : DomainModel.ValueObjects.Price.Create(Convert.ToDecimal(price.ToString()));
        }

        /// <summary>
        /// Parses and returns the expire time from the given string.
        /// </summary>
        /// <param name="dateTime">The zoned date time object.</param>
        /// <returns>The parsed expire time <see cref="string"/>.</returns>
        internal static ZonedDateTime ZonedDateTime(MessagePackObject dateTime)
        {
            return dateTime.ToString().ToZonedDateTimeFromIso();
        }

        /// <summary>
        /// Parses and returns the expire time from the given string.
        /// </summary>
        /// <param name="dateTime">The zoned date time.</param>
        /// <returns>The parsed expire time <see cref="string"/>.</returns>
        internal static ZonedDateTime? NullableZonedDateTime(MessagePackObject dateTime)
        {
            return dateTime == MessagePackObject.Nil
                ? null
                : dateTime.ToString().ToNullableZonedDateTimeFromIso();
        }
    }
}
