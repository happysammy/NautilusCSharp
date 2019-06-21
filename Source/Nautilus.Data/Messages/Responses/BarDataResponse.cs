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
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Network.Messages;
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
        /// <param name="bars">The response bars count.</param>
        /// <param name="correlationId">The response correlation identifier.</param>
        /// <param name="responseId">The response identifier.</param>
        /// <param name="responseTimestamp">The response timestamp.</param>
        public BarDataResponse(
            Symbol symbol,
            BarSpecification barSpec,
            byte[][] bars,
            Guid correlationId,
            Guid responseId,
            ZonedDateTime responseTimestamp)
            : base(
                typeof(BarDataResponse),
                correlationId,
                responseId,
                responseTimestamp)
        {
            Debug.NotDefault(responseId, nameof(responseId));
            Debug.NotDefault(responseTimestamp, nameof(responseTimestamp));

            this.Symbol = symbol;
            this.BarSpecification = barSpec;
            this.Bars = bars;
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
        /// Gets the responses bar data.
        /// </summary>
        public byte[][] Bars { get; }
    }
}
