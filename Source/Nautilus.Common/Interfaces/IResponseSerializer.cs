//--------------------------------------------------------------------------------------------------
// <copyright file="IResponseSerializer.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using Nautilus.Core;

    /// <summary>
    /// Provides a binary serializer for <see cref="Response"/> messages.
    /// </summary>
    public interface IResponseSerializer
    {
        /// <summary>
        /// Returns the serialized request bytes.
        /// </summary>
        /// <param name="request">The response to serialize.</param>
        /// <returns>The serialized response bytes.</returns>
        byte[] Serialize(Response request);

        /// <summary>
        /// Returns the deserialize <see cref="Response"/>.
        /// </summary>
        /// <param name="responseBytes">The response bytes to deserialize.</param>
        /// <returns>The deserialized <see cref="Response"/>.</returns>
        Response Deserialize(byte[] responseBytes);
    }
}
