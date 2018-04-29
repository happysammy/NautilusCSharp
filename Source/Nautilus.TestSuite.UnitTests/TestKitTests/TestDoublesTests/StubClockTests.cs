//--------------------------------------------------------------------------------------------------
// <copyright file="StubClockTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.TestKitTests.TestDoublesTests
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NodaTime;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class StubClockTests
    {
        [Fact]
        internal void TimeNow_WithStubSystemClock_ReturnsStubbedDateTime()
        {
            // Arrange
            var stubClock = new StubClock();

            // Act
            stubClock.FreezeSetTime(StubDateTime.Now());

            // Assert
            Assert.Equal(StubDateTime.Now(), stubClock.TimeNow());
        }

        [Fact]
        internal void FreezeSetTime_WithStubSystemClockFrozenThenUnFrozen_ReturnsExpectedTimes()
        {
            // Arrange
            var stubClock = new StubClock();

            // Act
            stubClock.FreezeSetTime(StubDateTime.Now());
            var result1 = stubClock.TimeNow();
            Task.Delay(300).Wait();
            stubClock.UnfreezeTime();
            var result2 = stubClock.TimeNow();

            // Assert
            Assert.True(result1.TickOfDay < result2.TickOfDay);
        }

        [Fact]
        internal void FreezeTimeNow_WithStubSystemClock_ReturnsExpectedTime()
        {
            // Arrange
            var stubClock = new StubClock();

            // Act
            stubClock.FreezeTimeNow();
            var result1 = stubClock.TimeNow();
            Task.Delay(100).Wait();
            var result2 = stubClock.TimeNow();

            // Assert
            Assert.Equal(result1, result2);
        }

        [Fact]
        internal void GetTimeZone_ReturnsCorrectTimeZone()
        {
            // Arrange
            var stubClock = new StubClock();

            // Act
            var result = stubClock.GetTimeZone();

            // Assert
            Assert.Equal(DateTimeZone.Utc, result);
        }
    }
}
