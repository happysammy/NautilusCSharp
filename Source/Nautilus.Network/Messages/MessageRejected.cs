//--------------------------------------------------------------------------------------------------
// <copyright file="MessageRejected.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Network.Messages
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Message;
    using NodaTime;

    /// <summary>
    /// Represents a response indicating rejection of a message.
    /// </summary>
    [Immutable]
    public sealed class MessageRejected : Response
    {
        private static readonly Type EventType = typeof(MessageRejected);

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRejected"/> class.
        /// </summary>
        /// <param name="message">The rejected message.</param>
        /// <param name="correlationId">The response correlation identifier.</param>
        /// <param name="id">The response identifier.</param>
        /// <param name="timestamp">The response timestamp.</param>
        public MessageRejected(
            string message,
            Guid correlationId,
            Guid id,
            ZonedDateTime timestamp)
            : base(
                EventType,
                correlationId,
                id,
                timestamp)
        {
            Debug.NotEmptyOrWhiteSpace(message, nameof(message));
            Debug.NotDefault(id, nameof(id));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.Message = message;
        }

        /// <summary>
        /// Gets the responses rejected message.
        /// </summary>
        public string Message { get; }
    }
}
