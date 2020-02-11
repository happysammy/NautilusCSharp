//--------------------------------------------------------------------------------------------------
// <copyright file="FloatingPointNumberTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.CoreTests.PrimitivesTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core.Primitives;
    using Xunit;

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

        private sealed class TestFloat : FloatingPointNumber
        {
            public TestFloat(double value)
                : base(value)
            {
            }
        }
    }
}
