//--------------------------------------------------------------------------------------------------
// <copyright file="MarketClosed.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messages.Events
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Message;
    using Nautilus.DomainModel.Identifiers;
    using NodaTime;

    /// <summary>
    /// Represents an event where a financial market has closed.
    /// </summary>
    [Immutable]
    public sealed class MarketClosed : Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarketClosed"/> class.
        /// </summary>
        /// <param name="symbol">The symbol of the market.</param>
        /// <param name="closedTime">The market closed time.</param>
        /// <param name="id">The event identifier.</param>
        /// <param name="timestamp">The event timestamp.</param>
        public MarketClosed(
            Symbol symbol,
            ZonedDateTime closedTime,
            Guid id,
            ZonedDateTime timestamp)
            : base(typeof(MarketClosed), id, timestamp)
        {
            Debug.NotDefault(id, nameof(id));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.Symbol = symbol;
            this.ClosedTime = closedTime;
        }

        /// <summary>
        /// Gets the events symbol.
        /// </summary>
        public Symbol Symbol { get; }

        /// <summary>
        /// Gets the events market closed time.
        /// </summary>
        public ZonedDateTime ClosedTime { get; }
    }
}
