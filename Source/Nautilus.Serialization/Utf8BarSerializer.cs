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
