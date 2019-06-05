//--------------------------------------------------------------------------------------------------
// <copyright file="ISerializer{T}.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    /// <summary>
    /// Provides a binary serializer for objects of type T.
    /// </summary>
    /// <typeparam name="T">The serializable type.</typeparam>
    public interface ISerializer<T>
    {
        /// <summary>
        /// Returns the serialized object bytes.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>The serialized object bytes.</returns>
        byte[] Serialize(T obj);

        /// <summary>
        /// Returns the deserialize object of type T.
        /// </summary>
        /// <param name="bytes">The bytes to deserialize.</param>
        /// <returns>The deserialized object.</returns>
        T Deserialize(byte[] bytes);
    }
}
