//--------------------------------------------------------------------------------------------------
// <copyright file="BarClosed.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Messages.Events
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The event where a trade bar was closed.
    /// </summary>
    [Immutable]
    public sealed class BarClosed : Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BarClosed"/> class.
        /// </summary>
        /// <param name="barType">The message bar type.</param>
        /// <param name="bar">The message bar.</param>
        /// <param name="lastTick">The message last tick.</param>
        /// <param name="averageSpread">The message average spread.</param>
        /// <param name="id">The message identifier.</param>
        public BarClosed(
            BarType barType,
            Bar bar,
            Tick lastTick,
            decimal averageSpread,
            Guid id)
            : base(id, bar.Timestamp)
        {
            Debug.NotNull(barType, nameof(barType));
            Debug.NotNull(bar, nameof(bar));
            Debug.NotNull(lastTick, nameof(lastTick));
            Debug.PositiveDecimal(averageSpread, nameof(averageSpread));
            Debug.NotDefault(id, nameof(id));

            this.BarType = barType;
            this.Bar = bar;
            this.LastTick = lastTick;
            this.AverageSpread = averageSpread;
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
        /// Gets the events last tick at bar close.
        /// </summary>
        public Tick LastTick { get; }

        /// <summary>
        /// Gets the events average spread.
        /// </summary>
        public decimal AverageSpread { get; }
    }
}
