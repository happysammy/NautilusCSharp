//--------------------------------------------------------------------------------------------------
// <copyright file="MarketOpened.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Events
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Message;
    using Nautilus.DomainModel.Identifiers;
    using NodaTime;

    /// <summary>
    /// Represents an event where a market has opened.
    /// </summary>
    [Immutable]
    public sealed class MarketOpened : Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarketOpened" /> class.
        /// </summary>
        /// <param name="symbol">The symbol of the market.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="eventTimestamp">The event timestamp.</param>
        public MarketOpened(
            Symbol symbol,
            Guid eventId,
            ZonedDateTime eventTimestamp)
            : base(typeof(MarketOpened), eventId, eventTimestamp)
        {
            Debug.NotDefault(eventId, nameof(eventId));
            Debug.NotDefault(eventTimestamp, nameof(eventTimestamp));

            this.Symbol = symbol;
        }

        /// <summary>
        /// Gets the market events symbol.
        /// </summary>
        public Symbol Symbol { get; }
    }
}
