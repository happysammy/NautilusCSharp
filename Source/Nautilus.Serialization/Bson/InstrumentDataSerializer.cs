// -------------------------------------------------------------------------------------------------
// <copyright file="InstrumentDataSerializer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization.Bson
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;

    /// <inheritdoc />
    public sealed class InstrumentDataSerializer : IDataSerializer<Instrument>
    {
        private const string Data = nameof(Data);
        private const string DataType = nameof(DataType);
        private const string MetaData = nameof(MetaData);

        /// <inheritdoc />
        public DataEncoding BlobEncoding => DataEncoding.Bson;

        /// <inheritdoc />
        public DataEncoding ObjectEncoding => DataEncoding.Bson;

        /// <inheritdoc />
        public byte[] Serialize(Instrument dataObject)
        {
            var bsonMap = new BsonDocument
            {
                { nameof(Instrument.Symbol), dataObject.Symbol.Value },
                { nameof(Instrument.BrokerSymbol), dataObject.BrokerSymbol.Value },
                { nameof(Instrument.QuoteCurrency), dataObject.QuoteCurrency.ToString() },
                { nameof(Instrument.SecurityType), dataObject.SecurityType.ToString() },
                { nameof(Instrument.PricePrecision), dataObject.PricePrecision },
                { nameof(Instrument.SizePrecision), dataObject.SizePrecision },
                { nameof(Instrument.MinStopDistanceEntry), dataObject.MinStopDistanceEntry },
                { nameof(Instrument.MinStopDistance), dataObject.MinStopDistance },
                { nameof(Instrument.MinLimitDistanceEntry), dataObject.MinLimitDistanceEntry },
                { nameof(Instrument.MinLimitDistance), dataObject.MinLimitDistance },
                { nameof(Instrument.TickSize), dataObject.TickSize.ToString() },
                { nameof(Instrument.RoundLotSize), dataObject.RoundLotSize.ToString() },
                { nameof(Instrument.MinTradeSize), dataObject.MinTradeSize.ToString() },
                { nameof(Instrument.MaxTradeSize), dataObject.MaxTradeSize.ToString() },
                { nameof(Instrument.RolloverInterestBuy), dataObject.RolloverInterestBuy.ToString(CultureInfo.InvariantCulture) },
                { nameof(Instrument.RolloverInterestSell), dataObject.RolloverInterestSell.ToString(CultureInfo.InvariantCulture) },
                { nameof(Instrument.Timestamp), dataObject.Timestamp.ToIsoString() },
            }.ToDictionary();

            if (dataObject is ForexInstrument forexCcy)
            {
                bsonMap.Add(nameof(ForexInstrument.BaseCurrency), forexCcy.BaseCurrency.ToString());
            }

            return bsonMap.ToBson();
        }

        /// <inheritdoc />
        [PerformanceOptimized]
        public byte[][] Serialize(Instrument[] dataObjects)
        {
            var output = new byte[dataObjects.Length][];
            for (var i = 0; i < dataObjects.Length; i++)
            {
                output[i] = this.Serialize(dataObjects[i]);
            }

            return output;
        }

        /// <inheritdoc />
        [PerformanceOptimized]
        public byte[] SerializeBlob(byte[][] dataObjectsArray, Dictionary<string, string> metadata)
        {
            var bson = new BsonDocument
            {
                { DataType, typeof(Instrument[]).Name },
                { Data, new BsonArray(dataObjectsArray) },
                { MetaData, metadata.ToBsonDocument() },
            };

            return bson.ToBson();
        }

        /// <inheritdoc />
        public Instrument Deserialize(byte[] dataBytes)
        {
            Debug.NotEmpty(dataBytes, nameof(dataBytes));

            var unpacked = BsonSerializer.Deserialize<BsonDocument>(dataBytes);

            var securityType = unpacked[nameof(Instrument.SecurityType)].AsString.ToEnum<SecurityType>();
            if (securityType == SecurityType.Forex)
            {
                return new ForexInstrument(
                    Symbol.FromString(unpacked[nameof(Instrument.Symbol)].AsString),
                    new BrokerSymbol(unpacked[nameof(Instrument.BrokerSymbol)].AsString),
                    unpacked[nameof(Instrument.PricePrecision)].AsInt32,
                    unpacked[nameof(Instrument.SizePrecision)].AsInt32,
                    unpacked[nameof(Instrument.MinStopDistanceEntry)].AsInt32,
                    unpacked[nameof(Instrument.MinLimitDistanceEntry)].AsInt32,
                    unpacked[nameof(Instrument.MinStopDistance)].AsInt32,
                    unpacked[nameof(Instrument.MinLimitDistance)].AsInt32,
                    Price.Create(unpacked[nameof(Instrument.TickSize)].AsString),
                    Quantity.Create(unpacked[nameof(Instrument.RoundLotSize)].AsString),
                    Quantity.Create(unpacked[nameof(Instrument.MinTradeSize)].AsString),
                    Quantity.Create(unpacked[nameof(Instrument.MaxTradeSize)].AsString),
                    Convert.ToDecimal(unpacked[nameof(Instrument.RolloverInterestBuy)].AsString),
                    Convert.ToDecimal(unpacked[nameof(Instrument.RolloverInterestSell)].AsString),
                    unpacked[nameof(Instrument.Timestamp)].AsString.ToZonedDateTimeFromIso());
            }

            return new Instrument(
                Symbol.FromString(unpacked[nameof(Instrument.Symbol)].AsString),
                new BrokerSymbol(unpacked[nameof(Instrument.BrokerSymbol)].AsString),
                unpacked[nameof(Instrument.QuoteCurrency)].AsString.ToEnum<Currency>(),
                securityType,
                unpacked[nameof(Instrument.PricePrecision)].AsInt32,
                unpacked[nameof(Instrument.SizePrecision)].AsInt32,
                unpacked[nameof(Instrument.MinStopDistanceEntry)].AsInt32,
                unpacked[nameof(Instrument.MinLimitDistanceEntry)].AsInt32,
                unpacked[nameof(Instrument.MinStopDistance)].AsInt32,
                unpacked[nameof(Instrument.MinLimitDistance)].AsInt32,
                Price.Create(unpacked[nameof(Instrument.TickSize)].AsString),
                Quantity.Create(unpacked[nameof(Instrument.RoundLotSize)].AsString),
                Quantity.Create(unpacked[nameof(Instrument.MinTradeSize)].AsString),
                Quantity.Create(unpacked[nameof(Instrument.MaxTradeSize)].AsString),
                Convert.ToDecimal(unpacked[nameof(Instrument.RolloverInterestBuy)].AsString),
                Convert.ToDecimal(unpacked[nameof(Instrument.RolloverInterestSell)].AsString),
                unpacked[nameof(Instrument.Timestamp)].AsString.ToZonedDateTimeFromIso());
        }

        /// <inheritdoc />
        [PerformanceOptimized]
        public Instrument[] Deserialize(byte[][] dataBytesArray, object? metadata = null)
        {
            Debug.NotEmpty(dataBytesArray, nameof(dataBytesArray));

            var output = new Instrument[dataBytesArray.Length];
            for (var i = 0; i < dataBytesArray.Length; i++)
            {
                output[i] = this.Deserialize(dataBytesArray[i]);
            }

            return output;
        }

        /// <inheritdoc />
        public Instrument[] DeserializeBlob(byte[] dataBytes)
        {
            Debug.NotEmpty(dataBytes, nameof(dataBytes));

            var data = BsonSerializer.Deserialize<BsonDocument>(dataBytes);
            var valueArray = data[Data].AsBsonArray;

            var instruments = new Instrument[valueArray.Count];
            for (var i = 0; i < valueArray.Count; i++)
            {
                instruments[i] = this.Deserialize(valueArray[i].AsByteArray);
            }

            return instruments;
        }
    }
}
