//--------------------------------------------------------------------------------------------------
// <copyright file="MarketOpened.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Represents an event where a financial market has opened.
    /// </summary>
    [Immutable]
    public sealed class MarketOpened : Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarketOpened"/> class.
        /// </summary>
        /// <param name="symbol">The symbol of the market.</param>
        /// <param name="openedTime">The market opened time.</param>
        /// <param name="id">The event identifier.</param>
        /// <param name="timestamp">The event timestamp.</param>
        public MarketOpened(
            Symbol symbol,
            ZonedDateTime openedTime,
            Guid id,
            ZonedDateTime timestamp)
            : base(typeof(MarketOpened), id, timestamp)
        {
            Debug.NotDefault(id, nameof(id));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.Symbol = symbol;
            this.OpenedTime = openedTime;
        }

        /// <summary>
        /// Gets the events symbol.
        /// </summary>
        public Symbol Symbol { get; }

        /// <summary>
        /// Gets the events market opened time.
        /// </summary>
        public ZonedDateTime OpenedTime { get; }
    }
}
