//--------------------------------------------------------------------------------------------------
// <copyright file="IEndpoint.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using System.Threading.Tasks;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Interfaces;
    using NodaTime;

    /// <summary>
    /// Provides a generic endpoint abstraction.
    /// </summary>
    public interface IEndpoint
    {
        /// <summary>
        /// Sends the given message to the endpoint.
        /// </summary>
        /// <param name="message">The message to send.</param>
        void Send(object message);

        /// <summary>
        /// Sends the given envelope to the endpoint.
        /// </summary>
        /// <param name="envelope">The envelope to send.</param>
        /// <typeparam name="T">The envelope message type.</typeparam>
        void Send<T>(Envelope<T> envelope)
            where T : ISendable<Message>;

        /// <summary>
        /// Sends the endpoint a command to stop gracefully.
        /// </summary>
        /// <param name="timeout">The timeout duration for task completion.</param>
        /// <returns>The result of the operation.</returns>
        Task<bool> GracefulStop(Duration timeout);
    }
}
