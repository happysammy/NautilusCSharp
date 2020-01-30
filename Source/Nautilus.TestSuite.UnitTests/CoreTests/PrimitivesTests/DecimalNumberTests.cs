//--------------------------------------------------------------------------------------------------
// <copyright file="DecimalNumberTests.cs" company="Nautech Systems Pty Ltd">
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

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("ReSharper", "SA1131", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class DecimalNumberTests
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

        private class TestDecimal : DecimalNumber
        {
            public TestDecimal(decimal value)
                : base(value, 1)
            {
            }
        }
    }
}
