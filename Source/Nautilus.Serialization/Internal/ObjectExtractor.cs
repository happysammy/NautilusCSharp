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
    using MsgPack;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Types;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
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
            return Convert.ToDecimal(unpacked.AsString());
        }

        /// <summary>
        /// Returns a Guid extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted Guid.</returns>
        internal static Guid Guid(MessagePackObject unpacked)
        {
            return System.Guid.Parse(unpacked.AsString());
        }

        /// <summary>
        /// Returns a Symbol extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted Symbol.</returns>
        internal static Symbol Symbol(MessagePackObjectDictionary unpacked)
        {
            return Nautilus.DomainModel.Identifiers.Symbol.FromString(unpacked[nameof(Symbol)].AsString());
        }

        /// <summary>
        /// Returns a BarSpecification extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted BarSpecification.</returns>
        internal static BarSpecification BarSpecification(MessagePackObjectDictionary unpacked)
        {
            return DomainModel.ValueObjects.BarSpecification.FromString(unpacked[nameof(BarSpecification)].AsString());
        }

        /// <summary>
        /// Returns a TraderId extracted from the given <see cref="MessagePackObjectDictionary"/>.
        /// </summary>
        /// <param name="unpacked">The dictionary to extract from.</param>
        /// <returns>The extracted TraderId.</returns>
        internal static TraderId TraderId(MessagePackObjectDictionary unpacked)
        {
            return Nautilus.DomainModel.Identifiers.TraderId.FromString(unpacked[nameof(TraderId)].AsString());
        }

        /// <summary>
        /// Returns a StrategyId extracted from the given <see cref="MessagePackObjectDictionary"/>.
        /// </summary>
        /// <param name="unpacked">The dictionary to extract from.</param>
        /// <returns>The extracted StrategyId.</returns>
        internal static StrategyId StrategyId(MessagePackObjectDictionary unpacked)
        {
            return Nautilus.DomainModel.Identifiers.StrategyId.FromString(unpacked[nameof(StrategyId)].AsString());
        }

        /// <summary>
        /// Returns an AccountId extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted AccountId.</returns>
        internal static AccountId AccountId(MessagePackObjectDictionary unpacked)
        {
            return Nautilus.DomainModel.Identifiers.AccountId.FromString(unpacked[nameof(AccountId)].AsString());
        }

        /// <summary>
        /// Returns a PositionId extracted from the given <see cref="MessagePackObjectDictionary"/>.
        /// </summary>
        /// <param name="unpacked">The dictionary to extract from.</param>
        /// <returns>The extracted PositionId.</returns>
        internal static PositionId PositionId(MessagePackObjectDictionary unpacked)
        {
            return new PositionId(unpacked[nameof(PositionId)].AsString());
        }

        /// <summary>
        /// Returns an OrderId extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <param name="orderField">The order identifier extraction field.</param>
        /// <returns>The extracted OrderId.</returns>
        internal static OrderId OrderId(MessagePackObjectDictionary unpacked, string orderField = nameof(OrderId))
        {
            return new OrderId(unpacked[orderField].AsString());
        }

        /// <summary>
        /// Returns an OrderIdBroker extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted OrderIdBroker.</returns>
        internal static OrderIdBroker OrderIdBroker(MessagePackObjectDictionary unpacked)
        {
            return new OrderIdBroker(unpacked[nameof(OrderIdBroker)].AsString());
        }

        /// <summary>
        /// Returns an ExecutionId extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted ExecutionId.</returns>
        internal static ExecutionId ExecutionId(MessagePackObjectDictionary unpacked)
        {
            return new ExecutionId(unpacked[nameof(ExecutionId)].AsString());
        }

        /// <summary>
        /// Returns an ExecutionTicket extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted ExecutionTicket.</returns>
        internal static PositionIdBroker PositionIdBroker(MessagePackObjectDictionary unpacked)
        {
            return new PositionIdBroker(unpacked[nameof(PositionIdBroker)].AsString());
        }

        /// <summary>
        /// Returns a Label extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted Label.</returns>
        internal static Label Label(MessagePackObjectDictionary unpacked)
        {
            return new Label(unpacked[nameof(Label)].AsString());
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
            return DomainModel.ValueObjects.Quantity.Create(unpacked.AsString());
        }

        /// <summary>
        /// Returns a Money extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <param name="currency">The currency.</param>
        /// <returns>The extracted Money.</returns>
        internal static Money Money(MessagePackObject unpacked, Currency currency)
        {
            return DomainModel.ValueObjects.Money.Create(Convert.ToDecimal(unpacked.AsString()), currency);
        }

        /// <summary>
        /// Returns a Price extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted Price.</returns>
        internal static Price Price(MessagePackObject unpacked)
        {
            return DomainModel.ValueObjects.Price.Create(Convert.ToDecimal(unpacked.AsString()));
        }

        /// <summary>
        /// Returns a Price? extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted Price?.</returns>
        internal static Price? NullablePrice(MessagePackObject unpacked)
        {
            var unpackedString = unpacked.AsString();
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
            return unpacked.AsString().ToZonedDateTimeFromIso();
        }

        /// <summary>
        /// Returns a <see cref="NodaTime.ZonedDateTime"/>? extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted <see cref="NodaTime.ZonedDateTime"/>?.</returns>
        internal static ZonedDateTime? NullableZonedDateTime(MessagePackObject unpacked)
        {
            var unpackedString = unpacked.AsString();
            return unpackedString == NONE
                ? null
                : unpackedString.ToNullableZonedDateTimeFromIso();
        }
    }
}
