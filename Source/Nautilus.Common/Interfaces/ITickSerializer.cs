//--------------------------------------------------------------------------------------------------
// <copyright file="ITickSerializer.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Provides a binary serializer for <see cref="Tick"/> objects.
    /// </summary>
    public interface ITickSerializer : IDataSerializer<Tick>
    {
        /// <summary>
        /// Returns the deserialize data object of type T.
        /// </summary>
        /// <param name="symbol">The symbol for the ticks.</param>
        /// <param name="valueBytes">The value bytes to deserialize.</param>
        /// <returns>The deserialized data object.</returns>
        Tick Deserialize(Symbol symbol, byte[] valueBytes);

        /// <summary>
        /// Returns the deserialize data object of type T array.
        /// </summary>
        /// <param name="symbol">The symbol for the ticks.</param>
        /// <param name="valueBytesArray">The value bytes to deserialize.</param>
        /// <returns>The deserialized data object.</returns>
        Tick[] Deserialize(Symbol symbol, byte[][] valueBytesArray);
    }
}
