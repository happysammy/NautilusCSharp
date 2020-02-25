// -------------------------------------------------------------------------------------------------
// <copyright file="ObjectDeserializer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization.MessageSerializers.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Types;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Network.Identifiers;
    using NodaTime;

    /// <summary>
    /// Provides methods for extracting objects from unpacked dictionaries.
    /// </summary>
    internal static class ObjectDeserializer
    {
        private const string None = nameof(None);

        private static readonly Func<byte[], string> Decode = Encoding.UTF8.GetString;

        /// <summary>
        /// Returns a <see cref="string"/> extracted from the given unpacked bytes.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted <see cref="decimal"/>.</returns>
        internal static string AsString(byte[] unpacked)
        {
            return Decode(unpacked);
        }

        /// <summary>
        /// Returns a <see cref="decimal"/> extracted from the given unpacked object.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted <see cref="decimal"/>.</returns>
        internal static decimal AsDecimal(byte[] unpacked)
        {
            return Convert.ToDecimal(Decode(unpacked));
        }

        /// <summary>
        /// Returns a Guid extracted from the given unpacked dictionary.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted Guid.</returns>
        internal static Guid AsGuid(byte[] unpacked)
        {
            return Guid.Parse(Decode(unpacked));
        }

        /// <summary>
        /// Returns a ServerId extracted from the given unpacked dictionary.
        /// </summary>
        /// <param name="unpacked">The dictionary to extract from.</param>
        /// <returns>The extracted ClientId.</returns>
        internal static ClientId AsClientId(Dictionary<string, byte[]> unpacked)
        {
            return new ClientId(Decode(unpacked[nameof(ClientId)]));
        }

        /// <summary>
        /// Returns a ServerId extracted from the given unpacked dictionary.
        /// </summary>
        /// <param name="unpacked">The dictionary to extract from.</param>
        /// <returns>The extracted ServerId.</returns>
        internal static ServerId AsServerId(Dictionary<string, byte[]> unpacked)
        {
            return new ServerId(Decode(unpacked[nameof(ServerId)]));
        }

        /// <summary>
        /// Returns a SessionId extracted from the given unpacked dictionary.
        /// </summary>
        /// <param name="unpacked">The dictionary to extract from.</param>
        /// <returns>The extracted SessionId.</returns>
        internal static SessionId AsSessionId(Dictionary<string, byte[]> unpacked)
        {
            return new SessionId(Decode(unpacked[nameof(SessionId)]));
        }

        /// <summary>
        /// Returns a TraderId extracted from the given unpacked dictionary.
        /// </summary>
        /// <param name="unpacked">The dictionary to extract from.</param>
        /// <returns>The extracted TraderId.</returns>
        internal static TraderId AsTraderId(Dictionary<string, byte[]> unpacked)
        {
            return TraderId.FromString(Decode(unpacked[nameof(TraderId)]));
        }

        /// <summary>
        /// Returns a PositionId extracted from the given unpacked dictionary.
        /// </summary>
        /// <param name="unpacked">The dictionary to extract from.</param>
        /// <returns>The extracted PositionId.</returns>
        internal static PositionId AsPositionId(Dictionary<string, byte[]> unpacked)
        {
            return new PositionId(Decode(unpacked[nameof(PositionId)]));
        }

        /// <summary>
        /// Returns an OrderId extracted from the given unpacked dictionary.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <param name="orderField">The order identifier extraction field.</param>
        /// <returns>The extracted OrderId.</returns>
        internal static OrderId AsOrderId(Dictionary<string, byte[]> unpacked, string orderField = nameof(OrderId))
        {
            return new OrderId(Decode(unpacked[orderField]));
        }

        /// <summary>
        /// Returns an OrderIdBroker extracted from the given unpacked dictionary.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted OrderIdBroker.</returns>
        internal static OrderIdBroker AsOrderIdBroker(Dictionary<string, byte[]> unpacked)
        {
            return new OrderIdBroker(Decode(unpacked[nameof(OrderIdBroker)]));
        }

        /// <summary>
        /// Returns an ExecutionId extracted from the given unpacked dictionary.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted ExecutionId.</returns>
        internal static ExecutionId AsExecutionId(Dictionary<string, byte[]> unpacked)
        {
            return new ExecutionId(Decode(unpacked[nameof(ExecutionId)]));
        }

        /// <summary>
        /// Returns an ExecutionTicket extracted from the given unpacked dictionary.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted ExecutionTicket.</returns>
        internal static PositionIdBroker AsPositionIdBroker(Dictionary<string, byte[]> unpacked)
        {
            return new PositionIdBroker(Decode(unpacked[nameof(PositionIdBroker)]));
        }

        /// <summary>
        /// Returns a Label extracted from the given unpacked dictionary.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted Label.</returns>
        internal static Label AsLabel(Dictionary<string, byte[]> unpacked)
        {
            return new Label(Decode(unpacked[nameof(Label)]));
        }

        /// <summary>
        /// Returns an enumerator extracted from the given unpacked dictionary.
        /// </summary>
        /// <typeparam name="TEnum">The enumerator type.</typeparam>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted <see cref="Enum"/>.</returns>
        internal static TEnum AsEnum<TEnum>(byte[] unpacked)
            where TEnum : struct
        {
            return Decode(unpacked).ToEnum<TEnum>();
        }

        /// <summary>
        /// Returns a Quantity extracted from the given unpacked dictionary.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted Quantity.</returns>
        internal static Quantity AsQuantity(byte[] unpacked)
        {
            return Quantity.Create(Decode(unpacked));
        }

        /// <summary>
        /// Returns a Money extracted from the given unpacked dictionary.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <param name="currency">The currency.</param>
        /// <returns>The extracted Money.</returns>
        internal static Money AsMoney(byte[] unpacked, Currency currency)
        {
            return Money.Create(Convert.ToDecimal(Decode(unpacked)), currency);
        }

        /// <summary>
        /// Returns a Price extracted from the given unpacked dictionary.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted Price.</returns>
        internal static Price AsPrice(byte[] unpacked)
        {
            return Price.Create(Convert.ToDecimal(Decode(unpacked)));
        }

        /// <summary>
        /// Returns a Price? extracted from the given unpacked dictionary.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted Price?.</returns>
        internal static Price? AsNullablePrice(byte[] unpacked)
        {
            var unpackedString = Decode(unpacked);
            return unpackedString == None
                ? null
                : Price.Create(Convert.ToDecimal(unpackedString));
        }

        /// <summary>
        /// Returns a <see cref="decimal"/> extracted from the given unpacked dictionary.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted <see cref="decimal"/>.</returns>
        internal static ZonedDateTime AsZonedDateTime(byte[] unpacked)
        {
            return Decode(unpacked).ToZonedDateTimeFromIso();
        }

        /// <summary>
        /// Returns a <see cref="NodaTime.ZonedDateTime"/>? extracted from the given unpacked dictionary.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted <see cref="NodaTime.ZonedDateTime"/>?.</returns>
        internal static ZonedDateTime? AsNullableZonedDateTime(byte[] unpacked)
        {
            var unpackedString = Decode(unpacked);
            return unpackedString == None
                ? null
                : unpackedString.ToNullableZonedDateTimeFromIso();
        }
    }
}
