//--------------------------------------------------------------------------------------------------
// <copyright file="IByteArrayArraySerializer.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using Nautilus.Common.Enums;

    /// <summary>
    /// Provides a binary serializer for array of byte arrays.
    /// </summary>
    public interface IByteArrayArraySerializer
    {
        /// <summary>
        /// Gets the serializers encoding.
        /// </summary>
        DataEncoding Encoding { get; }

        /// <summary>
        /// Return the data objects bytes array serialized to a single array.
        /// </summary>
        /// <param name="dataObject">The data bytes to deserialize.</param>
        /// <returns>The serialized data bytes.</returns>
        byte[] Serialize(byte[][] dataObject);

        /// <summary>
        /// Return the data objects bytes array deserialized to a two dimensional array.
        /// </summary>
        /// <param name="dataBytes">The data bytes to deserialize.</param>
        /// <returns>The deserialized data bytes array.</returns>
        byte[][] Deserialize(byte[] dataBytes);
    }
}
