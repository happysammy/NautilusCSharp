// -------------------------------------------------------------------------------------------------
// <copyright file="TickDataSerializer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization.Bson
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;

    /// <inheritdoc />
    [SuppressMessage("ReSharper", "SA1310", Justification = "Easier to read.")]
    public class TickDataSerializer : IDataSerializer<Tick>
    {
        private const string DATA = "Data";
        private const string DATA_TYPE = "DataType";
        private const string METADATA = "Metadata";

        private readonly ObjectCache<string, Symbol> cachedSymbols;

        /// <summary>
        /// Initializes a new instance of the <see cref="TickDataSerializer"/> class.
        /// </summary>
        public TickDataSerializer()
        {
            this.cachedSymbols = new ObjectCache<string, Symbol>(Symbol.FromString);
        }

        /// <inheritdoc />
        public DataEncoding BlobEncoding => DataEncoding.Bson;

        /// <inheritdoc />
        public DataEncoding ObjectEncoding => DataEncoding.Utf8;

        /// <inheritdoc />
        public byte[] Serialize(Tick tick)
        {
            return Encoding.UTF8.GetBytes(tick.ToString());
        }

        /// <inheritdoc />
        public byte[][] Serialize(Tick[] dataObjects)
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
        public byte[] SerializeBlob(byte[][] dataObjectsArray, Dictionary<string, string> metadata)
        {
            Debug.NotEmpty(dataObjectsArray, nameof(dataObjectsArray));

            return new BsonDocument
            {
                { DATA_TYPE, typeof(Tick[]).Name },
                { DATA, new BsonArray(dataObjectsArray) },
                { METADATA, metadata.ToBsonDocument() },
            }.ToBson();
        }

        /// <inheritdoc />
        public Tick Deserialize(byte[] dataBytes)
        {
            Debug.NotEmpty(dataBytes, nameof(dataBytes));

            return Tick.FromStringWithSymbol(Encoding.UTF8.GetString(dataBytes));
        }

        /// <inheritdoc />
        public Tick[] Deserialize(byte[][] dataBytesArray, object? metadata = null)
        {
            Debug.NotNull(metadata, nameof(metadata));

            var output = new Tick[dataBytesArray.Length];
            for (var i = 0; i < dataBytesArray.Length; i++)
            {
                output[i] = Tick.FromString((Symbol)metadata!, Encoding.UTF8.GetString(dataBytesArray[i]));
            }

            return output;
        }

        /// <inheritdoc/>
        public Tick[] DeserializeBlob(byte[] dataBytes)
        {
            Debug.NotEmpty(dataBytes, nameof(dataBytes));

            var data = BsonSerializer.Deserialize<BsonDocument>(dataBytes);

            var symbol = this.cachedSymbols.Get(data[METADATA][nameof(Tick.Symbol)].AsString);
            var valueArray = data[DATA].AsBsonArray;

            var ticks = new Tick[valueArray.Count];
            for (var i = 0; i < valueArray.Count; i++)
            {
                ticks[i] = Tick.FromString(symbol,  Encoding.UTF8.GetString(valueArray[i].AsByteArray));
            }

            return ticks;
        }
    }
}
