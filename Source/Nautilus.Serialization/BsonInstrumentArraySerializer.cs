// -------------------------------------------------------------------------------------------------
// <copyright file="BsonInstrumentArraySerializer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
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
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Entities;

    /// <summary>
    /// Provides a data serializer for the BSON specification.
    /// </summary>
    [SuppressMessage("ReSharper", "SA1310", Justification = "Easier to read.")]
    public class BsonInstrumentArraySerializer : IDataSerializer<Instrument[]>
    {
        private const string DATA_TYPE = "DataType";
        private const string DATA = "Data";

        private readonly BsonInstrumentSerializer instrumentSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="BsonInstrumentArraySerializer"/> class.
        /// </summary>
        public BsonInstrumentArraySerializer()
        {
            this.instrumentSerializer = new BsonInstrumentSerializer();
        }

        /// <inheritdoc />
        public DataEncoding DataEncoding => DataEncoding.Bson;

        /// <summary>
        /// Return the serialized array of byte arrays.
        /// </summary>
        /// <param name="instruments">The instruments to serialize.</param>
        /// <returns>The serialized byte array array.</returns>
        [PerformanceOptimized]
        public byte[] Serialize(Instrument[] instruments)
        {
            Debug.NotEmpty(instruments, nameof(instruments));

            var instrumentsLength = instruments.Length;
            var instrumentsBytes = new byte[instrumentsLength][];
            for (var i = 0; i < instrumentsLength; i++)
            {
                instrumentsBytes[i] = this.instrumentSerializer.Serialize(instruments[i]);
            }

            return new BsonDocument
            {
                { DATA_TYPE, nameof(Instrument) },
                { DATA, new BsonArray(instrumentsBytes) },
            }.ToBson();
        }

        /// <summary>
        /// Return the deserialized array of instruments.
        /// </summary>
        /// <param name="dataBytes">The instrument data bytes to deserialize.</param>
        /// <returns>The deserialized instrument array.</returns>
        [PerformanceOptimized]
        public Instrument[] Deserialize(byte[] dataBytes)
        {
            Debug.NotEmpty(dataBytes, nameof(dataBytes));

            var data = BsonSerializer.Deserialize<BsonDocument>(dataBytes);
            var valueArray = data[DATA].AsBsonArray;

            var instruments = new Instrument[valueArray.Count];
            for (var i = 0; i < valueArray.Count; i++)
            {
                instruments[i] = this.instrumentSerializer.Deserialize(valueArray[i].AsByteArray);
            }

            return instruments;
        }
    }
}
