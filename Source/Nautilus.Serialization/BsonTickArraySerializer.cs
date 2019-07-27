// -------------------------------------------------------------------------------------------------
// <copyright file="BsonTickArraySerializer.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Provides a data serializer for the BSON specification.
    /// </summary>
    [SuppressMessage("ReSharper", "SA1310", Justification = "Easier to read.")]
    public class BsonTickArraySerializer : IDataSerializer<Tick[]>
    {
        private const string DATA_TYPE = "DataType";
        private const string VALUES = "Values";

        /// <inheritdoc />
        public DataEncoding DataEncoding => DataEncoding.Bson;

        /// <inheritdoc />
        public byte[] Serialize(Tick[] ticks)
        {
            Debug.NotEmpty(ticks, nameof(ticks));

            var dataArray = new byte[ticks.Length][];
            for (var i = 0; i < ticks.Length; i++)
            {
                dataArray[i] = System.Text.Encoding.UTF8.GetBytes(ticks[i].ToString());
            }

            return new BsonDocument
            {
                { DATA_TYPE, nameof(Tick) },
                { nameof(Tick.Symbol), ticks[0].Symbol.ToString() },
                { VALUES, new BsonArray(dataArray) },
            }.ToBson();
        }

        /// <inheritdoc />
        public Tick[] Deserialize(byte[] dataBytes)
        {
            Debug.NotEmpty(dataBytes, nameof(dataBytes));

            var data = BsonSerializer.Deserialize<BsonDocument>(dataBytes);

            var symbol = DomainObjectParser.ParseSymbol(data[nameof(Tick.Symbol)].AsString);
            var valueArray = data[VALUES].AsBsonArray;

            var ticks = new Tick[valueArray.Count];
            for (var i = 0; i < valueArray.Count; i++)
            {
                var values = System.Text.Encoding.UTF8.GetString(valueArray[i].AsByteArray);
                ticks[i] = DomainObjectParser.ParseTick(symbol, values);
            }

            return ticks;
        }
    }
}
