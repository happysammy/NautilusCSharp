//--------------------------------------------------------------------------------------------------
// <copyright file="MarketDataEvent.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Events
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
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
        /// <param name="symbol">The event symbol.</param>
        /// <param name="barSpecification">The event bar profile.</param>
        /// <param name="bar">The event trade bar.</param>
        /// <param name="lastTick">The event last tick.</param>
        /// <param name="averageSpread">The event average spread.</param>
        /// <param name="isHistorical">The event is historical flag.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="eventTimestamp">The event timestamp.</param>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if any
        /// struct argument is the default value, or if the average spread is zero or negative.</exception>
        public BarDataEvent(
            Symbol symbol,
            BarSpecification barSpecification,
            Bar bar,
            Tick lastTick,
            decimal averageSpread,
            bool isHistorical,
            Guid eventId,
            ZonedDateTime eventTimestamp)
            : base(eventId, eventTimestamp)
        {
            Validate.NotNull(symbol, nameof(symbol));
            Validate.NotNull(barSpecification, nameof(barSpecification));
            Validate.NotNull(bar, nameof(bar));
            Validate.NotNull(lastTick, nameof(lastTick));
            Validate.DecimalNotOutOfRange(averageSpread, nameof(averageSpread), decimal.Zero, decimal.MaxValue, RangeEndPoints.Exclusive);

            this.Symbol = symbol;
            this.BarSpecification = barSpecification;
            this.Bar = bar;
            this.LastTick = lastTick;
            this.AverageSpread = averageSpread;
            this.IsHistorical = isHistorical;
        }

        /// <summary>
        /// Gets the events symbol.
        /// </summary>
        public Symbol Symbol { get; }

        /// <summary>
        /// Gets the events bar specification.
        /// </summary>
        public BarSpecification BarSpecification { get; }

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

        /// <summary>
        /// Returns a string representation of the <see cref="BarDataEvent"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => nameof(BarDataEvent);
    }
}
