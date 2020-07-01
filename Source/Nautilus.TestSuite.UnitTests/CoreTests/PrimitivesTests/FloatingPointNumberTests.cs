//--------------------------------------------------------------------------------------------------
// <copyright file="FloatingPointNumberTests.cs" company="Nautech Systems Pty Ltd">
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
    public sealed class FloatingPointNumberTests
    {
        [Fact]
        internal void Value_WithValidValueGiven_ReturnsExpectedValue()
        {
            // Arrange
            var number = new TestFloat(1);

            // Act
            var result = number.Value;

            // Assert
            Assert.Equal(1.0, result);
        }

        [Fact]
        internal void ArithmeticOperators_ReturnExpectedValues()
        {
            // Arrange
            var number0 = new TestFloat(0);
            var number1 = new TestFloat(1);

            // Act
            // Assert
            Assert.Equal(1, number0 + number1);
            Assert.Equal(1, number0 + 1);
            Assert.Equal(0, 0 + number0);
            Assert.Equal(0, number1 - number1);
            Assert.Equal(-1, number0 - 1);
            Assert.Equal(0, 1 - number1);
            Assert.Equal(0, number0 * number1);
            Assert.Equal(0, number0 * 1);
            Assert.Equal(0, 1 * number0);
            Assert.Equal(1, number1 * number1);
            Assert.Equal(2, number1 * 2);
            Assert.Equal(1, 1 / number1);
            Assert.Equal(0, number0 / number1);
            Assert.Equal(0, number0 / 1);
            Assert.Equal(2, 4 / 2);
        }

        [Fact]
        internal void EqualityOperators_ReturnExpectedValues()
        {
            // Arrange
            var number0 = new TestFloat(0);
            var number1 = new TestFloat(1);
            var number2 = new TestFloat(1);

            // Act
            // Assert
            // ReSharper disable once SuspiciousTypeConversion.Global
            Assert.False(number0.Equals("string"));
            Assert.False(number0 == null);
            Assert.True(number1 == number2);
            Assert.True(number1 == 1);
            Assert.True(1 == number1);

            Assert.True(number1.Equals(number1));
            Assert.False(number1.Equals(number0));
            Assert.False(number1.Equals(0.0));

            Assert.False(number1 != number2);
            Assert.False(number1 != 1);
            Assert.False(1 != number1);

            Assert.True(number1 > number0);
            Assert.False(number1 > 1);
            Assert.False(1 > number1);

            Assert.True(number1 >= number0);
            Assert.True(number1 >= 1);
            Assert.True(1 >= number1);

            Assert.True(number0 < number1);
            Assert.False(number1 < 1);
            Assert.False(1 < number1);

            Assert.True(number0 <= number1);
            Assert.True(number1 <= 1);
            Assert.True(1 <= number1);
        }

        [Fact]
        internal void CompareTo_WithVariousValues_ReturnExpectedValues()
        {
            // Arrange
            var number0 = new TestFloat(0);
            var number1 = new TestFloat(1);

            // Act
            // Assert
            Assert.Equal(-1, number0.CompareTo(number1));
            Assert.Equal(0, number1.CompareTo(number1));
            Assert.Equal(1, number1.CompareTo(number0));
            Assert.Equal(-1, number0.CompareTo(1));
            Assert.Equal(0, number1.CompareTo(1));
            Assert.Equal(1, number1.CompareTo(0));
        }

        [Fact]
        internal void GetHashCode_WithVariousValues_ReturnExpectedValues()
        {
            // Arrange
            var number0 = new TestFloat(0);
            var number1 = new TestFloat(1);

            // Act
            // Assert
            Assert.Equal(0, number0.GetHashCode());
            Assert.Equal(0.GetHashCode(), number0.GetHashCode());
            Assert.Equal(1.0.GetHashCode(), number1.GetHashCode());
        }

        [Fact]
        internal void ToString_ReturnsExpectedString()
        {
            // Arrange
            var number0 = new TestFloat(0);
            var number1 = new TestFloat(1000);

            // Act
            var result0 = number0.ToString();
            var result1 = number1.ToString();

            // Assert
            Assert.Equal("0", result0);
            Assert.Equal("1000", result1);
        }

        private sealed class TestFloat : FloatingPointNumber
        {
            public TestFloat(double value)
                : base(value)
            {
            }
        }
    }
}
