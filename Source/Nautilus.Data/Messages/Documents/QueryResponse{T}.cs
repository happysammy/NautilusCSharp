// -------------------------------------------------------------------------------------------------
// <copyright file="QueryResponse{T}.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Messages.Documents
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.CQS;
    using Nautilus.Core.Validation;
    using NodaTime;

    /// <summary>
    /// The base class for all response message types.
    /// </summary>
    /// <typeparam name="T">The data type.</typeparam>
    [Immutable]
    public sealed class QueryResponse<T> : Document
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryResponse{T}"/> class.
        /// </summary>
        /// <param name="result">The query result.</param>
        /// <param name="identifier">The response identifier.</param>
        /// <param name="timestamp">The response timestamp.</param>
        public QueryResponse(
            QueryResult<T> result,
            Guid identifier,
            ZonedDateTime timestamp)
            : base(identifier, timestamp)
        {
            Debug.NotNull(result, nameof(result));
            Debug.NotDefault(identifier, nameof(identifier));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.Result = result;
        }

        /// <summary>
        /// Gets the messages result.
        /// </summary>
        public QueryResult<T> Result { get; }

        /// <summary>
        /// Gets a value indicating whether the result of the response message is successful.
        /// </summary>
        public bool IsSuccess => Result.IsSuccess;

        /// <summary>
        /// Gets the message of the response message.
        /// </summary>
        public string Message => Result.Message;
    }
}
