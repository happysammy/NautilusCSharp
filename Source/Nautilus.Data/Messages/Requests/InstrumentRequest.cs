//--------------------------------------------------------------------------------------------------
// <copyright file="InstrumentRequest.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Represents a request for an instrument.
    /// </summary>
    [Immutable]
    public sealed class InstrumentRequest : Request
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InstrumentRequest"/> class.
        /// </summary>
        /// <param name="symbol">The request symbol.</param>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="requestTimestamp">The request timestamp.</param>
        public InstrumentRequest(
            Symbol symbol,
            Guid requestId,
            ZonedDateTime requestTimestamp)
            : base(
                typeof(InstrumentRequest),
                requestId,
                requestTimestamp)
        {
            Debug.NotDefault(requestId, nameof(requestId));
            Debug.NotDefault(requestTimestamp, nameof(requestTimestamp));

            this.Symbol = symbol;
        }

        /// <summary>
        /// Gets the requests instrument symbol.
        /// </summary>
        public Symbol Symbol { get; }
    }
}
