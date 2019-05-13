// -------------------------------------------------------------------------------------------------
// <copyright file="QueryRequest{T}.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Messages.Commands
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Messaging.Interfaces;
    using NodaTime;

    /// <summary>
    /// A query request for type T from and to the given datetime(s).
    /// </summary>
    /// <typeparam name="T">The data type.</typeparam>
    [Immutable]
    public sealed class QueryRequest<T> : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryRequest{T}"/> class.
        /// </summary>
        /// <param name="dataType">The query data type.</param>
        /// <param name="fromDateTime">The query from datetime.</param>
        /// <param name="toDateTime">The query to datetime.</param>
        /// <param name="sender">The query requester.</param>
        /// <param name="identifier">The query identifier.</param>
        /// <param name="timestamp">The query timestamp.</param>
        public QueryRequest(
            T dataType,
            ZonedDateTime fromDateTime,
            ZonedDateTime toDateTime,
            IEndpoint sender,
            Guid identifier,
            ZonedDateTime timestamp)
        : base(identifier, timestamp)
        {
            Debug.NotDefault(fromDateTime, nameof(fromDateTime));
            Debug.NotDefault(toDateTime, nameof(toDateTime));

            this.DataType = dataType;
            this.FromDateTime = fromDateTime;
            this.ToDateTime = toDateTime;
            this.Sender = sender;
        }

        /// <summary>
        /// Gets the query requests data type.
        /// </summary>
        public T DataType { get; }

        /// <summary>
        /// Gets the query requests from date time.
        /// </summary>
        public ZonedDateTime FromDateTime { get; }

        /// <summary>
        /// Gets the query requests to date time.
        /// </summary>
        public ZonedDateTime ToDateTime { get; }

        /// <summary>
        /// Gets the query requests sender.
        /// </summary>
        public IEndpoint Sender { get; }
    }
}
