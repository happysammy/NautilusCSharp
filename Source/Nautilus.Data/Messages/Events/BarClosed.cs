//--------------------------------------------------------------------------------------------------
// <copyright file="BarClosed.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Messages.Events
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
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
        /// <param name="id">The message identifier.</param>
        public BarClosed(
            BarType barType,
            Bar bar,
            Guid id)
            : base(id, bar.Timestamp)
        {
            this.BarType = barType;
            this.Bar = bar;
        }

        /// <summary>
        /// Gets the events bar type.
        /// </summary>
        public BarType BarType { get; }

        /// <summary>
        /// Gets the events bar.
        /// </summary>
        public Bar Bar { get; }
    }
}
