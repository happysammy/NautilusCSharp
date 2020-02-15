// -------------------------------------------------------------------------------------------------
// <copyright file="ObjectExtractor.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization.Internal
{
    using System;
    using System.Collections.Generic;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Types;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

#pragma warning disable CS8604
    /// <summary>
    /// Provides methods for extracting objects from unpacked dictionaries.
    /// </summary>
    internal static class ObjectExtractor
    {
        private const string None = nameof(None);

        /// <summary>
        /// Returns a <see cref="decimal"/> extracted from the given unpacked object.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted <see cref="decimal"/>.</returns>
        internal static decimal AsDecimal(object unpacked)
        {
            if (unpacked is null)
            {
                throw new ArgumentNullException(nameof(unpacked), "The unpacked argument was null.");
            }

            return Convert.ToDecimal(unpacked);
        }

        /// <summary>
        /// Returns a Guid extracted from the given unpacked dictionary.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted Guid.</returns>
        internal static Guid AsGuid(object unpacked)
        {
            if (unpacked is null)
            {
                throw new ArgumentNullException(nameof(unpacked), "The unpacked argument was null.");
            }

            return Guid.Parse(unpacked.ToString());
        }

        /// <summary>
        /// Returns a PositionId extracted from the given unpacked dictionary.
        /// </summary>
        /// <param name="unpacked">The dictionary to extract from.</param>
        /// <returns>The extracted PositionId.</returns>
        internal static PositionId AsPositionId(Dictionary<string, object> unpacked)
        {
            if (unpacked is null)
            {
                throw new ArgumentNullException(nameof(unpacked), "The unpacked argument was null.");
            }

            return new PositionId(unpacked[nameof(PositionId)].ToString());
        }

        /// <summary>
        /// Returns an OrderId extracted from the given unpacked dictionary.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <param name="orderField">The order identifier extraction field.</param>
        /// <returns>The extracted OrderId.</returns>
        internal static OrderId AsOrderId(Dictionary<string, object> unpacked, string orderField = nameof(OrderId))
        {
            return new OrderId(unpacked[orderField].ToString());
        }

        /// <summary>
        /// Returns an OrderIdBroker extracted from the given unpacked dictionary.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted OrderIdBroker.</returns>
        internal static OrderIdBroker AsOrderIdBroker(Dictionary<string, object> unpacked)
        {
            return new OrderIdBroker(unpacked[nameof(OrderIdBroker)].ToString());
        }

        /// <summary>
        /// Returns an ExecutionId extracted from the given unpacked dictionary.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted ExecutionId.</returns>
        internal static ExecutionId AsExecutionId(Dictionary<string, object> unpacked)
        {
            return new ExecutionId(unpacked[nameof(ExecutionId)].ToString());
        }

        /// <summary>
        /// Returns an ExecutionTicket extracted from the given unpacked dictionary.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted ExecutionTicket.</returns>
        internal static PositionIdBroker AsPositionIdBroker(Dictionary<string, object> unpacked)
        {
            return new PositionIdBroker(unpacked[nameof(PositionIdBroker)].ToString());
        }

        /// <summary>
        /// Returns a Label extracted from the given unpacked dictionary.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted Label.</returns>
        internal static Label AsLabel(Dictionary<string, object> unpacked)
        {
            return new Label(unpacked[nameof(Label)].ToString());
        }

        /// <summary>
        /// Returns an enumerator extracted from the given unpacked dictionary.
        /// </summary>
        /// <typeparam name="TEnum">The enumerator type.</typeparam>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted <see cref="Enum"/>.</returns>
        internal static TEnum AsEnum<TEnum>(object unpacked)
            where TEnum : struct
        {
            return unpacked.ToString().ToEnum<TEnum>();
        }

        /// <summary>
        /// Returns a Quantity extracted from the given unpacked dictionary.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted Quantity.</returns>
        internal static Quantity AsQuantity(object unpacked)
        {
            return Quantity.Create(unpacked.ToString());
        }

        /// <summary>
        /// Returns a Money extracted from the given unpacked dictionary.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <param name="currency">The currency.</param>
        /// <returns>The extracted Money.</returns>
        internal static Money AsMoney(object unpacked, Currency currency)
        {
            return Money.Create(Convert.ToDecimal(unpacked.ToString()), currency);
        }

        /// <summary>
        /// Returns a Price extracted from the given unpacked dictionary.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted Price.</returns>
        internal static Price AsPrice(object unpacked)
        {
            return Price.Create(Convert.ToDecimal(unpacked.ToString()));
        }

        /// <summary>
        /// Returns a Price? extracted from the given unpacked dictionary.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted Price?.</returns>
        internal static Price? AsNullablePrice(object unpacked)
        {
            var unpackedString = unpacked.ToString();
            return unpackedString == None
                ? null
                : Price.Create(Convert.ToDecimal(unpackedString));
        }

        /// <summary>
        /// Returns a <see cref="decimal"/> extracted from the given unpacked dictionary.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted <see cref="decimal"/>.</returns>
        internal static ZonedDateTime AsZonedDateTime(object unpacked)
        {
            return unpacked.ToString().ToZonedDateTimeFromIso();
        }

        /// <summary>
        /// Returns a <see cref="NodaTime.ZonedDateTime"/>? extracted from the given unpacked dictionary.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted <see cref="NodaTime.ZonedDateTime"/>?.</returns>
        internal static ZonedDateTime? AsNullableZonedDateTime(object unpacked)
        {
            var unpackedString = unpacked.ToString();
            return unpackedString == None
                ? null
                : unpackedString.ToNullableZonedDateTimeFromIso();
        }
    }
}
