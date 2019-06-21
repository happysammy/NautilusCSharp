//--------------------------------------------------------------------------------------------------
// <copyright file="InstrumentsRequest.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.DomainModel.Enums;
    using Nautilus.Network.Messages;
    using NodaTime;

    /// <summary>
    /// Represents a request for all instruments for a venue.
    /// </summary>
    [Immutable]
    public sealed class InstrumentsRequest : Request
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InstrumentsRequest"/> class.
        /// </summary>
        /// <param name="venue">The request venue.</param>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="requestTimestamp">The request timestamp.</param>
        public InstrumentsRequest(
            Venue venue,
            Guid requestId,
            ZonedDateTime requestTimestamp)
            : base(
                typeof(InstrumentsRequest),
                requestId,
                requestTimestamp)
        {
            Debug.NotDefault(requestId, nameof(requestId));
            Debug.NotDefault(requestTimestamp, nameof(requestTimestamp));

            this.Venue = venue;
        }

        /// <summary>
        /// Gets the requests instruments venue.
        /// </summary>
        public Venue Venue { get; }
    }
}
