//--------------------------------------------------------------------------------------------------
// <copyright file="WeeklyTime.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Types
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using NodaTime;

    /// <summary>
    /// Represents a particular time every week.
    /// </summary>
    public sealed class WeeklyTime
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WeeklyTime"/> class.
        /// </summary>
        /// <param name="dayOfWeek">The day of the week.</param>
        /// <param name="time">The time of the week.</param>
        public WeeklyTime(IsoDayOfWeek dayOfWeek, [CanBeDefault] LocalTime time)
        {
            Condition.NotDefault(dayOfWeek, nameof(dayOfWeek));

            this.DayOfWeek = dayOfWeek;
            this.Time = time;
        }

        /// <summary>
        /// Gets the day of the week.
        /// </summary>
        public IsoDayOfWeek DayOfWeek { get; }

        /// <summary>
        /// Gets the time of the week.
        /// </summary>
        public LocalTime Time { get; }
    }
}
