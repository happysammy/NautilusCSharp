//--------------------------------------------------------------------------------------------------
// <copyright file="TrimBarData.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Messages.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Represents a command to trim the bar data keys held in the database to be equal to the size
    /// of the given rolling window (days).
    /// </summary>
    [Immutable]
    public class TrimBarData : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrimBarData"/> class.
        /// </summary>
        /// <param name="barSpecifications">The bar specifications to trim.</param>
        /// <param name="rollingWindowDays">The bar data rolling window size to trim to.</param>
        /// <param name="id">The message identifier.</param>
        /// <param name="timestamp">The message timestamp.</param>
        public TrimBarData(
            IEnumerable<BarSpecification> barSpecifications,
            int rollingWindowDays,
            Guid id,
            ZonedDateTime timestamp)
            : base(id, timestamp)
        {
            Debug.PositiveInt32(rollingWindowDays, nameof(rollingWindowDays));

            this.Resolutions = barSpecifications.Select(b => b.Resolution).Distinct();
            this.RollingWindowDaysDays = rollingWindowDays;
        }

        /// <summary>
        /// Gets the resolutions for the bar data trimming operation.
        /// </summary>
        public IEnumerable<Resolution> Resolutions { get; }

        /// <summary>
        /// Gets the rolling window size for the bar data trimming operation.
        /// </summary>
        public int RollingWindowDaysDays { get; }
    }
}
