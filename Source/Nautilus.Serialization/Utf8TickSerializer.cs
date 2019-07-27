// -------------------------------------------------------------------------------------------------
// <copyright file="Utf8TickSerializer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization
{
    using System;
    using System.Text;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.ValueObjects;

    /// <inheritdoc />
    public class Utf8TickSerializer : IDataSerializer<Tick>
    {
        /// <inheritdoc />
        public DataEncoding DataEncoding => DataEncoding.Utf8;

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
