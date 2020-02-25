// -------------------------------------------------------------------------------------------------
// <copyright file="Connect.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.Network.Identifiers;
    using NodaTime;

    /// <summary>
    /// Represents a request to connect to a session.
    /// </summary>
    [Immutable]
    public sealed class Connect : Request
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Connect"/> class.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="id">The response identifier.</param>
        /// <param name="timestamp">The response timestamp.</param>
        public Connect(
            ClientId clientId,
            Guid id,
            ZonedDateTime timestamp)
            : base(
                typeof(Connect),
                id,
                timestamp)
        {
            Debug.NotDefault(id, nameof(id));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.ClientId = clientId;
        }

        /// <summary>
        /// Gets the requests client identifier.
        /// </summary>
        public ClientId ClientId { get; }
    }
}
