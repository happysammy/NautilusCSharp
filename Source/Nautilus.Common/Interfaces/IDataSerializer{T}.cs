//--------------------------------------------------------------------------------------------------
// <copyright file="IDataSerializer{T}.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.io
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
        DataEncoding DataEncoding { get;  }

        /// <summary>
        /// Returns the serialized data object bytes.
        /// </summary>
        /// <param name="data">The data object to serialize.</param>
        /// <returns>The serialized data object bytes.</returns>
        byte[] Serialize(T data);

        /// <summary>
        /// Returns the deserialize data object of type T.
        /// </summary>
        /// <param name="bytes">The bytes to deserialize.</param>
        /// <returns>The deserialized data object.</returns>
        T Deserialize(byte[] bytes);
    }
}
