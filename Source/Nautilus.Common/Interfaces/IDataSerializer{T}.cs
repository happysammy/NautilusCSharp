//--------------------------------------------------------------------------------------------------
// <copyright file="IDataSerializer{T}.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using Nautilus.Common.Enums;

    /// <summary>
    /// Provides a binary serializer for data objects.
    /// </summary>
    /// <typeparam name="T">The serializable data type.</typeparam>
    public interface IDataSerializer<T>
    {
        /// <summary>
        /// Gets the serializers encoding.
        /// </summary>
        DataEncoding Encoding { get; }

        /// <summary>
        /// Returns the serialized data object bytes.
        /// </summary>
        /// <param name="dataObject">The data object to serialize.</param>
        /// <returns>The serialized data object bytes.</returns>
        byte[] Serialize(T dataObject);

        /// <summary>
        /// Returns the deserialize data object of type T.
        /// </summary>
        /// <param name="dataBytes">The bytes to deserialize.</param>
        /// <returns>The deserialized data object.</returns>
        T Deserialize(byte[] dataBytes);

        /// <summary>
        /// Returns the serialized data object two dimensional bytes array.
        /// </summary>
        /// <param name="dataObjects">The data objects to serialize.</param>
        /// <returns>The serialized data object bytes.</returns>
        byte[][] Serialize(T[] dataObjects);

        /// <summary>
        /// Returns the deserialize data object of type T array.
        /// </summary>
        /// <param name="dataBytesArray">The bytes array to deserialize.</param>
        /// <returns>The deserialized data object.</returns>
        T[] Deserialize(byte[][] dataBytesArray);
    }
}
