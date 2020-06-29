//--------------------------------------------------------------------------------------------------
// <copyright file="NodaTimeExtensionsTests.cs" company="Nautech Systems Pty Ltd">
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

namespace Nautilus.TestSuite.UnitTests.CoreTests.ExtensionsTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core.Extensions;
    using Nautilus.TestSuite.TestKit.Stubs;
    using NodaTime;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class NodaTimeExtensionsTests
    {
        [Fact]
        internal void ToIso8601String_WithValidZonedDateTime_ReturnsExpectedString()
        {
            // Arrange
            var zonedDateTime = StubZonedDateTime.UnixEpoch();

            // Act
            var result = zonedDateTime.ToIso8601String();

            // Assert
            Assert.Equal("1970-01-01T00:00:00.000Z", result);
        }

        [Fact]
        internal void ToIso8601String_WithValidLocalDate_ReturnsExpectedString()
        {
            // Arrange
            var zonedDateTime = StubZonedDateTime.UnixEpoch();

            // Act
            var result = zonedDateTime.Date.ToIso8601String();

            // Assert
            Assert.Equal("1970-01-01", result);
        }

        [Fact]
        internal void ToZonedDateTimeFromIso_WithValidString_ReturnsExpectedTime()
        {
            // Arrange
            var zonedDateTimeString = StubZonedDateTime.UnixEpoch().ToIso8601String();

            // Act
            var result = zonedDateTimeString.ToZonedDateTimeFromIso();

            // Assert
            Assert.Equal(StubZonedDateTime.UnixEpoch(), result);
        }

        [Fact]
        internal void ToZonedDateTime_WithValidStringAndParsePattern_ReturnsExpectedString()
        {
            // Arrange
            var dateTimeString = "2018.01.12 23:59:00";

            // Act
            var result = dateTimeString.ToZonedDateTime("yyyy.MM.dd HH:mm:ss");

            // Assert
            Assert.Equal("2018-01-12T23:59:00.000Z", result.ToIso8601String());
        }

        [Fact]
        internal void Compare_WithVariousCombinations_ReturnsExpectedInt32()
        {
            // Arrange
            var time1 = StubZonedDateTime.UnixEpoch();
            var time2 = StubZonedDateTime.UnixEpoch() + Duration.FromMilliseconds(1);

            // Act
            // Assert
            Assert.True(time1.Compare(time1) >= 0);
            Assert.True(time1.Compare(time1) <= 0);
            Assert.True(time1.Compare(time2) <= 0);
            Assert.True(time2.Compare(time1) >= 0);
        }

        [Fact]
        internal void IsEqualTo_WithVariousCombinations_ReturnsExpectedInt32()
        {
            // Arrange
            var time1 = StubZonedDateTime.UnixEpoch();
            var time2 = StubZonedDateTime.UnixEpoch() + Duration.FromMilliseconds(1);

            // Act
            var result1 = time1.IsEqualTo(time1);
            var result2 = time1.IsEqualTo(time2);
            var result3 = time2.IsEqualTo(time1);

            // Assert
            Assert.True(result1);
            Assert.False(result2);
            Assert.False(result3);
        }

        [Fact]
        internal void IsGreaterThan_WithVariousCombinations_ReturnsExpectedInt32()
        {
            // Arrange
            var time1 = StubZonedDateTime.UnixEpoch();
            var time2 = StubZonedDateTime.UnixEpoch() + Duration.FromMilliseconds(1);

            // Act
            var result1 = time1.IsGreaterThan(time1);
            var result2 = time1.IsGreaterThan(time2);
            var result3 = time2.IsGreaterThan(time1);

            // Assert
            Assert.False(result1);
            Assert.False(result2);
            Assert.True(result3);
        }

        [Fact]
        internal void IsGreaterThanOrEqualTo_WithVariousCombinations_ReturnsExpectedInt32()
        {
            // Arrange
            var time1 = StubZonedDateTime.UnixEpoch();
            var time2 = StubZonedDateTime.UnixEpoch() + Duration.FromMilliseconds(1);

            // Act
            var result1 = time1.IsGreaterThanOrEqualTo(time1);
            var result2 = time1.IsGreaterThanOrEqualTo(time2);
            var result3 = time2.IsGreaterThanOrEqualTo(time1);

            // Assert
            Assert.True(result1);
            Assert.False(result2);
            Assert.True(result3);
        }

        [Fact]
        internal void IsLessThan_WithVariousCombinations_ReturnsExpectedInt32()
        {
            // Arrange
            var time1 = StubZonedDateTime.UnixEpoch();
            var time2 = StubZonedDateTime.UnixEpoch() + Duration.FromMilliseconds(1);

            // Act
            var result1 = time1.IsLessThan(time1);
            var result2 = time1.IsLessThan(time2);
            var result3 = time2.IsLessThan(time1);

            // Assert
            Assert.False(result1);
            Assert.True(result2);
            Assert.False(result3);
        }

        [Fact]
        internal void IsLessThanOrEqualTo_WithVariousCombinations_ReturnsExpectedInt32()
        {
            // Arrange
            var time1 = StubZonedDateTime.UnixEpoch();
            var time2 = StubZonedDateTime.UnixEpoch() + Duration.FromMilliseconds(1);

            // Act
            var result1 = time1.IsLessThanOrEqualTo(time1);
            var result2 = time1.IsLessThanOrEqualTo(time2);
            var result3 = time2.IsLessThanOrEqualTo(time1);

            // Assert
            Assert.True(result1);
            Assert.True(result2);
            Assert.False(result3);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(999, 999)]
        [InlineData(1000, 0)]
        [InlineData(1500, 500)]
        [InlineData(32010, 10)]
        internal void FloorOffset_SecondWithVariousDurations_ReturnsExpectedFlooredTime(
            int offset,
            int expectedFloor)
        {
            // Arrange
            var time = StubZonedDateTime.UnixEpoch() + Duration.FromMilliseconds(offset);
            var duration = Duration.FromSeconds(1);

            // Act
            var result = time.FloorOffsetMilliseconds(duration);

            // Assert
            Assert.Equal(expectedFloor, result);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(999, 1)]
        [InlineData(1000, 0)]
        [InlineData(1500, 500)]
        [InlineData(32010, 990)]
        internal void CeilingOffset_SecondWithVariousDurations_ReturnsExpectedFlooredTime(
            int offset,
            int expectedCeiling)
        {
            // Arrange
            var time = StubZonedDateTime.UnixEpoch() + Duration.FromMilliseconds(offset);
            var duration = Duration.FromSeconds(1);

            // Act
            var result = time.CeilingOffsetMilliseconds(duration);

            // Assert
            Assert.Equal(expectedCeiling, result);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(999, 0)]
        [InlineData(1000, 1000)]
        [InlineData(1500, 1000)]
        [InlineData(32010, 32000)]
        internal void Floor_SecondWithVariousDurations_ReturnsExpectedFlooredTime(
            int offset,
            int expectedFloor)
        {
            // Arrange
            var time = StubZonedDateTime.UnixEpoch() + Duration.FromMilliseconds(offset);
            var duration = Duration.FromSeconds(1);

            // Act
            var result = time.Floor(duration);

            // Assert
            Assert.Equal(StubZonedDateTime.UnixEpoch() + Duration.FromMilliseconds(expectedFloor), result);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(999, 1000)]
        [InlineData(1000, 1000)]
        [InlineData(1500, 2000)]
        [InlineData(32010, 33000)]
        internal void Ceiling_SecondWithVariousDurations_ReturnsExpectedFlooredTime(
            int offset,
            int expectedCeiling)
        {
            // Arrange
            var time = StubZonedDateTime.UnixEpoch() + Duration.FromMilliseconds(offset);
            var duration = Duration.FromSeconds(1);

            // Act
            var result = time.Ceiling(duration);

            // Assert
            Assert.Equal(StubZonedDateTime.UnixEpoch() + Duration.FromMilliseconds(expectedCeiling), result);
        }
    }
}
