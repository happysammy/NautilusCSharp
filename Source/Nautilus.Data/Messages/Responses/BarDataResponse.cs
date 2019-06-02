//--------------------------------------------------------------------------------------------------
// <copyright file="BarDataResponse.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Messages.Responses
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Data.Messages.Requests;
    using Nautilus.Data.Types;
    using NodaTime;

    /// <summary>
    /// Represents a response of historical bar data.
    /// </summary>
    [Immutable]
    public sealed class BarDataResponse : Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BarDataResponse"/> class.
        /// </summary>
        /// <param name="barData">The response bar data.</param>
        /// <param name="correlationId">The response correlation identifier.</param>
        /// <param name="id">The response identifier.</param>
        /// <param name="timestamp">The response timestamp.</param>
        public BarDataResponse(
            BarDataFrame barData,
            Guid correlationId,
            Guid id,
            ZonedDateTime timestamp)
            : base(
                typeof(BarDataRequest),
                correlationId,
                id,
                timestamp)
        {
            Debug.NotDefault(id, nameof(id));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.BarData = barData;
        }

        /// <summary>
        /// Gets the responses <see cref="BarDataFrame"/>.
        /// </summary>
        public BarDataFrame BarData { get; }
    }
}
