// -------------------------------------------------------------------------------------------------
// <copyright file="Disconnected.cs" company="Nautech Systems Pty Ltd">
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
    public sealed class Disconnected : Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Disconnected"/> class.
        /// </summary>
        /// <param name="message">The disconnected message.</param>
        /// <param name="serverId">The server identifier.</param>
        /// <param name="sessionId">The disconnected session identifier.</param>
        /// <param name="correlationId">The response correlation identifier.</param>
        /// <param name="id">The response identifier.</param>
        /// <param name="timestamp">The response timestamp.</param>
        public Disconnected(
            string message,
            string serverId,
            SessionId sessionId,
            Guid correlationId,
            Guid id,
            ZonedDateTime timestamp)
            : base(
                typeof(Disconnected),
                correlationId,
                id,
                timestamp)
        {
            Debug.NotEmptyOrWhiteSpace(message, nameof(message));
            Debug.NotEmptyOrWhiteSpace(serverId, nameof(serverId));
            Debug.NotDefault(id, nameof(id));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.Message = message;
            this.ServerId = serverId;
            this.SessionId = sessionId;
        }

        /// <summary>
        /// Gets the responses message.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the responses service name.
        /// </summary>
        public string ServerId { get; }

        /// <summary>
        /// Gets the responses session identifier.
        /// </summary>
        public SessionId SessionId { get; }
    }
}
