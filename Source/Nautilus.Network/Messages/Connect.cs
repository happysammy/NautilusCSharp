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
    using NodaTime;

    /// <summary>
    /// Represents a response acknowledging receipt of a message.
    /// </summary>
    [Immutable]
    public sealed class Connect : Request
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Connect"/> class.
        /// </summary>
        /// <param name="socketIdentifier">The socket identifier.</param>
        /// <param name="id">The response identifier.</param>
        /// <param name="timestamp">The response timestamp.</param>
        public Connect(
            string socketIdentifier,
            Guid id,
            ZonedDateTime timestamp)
            : base(
                typeof(Connect),
                id,
                timestamp)
        {
            Debug.NotEmptyOrWhiteSpace(socketIdentifier, nameof(socketIdentifier));
            Debug.NotDefault(id, nameof(id));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.SocketIdentifier = socketIdentifier;
        }

        /// <summary>
        /// Gets the senders socket identifier.
        /// </summary>
        public string SocketIdentifier { get; }
    }
}
