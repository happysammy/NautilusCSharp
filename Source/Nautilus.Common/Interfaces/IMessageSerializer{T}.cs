//--------------------------------------------------------------------------------------------------
// <copyright file="IMessageSerializer{T}.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using Nautilus.Core;

    /// <summary>
    /// Provides a binary serializer for <see cref="Message"/>s of type T.
    /// </summary>
    /// <typeparam name="T">The <see cref="Message"/> type.</typeparam>
    public interface IMessageSerializer<T>
        where T : Message
    {
        /// <summary>
        /// Returns the serialized message bytes.
        /// </summary>
        /// <param name="message">The message to serialize.</param>
        /// <returns>The serialized message bytes.</returns>
        byte[] Serialize(T message);

        /// <summary>
        /// Returns the deserialize <see cref="Message"/>.
        /// </summary>
        /// <param name="commandBytes">The message bytes to deserialize.</param>
        /// <returns>The deserialized <see cref="Message"/>.</returns>
        T Deserialize(byte[] commandBytes);
    }
}
