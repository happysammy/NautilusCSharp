// -------------------------------------------------------------------------------------------------
// <copyright file="ObjectExtractor.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization.Internal
{
    using System;
    using MsgPack;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel;
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
        private const string NONE = nameof(NONE);

        /// <summary>
        /// Returns a <see cref="decimal"/> extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted <see cref="decimal"/>.</returns>
        internal static decimal Decimal(MessagePackObject unpacked)
        {
            return Convert.ToDecimal(unpacked.ToString());
        }

        /// <summary>
        /// Returns a Guid extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted Guid.</returns>
        internal static Guid Guid(MessagePackObject unpacked)
        {
            return System.Guid.Parse(unpacked.ToString());
        }

        /// <summary>
        /// Returns a Symbol extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted Symbol.</returns>
        internal static Symbol Symbol(MessagePackObjectDictionary unpacked)
        {
            return Nautilus.DomainModel.Identifiers.Symbol.FromString(unpacked[nameof(Symbol)].ToString());
        }

        /// <summary>
        /// Returns a BarSpecification extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted BarSpecification.</returns>
        internal static BarSpecification BarSpecification(MessagePackObjectDictionary unpacked)
        {
            return DomainObjectParser.ParseBarSpecification(unpacked[nameof(BarSpecification)].ToString());
        }

        /// <summary>
        /// Returns a TraderId extracted from the given <see cref="MessagePackObjectDictionary"/>.
        /// </summary>
        /// <param name="unpacked">The dictionary to extract from.</param>
        /// <returns>The extracted TraderId.</returns>
        internal static TraderId TraderId(MessagePackObjectDictionary unpacked)
        {
            return new TraderId(unpacked[nameof(TraderId)].ToString());
        }

        /// <summary>
        /// Returns a StrategyId extracted from the given <see cref="MessagePackObjectDictionary"/>.
        /// </summary>
        /// <param name="unpacked">The dictionary to extract from.</param>
        /// <returns>The extracted StrategyId.</returns>
        internal static StrategyId StrategyId(MessagePackObjectDictionary unpacked)
        {
            return new StrategyId(unpacked[nameof(StrategyId)].ToString());
        }

        /// <summary>
        /// Returns a PositionId extracted from the given <see cref="MessagePackObjectDictionary"/>.
        /// </summary>
        /// <param name="unpacked">The dictionary to extract from.</param>
        /// <returns>The extracted PositionId.</returns>
        internal static PositionId PositionId(MessagePackObjectDictionary unpacked)
        {
            return new PositionId(unpacked.ToString());
        }

        /// <summary>
        /// Returns an OrderId extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted OrderId.</returns>
        internal static OrderId OrderId(MessagePackObject unpacked)
        {
            return new OrderId(unpacked.ToString());
        }

        /// <summary>
        /// Returns an ExecutionId extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted ExecutionId.</returns>
        internal static ExecutionId ExecutionId(MessagePackObjectDictionary unpacked)
        {
            return new ExecutionId(unpacked[nameof(ExecutionId)].ToString());
        }

        /// <summary>
        /// Returns an ExecutionTicket extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted ExecutionTicket.</returns>
        internal static ExecutionTicket ExecutionTicket(MessagePackObjectDictionary unpacked)
        {
            return new ExecutionTicket(unpacked[nameof(ExecutionTicket)].ToString());
        }

        /// <summary>
        /// Returns a Label extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted Label.</returns>
        internal static Label Label(MessagePackObjectDictionary unpacked)
        {
            return new Label(unpacked[nameof(Label)].ToString());
        }

        /// <summary>
        /// Returns an enumerator extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <typeparam name="TEnum">The enumerator type.</typeparam>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted <see cref="Enum"/>.</returns>
        internal static TEnum Enum<TEnum>(MessagePackObject unpacked)
            where TEnum : struct
        {
            return unpacked.ToString().ToEnum<TEnum>();
        }

        /// <summary>
        /// Returns a Quantity extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted Quantity.</returns>
        internal static Quantity Quantity(MessagePackObject unpacked)
        {
            return DomainModel.ValueObjects.Quantity.Create(unpacked.AsInt32());
        }

        /// <summary>
        /// Returns a Money extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <param name="currency">The currency.</param>
        /// <returns>The extracted Money.</returns>
        internal static Money Money(MessagePackObject unpacked, Currency currency)
        {
            return DomainModel.ValueObjects.Money.Create(Convert.ToDecimal(unpacked.ToString()), currency);
        }

        /// <summary>
        /// Returns a Price extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted Price.</returns>
        internal static Price Price(MessagePackObject unpacked)
        {
            return DomainModel.ValueObjects.Price.Create(Convert.ToDecimal(unpacked.ToString()));
        }

        /// <summary>
        /// Returns a Price? extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted Price?.</returns>
        internal static Price? NullablePrice(MessagePackObject unpacked)
        {
            var unpackedString = unpacked.ToString();
            return unpackedString == NONE
                ? null
                : DomainModel.ValueObjects.Price.Create(Convert.ToDecimal(unpackedString));
        }

        /// <summary>
        /// Returns a <see cref="decimal"/> extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted <see cref="decimal"/>.</returns>
        internal static ZonedDateTime ZonedDateTime(MessagePackObject unpacked)
        {
            return unpacked.ToString().ToZonedDateTimeFromIso();
        }

        /// <summary>
        /// Returns a <see cref="NodaTime.ZonedDateTime"/>? extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted <see cref="NodaTime.ZonedDateTime"/>?.</returns>
        internal static ZonedDateTime? NullableZonedDateTime(MessagePackObject unpacked)
        {
            var unpackedString = unpacked.ToString();
            return unpackedString == NONE
                ? null
                : unpackedString.ToNullableZonedDateTimeFromIso();
        }
    }
}
