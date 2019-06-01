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
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel.ValueObjects;
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
        /// <param name="id">The requests identifier.</param>
        /// <param name="timestamp">The requests timestamp.</param>
        public TickDataRequest(
            Symbol symbol,
            ZonedDateTime fromDateTime,
            ZonedDateTime toDateTime,
            Guid id,
            ZonedDateTime timestamp)
            : base(
                typeof(BarDataRequest),
                id,
                timestamp)
        {
            Condition.True(fromDateTime.IsGreaterThan(toDateTime), "fromDateTime > to DateTime");
            Debug.NotDefault(id, nameof(id));
            Debug.NotDefault(timestamp, nameof(timestamp));

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
