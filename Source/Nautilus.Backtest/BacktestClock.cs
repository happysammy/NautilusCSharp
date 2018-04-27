// -------------------------------------------------------------------------------------------------
// <copyright file="BacktestClock.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Backtest
{
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core.Interfaces;
    using NodaTime;

    /// <summary>
    /// The system clock.
    /// </summary>
    public class BacktestClock : IZonedClock
    {
        private readonly DateTimeZone dateTimeZone;
        private readonly Period timeIncrement;
        private ZonedDateTime timeNow;

        /// <summary>
        /// Initializes a new instance of the <see cref="BacktestClock"/> class.
        /// </summary>
        /// <param name="startTime">
        /// The start time.
        /// </param>
        /// <param name="timeIncrement">
        /// The time increment.
        /// </param>
        public BacktestClock(ZonedDateTime startTime, Period timeIncrement)
        {
            Validate.NotNull(timeIncrement, nameof(timeIncrement));

            this.timeNow = startTime;
            this.dateTimeZone = startTime.Zone;
            this.timeIncrement = timeIncrement;
        }

        /// <summary>
        /// The time now.
        /// </summary>
        /// <returns>
        /// The <see cref="ZonedDateTime"/>.
        /// </returns>
        public ZonedDateTime TimeNow()
        {
            return this.timeNow;
        }

        /// <summary>
        /// The increment time.
        /// </summary>
        public void IncrementTime()
        {
            this.timeNow += this.timeIncrement.ToDuration();
        }

        /// <summary>
        /// The set time.
        /// </summary>
        /// <param name="time">
        /// The time.
        /// </param>
        public void SetTime(ZonedDateTime time)
        {
            Validate.True(ZonedDateTime.Comparer.Instant.Compare(time, this.timeNow) > 0, nameof(time));

            this.timeNow = time;
        }

        /// <summary>
        /// The get time zone.
        /// </summary>
        /// <returns>
        /// The <see cref="DateTimeZone"/>.
        /// </returns>
        public DateTimeZone GetTimeZone()
        {
            return this.dateTimeZone;
        }
    }
}
