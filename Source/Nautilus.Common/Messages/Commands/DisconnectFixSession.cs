//--------------------------------------------------------------------------------------------------
// <copyright file="DisconnectFixSession.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messages.Commands
{
    using System;
    using Nautilus.Common.Messages.Commands.Base;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using NodaTime;

    /// <summary>
    /// Represents a command to disconnect from a FIX session.
    /// </summary>
    [Immutable]
    public sealed class DisconnectFixSession : SystemCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisconnectFixSession"/> class.
        /// </summary>
        /// <param name="sessionId">The FIX session identifier to disconnect from.</param>
        /// <param name="identifier">The commands identifier (cannot be default).</param>
        /// <param name="timestamp">The commands timestamp (cannot be default).</param>
        public DisconnectFixSession(
            string sessionId,
            Guid identifier,
            ZonedDateTime timestamp)
            : base(identifier, timestamp)
        {
            Debug.NotNull(sessionId, nameof(sessionId));
            Debug.NotDefault(identifier, nameof(identifier));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.SessionId = sessionId;
        }

        /// <summary>
        /// Gets the FIX session identifier to disconnect from.
        /// </summary>
        public string SessionId { get; }
    }
}
