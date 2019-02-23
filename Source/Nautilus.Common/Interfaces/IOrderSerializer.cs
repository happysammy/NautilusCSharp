//--------------------------------------------------------------------------------------------------
// <copyright file="IOrderSerializer.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using Nautilus.DomainModel.Aggregates;

    /// <summary>
    /// Provides an interface for order serializers.
    /// </summary>
    public interface IOrderSerializer
    {
        /// <summary>
        /// Serialize the given order object.
        /// </summary>
        /// <param name="order">The order to serialize.</param>
        /// <returns>The serialized order.</returns>
        byte[] Serialize(Order order);

        /// <summary>
        /// Deserialize the given command byte[].
        /// </summary>
        /// <param name="orderBytes">The order bytes to deserialize.</param>
        /// <returns>The deserialized order.</returns>
        Order Deserialize(byte[] orderBytes);
    }
}
