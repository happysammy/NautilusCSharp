//--------------------------------------------------------------------------------------------------
// <copyright file="TickDataRequest.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Messages.Requests
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Network.Messages;
    using NodaTime;

    /// <summary>
    /// Represents a request for historical tick data.
    /// </summary>
    [Immutable]
    public sealed class TickDataRequest : Request
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TickDataRequest"/> class.
        /// </summary>
        /// <param name="symbol">The request tick symbol.</param>
        /// <param name="fromDateTime">The request from date time.</param>
        /// <param name="toDateTime">The request to date time.</param>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="requestTimestamp">The request timestamp.</param>
        public TickDataRequest(
            Symbol symbol,
            ZonedDateTime fromDateTime,
            ZonedDateTime toDateTime,
            Guid requestId,
            ZonedDateTime requestTimestamp)
            : base(
                typeof(TickDataRequest),
                requestId,
                requestTimestamp)
        {
            Condition.True(fromDateTime.IsLessThanOrEqualTo(toDateTime), "fromDateTime <= toDateTime");
            Debug.NotDefault(requestId, nameof(requestId));
            Debug.NotDefault(requestTimestamp, nameof(requestTimestamp));

            this.Symbol = symbol;
            this.FromDateTime = fromDateTime;
            this.ToDateTime = toDateTime;
        }

        /// <summary>
        /// Gets the requests tick symbol.
        /// </summary>
        public Symbol Symbol { get; }

        /// <summary>
        /// Gets the requests from date time.
        /// </summary>
        public ZonedDateTime FromDateTime { get; }

        /// <summary>
        /// Gets the requests to date time.
        /// </summary>
        public ZonedDateTime ToDateTime { get; }
    }
}
