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
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// A factory which creates valid <see cref="OrderId"/>(s) for the system.
    /// </summary>
    [Stateless]
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
            Debug.PositiveInt32(orderCount, nameof(orderCount));

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
            Debug.PositiveInt32(orderIdCount, nameof(orderIdCount));

            return new OrderId($"{orderId}_R{orderIdCount}");
        }

        /// <summary>
        /// Creates and returns a new and valid account <see cref="AccountId"/> from the given inputs.
        /// </summary>
        /// <param name="broker">The account broker.</param>
        /// <param name="accountNumber">The account number.</param>
        /// <returns>A <see cref="AccountId"/>.</returns>
        public static AccountId Account(Brokerage broker, string accountNumber)
        {
            Debug.NotNull(accountNumber, nameof(accountNumber));

            return new AccountId($"{broker}-{accountNumber}");
        }

        private static string GetTimeString(ZonedDateTime time)
        {
            Debug.NotDefault(time, nameof(time));

            return time.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
        }
    }
}
