// -------------------------------------------------------------------------------------------------
// <copyright file="DataStatusResponse.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using Nautilus.Core.CQS;
using Nautilus.Core.Validation;
using NodaTime;

namespace Nautilus.Database.Core.Messages.Queries
{
    public sealed class DataStatusResponse : QueryResponseMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataStatusResponse"/> class.
        /// </summary>
        /// <param name="lastTimestampQueryResult">The last timestamp query result.</param>
        /// <param name="identifier">The identifier.</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public DataStatusResponse(
            QueryResult<ZonedDateTime> lastTimestampQueryResult,
            Guid identifier,
            ZonedDateTime timestamp)
            : base(
                lastTimestampQueryResult.IsSuccess,
                lastTimestampQueryResult.FullMessage,
                identifier,
                timestamp)
        {
            Validate.NotNull(lastTimestampQueryResult, nameof(lastTimestampQueryResult));

            this.LastTimestampQueryResult = lastTimestampQueryResult;
        }

        /// <summary>
        /// Gets the responses last timestamp query result.
        /// </summary>
        public QueryResult<ZonedDateTime> LastTimestampQueryResult { get; }

        /// <summary>
        /// Gets a string representation of the <see cref="StartSystem"/> message.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{nameof(DataStatusResponse)}-{this.Id}";
    }
}