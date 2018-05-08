// -------------------------------------------------------------------------------------------------
// <copyright file="DataCollectionScheduleTests.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DatabaseTests.CoreTests
{
    using System.Diagnostics.CodeAnalysis;
    using NautechSystems.CSharp.Extensions;
    using NodaTime;
    using Xunit;
    using Xunit.Abstractions;
    using Nautilus.Database.Core.Orchestration;
    using Nautilus.TestSuite.TestKit.TestDoubles;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class DataCollectionScheduleTests
    {
        private readonly ITestOutputHelper output;

        public DataCollectionScheduleTests(ITestOutputHelper output)
        {
            // Fixture Sett
            this.output = output;
        }

        [Fact]
        internal void UpdateLastCollectedTime_WithValidTime_UpdatesValues()
        {
            // Arrange
            var clock = new StubClock();
            clock.FreezeSetTime(StubZonedDateTime.UnixEpoch());


            var schedule = new DataCollectionSchedule(
                clock.TimeNow(),
                IsoDayOfWeek.Saturday,
                12,
                0,
                false,
                0);

            // Act
            schedule.UpdateLastCollectedTime(clock.TimeNow());
            var result = schedule.LastCollectedTime;

            // Assert
            Assert.Equal(clock.TimeNow(), result);
        }

        [Fact]
        internal void GetNextCollectionTimeToGo_WithMajorCollectionTime_ReturnsExpectedDuration()
        {
            // Arrange
            var clock = new StubClock();
            clock.FreezeSetTime(StubZonedDateTime.UnixEpoch());


            var schedule = new DataCollectionSchedule(
                clock.TimeNow(),
                IsoDayOfWeek.Saturday,
                12,
                0,
                false,
                0);

            schedule.UpdateLastCollectedTime(clock.TimeNow());

            // Act
            var result = schedule.GetNextCollectionTimeToGo(clock.TimeNow());

            // Assert
            Assert.Equal(Duration.FromDays(2) + Duration.FromHours(12), result);
        }

        [Fact]
        internal void GetNextCollectionTimeToGo_WithIntervalicCollectionTime_ReturnsExpectedDuration()
        {
            // Arrange
            var clock = new StubClock();
            clock.FreezeSetTime(StubZonedDateTime.UnixEpoch());


            var schedule = new DataCollectionSchedule(
                clock.TimeNow(),
                IsoDayOfWeek.Saturday,
                12,
                0,
                true,
                1);

            schedule.UpdateLastCollectedTime(clock.TimeNow());

            // Act
            var result = schedule.GetNextCollectionTimeToGo(clock.TimeNow());

            // Assert
            Assert.Equal(Duration.FromMinutes(1), result);
        }

        [Theory]
        [InlineData(-1, false)]
        [InlineData(1, true)]
        [InlineData(2, true)]
        internal void IsDataDueForCollection_WithMajorCollectionTime_ReturnsExpectedDuration(
            int offsetMinutes,
            bool expected)
        {
            // Arrange
            var clock = new StubClock();
            clock.FreezeSetTime(StubZonedDateTime.UnixEpoch());

            var schedule = new DataCollectionSchedule(
                clock.TimeNow(),
                IsoDayOfWeek.Saturday,
                12,
                0,
                true,
                1);

            schedule.UpdateLastCollectedTime(clock.TimeNow());

            clock.FreezeSetTime(clock.TimeNow() + Duration.FromMinutes(offsetMinutes));

            // Act
            var result = schedule.IsDataDueForCollection(clock.TimeNow());

            // Assert
            this.output.WriteLine($"      ClockTimeNow={clock.TimeNow().ToIsoString()}");
            this.output.WriteLine($"NextCollectionTime={schedule.NextCollectionTime.ToIsoString()}");
            Assert.Equal(expected, result);
        }

        [Fact]
        internal void CanScheduleMajorCollectionTimes_WithVariousInputs_ReturnsExpectedTimes()
        {
            // Arrange
            var clock = new StubClock();
            clock.FreezeSetTime(StubZonedDateTime.UnixEpoch());

            var schedule = new DataCollectionSchedule(
                clock.TimeNow(),
                IsoDayOfWeek.Saturday,
                12,
                0,
                false,
                0);

            var nextSatCollection = clock.TimeNow() + Duration.FromDays(2) + Duration.FromHours(12);

            // Act
            schedule.UpdateLastCollectedTime(clock.TimeNow());

            // Assert
            Assert.Equal(nextSatCollection, schedule.NextCollectionTime);
        }

        [Fact]
        internal void CanScheduleIntervalicCollectionTimes_WithVariousInputs_ReturnsExpectedTimes()
        {
            // Arrange
            var clock = new StubClock();
            clock.FreezeSetTime(StubZonedDateTime.UnixEpoch());

            var schedule = new DataCollectionSchedule(
                clock.TimeNow(),
                IsoDayOfWeek.Saturday,
                12,
                0,
                true,
                60);

            // Act
            schedule.UpdateLastCollectedTime(clock.TimeNow());

            // Assert
            Assert.Equal(StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(60), schedule.NextCollectionTime);
        }
    }
}
