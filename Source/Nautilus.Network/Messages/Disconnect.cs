// -------------------------------------------------------------------------------------------------
// <copyright file="Disconnect.cs" company="Nautech Systems Pty Ltd">
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
    /// Represents a request to disconnect from a session.
    /// </summary>
    [Immutable]
    public sealed class Disconnect : Request
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Disconnect"/> class.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="sessionId">The session identifier to disconnect from.</param>
        /// <param name="id">The response identifier.</param>
        /// <param name="timestamp">The response timestamp.</param>
        public Disconnect(
            string clientId,
            SessionId sessionId,
            Guid id,
            ZonedDateTime timestamp)
            : base(
                typeof(Disconnect),
                id,
                timestamp)
        {
            Debug.NotDefault(id, nameof(id));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.ClientId = clientId;
            this.SessionId = sessionId;
        }

        /// <summary>
        /// Gets the requests client identifier.
        /// </summary>
        public string ClientId { get; }

        /// <summary>
        /// Gets the requests session identifier.
        /// </summary>
        public SessionId SessionId { get; }
    }
}
