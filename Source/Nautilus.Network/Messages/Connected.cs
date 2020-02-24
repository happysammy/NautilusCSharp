// -------------------------------------------------------------------------------------------------
// <copyright file="Connected.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Network.Messages
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Message;
    using NodaTime;

    /// <summary>
    /// Represents a response confirming the connection of a session.
    /// </summary>
    [Immutable]
    public sealed class Connected : Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Connected"/> class.
        /// </summary>
        /// <param name="serviceName">The service name.</param>
        /// <param name="sessionId">The connected session identifier.</param>
        /// <param name="message">The connected message.</param>
        /// <param name="correlationId">The response correlation identifier.</param>
        /// <param name="id">The response identifier.</param>
        /// <param name="timestamp">The response timestamp.</param>
        public Connected(
            string serviceName,
            string message,
            SessionId sessionId,
            Guid correlationId,
            Guid id,
            ZonedDateTime timestamp)
            : base(
                typeof(Connected),
                correlationId,
                id,
                timestamp)
        {
            Debug.NotEmptyOrWhiteSpace(serviceName, nameof(serviceName));
            Debug.NotEmptyOrWhiteSpace(message, nameof(message));
            Debug.NotDefault(id, nameof(id));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.ServiceName = serviceName;
            this.Message = message;
            this.SessionId = sessionId;
        }

        /// <summary>
        /// Gets the responses service name.
        /// </summary>
        public string ServiceName { get; }

        /// <summary>
        /// Gets the responses message.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the responses session identifier.
        /// </summary>
        public SessionId SessionId { get; }
    }
}
