// -------------------------------------------------------------------------------------------------
// <copyright file="TickDataSerializer.cs" company="Nautech Systems Pty Ltd">
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
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Nautilus.Common.Componentry;
using Nautilus.Common.Enums;
using Nautilus.Common.Interfaces;
using Nautilus.Core.Correctness;
using Nautilus.DomainModel.Identifiers;
using Nautilus.DomainModel.ValueObjects;

namespace Nautilus.Serialization.DataSerializers
{
    /// <inheritdoc />
    public sealed class TradeTickSerializer : IDataSerializer<TradeTick>
    {
        private const string Data = nameof(Data);
        private const string DataType = nameof(DataType);
        private const string Metadata = nameof(Metadata);

        private readonly ObjectCache<string, Symbol> cachedSymbols;

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeTickSerializer"/> class.
        /// </summary>
        public TradeTickSerializer()
        {
            this.cachedSymbols = new ObjectCache<string, Symbol>(Symbol.FromString);
        }

        /// <inheritdoc />
        public DataEncoding BlobEncoding => DataEncoding.Bson;

        /// <inheritdoc />
        public DataEncoding ObjectEncoding => DataEncoding.Utf8;

        /// <inheritdoc />
        public byte[] Serialize(TradeTick tick)
        {
            return Encoding.UTF8.GetBytes(tick.ToSerializableString());
        }

        /// <inheritdoc />
        public byte[][] Serialize(TradeTick[] dataObjects)
        {
            Debug.NotEmpty(dataObjects, nameof(dataObjects));

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
            Condition.NotNull(metadata, nameof(metadata));
            Condition.NotEmpty(dataObjectsArray, nameof(dataObjectsArray));

            return new BsonDocument
            {
                { DataType, typeof(QuoteTick[]).Name },
                { Data, new BsonArray(dataObjectsArray) },
                { Metadata, metadata.ToBsonDocument() },
            }.ToBson();
        }

        /// <inheritdoc />
        public TradeTick Deserialize(byte[] dataBytes)
        {
            var pieces = Encoding.UTF8.GetString(dataBytes).Split(',', 2);
            var symbol = this.cachedSymbols.Get(pieces[0]);

            return TradeTick.FromSerializableString(symbol, pieces[1]);
        }

        /// <inheritdoc />
        public TradeTick[] Deserialize(byte[][] dataBytesArray, Dictionary<string, string>? metadata)
        {
            Debug.NotNull(metadata, nameof(metadata));

            if (metadata is null)
            {
                return new TradeTick[]{ };
            }

            var symbol = this.cachedSymbols.Get(metadata[nameof(Symbol)]);

            var output = new TradeTick[dataBytesArray.Length];
            for (var i = 0; i < dataBytesArray.Length; i++)
            {
                output[i] = TradeTick.FromSerializableString(symbol, Encoding.UTF8.GetString(dataBytesArray[i]));
            }

            return output;
        }

        /// <inheritdoc/>
        public TradeTick[] DeserializeBlob(byte[] dataBytes)
        {
            Debug.NotEmpty(dataBytes, nameof(dataBytes));

            var data = BsonSerializer.Deserialize<BsonDocument>(dataBytes);

            var symbol = this.cachedSymbols.Get(data[Metadata][nameof(Tick.Symbol)].AsString);

            var valueArray = data[Data].AsBsonArray;

            var output = new TradeTick[valueArray.Count];
            for (var i = 0; i < valueArray.Count; i++)
            {
                output[i] = TradeTick.FromSerializableString(symbol, Encoding.UTF8.GetString(valueArray[i].AsByteArray));
            }

            return output;
        }
    }
}
