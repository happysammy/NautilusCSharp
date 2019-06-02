//--------------------------------------------------------------------------------------------------
// <copyright file="InstrumentDataResponse.cs" company="Nautech Systems Pty Ltd">
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
    using NodaTime;

    /// <summary>
    /// Represents a response of instrument data.
    /// </summary>
    [Immutable]
    public sealed class InstrumentDataResponse : Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InstrumentDataResponse"/> class.
        /// </summary>
        /// <param name="instruments">The response instruments.</param>
        /// <param name="correlationId">The response correlation identifier.</param>
        /// <param name="id">The response identifier.</param>
        /// <param name="timestamp">The response timestamp.</param>
        public InstrumentDataResponse(
            byte[][] instruments,
            Guid correlationId,
            Guid id,
            ZonedDateTime timestamp)
            : base(
                typeof(BarDataResponse),
                correlationId,
                id,
                timestamp)
        {
            Debug.NotDefault(id, nameof(id));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.Instruments = instruments;
        }

        /// <summary>
        /// Gets the tick data.
        /// </summary>
        public byte[][] Instruments { get; }
    }
}
