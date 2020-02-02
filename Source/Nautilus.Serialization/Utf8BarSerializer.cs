// -------------------------------------------------------------------------------------------------
// <copyright file="Utf8BarSerializer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization
{
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.ValueObjects;

    /// <inheritdoc />
    public class Utf8BarSerializer : IDataSerializer<Bar>
    {
        /// <inheritdoc />
        public DataEncoding Encoding => DataEncoding.Utf8;

        /// <inheritdoc />
        public byte[] Serialize(Bar dataObject)
        {
            return System.Text.Encoding.UTF8.GetBytes(dataObject.ToString());
        }

        /// <inheritdoc />
        public Bar Deserialize(byte[] dataBytes)
        {
            return Bar.FromString(System.Text.Encoding.UTF8.GetString(dataBytes));
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
        public Bar[] Deserialize(byte[][] dataBytesArray)
        {
            var output = new Bar[dataBytesArray.Length];
            for (var i = 0; i < dataBytesArray.Length; i++)
            {
                output[i] = this.Deserialize(dataBytesArray[i]);
            }

            return output;
        }
    }
}
