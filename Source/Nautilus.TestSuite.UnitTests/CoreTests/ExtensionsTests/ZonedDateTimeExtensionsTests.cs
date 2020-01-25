//--------------------------------------------------------------------------------------------------
// <copyright file="ZonedDateTimeExtensionsTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.CoreTests.ExtensionsTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core.Extensions;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NodaTime;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class ZonedDateTimeExtensionsTests
    {
        [Fact]
        internal void ToIsoString_WithValidZonedDateTime_ReturnsExpectedString()
        {
            // Arrange
            var zonedDateTime = StubZonedDateTime.UnixEpoch();

            // Act
            var result = zonedDateTime.ToIsoString();

            // Assert
            Assert.Equal("1970-01-01T00:00:00.000Z", result);
        }

        [Fact]
        internal void ToStringWithParsePattern_WithValidZonedDateTimeAndParsePattern_ReturnsExpectedString()
        {
            // Arrange
            var zonedDateTime = StubZonedDateTime.UnixEpoch();

            // Act
            var result = zonedDateTime.ToStringWithParsePattern("yyyy.MM.dd HH:mm:ss");

            // Assert
            Assert.Equal("1970.01.01 00:00:00Z", result);
        }

        [Fact]
        internal void ToZonedDateTimeFromIso_WithValidString_ReturnsExpectedTime()
        {
            // Arrange
            var zonedDateTimeString = StubZonedDateTime.UnixEpoch().ToIsoString();

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
            Assert.Equal("2018-01-12T23:59:00.000Z", result.ToIsoString());
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
