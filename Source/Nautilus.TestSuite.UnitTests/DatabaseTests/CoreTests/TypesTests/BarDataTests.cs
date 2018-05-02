// -------------------------------------------------------------------------------------------------
// <copyright file="BarDataTests.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DatabaseTests.CoreTests.TypesTests
{
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using NodaTime;
    using Xunit;
    using Xunit.Abstractions;
    using Nautilus.Core.Extensions;
    using Nautilus.Database.Core.Extensions;
    using Nautilus.Database.Core.Types;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.TestDoubles;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class BarDataTests
    {
        private readonly ITestOutputHelper output;

        public BarDataTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;
        }

        [Fact]
        internal void Equals_WithNullObject_ReturnsFalse()
        {
            // Arrange
            var bar = new BarData(
                0.80000M,
                0.80010M,
                0.79990M,
                0.80001M,
                1000000,
                StubZonedDateTime.UnixEpoch());

            // Act
            var result1 = bar.Equals(null);
            var result2 = bar == null;

            // Assert
            Assert.False(result1);
            Assert.False(result2);
        }

        [Fact]
        internal void Equals_WithEqualObject_ReturnsTrue()
        {
            // Arrange
            var bar1 = new BarData(
                0.80000M,
                0.80010M,
                0.79990M,
                0.80001M,
                1000000,
                StubZonedDateTime.UnixEpoch());

            var bar2 = new BarData(
                0.80000M,
                0.80010M,
                0.79990M,
                0.80001M,
                1000000,
                StubZonedDateTime.UnixEpoch());

            // Act
            var result1 = bar1.Equals(bar2);
            var result2 = bar1 == bar2;

            // Assert
            Assert.True(result1);
            Assert.True(result2);
        }

        [Fact]
        internal void Equals_WithUnequalObject_ReturnsFalse()
        {
            // Arrange
            var bar1 = new BarData(
                0.80000M,
                0.80010M,
                0.79990M,
                0.80001M,
                1000000,
                StubZonedDateTime.UnixEpoch());

            var bar2 = new BarData(
                0.80000M,
                0.80010M,
                0.79990M,
                0.80001M,
                1000000,
                StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(1));

            // Act
            var result1 = bar1.Equals(bar2);
            var result2 = bar1 == bar2;

            // Assert
            Assert.False(result1);
            Assert.False(result2);
        }

        [Fact]
        internal void GetHashCode_ReturnsExpectedInt32()
        {
            // Arrange
            var bar = new BarData(
                0.80000M,
                0.80010M,
                0.79990M,
                0.80001M,
                1000000,
                StubZonedDateTime.UnixEpoch());

            // Act
            var result = bar.GetHashCode();

            // Assert
            Assert.NotEqual(0, result);
        }

        [Fact]
        internal void ToString_ReturnsExpectedString()
        {
            // Arrange
            var bar = new BarData(
                0.80000M,
                0.80010M,
                0.79990M,
                0.80001M,
                1000000,
                StubZonedDateTime.UnixEpoch());

            // Act
            var result = bar.ToString();

            // Assert
            Assert.Equal("1970-01-01T00:00:00.000,0.80000,0.80010,0.79990,0.80001,1000000", result);
        }

        [Fact]
        internal void ConvertToBytes_WithValidBar_ReturnsExpectedBar()
        {
            // Arrange
            var bar = new BarData(
                0.80000M,
                0.80010M,
                0.79990M,
                0.80001M,
                1000000,
                StubZonedDateTime.UnixEpoch());

            // Act
            var result = bar.ToUtf8Bytes();

            // Assert
            Assert.Equal(typeof(byte[]), result.GetType());
            Assert.Equal(bar, result.ToBarData());
        }

        [Fact]
        internal void ConvertValuesToString_WithValidBar_ReturnsExpectedBar()
        {
            // Arrange
            var bar = new BarData(
                0.80000M,
                0.80010M,
                0.79990M,
                0.80001M,
                1000000,
                StubZonedDateTime.UnixEpoch());

            // Act
            var result = bar.ValuesToString();

            // Assert
            Assert.Equal("0.80000,0.80010,0.79990,0.80005,500000", result);
        }

        [Fact]
        internal void ConvertValuesToBytes_WithValidBar_ReturnsExpectedBar()
        {
            // Arrange
            var bar = new BarData(
                0.80000M,
                0.80010M,
                0.79990M,
                0.80005M,
                500000,
                StubZonedDateTime.UnixEpoch());

            // Act
            var result = bar.ValuesToUtf8Bytes();

            // Assert
            Assert.Equal(typeof(byte[]), result.GetType());
            Assert.Equal("0.80000,0.80010,0.79990,0.80005,500000", Encoding.Default.GetString(result));
        }
    }
}
