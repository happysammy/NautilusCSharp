// -------------------------------------------------------------------------------------------------
// <copyright file="BarDataSerializer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization.Bson
{
    using System.Collections.Generic;
    using System.Text;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.ValueObjects;

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
