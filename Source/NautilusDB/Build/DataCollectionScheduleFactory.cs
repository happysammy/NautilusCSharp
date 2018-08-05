//--------------------------------------------------------------------------------------------------
// <copyright file="DataCollectionScheduleFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusDB.Build
{
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Validation;
    using Nautilus.Data.Orchestration;
    using Newtonsoft.Json.Linq;
    using NodaTime;

    /// <summary>
    /// Provides a factory for creating a <see cref="DataCollectionSchedule"/> from the given time and
    /// configuration.
    /// </summary>
    public static class DataCollectionScheduleFactory
    {
        /// <summary>
        /// Returns a new <see cref="DataCollectionSchedule"/> from the given time and configuration.
        /// </summary>
        /// <param name="timeNow">The time now (provided by a <see cref="IZonedClock"/>.</param>
        /// <param name="config">The data collection schedule configuration JSON object.</param>
        /// <returns>A <see cref="DataCollectionSchedule"/>.</returns>
        public static DataCollectionSchedule Create(ZonedDateTime timeNow, JObject config)
        {
            Validate.NotDefault(timeNow, nameof(timeNow));
            Validate.NotNull(config, nameof(config));

            var collectionDay = (string)config["collectionDay"];

            return new DataCollectionSchedule(
                timeNow,
                collectionDay.ToEnum<IsoDayOfWeek>(),
                (int)config["collectionHourUtc"],
                (int)config["collectionMinuteUtc"],
                (bool)config["intervalicCollection"],
                (int)config["intervalMinutes"]);
        }
    }
}
