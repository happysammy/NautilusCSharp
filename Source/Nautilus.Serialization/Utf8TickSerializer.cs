// -------------------------------------------------------------------------------------------------
// <copyright file="Utf8TickSerializer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization
{
    using System;
    using System.Text;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.ValueObjects;

    /// <inheritdoc />
    public class Utf8TickSerializer : ISerializer<Tick>
    {
        /// <summary>
        /// Return the serialized array of byte arrays.
        /// </summary>
        /// <param name="ticks">The ticks to serialize.</param>
        /// <returns>The serialized array of byte arrays.</returns>
        public byte[][] Serialize(Tick[] ticks)
        {
            var tickLength = ticks.Length;
            var ticksBytes = new byte[tickLength][];
            for (var i = 0; i < tickLength; i++)
            {
                ticksBytes[i] = Encoding.UTF8.GetBytes(ticks[i].ToString());
            }

            return ticksBytes;
        }

        /// <summary>
        /// Return the deserialized array of <see cref="Tick"/>s.
        /// </summary>
        /// <param name="tickSymbol">The tick symbol.</param>
        /// <param name="tickBytes">The tick bytes to deserialize.</param>
        /// <returns>The deserialized tick array.</returns>
        public Tick[] Deserialize(Symbol tickSymbol, byte[][] tickBytes)
        {
            var tickLength = tickBytes.Length;
            var ticks = new Tick[tickLength];
            for (var i = 0; i < tickLength; i++)
            {
                ticks[i] = DomainObjectParser.ParseTick(tickSymbol, Encoding.UTF8.GetString(tickBytes[i]));
            }

            return ticks;
        }

        /// <inheritdoc />
        public byte[] Serialize(Tick tick)
        {
            return Encoding.UTF8.GetBytes(tick.ToString());
        }

        /// <inheritdoc />
        public Tick Deserialize(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }
}
