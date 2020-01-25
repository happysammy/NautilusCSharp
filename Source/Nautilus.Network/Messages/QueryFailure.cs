//--------------------------------------------------------------------------------------------------
// <copyright file="QueryFailure.cs" company="Nautech Systems Pty Ltd">
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
    /// Represents a response indicating a query failure.
    /// </summary>
    [Immutable]
    public sealed class QueryFailure : Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryFailure"/> class.
        /// </summary>
        /// <param name="failureMessage">The query failure message.</param>
        /// <param name="correlationId">The response correlation identifier.</param>
        /// <param name="id">The response identifier.</param>
        /// <param name="timestamp">The response timestamp.</param>
        public QueryFailure(
            string failureMessage,
            Guid correlationId,
            Guid id,
            ZonedDateTime timestamp)
            : base(
                typeof(QueryFailure),
                correlationId,
                id,
                timestamp)
        {
            Debug.NotEmptyOrWhiteSpace(failureMessage, nameof(failureMessage));
            Debug.NotDefault(id, nameof(id));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.Message = failureMessage;
        }

        /// <summary>
        /// Gets the responses query failure message.
        /// </summary>
        public string Message { get; }
    }
}
