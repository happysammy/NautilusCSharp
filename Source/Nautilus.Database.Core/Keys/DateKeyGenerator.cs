//--------------------------------------------------------------------------------------------------
// <copyright file="DateKeyGenerator.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Core.Keys
{
    using System;
    using System.Collections.Generic;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Validation;
    using NodaTime;

    public static class DateKeyGenerator
    {
        /// <summary>
        /// Returns an array of <see cref="DateKey"/>s based on the given from and to <see cref="ZonedDateTime"/> range.
        /// </summary>
        /// <param name="fromDateTime">The from date time.</param>
        /// <param name="toDateTime">The two date time</param>
        /// <returns>An array of <see cref="DateKey"/>.</returns>
        /// <remarks>The given time range should have been previously validated.</remarks>
        public static List<DateKey> GetDateKeys(ZonedDateTime fromDateTime, ZonedDateTime toDateTime)
        {
            Validate.NotDefault(fromDateTime, nameof(fromDateTime));
            Validate.NotDefault(toDateTime, nameof(toDateTime));
            Validate.True(!toDateTime.IsLessThan(fromDateTime), nameof(toDateTime));

            var difference = (toDateTime - fromDateTime) / Duration.FromDays(1);

            if (difference <= 1)
            {
                return new List<DateKey> { new DateKey(fromDateTime) };
            }

            var iterationCount = Convert.ToInt32(Math.Floor(difference));
            var dateKeys = new List<DateKey> { new DateKey(fromDateTime) };

            for (var i = 0; i < iterationCount; i++)
            {
                dateKeys.Add(new DateKey(fromDateTime + Duration.FromDays(i + 1)));
            }

            return dateKeys;
        }
    }
}
