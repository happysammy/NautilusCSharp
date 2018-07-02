//--------------------------------------------------------------------------------------------------
// <copyright file="MarketClosed.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Messages.Events
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using NodaTime;

    /// <summary>
    /// Represents an event where a financial market has closed.
    /// </summary>
    [Immutable]
    public class MarketClosed : Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarketClosed"/> class.
        /// </summary>
        /// <param name="id">The event identifier.</param>
        /// <param name="timestamp">The event timestamp.</param>
        public MarketClosed(Guid id, ZonedDateTime timestamp) : base(id, timestamp)
        {
            Debug.NotDefault(id, nameof(id));
            Debug.NotDefault(timestamp, nameof(timestamp));
        }
    }
}
