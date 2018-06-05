//--------------------------------------------------------------------------------------------------
// <copyright file="IndicatorBase.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Validation;
    using NodaTime;

    /// <summary>
    /// The abstract <see cref="IndicatorBase"/> class. The base class for all market indicators.
    /// </summary>
    public abstract class IndicatorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndicatorBase"/> class.
        /// </summary>
        /// <param name="name">The indicator name.</param>
        protected IndicatorBase(string name)
        {
            Validate.NotNull(name, nameof(name));

            this.Name = name;
        }

        /// <summary>
        /// Gets the indicators name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets a value indicating whether the indicator is initialized.
        /// </summary>
        public bool Initialized { get; protected set; }

        /// <summary>
        /// Gets the last time the indicator was updated (optional).
        /// </summary>
        public Option<ZonedDateTime?> LastTime { get; protected set; }

        /// <summary>
        /// Returns the number of decimal places from the given tick size decimal.
        /// </summary>
        /// <param name="tickSize">The tick size.</param>
        /// <returns>A <see cref="int"/>.</returns>
        protected int GetDecimalPlaces(decimal tickSize)
        {
            Validate.DecimalNotOutOfRange(tickSize, nameof(tickSize), decimal.Zero, decimal.MaxValue, RangeEndPoints.Exclusive);

            return Math.Max(tickSize.GetDecimalPlaces(), 2);
        }
    }
}
