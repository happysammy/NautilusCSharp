//--------------------------------------------------------------------------------------------------
// <copyright file="IEndpoint.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Messaging.Interfaces
{
    using System;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;

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

        /// <summary>
        /// Sends the given message to the endpoint.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>The result of the sending operation.</returns>
        Task<bool> SendAsync(object message);

        /// <summary>
        /// Gets the endpoints target block.
        /// </summary>
        /// <returns>The target block.</returns>
        ITargetBlock<object> GetLink();
    }
}
