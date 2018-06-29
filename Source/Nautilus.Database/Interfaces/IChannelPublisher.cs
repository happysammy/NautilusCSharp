//--------------------------------------------------------------------------------------------------
// <copyright file="IChannelPublisher.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Interfaces
{
    /// <summary>
    /// Provides a generic channel publisher interface.
    /// </summary>
    public interface IChannelPublisher
    {
        /// <summary>
        /// Publishes the given message to the given channel.
        /// </summary>
        /// <param name="channel">The channel name.</param>
        /// <param name="message">The message content.</param>
        void Publish(string channel, string message);
    }
}
