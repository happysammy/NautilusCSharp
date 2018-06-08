//--------------------------------------------------------------------------------------------------
// <copyright file="TickTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DomainModelTests.ValueObjectsTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NodaTime;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class TickTests
    {
        private readonly Symbol symbol;

        public TickTests()
        {
            // Fixture Setup
            this.symbol = new Symbol("AUDUSD", Exchange.FXCM);
        }
        [Fact]
        internal void Zero_ReturnsPriceWithAValueOfZero()
        {
            // Arrange
            // Act
            var tick = new Tick(symbol, 1.00000m, 1.00000m, StubZonedDateTime.UnixEpoch());

            // Assert
            Assert.Equal("AUDUSD", tick.Symbol.Code);
            Assert.Equal(decimal.One, tick.Bid.Value);
            Assert.Equal(decimal.One, tick.Ask.Value);
            Assert.Equal(1970, tick.Timestamp.Year);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, -1)]
        [InlineData(-1, 1)]
        internal void ComapreTo_VariousPrices_ReturnsExpectedResult(int millisecondsOffset, int expected)
        {
            // Arrange
            var tick1 = new Tick(symbol, 1.00000m, 1.00000m, StubZonedDateTime.UnixEpoch());
            var tick2 = new Tick(symbol, 1.00000m, 1.00000m, StubZonedDateTime.UnixEpoch() + Duration.FromMilliseconds(millisecondsOffset));

            // Act
            var result = tick1.CompareTo(tick2);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        internal void ToString_ReturnsExpectedString()
        {
            // Arrange
            // Act
            var tick = new Tick(symbol, 1.00000m, 1.00000m, StubZonedDateTime.UnixEpoch());

            // Assert
            Assert.Equal("AUDUSD.FXCM,1.00000,1.00000,1970-01-01T00:00:00.000Z", tick.ToString());
        }

        [Theory]
        [InlineData(1.00000, 1.00000, 0, true)]
        [InlineData(1.00001, 1.00000, 0, false)]
        [InlineData(1.00000, 1.00000, 1, false)]
        internal void Equals_VariousValues_ReturnsExpectedResult(decimal price1, decimal price2, int millisecondsOffset, bool expected)
        {
            // Arrange
            var tick1 = new Tick(symbol, price1, price1, StubZonedDateTime.UnixEpoch());
            var tick2 = new Tick(symbol, price2, price1, StubZonedDateTime.UnixEpoch() + Duration.FromMilliseconds(millisecondsOffset));

            // Act
            var result1 = tick1.Equals(tick2);
            var result2 = tick1 == tick2;

            // Assert
            Assert.Equal(expected, result1);
            Assert.Equal(expected, result2);
        }

        [Fact]
        internal void ToUtf8Bytes_WithValidBar_ReturnsExpectedBar()
        {
            // Arrange
            var tick = new Tick(symbol, 1.00000m, 1.00000m, StubZonedDateTime.UnixEpoch());

            // Act
            var result = tick.ToUtf8Bytes();

            // Assert
            Assert.Equal(typeof(byte[]), result.GetType());
            Assert.Equal(tick, Tick.GetFromBytes(result));
        }

        [Fact]
        internal void GetFromString_WithValidString_ReturnsExpectedBar()
        {
            // Arrange
            var tick = new Tick(symbol, 1.00000m, 1.00000m, StubZonedDateTime.UnixEpoch());

            // Act
            var tickString = tick.ToString();
            var result = Tick.GetFromString(tickString);

            // Assert
            Assert.Equal(tick, result);
        }

        [Fact]
        internal void GetFromBytes_WithValidBytesArray_ReturnsExpectedBar()
        {
            // Arrange
            var tick = new Tick(symbol, 1.00000m, 1.00000m, StubZonedDateTime.UnixEpoch());

            // Act
            var tickBytes = tick.ToUtf8Bytes();
            var result = Tick.GetFromBytes(tickBytes);

            // Assert
            Assert.Equal(tick, result);
        }
    }
}
