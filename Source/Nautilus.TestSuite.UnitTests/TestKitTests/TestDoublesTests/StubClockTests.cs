//--------------------------------------------------------------------------------------------------
// <copyright file="StubClockTests.cs" company="Nautech Systems Pty Ltd">
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

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Nautilus.TestSuite.TestKit.Components;
using Nautilus.TestSuite.TestKit.Stubs;
using NodaTime;
using Xunit;

namespace Nautilus.TestSuite.UnitTests.TestKitTests.TestDoublesTests
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class StubClockTests
    {
        [Fact]
        internal void TimeNow_WithStubSystemClock_ReturnsStubbedDateTime()
        {
            // Arrange
            var stubClock = new TestClock();

            // Act
            stubClock.FreezeSetTime(StubZonedDateTime.UnixEpoch());

            // Assert
            Assert.Equal(StubZonedDateTime.UnixEpoch(), stubClock.TimeNow());
        }

        [Fact]
        internal void FreezeSetTime_WithStubSystemClockFrozenThenUnFrozen_ReturnsExpectedTimes()
        {
            // Arrange
            var stubClock = new TestClock();

            // Act
            stubClock.FreezeSetTime(StubZonedDateTime.UnixEpoch());
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
            var stubClock = new TestClock();

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
            var stubClock = new TestClock();

            // Act
            var result = stubClock.GetTimeZone();

            // Assert
            Assert.Equal(DateTimeZone.Utc, result);
        }
    }
}
