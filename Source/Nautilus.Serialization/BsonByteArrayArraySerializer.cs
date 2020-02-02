// -------------------------------------------------------------------------------------------------
// <copyright file="BsonByteArrayArraySerializer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;

    /// <summary>
    /// Provides a data serializer for the BSON specification.
    /// </summary>
    public class BsonByteArrayArraySerializer : IByteArrayArraySerializer
    {
        private const string DATA = "DATA";

        /// <inheritdoc />
        public DataEncoding Encoding => DataEncoding.Bson;

        /// <inheritdoc />
        [PerformanceOptimized]
        public byte[] Serialize(byte[][] dataObject)
        {
            Debug.NotEmpty(dataObject, nameof(dataObject));

            var bsonArray = new BsonArray(dataObject.Length);
            for (var i = 0; i < bsonArray.Count; i++)
            {
                bsonArray[i] = dataObject[i];
            }

            return new BsonDocument { { DATA, new BsonArray(dataObject) } }.ToBson();
        }

        /// <inheritdoc />
        [PerformanceOptimized]
        public byte[][] Deserialize(byte[] dataBytes)
        {
            Debug.NotEmpty(dataBytes, nameof(dataBytes));

            var bsonArray = BsonSerializer.Deserialize<BsonDocument>(dataBytes)[DATA].AsBsonArray;
            var dataArray = new byte[bsonArray.Count][];
            for (var i = 0; i < dataArray.Length; i++)
            {
                dataArray[i] = bsonArray[i].AsByteArray;
            }

            return dataArray;
        }
    }
}
