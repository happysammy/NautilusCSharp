// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackInstrumentSerializer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization
{
    using System.Globalization;
    using MsgPack;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.Serialization.Internal;

    /// <summary>
    /// Provides an <see cref="Instrument"/> binary serializer for the MessagePack specification.
    /// </summary>
    public sealed class MsgPackInstrumentSerializer : IInstrumentSerializer
    {
        /// <inheritdoc />
        public byte[] Serialize(Instrument instrument)
        {
            return MsgPackSerializer.Serialize(new MessagePackObjectDictionary
            {
                { Key.Id, instrument.Id.ToString() },
                { Key.Symbol, instrument.Symbol.ToString() },
                { Key.BrokerSymbol, instrument.BrokerSymbol.ToString() },
                { Key.QuoteCurrency, instrument.QuoteCurrency.ToString() },
                { Key.SecurityType, instrument.SecurityType.ToString() },
                { Key.TickPrecision, instrument.TickPrecision },
                { Key.TickSize, instrument.TickSize.ToString(CultureInfo.InvariantCulture) },
                { Key.RoundLotSize, instrument.RoundLotSize },
                { Key.MinStopDistanceEntry, instrument.MinStopDistanceEntry },
                { Key.MinStopDistance, instrument.MinStopDistance },
                { Key.MinLimitDistanceEntry, instrument.MinLimitDistanceEntry },
                { Key.MinLimitDistance, instrument.MinLimitDistance },
                { Key.MinTradeSize, instrument.MinTradeSize },
                { Key.MaxTradeSize, instrument.MaxTradeSize },
                { Key.RolloverInterestBuy, instrument.RolloverInterestBuy.ToString(CultureInfo.InvariantCulture) },
                { Key.RolloverInterestSell, instrument.RolloverInterestSell.ToString(CultureInfo.InvariantCulture) },
                { Key.Timestamp, instrument.Timestamp.ToIsoString() },
            });
        }

        /// <inheritdoc />
        public Instrument Deserialize(byte[] serialized)
        {
            var unpacked = MsgPackSerializer.Deserialize<MessagePackObjectDictionary>(serialized);

            return new Instrument(
                ObjectExtractor.InstrumentId(unpacked[Key.Id]),
                ObjectExtractor.Symbol(unpacked[Key.Symbol]),
                ObjectExtractor.BrokerSymbol(unpacked[Key.BrokerSymbol]),
                ObjectExtractor.Enum<Currency>(unpacked[Key.QuoteCurrency]),
                ObjectExtractor.Enum<SecurityType>(unpacked[Key.SecurityType]),
                unpacked[Key.TickPrecision].AsInt32(),
                ObjectExtractor.Decimal(unpacked[Key.TickSize]),
                unpacked[Key.RoundLotSize].AsInt32(),
                unpacked[Key.MinStopDistanceEntry].AsInt32(),
                unpacked[Key.MinLimitDistanceEntry].AsInt32(),
                unpacked[Key.MinStopDistance].AsInt32(),
                unpacked[Key.MinLimitDistance].AsInt32(),
                unpacked[Key.MinTradeSize].AsInt32(),
                unpacked[Key.MaxTradeSize].AsInt32(),
                ObjectExtractor.Decimal(unpacked[Key.RolloverInterestBuy]),
                ObjectExtractor.Decimal(unpacked[Key.RolloverInterestSell]),
                ObjectExtractor.ZonedDateTime(unpacked[Key.Timestamp]));
        }
    }
}
