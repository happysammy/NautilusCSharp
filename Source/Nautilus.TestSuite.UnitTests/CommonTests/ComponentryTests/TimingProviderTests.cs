//--------------------------------------------------------------------------------------------------
// <copyright file="TimingProviderTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.CommonTests.ComponentryTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Common.Componentry;
    using Nautilus.Core.Types;
    using Nautilus.TestSuite.TestKit.Stubs;
    using NodaTime;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global", Justification = "Test Suite")]
    public sealed class TimingProviderTests
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
        internal void IsInsideInterval_WithVariousDateTimes_ReturnsExpectedResult(
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
            var start = new WeeklyTime(IsoDayOfWeek.Saturday, new LocalTime(20, 00));
            var end = new WeeklyTime(IsoDayOfWeek.Sunday, new LocalTime(21, 00));
            var result = TimingProvider.IsInsideInterval(start, end, utcNow.ToInstant());

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
        internal void IsOutSideInterval_WithVariousDateTimes_ReturnsExpectedResult(
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
            var start = new WeeklyTime(IsoDayOfWeek.Saturday, new LocalTime(20, 00));
            var end = new WeeklyTime(IsoDayOfWeek.Sunday, new LocalTime(21, 00));
            var result = TimingProvider.IsOutsideInterval(start, end, utcNow.ToInstant());

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        internal void GetNextUtc_FromUnixEpoch_ReturnsExpectedResult()
        {
            // Arrange
            var utcNow = StubZonedDateTime.UnixEpoch();

            // Act
            var result = TimingProvider.GetNextUtc(new WeeklyTime(IsoDayOfWeek.Sunday, LocalTime.Midnight), utcNow.ToInstant());

            // Assert
            Assert.Equal(StubZonedDateTime.UnixEpoch() + Duration.FromDays(3), result);
        }

        [Fact]
        internal void GetDurationToNextUtc_WithUnixEpochPlusOneMinutes_ReturnsExpectedResult()
        {
            // Arrange
            var durationToNext = Duration.FromMinutes(1);
            var next = StubZonedDateTime.UnixEpoch() + durationToNext;
            var now = StubZonedDateTime.UnixEpoch().ToInstant();

            // Act
            var result = TimingProvider.GetDurationToNextUtc(next, now);

            // Assert
            Assert.Equal(durationToNext, result);
        }

        [Theory]
        [InlineData(250, 1000, 750)]
        [InlineData(500, 1000, 500)]
        [InlineData(999, 1000, 1)]
        internal void GetDelayToNextDuration_WithVariousDurations_ReturnsExpectedResult(
            int millisecondsInitialOffset,
            int millisecondsDuration,
            int millisecondsDelay)
        {
            // Arrange
            var utcNow = StubZonedDateTime.UnixEpoch() + Duration.FromMilliseconds(millisecondsInitialOffset);
            var duration = Duration.FromMilliseconds(millisecondsDuration);

            // Act
            var result = TimingProvider.GetDelayToNextDuration(utcNow, duration);

            // Assert
            Assert.Equal(Duration.FromMilliseconds(millisecondsDelay), result);
        }
    }
}
