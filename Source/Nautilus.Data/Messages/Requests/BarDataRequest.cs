//--------------------------------------------------------------------------------------------------
// <copyright file="BarDataRequest.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.Network;
    using NodaTime;

    /// <summary>
    /// Represents a request for historical bar data.
    /// </summary>
    [Immutable]
    public sealed class BarDataRequest : Request
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BarDataRequest"/> class.
        /// </summary>
        /// <param name="barSymbol">The request bar symbol.</param>
        /// <param name="barSpec">The request bar specification.</param>
        /// <param name="fromDateTime">The request from date time.</param>
        /// <param name="toDateTime">The request to date time.</param>
        /// <param name="id">The requests identifier.</param>
        /// <param name="timestamp">The requests timestamp.</param>
        public BarDataRequest(
            Symbol barSymbol,
            BarSpecification barSpec,
            ZonedDateTime fromDateTime,
            ZonedDateTime toDateTime,
            Guid id,
            ZonedDateTime timestamp)
            : base(
                typeof(BarDataRequest),
                id,
                timestamp)
        {
            Condition.True(fromDateTime.IsLessThanOrEqualTo(toDateTime), "fromDateTime <= toDateTime");
            Debug.NotDefault(id, nameof(id));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.Symbol = barSymbol;
            this.BarSpecification = barSpec;
            this.FromDateTime = fromDateTime;
            this.ToDateTime = toDateTime;
        }

        /// <summary>
        /// Gets the requests bar type.
        /// </summary>
        public Symbol Symbol { get; }

        /// <summary>
        /// Gets the requests bar specification.
        /// </summary>
        public BarSpecification BarSpecification { get; }

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
