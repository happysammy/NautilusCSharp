//--------------------------------------------------------------------------------------------------
// <copyright file="TickTests.cs" company="Nautech Systems Pty Ltd">
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
using Nautilus.DomainModel.Identifiers;
using Nautilus.DomainModel.ValueObjects;
using Nautilus.TestSuite.TestKit.Stubs;
using NodaTime;
using Xunit;

namespace Nautilus.TestSuite.UnitTests.DomainModelTests.ValueObjectsTests
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class TickTests
    {
        private readonly Symbol symbol;

        public TickTests()
        {
            // Fixture Setup
            this.symbol = new Symbol("AUD/USD", new Venue("FXCM"));
        }

        [Fact]
        internal void InitializedTick_HasExpectedProperties()
        {
            // Arrange
            // Act
            var tick = new Tick(
                this.symbol,
                Price.Create(1.00000m),
                Price.Create(1.00000m),
                Quantity.One(),
                Quantity.One(),
                StubZonedDateTime.UnixEpoch());

            // Assert
            Assert.Equal("AUD/USD", tick.Symbol.Code);
            Assert.Equal(decimal.One, tick.Bid.Value);
            Assert.Equal(decimal.One, tick.Ask.Value);
            Assert.Equal(1970, tick.Timestamp.Year);
        }

        [Fact]
        internal void FromString_WithValidString_ReturnsExpectedTick()
        {
            // Arrange
            var tick = new Tick(
                this.symbol,
                Price.Create(1.00000m),
                Price.Create(1.00000m),
                Quantity.One(),
                Quantity.One(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var tickString = tick.ToSerializableString();
            var result = Tick.FromSerializableString(this.symbol, tickString);

            // Assert
            Assert.Equal(tick, result);
        }

        [Fact]
        internal void ToString_ReturnsExpectedString()
        {
            // Arrange
            // Act
            var tick = new Tick(
                this.symbol,
                Price.Create(1.00000m),
                Price.Create(1.00010m),
                Quantity.One(),
                Quantity.One(),
                StubZonedDateTime.UnixEpoch());

            // Assert
            Assert.Equal("AUD/USD.FXCM,1.00000,1.00010,1,1,1970-01-01T00:00:00.000Z", tick.ToString());
        }

        [Theory]
        [InlineData(1.00000, 1.00000, 0, true)]
        [InlineData(1.00001, 1.00000, 0, false)]
        [InlineData(1.00000, 1.00000, 1, false)]
        internal void Equals_VariousValues_ReturnsExpectedResult(decimal price1, decimal price2, int millisecondsOffset, bool expected)
        {
            // Arrange
            var tick1 = new Tick(
                this.symbol,
                Price.Create(price1),
                Price.Create(5),
                Quantity.One(),
                Quantity.One(),
                StubZonedDateTime.UnixEpoch());

            var tick2 = new Tick(
                this.symbol,
                Price.Create(price2),
                Price.Create(5),
                Quantity.One(),
                Quantity.One(),
                StubZonedDateTime.UnixEpoch() + Duration.FromMilliseconds(millisecondsOffset));

            // Act
            var result1 = tick1.Equals(tick2);
            var result2 = tick1 == tick2;

            // Assert
            Assert.Equal(expected, result1);
            Assert.Equal(expected, result2);
        }
    }
}
