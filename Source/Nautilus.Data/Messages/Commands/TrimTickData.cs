// -------------------------------------------------------------------------------------------------
// <copyright file="TrimTickData.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Messages.Commands
{
    using System;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Message;
    using NodaTime;

    /// <summary>
    /// Represents a scheduled command to trim the tick data with timestamps prior to the trim from date time.
    /// </summary>
    [Immutable]
    public sealed class TrimTickData : Command, IScheduledJob
    {
        private static readonly Type EventType = typeof(TrimTickData);

        /// <summary>
        /// Initializes a new instance of the <see cref="TrimTickData"/> class.
        /// </summary>
        /// <param name="rollingDays">The date time the tick data should be trimmed from.</param>
        /// <param name="scheduledTime">The command scheduled time.</param>
        /// <param name="commandId">The command identifier.</param>
        /// <param name="commandTimestamp">The command timestamp.</param>
        public TrimTickData(
            int rollingDays,
            ZonedDateTime scheduledTime,
            Guid commandId,
            ZonedDateTime commandTimestamp)
            : base(
                EventType,
                commandId,
                commandTimestamp)
        {
            Condition.PositiveInt32(rollingDays, nameof(rollingDays));

            this.RollingDays = rollingDays;
            this.ScheduledTime = scheduledTime;
        }

        /// <inheritdoc />
        public ZonedDateTime ScheduledTime { get; }

        /// <summary>
        /// Gets the commands rolling days to trim to.
        /// </summary>
        public int RollingDays { get; }
    }
}
