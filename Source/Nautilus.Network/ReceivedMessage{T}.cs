// -------------------------------------------------------------------------------------------------
// <copyright file="ReceivedMessage{T}.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Network
{
    using Nautilus.Core;
    using Nautilus.Core.Annotations;

    /// <summary>
    /// Represents a message received by a server.
    /// </summary>
    /// <typeparam name="T">The type of message received.</typeparam>
    [Immutable]
    public sealed class ReceivedMessage<T>
        where T : Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReceivedMessage{T}"/> class.
        /// </summary>
        /// <param name="senderId">The received message sender identifier.</param>
        /// <param name="payload">The received message payload.</param>
        public ReceivedMessage(
            byte[] senderId,
            T payload)
        {
            this.SenderId = senderId;
            this.Payload = payload;
        }

        /// <summary>
        /// Gets the received messages sender identifier.
        /// </summary>
        public byte[] SenderId { get; }

        /// <summary>
        /// Gets the received messages payload.
        /// </summary>
        public T Payload { get; }
    }
}
