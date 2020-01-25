// -------------------------------------------------------------------------------------------------
// <copyright file="BsonInstrumentSerializer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization
{
    using System.Diagnostics.CodeAnalysis;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Identifiers;

    /// <summary>
    /// Provides a data serializer for the BSON specification.
    /// </summary>
    [SuppressMessage("ReSharper", "SA1310", Justification = "Easier to read.")]
    public class BsonInstrumentSerializer : IDataSerializer<Instrument>
    {
        /// <inheritdoc />
        public DataEncoding DataEncoding => DataEncoding.Bson;

        /// <inheritdoc />
        public byte[] Serialize(Instrument instrument)
        {
            return new BsonDocument
            {
                { nameof(Instrument.Symbol), instrument.Symbol.Value },
                { nameof(Instrument.BrokerSymbol), instrument.BrokerSymbol.Value },
                { nameof(Instrument.BaseCurrency), instrument.BaseCurrency.ToString() },
                { nameof(Instrument.SecurityType), instrument.SecurityType.ToString() },
                { nameof(Instrument.TickPrecision), instrument.TickPrecision },
                { nameof(Instrument.TickSize), instrument.TickSize },
                { nameof(Instrument.RoundLotSize), instrument.RoundLotSize },
                { nameof(Instrument.MinStopDistanceEntry), instrument.MinStopDistanceEntry },
                { nameof(Instrument.MinStopDistance), instrument.MinStopDistance },
                { nameof(Instrument.MinLimitDistanceEntry), instrument.MinLimitDistanceEntry },
                { nameof(Instrument.MinLimitDistance), instrument.MinLimitDistance },
                { nameof(Instrument.MinTradeSize), instrument.MinTradeSize },
                { nameof(Instrument.MaxTradeSize), instrument.MaxTradeSize },
                { nameof(Instrument.RolloverInterestBuy), instrument.RolloverInterestBuy },
                { nameof(Instrument.RolloverInterestSell), instrument.RolloverInterestSell },
                { nameof(Instrument.Timestamp), instrument.Timestamp.ToIsoString() },
            }.ToBson();
        }

        /// <inheritdoc />
        public Instrument Deserialize(byte[] serialized)
        {
            Debug.NotEmpty(serialized, nameof(serialized));

            var unpacked = BsonSerializer.Deserialize<BsonDocument>(serialized);

            return new Instrument(
                Symbol.FromString(unpacked[nameof(Instrument.Symbol)].AsString),
                new BrokerSymbol(unpacked[nameof(Instrument.BrokerSymbol)].AsString),
                unpacked[nameof(Instrument.BaseCurrency)].AsString.ToEnum<Currency>(),
                unpacked[nameof(Instrument.SecurityType)].AsString.ToEnum<SecurityType>(),
                unpacked[nameof(Instrument.TickPrecision)].AsInt32,
                unpacked[nameof(Instrument.TickSize)].AsDecimal,
                unpacked[nameof(Instrument.RoundLotSize)].AsInt32,
                unpacked[nameof(Instrument.MinStopDistanceEntry)].AsInt32,
                unpacked[nameof(Instrument.MinLimitDistanceEntry)].AsInt32,
                unpacked[nameof(Instrument.MinStopDistance)].AsInt32,
                unpacked[nameof(Instrument.MinLimitDistance)].AsInt32,
                unpacked[nameof(Instrument.MinTradeSize)].AsInt32,
                unpacked[nameof(Instrument.MaxTradeSize)].AsInt32,
                unpacked[nameof(Instrument.RolloverInterestBuy)].AsDecimal,
                unpacked[nameof(Instrument.RolloverInterestSell)].AsDecimal,
                unpacked[nameof(Instrument.Timestamp)].AsString.ToZonedDateTimeFromIso());
        }
    }
}
