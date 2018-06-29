//--------------------------------------------------------------------------------------------------
// <copyright file="IChannelPublisherFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Interfaces
{
    /// <summary>
    /// An abstract factory interface for creating channel publishers.
    /// </summary>
    public interface IChannelPublisherFactory
    {
        /// <summary>
        /// Creates and returns a new channel publisher.
        /// </summary>
        /// <returns></returns>
        IChannelPublisher Create();
    }
}
