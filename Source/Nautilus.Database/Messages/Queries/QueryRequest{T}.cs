// -------------------------------------------------------------------------------------------------
// <copyright file="MarketDataQueryRequest.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Messages.Queries
{
    using System;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using NodaTime;

    /// <summary>
    /// A query request for type T from and to the given datetimes.
    /// </summary>
    /// <typeparam name="T">The query type.</typeparam>
    [Immutable]
    public sealed class QueryRequest<T> : Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryRequest{T}"/> class.
        /// </summary>
        /// <param name="dataType">The query data type.</param>
        /// <param name="fromDateTime">The query from datetime.</param>
        /// <param name="toDateTime">The query to datetime.</param>
        /// <param name="identifier">The query identifier.</param>
        /// <param name="timestamp">The query timestamp.</param>
        public QueryRequest(
            T dataType,
            ZonedDateTime fromDateTime,
            ZonedDateTime toDateTime,
            Guid identifier,
            ZonedDateTime timestamp)
        : base(identifier, timestamp)
        {
            Debug.NotNull(dataType, nameof(dataType));
            Debug.NotDefault(fromDateTime, nameof(fromDateTime));
            Debug.NotDefault(toDateTime, nameof(toDateTime));
            Debug.NotDefault(identifier, nameof(identifier));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.DataType = dataType;
            this.FromDateTime = fromDateTime;
            this.ToDateTime = toDateTime;
        }
        /// <summary>
        /// Gets the query messages data type.
        /// </summary>
        public T DataType { get; }

        /// <summary>
        /// Gets the query messages from date time.
        /// </summary>
        public ZonedDateTime FromDateTime { get; }

        /// <summary>
        /// Gets the query messages to date time.
        /// </summary>
        public ZonedDateTime ToDateTime { get; }
    }
}
