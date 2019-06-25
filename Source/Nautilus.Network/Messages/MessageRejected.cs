//--------------------------------------------------------------------------------------------------
// <copyright file="MessageRejected.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Network.Messages
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using NodaTime;

    /// <summary>
    /// A response indicating rejection of a message.
    /// </summary>
    [Immutable]
    public sealed class MessageRejected : Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRejected"/> class.
        /// </summary>
        /// <param name="rejectedMessage">The rejected message.</param>
        /// <param name="correlationId">The response correlation identifier.</param>
        /// <param name="id">The response identifier.</param>
        /// <param name="timestamp">The response timestamp.</param>
        public MessageRejected(
            string rejectedMessage,
            Guid correlationId,
            Guid id,
            ZonedDateTime timestamp)
            : base(
                typeof(MessageRejected),
                correlationId,
                id,
                timestamp)
        {
            Debug.NotEmptyOrWhiteSpace(rejectedMessage, nameof(rejectedMessage));
            Debug.NotDefault(id, nameof(id));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.Message = rejectedMessage;
        }

        /// <summary>
        /// Gets the responses rejected message.
        /// </summary>
        public string Message { get; }
    }
}
