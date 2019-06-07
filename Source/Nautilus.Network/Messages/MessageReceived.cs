// -------------------------------------------------------------------------------------------------
// <copyright file="MessageReceived.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Network.Messages
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using NodaTime;

    /// <summary>
    /// A response acknowledging receipt of a message.
    /// </summary>
    [Immutable]
    public sealed class MessageReceived : Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageReceived"/> class.
        /// </summary>
        /// <param name="receivedType">The received message type.</param>
        /// <param name="correlationId">The response correlation identifier.</param>
        /// <param name="id">The response identifier.</param>
        /// <param name="timestamp">The response timestamp.</param>
        public MessageReceived(
            string receivedType,
            Guid correlationId,
            Guid id,
            ZonedDateTime timestamp)
            : base(
                typeof(MessageReceived),
                correlationId,
                id,
                timestamp)
        {
            Debug.NotEmptyOrWhiteSpace(receivedType, nameof(receivedType));
            Debug.NotDefault(id, nameof(id));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.ReceivedType = receivedType;
        }

        /// <summary>
        /// Gets the responses received message type.
        /// </summary>
        public string ReceivedType { get; }
    }
}
