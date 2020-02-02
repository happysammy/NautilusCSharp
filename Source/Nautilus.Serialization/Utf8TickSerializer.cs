// -------------------------------------------------------------------------------------------------
// <copyright file="Utf8TickSerializer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization
{
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;

    /// <inheritdoc />
    public class Utf8TickSerializer : ITickSerializer
    {
        /// <inheritdoc />
        public DataEncoding Encoding => DataEncoding.Utf8;

        /// <inheritdoc />
        public byte[] Serialize(Tick dataObject)
        {
            return System.Text.Encoding.UTF8.GetBytes(dataObject.ToString());
        }

        /// <inheritdoc />
        public Tick Deserialize(byte[] dataBytes)
        {
            return Tick.FromStringWithSymbol(System.Text.Encoding.UTF8.GetString(dataBytes));
        }

        /// <inheritdoc />
        public byte[][] Serialize(Tick[] dataObjects)
        {
            var output = new byte[dataObjects.Length][];
            for (var i = 0; i < dataObjects.Length; i++)
            {
                output[i] = this.Serialize(dataObjects[i]);
            }

            return output;
        }

        /// <inheritdoc />
        public Tick[] Deserialize(byte[][] dataBytesArray)
        {
            var output = new Tick[dataBytesArray.Length];
            for (var i = 0; i < dataBytesArray.Length; i++)
            {
                output[i] = this.Deserialize(dataBytesArray[i]);
            }

            return output;
        }

        /// <inheritdoc />
        public Tick Deserialize(Symbol symbol, byte[] valueBytes)
        {
            return Tick.FromString(symbol, System.Text.Encoding.UTF8.GetString(valueBytes));
        }

        /// <inheritdoc />
        public Tick[] Deserialize(Symbol symbol, byte[][] valueBytesArray)
        {
            var output = new Tick[valueBytesArray.Length];
            for (var i = 0; i < valueBytesArray.Length; i++)
            {
                output[i] = this.Deserialize(symbol, valueBytesArray[i]);
            }

            return output;
        }
    }
}
