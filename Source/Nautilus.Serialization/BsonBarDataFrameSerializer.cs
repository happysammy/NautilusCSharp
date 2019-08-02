// -------------------------------------------------------------------------------------------------
// <copyright file="BsonBarDataFrameSerializer.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.DomainModel.Frames;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Provides a data serializer for the BSON specification.
    /// </summary>
    [SuppressMessage("ReSharper", "SA1310", Justification = "Easier to read.")]
    public class BsonBarDataFrameSerializer : IDataSerializer<BarDataFrame>
    {
        private const string DATA_TYPE = "DataType";
        private const string DATA = "Data";

        /// <inheritdoc />
        public DataEncoding DataEncoding => DataEncoding.Bson;

        /// <inheritdoc />
        public byte[] Serialize(BarDataFrame data)
        {
            Debug.NotEmpty(data.Bars, nameof(data.Bars));

            var dataArray = new byte[data.Bars.Length][];
            for (var i = 0; i < data.Bars.Length; i++)
            {
                dataArray[i] = System.Text.Encoding.UTF8.GetBytes(data.Bars[i].ToString());
            }

            return new BsonDocument
            {
                { DATA_TYPE, nameof(Bar) },
                { nameof(data.BarType.Symbol), data.BarType.Symbol.ToString() },
                { nameof(data.BarType.Specification), data.BarType.Specification.ToString() },
                { DATA, new BsonArray(dataArray) },
            }.ToBson();
        }

        /// <inheritdoc />
        public BarDataFrame Deserialize(byte[] dataBytes)
        {
            Debug.NotEmpty(dataBytes, nameof(dataBytes));

            var data = BsonSerializer.Deserialize<BsonDocument>(dataBytes);

            var symbol = DomainObjectParser.ParseSymbol(data[nameof(BarType.Symbol)].AsString);
            var barSpec = DomainObjectParser.ParseBarSpecification(data[nameof(BarType.Specification)].AsString);
            var barType = new BarType(symbol, barSpec);
            var valuesArray = data[DATA].AsBsonArray;

            var bars = new Bar[valuesArray.Count];
            for (var i = 0; i < valuesArray.Count; i++)
            {
                var values = System.Text.Encoding.UTF8.GetString(valuesArray[i].AsByteArray);
                bars[i] = DomainObjectParser.ParseBar(values);
            }

            return new BarDataFrame(barType, bars);
        }
    }
}
