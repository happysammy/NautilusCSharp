//--------------------------------------------------------------------------------------------------
// <copyright file="IDataFeedClient.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using Nautilus.DomainModel.Events;

    /// <summary>
    /// Provides an interface for event serializers.
    /// </summary>
    public interface IEventSerializer
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="orderEvent"></param>
        /// <returns></returns>
        byte[] SerializeOrderEvent(OrderEvent orderEvent);
    }
}