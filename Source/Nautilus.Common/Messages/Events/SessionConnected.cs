//--------------------------------------------------------------------------------------------------
// <copyright file="SessionConnected.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messages.Events
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Message;
    using Nautilus.DomainModel.Identifiers;
    using NodaTime;

    /// <summary>
    /// Represents an event where a brokerage session has been connected.
    /// </summary>
    [Immutable]
    public sealed class SessionConnected : Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SessionConnected"/> class.
        /// </summary>
        /// <param name="broker">The events brokerage connected to.</param>
        /// <param name="sessionId">The events session identifier.</param>
        /// <param name="id">The events identifier.</param>
        /// <param name="timestamp">The events timestamp.</param>
        public SessionConnected(
            Brokerage broker,
            string sessionId,
            Guid id,
            ZonedDateTime timestamp)
            : base(typeof(SessionConnected), id, timestamp)
        {
            Debug.NotEmptyOrWhiteSpace(sessionId, nameof(sessionId));

            this.Broker = broker;
            this.SessionId = sessionId;
        }

        /// <summary>
        /// Gets the connection events broker.
        /// </summary>
        public Brokerage Broker { get; }

        /// <summary>
        /// Gets the connection events session identifier.
        /// </summary>
        public string SessionId { get; }
    }
}
