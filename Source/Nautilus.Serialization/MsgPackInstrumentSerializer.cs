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
                { nameof(Instrument.Id), instrument.Id.ToString() },
                { nameof(Instrument.Symbol), instrument.Symbol.ToString() },
                { nameof(Instrument.BrokerSymbol), instrument.BrokerSymbol.ToString() },
                { nameof(Instrument.QuoteCurrency), instrument.QuoteCurrency.ToString() },
                { nameof(Instrument.SecurityType), instrument.SecurityType.ToString() },
                { nameof(Instrument.TickPrecision), instrument.TickPrecision },
                { nameof(Instrument.TickSize), instrument.TickSize.ToString(CultureInfo.InvariantCulture) },
                { nameof(Instrument.RoundLotSize), instrument.RoundLotSize },
                { nameof(Instrument.MinStopDistanceEntry), instrument.MinStopDistanceEntry },
                { nameof(Instrument.MinStopDistance), instrument.MinStopDistance },
                { nameof(Instrument.MinLimitDistanceEntry), instrument.MinLimitDistanceEntry },
                { nameof(Instrument.MinLimitDistance), instrument.MinLimitDistance },
                { nameof(Instrument.MinTradeSize), instrument.MinTradeSize },
                { nameof(Instrument.MaxTradeSize), instrument.MaxTradeSize },
                { nameof(Instrument.RolloverInterestBuy), instrument.RolloverInterestBuy.ToString(CultureInfo.InvariantCulture) },
                { nameof(Instrument.RolloverInterestSell), instrument.RolloverInterestSell.ToString(CultureInfo.InvariantCulture) },
                { nameof(Instrument.Timestamp), instrument.Timestamp.ToIsoString() },
            });
        }

        /// <inheritdoc />
        public Instrument Deserialize(byte[] serialized)
        {
            var unpacked = MsgPackSerializer.Deserialize<MessagePackObjectDictionary>(serialized);

            return new Instrument(
                ObjectExtractor.InstrumentId(unpacked[nameof(Instrument.Id)]),
                ObjectExtractor.Symbol(unpacked[nameof(Instrument.Symbol)]),
                ObjectExtractor.BrokerSymbol(unpacked[nameof(Instrument.BrokerSymbol)]),
                ObjectExtractor.Enum<Currency>(unpacked[nameof(Instrument.QuoteCurrency)]),
                ObjectExtractor.Enum<SecurityType>(unpacked[nameof(Instrument.SecurityType)]),
                unpacked[nameof(Instrument.TickPrecision)].AsInt32(),
                ObjectExtractor.Decimal(unpacked[nameof(Instrument.TickSize)]),
                unpacked[nameof(Instrument.RoundLotSize)].AsInt32(),
                unpacked[nameof(Instrument.MinStopDistanceEntry)].AsInt32(),
                unpacked[nameof(Instrument.MinLimitDistanceEntry)].AsInt32(),
                unpacked[nameof(Instrument.MinStopDistance)].AsInt32(),
                unpacked[nameof(Instrument.MinLimitDistance)].AsInt32(),
                unpacked[nameof(Instrument.MinTradeSize)].AsInt32(),
                unpacked[nameof(Instrument.MaxTradeSize)].AsInt32(),
                ObjectExtractor.Decimal(unpacked[nameof(Instrument.RolloverInterestBuy)]),
                ObjectExtractor.Decimal(unpacked[nameof(Instrument.RolloverInterestSell)]),
                ObjectExtractor.ZonedDateTime(unpacked[nameof(Instrument.Timestamp)]));
        }
    }
}
