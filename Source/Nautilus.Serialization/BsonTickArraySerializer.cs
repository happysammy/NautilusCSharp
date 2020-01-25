// -------------------------------------------------------------------------------------------------
// <copyright file="BsonTickArraySerializer.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Provides a data serializer for the BSON specification.
    /// </summary>
    [SuppressMessage("ReSharper", "SA1310", Justification = "Easier to read.")]
    public class BsonTickArraySerializer : IDataSerializer<Tick[]>
    {
        private const string DATA_TYPE = "DataType";
        private const string DATA = "Data";

        private readonly ObjectCache<string, Symbol> cachedSymbols;

        /// <summary>
        /// Initializes a new instance of the <see cref="BsonTickArraySerializer"/> class.
        /// </summary>
        public BsonTickArraySerializer()
        {
            this.cachedSymbols = new ObjectCache<string, Symbol>(Symbol.FromString);
        }

        /// <inheritdoc />
        public DataEncoding DataEncoding => DataEncoding.Bson;

        /// <inheritdoc />
        [PerformanceOptimized]
        public byte[] Serialize(Tick[] ticks)
        {
            Debug.NotEmpty(ticks, nameof(ticks));

            var dataArray = new string[ticks.Length];
            for (var i = 0; i < ticks.Length; i++)
            {
                dataArray[i] = ticks[i].ToString();
            }

            return new BsonDocument
            {
                { DATA_TYPE, nameof(Tick) },
                { nameof(Tick.Symbol), ticks[0].Symbol.Value },
                { DATA, new BsonArray(dataArray) },
            }.ToBson();
        }

        /// <inheritdoc />
        [PerformanceOptimized]
        public Tick[] Deserialize(byte[] dataBytes)
        {
            Debug.NotEmpty(dataBytes, nameof(dataBytes));

            var data = BsonSerializer.Deserialize<BsonDocument>(dataBytes);

            var symbol = this.cachedSymbols.Get(data[nameof(Tick.Symbol)].AsString);
            var valueArray = data[DATA].AsBsonArray;

            var ticks = new Tick[valueArray.Count];
            for (var i = 0; i < valueArray.Count; i++)
            {
                ticks[i] = Tick.FromString(symbol, valueArray[i].AsString);
            }

            return ticks;
        }
    }
}
