//--------------------------------------------------------------------------------------------------
// <copyright file="IEndpoint.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Messaging.Interfaces
{
    /// <summary>
    /// Provides a messaging endpoint.
    /// </summary>
    public interface IEndpoint
    {
        /// <summary>
        /// Sends the given message to the endpoint.
        /// </summary>
        /// <param name="message">The message to send.</param>
        void Send(object message);
    }
}
