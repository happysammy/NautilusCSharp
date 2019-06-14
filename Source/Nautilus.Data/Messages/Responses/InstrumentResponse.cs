//--------------------------------------------------------------------------------------------------
// <copyright file="InstrumentResponse.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Messages.Responses
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Network.Messages;
    using NodaTime;

    /// <summary>
    /// Represents a response of instrument data.
    /// </summary>
    [Immutable]
    public sealed class InstrumentResponse : Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InstrumentResponse"/> class.
        /// </summary>
        /// <param name="instruments">The response instruments.</param>
        /// <param name="correlationId">The response correlation identifier.</param>
        /// <param name="id">The response identifier.</param>
        /// <param name="timestamp">The response timestamp.</param>
        public InstrumentResponse(
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
        /// Gets the responses instruments data.
        /// </summary>
        public byte[][] Instruments { get; }
    }
}
