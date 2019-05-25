//--------------------------------------------------------------------------------------------------
// <copyright file="TimeProviderTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.CommonTests.ComponentryTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Common.Componentry;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NodaTime;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class TimeProviderTests
    {
        [Theory]
        [InlineData(2018, 6, 30, 19, 59, false)]
        [InlineData(2018, 7, 1, 20, 00, false)]
        [InlineData(2018, 7, 1, 20, 59, false)]
        [InlineData(2018, 7, 1, 21, 0, false)]
        [InlineData(2018, 7, 2, 12, 0, false)]
        [InlineData(2018, 7, 3, 21, 0, false)]
        [InlineData(2018, 7, 4, 00, 0, false)]
        [InlineData(2018, 7, 7, 19, 59, false)]
        [InlineData(2018, 7, 7, 20, 00, true)]
        [InlineData(2018, 7, 7, 20, 01, true)]
        [InlineData(2008, 1, 26, 19, 59, false)]
        [InlineData(2008, 1, 26, 20, 00, true)]
        [InlineData(2008, 1, 26, 20, 59, true)]
        [InlineData(2008, 1, 27, 21, 0, false)]
        [InlineData(2008, 1, 28, 00, 0, false)]
        [InlineData(2008, 1, 29, 00, 0, false)]
        [InlineData(2008, 2, 2, 19, 59, false)]
        [InlineData(2008, 2, 2, 20, 00, true)]
        [InlineData(2008, 2, 2, 20, 01, true)]
        internal void IsInsideWeeklyInterval_WithVariousDateTimes_ReturnsExpectedResult(
            int year,
            int month,
            int day,
            int hour,
            int minute,
            bool expected)
        {
            // Arrange
            var localUtc = new LocalDateTime(year, month, day, hour, minute);
            var utcNow = new ZonedDateTime(localUtc, DateTimeZone.Utc, Offset.Zero);

            // Act
            var intervalStart = (IsoDayOfWeek.Saturday, new LocalTime(20, 00));
            var intervalEnd = (IsoDayOfWeek.Sunday, new LocalTime(21, 00));
            var result = TimeProvider.IsInsideWeeklyInterval(intervalStart, intervalEnd, utcNow.ToInstant());

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(2018, 6, 30, 19, 59, true)]
        [InlineData(2018, 7, 1, 20, 00, true)]
        [InlineData(2018, 7, 1, 20, 59, true)]
        [InlineData(2018, 7, 1, 21, 0, true)]
        [InlineData(2018, 7, 2, 12, 0, true)]
        [InlineData(2018, 7, 3, 21, 0, true)]
        [InlineData(2018, 7, 4, 00, 0, true)]
        [InlineData(2018, 7, 7, 19, 59, true)]
        [InlineData(2018, 7, 7, 20, 00, false)]
        [InlineData(2018, 7, 7, 20, 01, false)]
        [InlineData(2008, 1, 26, 19, 59, true)]
        [InlineData(2008, 1, 26, 20, 00, false)]
        [InlineData(2008, 1, 26, 20, 59, false)]
        [InlineData(2008, 1, 27, 21, 0, true)]
        [InlineData(2008, 1, 28, 00, 0, true)]
        [InlineData(2008, 1, 29, 00, 0, true)]
        [InlineData(2008, 2, 2, 19, 59, true)]
        [InlineData(2008, 2, 2, 20, 00, false)]
        [InlineData(2008, 2, 2, 20, 01, false)]
        internal void IsOutSideWeeklyInterval_WithVariousDateTimes_ReturnsExpectedResult(
            int year,
            int month,
            int day,
            int hour,
            int minute,
            bool expected)
        {
            // Arrange
            var localUtc = new LocalDateTime(year, month, day, hour, minute);
            var utcNow = new ZonedDateTime(localUtc, DateTimeZone.Utc, Offset.Zero);

            // Act
            var intervalStart = (IsoDayOfWeek.Saturday, new LocalTime(20, 00));
            var intervalEnd = (IsoDayOfWeek.Sunday, new LocalTime(21, 00));
            var result = TimeProvider.IsOutsideWeeklyInterval(intervalStart, intervalEnd, utcNow.ToInstant());

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(250, 1000, 750)]
        [InlineData(500, 1000, 500)]
        [InlineData(999, 1000, 1)]
        internal void GetDelayForDuration_WithVariousDurations_ReturnsExpectedResult(
            int millisecondsInitialOffset,
            int millisecondsDuration,
            int millisecondsDelay)
        {
            // Arrange
            var utcNow = StubZonedDateTime.UnixEpoch() + Duration.FromMilliseconds(millisecondsInitialOffset);
            var duration = Duration.FromMilliseconds(millisecondsDuration);

            // Act
            var result = TimeProvider.GetDelayForDuration(utcNow, duration);

            // Assert
            Assert.Equal(Duration.FromMilliseconds(millisecondsDelay), result);
        }
    }
}
