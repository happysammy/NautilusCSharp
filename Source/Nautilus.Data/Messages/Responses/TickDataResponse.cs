//--------------------------------------------------------------------------------------------------
// <copyright file="TickDataResponse.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Network;
    using NodaTime;

    /// <summary>
    /// Represents a response of historical tick data.
    /// </summary>
    [Immutable]
    public sealed class TickDataResponse : Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TickDataResponse"/> class.
        /// </summary>
        /// <param name="symbol">The response tick data symbol.</param>
        /// <param name="ticks">The response ticks.</param>
        /// <param name="correlationId">The response correlation identifier.</param>
        /// <param name="id">The response identifier.</param>
        /// <param name="timestamp">The response timestamp.</param>
        public TickDataResponse(
            Symbol symbol,
            byte[][] ticks,
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

            this.Symbol = symbol;
            this.Ticks = ticks;
        }

        /// <summary>
        /// Gets the responses tick data symbol.
        /// </summary>
        public Symbol Symbol { get; }

        /// <summary>
        /// Gets the tick data.
        /// </summary>
        public byte[][] Ticks { get; }
    }
}
