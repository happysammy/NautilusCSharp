// -------------------------------------------------------------------------------------------------
// <copyright file="TrimBarData.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Messages.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Message;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Represents a scheduled command to trim the bar data keys held in the database
    /// to be equal to the size of the rolling window.
    /// </summary>
    [Immutable]
    public sealed class TrimBarData : Command, IScheduledJob
    {
        private static readonly Type EventType = typeof(TrimBarData);

        /// <summary>
        /// Initializes a new instance of the <see cref="TrimBarData"/> class.
        /// </summary>
        /// <param name="barSpecifications">The bar specifications to trim.</param>
        /// <param name="rollingDays">The number of days in the rolling window.</param>
        /// <param name="scheduledTime">The command scheduled time.</param>
        /// <param name="commandId">The command identifier.</param>
        /// <param name="commandTimestamp">The command timestamp.</param>
        public TrimBarData(
            IEnumerable<BarSpecification> barSpecifications,
            int rollingDays,
            ZonedDateTime scheduledTime,
            Guid commandId,
            ZonedDateTime commandTimestamp)
            : base(
                EventType,
                commandId,
                commandTimestamp)
        {
            Debug.PositiveInt32(rollingDays, nameof(rollingDays));
            Debug.NotDefault(scheduledTime, nameof(scheduledTime));

            this.ScheduledTime = scheduledTime;
            this.RollingDays = rollingDays;
            this.BarStructures = barSpecifications.Select(b => b.BarStructure).Distinct();
        }

        /// <inheritdoc />
        public ZonedDateTime ScheduledTime { get; }

        /// <summary>
        /// Gets the commands rolling days to trim to.
        /// </summary>
        public int RollingDays { get; }

        /// <summary>
        /// Gets the commands bar structures to trim.
        /// </summary>
        public IEnumerable<BarStructure> BarStructures { get;  }
    }
}
