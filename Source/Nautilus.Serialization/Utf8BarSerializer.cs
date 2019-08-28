// -------------------------------------------------------------------------------------------------
// <copyright file="Utf8BarSerializer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization
{
    using System.Text;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.ValueObjects;

    /// <inheritdoc />
    public class Utf8BarSerializer : IDataSerializer<Bar>
    {
        /// <inheritdoc />
        public DataEncoding DataEncoding => DataEncoding.Utf8;

        /// <inheritdoc />
        public byte[] Serialize(Bar bar)
        {
            return Encoding.UTF8.GetBytes(bar.ToString());
        }

        /// <inheritdoc />
        public Bar Deserialize(byte[] bytes)
        {
            return Bar.FromString(Encoding.UTF8.GetString(bytes));
        }
    }
}
