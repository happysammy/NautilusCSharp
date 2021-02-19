//--------------------------------------------------------------------------------------------------
// <copyright file="DecimalNumberTests.cs" company="Nautech Systems Pty Ltd">
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
using Nautilus.Core.Primitives;
using Xunit;

namespace Nautilus.TestSuite.UnitTests.CoreTests.PrimitivesTests
{
    // Required warning suppression for tests
    // (do not remove even if compiler doesn't initially complain).
#pragma warning disable 8602
#pragma warning disable 8604
#pragma warning disable 8625
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    [SuppressMessage("ReSharper", "SA1131", Justification = "Test Suite")]
    public sealed class Decimal64Tests
    {
        [Fact]
        internal void Value_WithValidValueGiven_ReturnsExpectedValue()
        {
            // Arrange
            var number = new TestDecimal(decimal.One);

            // Act
            var result = number.Value;

            // Assert
            Assert.Equal(decimal.One, result);
        }

        [Fact]
        internal void ArithmeticOperators_ReturnExpectedValues()
        {
            // Arrange
            var number0 = new TestDecimal(decimal.Zero);
            var number1 = new TestDecimal(decimal.One);

            // Act
            // Assert
            Assert.Equal(decimal.One, number0 + number1);
            Assert.Equal(decimal.One, number0 + decimal.One);
            Assert.Equal(decimal.One, decimal.One + number0);
            Assert.Equal(decimal.Zero, number1 - number1);
            Assert.Equal(-1.0m, number0 - decimal.One);
            Assert.Equal(decimal.Zero, decimal.One - number1);
            Assert.Equal(decimal.Zero, number0 * number1);
            Assert.Equal(decimal.Zero, number0 * decimal.One);
            Assert.Equal(decimal.Zero, decimal.One * number0);
            Assert.Equal(decimal.One, number1 * number1);
            Assert.Equal(2.0m, number1 * 2.0m);
            Assert.Equal(decimal.One, decimal.One / number1);
            Assert.Equal(decimal.Zero, number0 / number1);
            Assert.Equal(decimal.Zero, number0 / decimal.One);
            Assert.Equal(0.5m, decimal.One / 2.0m);
        }

        [Fact]
        internal void EqualityOperators_ReturnExpectedValues()
        {
            // Arrange
            var number0 = new TestDecimal(decimal.Zero);
            var number1 = new TestDecimal(decimal.One);
            var number2 = new TestDecimal(decimal.One);

            // Act
            // Assert
            // ReSharper disable once SuspiciousTypeConversion.Global
            Assert.False(number0.Equals("string"));
            Assert.True(number1 == number2);
            Assert.True(number1 == decimal.One);
            Assert.True(decimal.One == number1);

            Assert.True(number1.Equals(number1));
            Assert.False(number1.Equals(number0));
            Assert.False(number1.Equals(decimal.Zero));

            Assert.False(number1 != number2);
            Assert.False(number1 != decimal.One);
            Assert.False(decimal.One != number1);

            Assert.True(number1 > number0);
            Assert.False(number1 > decimal.One);
            Assert.False(decimal.One > number1);

            Assert.True(number1 >= number0);
            Assert.True(number1 >= decimal.One);
            Assert.True(decimal.One >= number1);

            Assert.True(number0 < number1);
            Assert.False(number1 < decimal.One);
            Assert.False(decimal.One < number1);

            Assert.True(number0 <= number1);
            Assert.True(number1 <= decimal.One);
            Assert.True(decimal.One <= number1);
        }

        [Fact]
        internal void CompareTo_WithVariousValues_ReturnExpectedValues()
        {
            // Arrange
            var number0 = new TestDecimal(decimal.Zero);
            var number1 = new TestDecimal(decimal.One);

            // Act
            // Assert
            Assert.Equal(-1, number0.CompareTo(number1));
            Assert.Equal(0, number1.CompareTo(number1));
            Assert.Equal(1, number1.CompareTo(number0));
            Assert.Equal(-1, number0.CompareTo(decimal.One));
            Assert.Equal(0, number1.CompareTo(decimal.One));
            Assert.Equal(1, number1.CompareTo(decimal.Zero));
        }

        [Fact]
        internal void GetHashCode_WithVariousValues_ReturnExpectedValues()
        {
            // Arrange
            var number0 = new TestDecimal(decimal.Zero);
            var number1 = new TestDecimal(decimal.One);

            // Act
            // Assert
            Assert.Equal(0, number0.GetHashCode());
            Assert.Equal(decimal.Zero.GetHashCode(), number0.GetHashCode());
            Assert.Equal(decimal.One.GetHashCode(), number1.GetHashCode());
        }

        [Fact]
        internal void ToString_ReturnsExpectedString()
        {
            // Arrange
            var number0 = new TestDecimal(decimal.Zero, 0);
            var number1 = new TestDecimal(decimal.Zero);

            // Act
            // Assert
            Assert.Equal("0", number0.ToString());
            Assert.Equal("0.0", number1.ToString());
        }

        private sealed class TestDecimal : Decimal64
        {
            public TestDecimal(decimal value, byte precision = 1)
                : base(value, precision)
            {
            }
        }
    }
}
