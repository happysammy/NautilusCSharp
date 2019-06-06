// -------------------------------------------------------------------------------------------------
// <copyright file="BarSerializer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization
{
    using System.Text;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Provides a binary serializer for <see cref="Bar"/>s.
    /// </summary>
    public class BarSerializer : ISerializer<Bar>
    {
        /// <inheritdoc />
        public byte[] Serialize(Bar bar)
        {
            return Encoding.UTF8.GetBytes(bar.ToString());
        }

        /// <inheritdoc />
        public Bar Deserialize(byte[] bytes)
        {
            return BarFactory.Create(Encoding.UTF8.GetString(bytes));
        }
    }
}
