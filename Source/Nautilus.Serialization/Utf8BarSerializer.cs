// -------------------------------------------------------------------------------------------------
// <copyright file="Utf8BarSerializer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization
{
    using System.Text;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.ValueObjects;

    /// <inheritdoc />
    public class Utf8BarSerializer : ISerializer<Bar>
    {
        /// <summary>
        /// Return the serialized array of byte arrays.
        /// </summary>
        /// <param name="bars">The bars to serialize.</param>
        /// <returns>The serialized array of byte arrays.</returns>
        public byte[][] Serialize(Bar[] bars)
        {
            var barsLength = bars.Length;
            var barsBytes = new byte[barsLength][];
            for (var i = 0; i < barsLength; i++)
            {
                barsBytes[i] = Encoding.UTF8.GetBytes(bars[i].ToString());
            }

            return barsBytes;
        }

        /// <summary>
        /// Return the deserialized array of <see cref="Bar"/>s.
        /// </summary>
        /// <param name="barBytes">The bar bytes to deserialize.</param>
        /// <returns>The deserialized bars array.</returns>
        public Bar[] Deserialize(byte[][] barBytes)
        {
            var barsLength = barBytes.Length;
            var bars = new Bar[barsLength];
            for (var i = 0; i < barsLength; i++)
            {
                bars[i] = DomainObjectParser.ParseBar(Encoding.UTF8.GetString(barBytes[i]));
            }

            return bars;
        }

        /// <inheritdoc />
        public byte[] Serialize(Bar bar)
        {
            return Encoding.UTF8.GetBytes(bar.ToString());
        }

        /// <inheritdoc />
        public Bar Deserialize(byte[] bytes)
        {
            return DomainObjectParser.ParseBar(Encoding.UTF8.GetString(bytes));
        }
    }
}
