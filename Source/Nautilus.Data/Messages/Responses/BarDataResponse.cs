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
    using System.Collections.Generic;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Data.Messages.Requests;
    using Nautilus.DomainModel.ValueObjects;
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
        /// <param name="symbol">The response bar data symbol.</param>
        /// <param name="barSpec">The response bar data specification.</param>
        /// <param name="barsCount">The response bars count.</param>
        /// <param name="correlationId">The response correlation identifier.</param>
        /// <param name="id">The response identifier.</param>
        /// <param name="timestamp">The response timestamp.</param>
        public BarDataResponse(
            Symbol symbol,
            BarSpecification barSpec,
            int barsCount,
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

            this.Symbol = symbol;
            this.BarSpecification = barSpec;
            this.BarsCount = barsCount;
        }

        /// <summary>
        /// Gets the responses bar data symbol.
        /// </summary>
        public Symbol Symbol { get; }

        /// <summary>
        /// Gets the responses bar data specification.
        /// </summary>
        public BarSpecification BarSpecification { get; }

        /// <summary>
        /// Gets the count of bars about to be sent.
        /// </summary>
        public int BarsCount { get; }
    }
}
