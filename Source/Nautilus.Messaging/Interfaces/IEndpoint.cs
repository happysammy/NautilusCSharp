//--------------------------------------------------------------------------------------------------
// <copyright file="IEndpoint.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Messaging.Interfaces
{
    using System;

    /// <summary>
    /// Provides a messaging endpoint.
    /// </summary>
    public interface IEndpoint : IEquatable<object>, IEquatable<Endpoint>
    {
        /// <summary>
        /// Sends the given message to the endpoint.
        /// </summary>
        /// <param name="message">The message to send.</param>
        void Send(object message);
    }
}
