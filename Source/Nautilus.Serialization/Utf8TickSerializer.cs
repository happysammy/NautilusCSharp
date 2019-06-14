// -------------------------------------------------------------------------------------------------
// <copyright file="Utf8TickSerializer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization
{
    using System.Text;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Provides a binary serializer for <see cref="Bar"/>s.
    /// </summary>
    public class Utf8TickSerializer : ISerializer<Tick>
    {
        /// <inheritdoc />
        public byte[] Serialize(Tick tick)
        {
            return Encoding.UTF8.GetBytes(tick.ToString());
        }

        /// <inheritdoc />
        public Tick Deserialize(byte[] bytes)
        {
            throw new System.NotImplementedException();
        }
    }
}
