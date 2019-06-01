//--------------------------------------------------------------------------------------------------
// <copyright file="IRequestSerializer.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using Nautilus.Core;

    /// <summary>
    /// Provides a binary serializer for <see cref="Request"/> messages.
    /// </summary>
    public interface IRequestSerializer
    {
        /// <summary>
        /// Returns the serialized request bytes.
        /// </summary>
        /// <param name="request">The request to serialize.</param>
        /// <returns>The serialized request bytes.</returns>
        byte[] Serialize(Request request);

        /// <summary>
        /// Returns the deserialize <see cref="Request"/>.
        /// </summary>
        /// <param name="requestBytes">The request bytes to deserialize.</param>
        /// <returns>The deserialized <see cref="Request"/>.</returns>
        Request Deserialize(byte[] requestBytes);
    }
}
