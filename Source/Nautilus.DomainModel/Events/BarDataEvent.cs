//--------------------------------------------------------------------------------------------------
// <copyright file="BarDataEvent.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Events
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Represents an event where a trade bar was closed.
    /// </summary>
    [Immutable]
    public sealed class BarDataEvent : Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BarDataEvent"/> class.
        /// </summary>
        /// <param name="barType">The bar type.</param>
        /// <param name="bar">The event trade bar.</param>
        /// <param name="lastTick">The event last tick.</param>
        /// <param name="averageSpread">The event average spread.</param>
        /// <param name="isHistorical">The event is historical flag.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="eventTimestamp">The event timestamp.</param>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if any
        /// struct argument is the default value, or if the average spread is zero or negative.</exception>
        public BarDataEvent(
            BarType barType,
            Bar bar,
            Tick lastTick,
            decimal averageSpread,
            bool isHistorical,
            Guid eventId,
            ZonedDateTime eventTimestamp)
            : base(eventId, eventTimestamp)
        {
            Debug.NotNull(barType, nameof(barType));
            Debug.NotNull(bar, nameof(bar));
            Debug.NotNull(lastTick, nameof(lastTick));
            Debug.PositiveDecimal(averageSpread, nameof(averageSpread));

            this.BarType = barType;
            this.Bar = bar;
            this.LastTick = lastTick;
            this.AverageSpread = averageSpread;
            this.IsHistorical = isHistorical;
        }

        /// <summary>
        /// Gets the events bar type.
        /// </summary>
        public BarType BarType { get; }

        /// <summary>
        /// Gets the events bar.
        /// </summary>
        public Bar Bar { get; }

        /// <summary>
        /// Gets the events last quote.
        /// </summary>
        public Tick LastTick { get; }

        /// <summary>
        /// Gets the events average spread.
        /// </summary>
        public decimal AverageSpread { get; }

        /// <summary>
        /// Gets a value indicating whether the market event is historical.
        /// </summary>
        public bool IsHistorical { get; }
    }
}
