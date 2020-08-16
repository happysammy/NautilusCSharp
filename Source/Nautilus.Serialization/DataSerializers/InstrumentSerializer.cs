// -------------------------------------------------------------------------------------------------
// <copyright file="InstrumentDataSerializer.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Globalization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Nautilus.Common.Enums;
using Nautilus.Common.Interfaces;
using Nautilus.Core;
using Nautilus.Core.Annotations;
using Nautilus.Core.Correctness;
using Nautilus.Core.Extensions;
using Nautilus.DomainModel.Entities;
using Nautilus.DomainModel.Enums;
using Nautilus.DomainModel.Identifiers;
using Nautilus.DomainModel.ValueObjects;

namespace Nautilus.Serialization.DataSerializers
{
    /// <inheritdoc />
    public sealed class InstrumentSerializer : IDataSerializer<Instrument>
    {
        private const string Data = nameof(Data);
        private const string DataType = nameof(DataType);
        private const string Metadata = nameof(Metadata);

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
                { nameof(Instrument.Timestamp), dataObject.Timestamp.ToIso8601String() },
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
        public byte[] SerializeBlob(byte[][] dataObjectsArray, Dictionary<string, string>? metadata)
        {
            if (metadata is null)
            {
                return new byte[]{};
            }

            var bson = new BsonDocument
            {
                { DataType, typeof(Instrument[]).Name },
                { Data, new BsonArray(dataObjectsArray) },
                { Metadata, metadata.ToBsonDocument() },
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
                    Parser.ToDecimal(unpacked[nameof(Instrument.RolloverInterestBuy)].AsString),
                    Parser.ToDecimal(unpacked[nameof(Instrument.RolloverInterestSell)].AsString),
                    unpacked[nameof(Instrument.Timestamp)].AsString.ToZonedDateTimeFromIso());
            }

            return new Instrument(
                Symbol.FromString(unpacked[nameof(Instrument.Symbol)].AsString),
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
                Parser.ToDecimal(unpacked[nameof(Instrument.RolloverInterestBuy)].AsString),
                Parser.ToDecimal(unpacked[nameof(Instrument.RolloverInterestSell)].AsString),
                unpacked[nameof(Instrument.Timestamp)].AsString.ToZonedDateTimeFromIso());
        }

        /// <inheritdoc />
        [PerformanceOptimized]
        public Instrument[] Deserialize(byte[][] dataBytesArray, Dictionary<string, string>? metadata)
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
