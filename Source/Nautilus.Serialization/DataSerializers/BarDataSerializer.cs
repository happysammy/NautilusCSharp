// -------------------------------------------------------------------------------------------------
// <copyright file="BarDataSerializer.cs" company="Nautech Systems Pty Ltd">
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
using Nautilus.Common.Enums;
using Nautilus.Common.Interfaces;
using Nautilus.Core.Correctness;
using Nautilus.DomainModel.ValueObjects;

namespace Nautilus.Serialization.DataSerializers
{
    /// <inheritdoc />
    public sealed class BarDataSerializer : IDataSerializer<Bar>
    {
        private const string Data = nameof(Data);
        private const string DataType = nameof(DataType);
        private const string Metadata = nameof(Metadata);

        /// <inheritdoc />
        public DataEncoding BlobEncoding => DataEncoding.Bson;

        /// <inheritdoc />
        public DataEncoding ObjectEncoding => DataEncoding.Utf8;

        /// <inheritdoc />
        public byte[] Serialize(Bar dataObject)
        {
            return Encoding.UTF8.GetBytes(dataObject.ToString());
        }

        /// <inheritdoc />
        public byte[][] Serialize(Bar[] dataObjects)
        {
            var output = new byte[dataObjects.Length][];
            for (var i = 0; i < dataObjects.Length; i++)
            {
                output[i] = this.Serialize(dataObjects[i]);
            }

            return output;
        }

        /// <inheritdoc />
        public byte[] SerializeBlob(byte[][] dataObjectsArray, Dictionary<string, string> metadata)
        {
            Debug.NotEmpty(dataObjectsArray, nameof(dataObjectsArray));

            return new BsonDocument
            {
                { DataType, typeof(Bar[]).Name },
                { Data, new BsonArray(dataObjectsArray) },
                { Metadata, metadata.ToBsonDocument() },
            }.ToBson();
        }

        /// <inheritdoc />
        public Bar Deserialize(byte[] dataBytes)
        {
            return Bar.FromString(Encoding.UTF8.GetString(dataBytes));
        }

        /// <inheritdoc />
        public Bar[] Deserialize(byte[][] dataBytesArray, object? metadata = null)
        {
            Debug.NotEmpty(dataBytesArray, nameof(dataBytesArray));

            var output = new Bar[dataBytesArray.Length];
            for (var i = 0; i < dataBytesArray.Length; i++)
            {
                output[i] = this.Deserialize(dataBytesArray[i]);
            }

            return output;
        }

        /// <inheritdoc />
        public Bar[] DeserializeBlob(byte[] dataBytes)
        {
            Debug.NotEmpty(dataBytes, nameof(dataBytes));

            var data = BsonSerializer.Deserialize<BsonDocument>(dataBytes);
            var valueArray = data[Data].AsBsonArray;

            var bars = new Bar[valueArray.Count];
            for (var i = 0; i < valueArray.Count; i++)
            {
                bars[i] = this.Deserialize(valueArray[i].AsByteArray);
            }

            return bars;
        }
    }
}
