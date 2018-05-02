// -------------------------------------------------------------------------------------------------
// <copyright file="ZonedDateTimeExtensionsTests.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.CoreTests.ExtensionsTests
{
    using Nautilus.Core.Extensions;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using NodaTime;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
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
            Assert.Equal("1970-01-01T00:00:00.000", result);
        }

        [Fact]
        internal void ToIsoString_WithValidNullableZonedDateTime_ReturnsExpectedString()
        {
            // Arrange
            var nullableZonedDateTime = (ZonedDateTime?)StubZonedDateTime.UnixEpoch();

            // Act
            var result = nullableZonedDateTime.ToIsoString();

            // Assert
            Assert.Equal("1970-01-01T00:00:00.000", result);
        }

        [Fact]
        internal void ToIsoString_WhenInputValueNull_ReturnsEmptyString()
        {
            // Arrange
            var nullableZonedDateTime = (ZonedDateTime?)null;

            // Act - Ignore warning as this is the point of the test.
            var result = nullableZonedDateTime.ToIsoString();

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        internal void ToStringWithParsePattern_WithValidZonedDateTimeAndParsePattern_ReturnsExpectedString()
        {
            // Arrange
            var zonedDateTime = StubZonedDateTime.UnixEpoch();

            // Act
            var result = zonedDateTime.ToStringWithParsePattern("yyyy.MM.dd HH:mm:ss");

            // Assert
            Assert.Equal("1970.01.01 00:00:00", result);
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
            Assert.Equal("2018-01-12T23:59:00.000", result.ToIsoString());
        }

        [Fact]
        internal void ToBytes_WithValidZonedDateTime_ReturnsExpectedResults()
        {
            // Arrange
            var time = StubZonedDateTime.UnixEpoch();

            // Act
            var result = time.ToBytes();

            // Assert
            Assert.Equal(typeof(byte[]), result.GetType());
            Assert.Equal("1970-01-01T00:00:00.000", Encoding.Default.GetString(result));
            Assert.Equal(time, Encoding.Default.GetString(result).ToZonedDateTimeFromIso());
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
    }
}
