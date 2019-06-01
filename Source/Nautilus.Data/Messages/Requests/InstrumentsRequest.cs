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
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Enums;
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
        /// <param name="id">The requests identifier.</param>
        /// <param name="timestamp">The requests timestamp.</param>
        public InstrumentsRequest(
            Venue venue,
            Guid id,
            ZonedDateTime timestamp)
            : base(
                typeof(InstrumentsRequest),
                id,
                timestamp)
        {
            Debug.NotDefault(id, nameof(id));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.Venue = venue;
        }

        /// <summary>
        /// Gets the requests instruments venue.
        /// </summary>
        public Venue Venue { get; }
    }
}
