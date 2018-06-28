//--------------------------------------------------------------------------------------------------
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
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.DomainModel;
    using NodaTime;

    /// <summary>
    /// A factory which creates valid <see cref="EntityId"/>(s) for the system.
    /// </summary>
    [Immutable]
    public static class EntityIdFactory
    {
        /// <summary>
        /// Creates and returns a new and valid order <see cref="EntityId"/> from the given inputs.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="symbol">The symbol.</param>
        /// <param name="orderCount">The order count.</param>
        /// <returns>A <see cref="EntityId"/>.</returns>
        /// <exception cref="ValidationException">Throws if the time is the default value, if the
        /// symbol is null, or if the order count is negative.</exception>
        public static EntityId Order(ZonedDateTime time, Symbol symbol, int orderCount)
        {
            Debug.NotDefault(time, nameof(time));
            Debug.NotNull(symbol, nameof(symbol));
            Debug.Int32NotOutOfRange(orderCount, nameof(orderCount), 0, int.MaxValue);

            return new EntityId($"{GetTimeString(time)}_{symbol.Code}_{orderCount}");
        }

        /// <summary>
        /// Creates and returns a new order modified <see cref="EntityId"/> from the given inputs.
        /// </summary>
        /// <param name="orderId">The modified order identifier.</param>
        /// <param name="orderIdCount">The order identifier count.</param>
        /// <returns>A <see cref="EntityId"/>.</returns>
        /// <exception cref="ValidationException">Throws if the order identifier is null, or if the
        /// order identifier count is negative.</exception>
        public static EntityId ModifiedOrderId(EntityId orderId, int orderIdCount)
        {
            Debug.NotNull(orderId, nameof(orderId));
            Debug.Int32NotOutOfRange(orderIdCount, nameof(orderIdCount), 0, int.MaxValue);

            return new EntityId($"{orderId}_R{orderIdCount}");
        }

        /// <summary>
        /// Creates and returns a new and valid signal <see cref="EntityId"/> from the given inputs.
        /// </summary>
        /// <param name="time">The signal time.</param>
        /// <param name="symbol">The signal symbol.</param>
        /// <param name="orderSide">The signal order side.</param>
        /// <param name="tradeType">The signal trade type.</param>
        /// <param name="signalLabel">The signal label.</param>
        /// <param name="signalCount">The signal count.</param>
        /// <returns>A <see cref="EntityId"/>.</returns>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if the
        /// order side is the default value, or if the signal count is negative.</exception>
        public static EntityId Signal(
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
            Debug.Int32NotOutOfRange(signalCount, nameof(signalCount), 0, int.MaxValue);

            return new EntityId($"{GetTimeString(time)}|{GetSignalIdString(symbol, tradeType, orderSide)}-{signalLabel}-{signalCount}");
        }

        /// <summary>
        /// Creates and returns a new and valid trade unit <see cref="EntityId"/> from the given
        /// inputs.
        /// </summary>
        /// <param name="tradeId">The trade identifier.</param>
        /// <param name="tradeUnit">The trade unit.</param>
        /// <returns>A <see cref="EntityId"/>.</returns>
        /// <exception cref="ValidationException">Throws if the trade identifier is null, or if the
        /// trade unit is negative.</exception>
        public static EntityId TradeUnit(EntityId tradeId, int tradeUnit)
        {
            Debug.NotNull(tradeId, nameof(tradeId));
            Debug.Int32NotOutOfRange(tradeUnit, nameof(tradeUnit), 0, int.MaxValue);

            return new EntityId($"{tradeId}_U{tradeUnit}");
        }

        /// <summary>
        /// Creates and returns a new and valid account <see cref="EntityId"/> from the given inputs.
        /// </summary>
        /// <param name="broker">The account broker.</param>
        /// <param name="accountNumber">The account number.</param>
        /// <returns>A <see cref="EntityId"/>.</returns>
        /// <exception cref="ValidationException">Throws if the account number is null or white space.</exception>
        public static EntityId Account(Broker broker, string accountNumber)
        {
            Debug.NotNull(accountNumber, nameof(accountNumber));

            return new EntityId($"{broker}-{accountNumber}");
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
