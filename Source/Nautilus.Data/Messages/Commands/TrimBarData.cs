// -------------------------------------------------------------------------------------------------
// <copyright file="TrimBarData.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Messages.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Represents a command to trim the bar data keys held in the database
    /// to be equal to the size of the rolling window at a scheduled time.
    /// </summary>
    [Immutable]
    public class TrimBarData : Command, IScheduledJob
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrimBarData"/> class.
        /// </summary>
        /// <param name="barSpecifications">The bar specifications to trim.</param>
        /// <param name="rollingWindowDays">The number of days in the rolling window.</param>
        /// <param name="scheduledTime">The commands scheduled time.</param>
        /// <param name="id">The commands identifier.</param>
        /// <param name="timestamp">The commands creation timestamp.</param>
        public TrimBarData(
            IEnumerable<BarSpecification> barSpecifications,
            int rollingWindowDays,
            ZonedDateTime scheduledTime,
            Guid id,
            ZonedDateTime timestamp)
            : base(id, timestamp)
        {
            Debug.PositiveInt32(rollingWindowDays, nameof(rollingWindowDays));
            Debug.NotDefault(scheduledTime, nameof(scheduledTime));

            this.ScheduledTime = scheduledTime;
            this.RollingWindowDays = rollingWindowDays;
            this.Resolutions = barSpecifications.Select(b => b.Resolution).Distinct();
        }

        /// <inheritdoc />
        public ZonedDateTime ScheduledTime { get; }

        /// <summary>
        /// Gets the number of days in the rolling window to trim to.
        /// </summary>
        public int RollingWindowDays { get; }

        /// <summary>
        /// Gets the bar specifications to trim.
        /// </summary>
        public IEnumerable<Resolution> Resolutions { get;  }
    }
}
