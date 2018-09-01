﻿//--------------------------------------------------------------------------------------------------
// <copyright file="EntityIdFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Factories
{
    using System.Globalization;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// A factory which creates valid <see cref="OrderId"/>(s) for the system.
    /// </summary>
    [Immutable]
    public static class EntityIdFactory
    {
        /// <summary>
        /// Creates and returns a new and valid order <see cref="OrderId"/> from the given inputs.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="symbol">The symbol.</param>
        /// <param name="orderCount">The order count.</param>
        /// <returns>A <see cref="OrderId"/>.</returns>
        public static OrderId Order(ZonedDateTime time, Symbol symbol, int orderCount)
        {
            Debug.NotDefault(time, nameof(time));
            Debug.NotNull(symbol, nameof(symbol));
            Debug.NotOutOfRangeInt32(orderCount, nameof(orderCount), 0, int.MaxValue);

            return new OrderId($"{GetTimeString(time)}_{symbol.Code}_{orderCount}");
        }

        /// <summary>
        /// Creates and returns a new order modified <see cref="OrderId"/> from the given inputs.
        /// </summary>
        /// <param name="orderId">The modified order identifier.</param>
        /// <param name="orderIdCount">The order identifier count.</param>
        /// <returns>A <see cref="OrderId"/>.</returns>
        public static OrderId ModifiedOrder(OrderId orderId, int orderIdCount)
        {
            Debug.NotNull(orderId, nameof(orderId));
            Debug.NotOutOfRangeInt32(orderIdCount, nameof(orderIdCount), 0, int.MaxValue);

            return new OrderId($"{orderId}_R{orderIdCount}");
        }

        /// <summary>
        /// Creates and returns a new and valid signal <see cref="SignalId"/> from the given inputs.
        /// </summary>
        /// <param name="time">The signal time.</param>
        /// <param name="symbol">The signal symbol.</param>
        /// <param name="orderSide">The signal order side.</param>
        /// <param name="tradeType">The signal trade type.</param>
        /// <param name="signalLabel">The signal label.</param>
        /// <param name="signalCount">The signal count.</param>
        /// <returns>A <see cref="SignalId"/>.</returns>
        public static SignalId Signal(
            ZonedDateTime time,
            Symbol symbol,
            OrderSide orderSide,
            TradeType tradeType,
            Label signalLabel,
            int signalCount)
        {
            Debug.NotDefault(time, nameof(time));
            Debug.NotNull(symbol, nameof(symbol));
            Debug.NotDefault(orderSide, nameof(orderSide));
            Debug.NotNull(tradeType, nameof(tradeType));
            Debug.NotNull(signalLabel, nameof(signalLabel));
            Debug.NotOutOfRangeInt32(signalCount, nameof(signalCount), 0, int.MaxValue);

            return new SignalId($"{GetTimeString(time)}|{GetSignalIdString(symbol, tradeType, orderSide)}-{signalLabel}-{signalCount}");
        }

        /// <summary>
        /// Creates and returns a new and valid trade unit <see cref="TradeUnitId"/> from the given
        /// inputs.
        /// </summary>
        /// <param name="tradeId">The trade identifier.</param>
        /// <param name="tradeUnit">The trade unit.</param>
        /// <returns>A <see cref="TradeUnitId"/>.</returns>
        public static TradeUnitId TradeUnit(TradeId tradeId, int tradeUnit)
        {
            Debug.NotNull(tradeId, nameof(tradeId));
            Debug.NotOutOfRangeInt32(tradeUnit, nameof(tradeUnit), 0, int.MaxValue);

            return new TradeUnitId($"{tradeId}_U{tradeUnit}");
        }

        /// <summary>
        /// Creates and returns a new and valid account <see cref="AccountId"/> from the given inputs.
        /// </summary>
        /// <param name="broker">The account broker.</param>
        /// <param name="accountNumber">The account number.</param>
        /// <returns>A <see cref="AccountId"/>.</returns>
        public static AccountId Account(Broker broker, string accountNumber)
        {
            Debug.NotNull(accountNumber, nameof(accountNumber));

            return new AccountId($"{broker}-{accountNumber}");
        }

        private static string GetSignalIdString(Symbol symbol, TradeType tradeType, OrderSide orderSide)
        {
            Debug.NotNull(symbol, nameof(symbol));
            Debug.NotNull(tradeType, nameof(tradeType));
            Debug.NotDefault(orderSide, nameof(orderSide));

            return $"{symbol}|{tradeType}-{orderSide}";
        }

        private static string GetTimeString(ZonedDateTime time)
        {
            Debug.NotDefault(time, nameof(time));

            return time.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
        }
    }
}
