// -------------------------------------------------------------------------------------------------
// <copyright file="DataStatusResponse.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Messages.Documents
{
    using System;
    using Nautilus.Core.CQS;
    using Nautilus.Core.Validation;
    using NodaTime;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;

    /// <summary>
    /// A data status response message.
    /// </summary>
    /// <typeparam name="T">The data type.</typeparam>
    [Immutable]
    public sealed class DataStatusResponse<T> : Document
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataStatusResponse{T}"/> class.
        /// </summary>
        /// <param name="lastTimestampQuery">The last timestamp query result.</param>
        /// <param name="identifier">The identifier.</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public DataStatusResponse(
            QueryResult<ZonedDateTime> lastTimestampQuery,
            Guid identifier,
            ZonedDateTime timestamp)
            : base(identifier, timestamp)
        {
            Debug.NotNull(lastTimestampQuery, nameof(lastTimestampQuery));

            this.LastTimestampQuery = lastTimestampQuery;
        }

        /// <summary>
        /// Gets the responses last timestamp query result.
        /// </summary>
        public QueryResult<ZonedDateTime> LastTimestampQuery { get; }
    }
}
