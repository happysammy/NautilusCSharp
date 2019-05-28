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
        /// Returns a <see cref="decimal"/> extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="packed">The MessagePack object to extract from.</param>
        /// <returns>The extracted <see cref="decimal"/>.</returns>
        internal static decimal Decimal(MessagePackObject packed)
        {
            return Convert.ToDecimal(packed.ToString());
        }

        /// <summary>
        /// Returns a Guid extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="packed">The MessagePack object to extract from.</param>
        /// <returns>The extracted Guid.</returns>
        internal static Guid Guid(MessagePackObject packed)
        {
            return System.Guid.Parse(packed.ToString());
        }

        /// <summary>
        /// Returns a Symbol extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="packed">The MessagePack object to extract from.</param>
        /// <returns>The extracted Symbol.</returns>
        internal static Symbol Symbol(MessagePackObject packed)
        {
            return DomainModel.ValueObjects.Symbol.Create(packed.ToString());
        }

        /// <summary>
        /// Returns a BrokerSymbol extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="packed">The MessagePack object to extract from.</param>
        /// <returns>The extracted BrokerSymbol.</returns>
        internal static BrokerSymbol BrokerSymbol(MessagePackObject packed)
        {
            return new BrokerSymbol(packed.ToString());
        }

        /// <summary>
        /// Returns a TraderId extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="packed">The MessagePack object to extract from.</param>
        /// <returns>The extracted TraderId.</returns>
        internal static TraderId TraderId(MessagePackObject packed)
        {
            return new TraderId(packed.ToString());
        }

        /// <summary>
        /// Returns a StrategyId extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="packed">The MessagePack object to extract from.</param>
        /// <returns>The extracted StrategyId.</returns>
        internal static StrategyId StrategyId(MessagePackObject packed)
        {
            return new StrategyId(packed.ToString());
        }

        /// <summary>
        /// Returns a PositionId extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="packed">The MessagePack object to extract from.</param>
        /// <returns>The extracted PositionId.</returns>
        internal static PositionId PositionId(MessagePackObject packed)
        {
            return new PositionId(packed.ToString());
        }

        /// <summary>
        /// Returns an OrderId extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="packed">The MessagePack object to extract from.</param>
        /// <returns>The extracted OrderId.</returns>
        internal static OrderId OrderId(MessagePackObject packed)
        {
            return new OrderId(packed.ToString());
        }

        /// <summary>
        /// Returns an ExecutionId extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="packed">The MessagePack object to extract from.</param>
        /// <returns>The extracted ExecutionId.</returns>
        internal static ExecutionId ExecutionId(MessagePackObject packed)
        {
            return new ExecutionId(packed.ToString());
        }

        /// <summary>
        /// Returns an InstrumentId extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="packed">The MessagePack object to extract from.</param>
        /// <returns>The extracted InstrumentId.</returns>
        internal static InstrumentId InstrumentId(MessagePackObject packed)
        {
            return new InstrumentId(packed.ToString());
        }

        /// <summary>
        /// Returns an ExecutionTicket extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="packed">The MessagePack object to extract from.</param>
        /// <returns>The extracted ExecutionTicket.</returns>
        internal static ExecutionTicket ExecutionTicket(MessagePackObject packed)
        {
            return new ExecutionTicket(packed.ToString());
        }

        /// <summary>
        /// Returns a Label extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="packed">The MessagePack object to extract from.</param>
        /// <returns>The extracted Label.</returns>
        internal static Label Label(MessagePackObject packed)
        {
            return new Label(packed.ToString());
        }

        /// <summary>
        /// Returns an enumerator extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <typeparam name="TEnum">The enumerator type.</typeparam>
        /// <param name="packed">The MessagePack object to extract from.</param>
        /// <returns>The extracted <see cref="Enum"/>.</returns>
        internal static TEnum Enum<TEnum>(MessagePackObject packed)
            where TEnum : struct
        {
            return packed.ToString().ToEnum<TEnum>();
        }

        /// <summary>
        /// Returns a Quantity extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="packed">The MessagePack object to extract from.</param>
        /// <returns>The extracted Quantity.</returns>
        internal static Quantity Quantity(MessagePackObject packed)
        {
            return DomainModel.ValueObjects.Quantity.Create(packed.AsInt32());
        }

        /// <summary>
        /// Returns a Money extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="packed">The MessagePack object to extract from.</param>
        /// <param name="currency">The currency.</param>
        /// <returns>The extracted Money.</returns>
        internal static Money Money(MessagePackObject packed, Currency currency)
        {
            return DomainModel.ValueObjects.Money.Create(Convert.ToDecimal(packed.ToString()), currency);
        }

        /// <summary>
        /// Returns a Price extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="packed">The MessagePack object to extract from.</param>
        /// <returns>The extracted Price.</returns>
        internal static Price Price(MessagePackObject packed)
        {
            Debug.NotEmptyOrWhiteSpace(packed.ToString(), nameof(packed));

            return DomainModel.ValueObjects.Price.Create(Convert.ToDecimal(packed.ToString()));
        }

        /// <summary>
        /// Returns a Price? extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="packed">The MessagePack object to extract from.</param>
        /// <returns>The extracted Price?.</returns>
        internal static Price? NullablePrice(MessagePackObject packed)
        {
            return packed.Equals(MessagePackObject.Nil)
                ? null
                : DomainModel.ValueObjects.Price.Create(Convert.ToDecimal(packed.ToString()));
        }

        /// <summary>
        /// Returns a <see cref="decimal"/> extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="packed">The MessagePack object to extract from.</param>
        /// <returns>The extracted <see cref="decimal"/>.</returns>
        internal static ZonedDateTime ZonedDateTime(MessagePackObject packed)
        {
            return packed.ToString().ToZonedDateTimeFromIso();
        }

        /// <summary>
        /// Returns a <see cref="NodaTime.ZonedDateTime"/>? extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="packed">The MessagePack object to extract from.</param>
        /// <returns>The extracted <see cref="NodaTime.ZonedDateTime"/>?.</returns>
        internal static ZonedDateTime? NullableZonedDateTime(MessagePackObject packed)
        {
            return packed == MessagePackObject.Nil
                ? null
                : packed.ToString().ToNullableZonedDateTimeFromIso();
        }
    }
}
