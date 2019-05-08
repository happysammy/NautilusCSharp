//--------------------------------------------------------------------------------------------------
// <copyright file="PriceTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DomainModelTests.ValueObjectsTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.ValueObjects;
    using Xunit;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class PriceTests
    {
        [Theory]
        [InlineData(0.1, 1, 0.1)]
        [InlineData(0.001, 3, 0.001)]
        [InlineData(1, 0, 1)]
        [InlineData(100, 0, 100)]
        [InlineData(500.5, 1, 500.5)]
        [InlineData(424242, 0, 424242)]
        internal void Create_VariousValues_ReturnsExpectedValue(
            decimal value,
            int decimalPrecision,
            decimal expected)
        {
            // Arrange
            // Act
            var result = Price.Create(value);

            // Assert
            Assert.Equal(expected, result.Value);
            Assert.Equal(decimalPrecision, result.DecimalPrecision);
        }

        [Theory]
        [InlineData(0.1, 1, 0.1)]
        [InlineData(0.001, 3, 0.001)]
        [InlineData(1, 0, 1)]
        [InlineData(100, 0, 100)]
        [InlineData(500.5, 1, 500.5)]
        [InlineData(424242, 0, 424242)]
        internal void Create_VariousValuesWithDecimalPrecision_ReturnsExpectedValue(
            decimal value,
            int decimals,
            decimal expected)
        {
            // Arrange
            // Act
            var result = Price.Create(value, decimals);

            // Assert
            Assert.Equal(expected, result.Value);
            Assert.Equal(decimals, result.DecimalPrecision);
        }

        [Theory]
        [InlineData(1, 1, 2)]
        [InlineData(2.2, 2.2, 4.4)]
        [InlineData(100.50, 100, 200.50)]
        [InlineData(25, 15, 40)]
        [InlineData(1, 0.00001, 1.00001)]
        internal void Add_VariousPrices_ReturnsExpectedPriceValues(decimal value1, decimal value2, decimal expected)
        {
            // Arrange
            var price1 = Price.Create(value1, 5);
            var price2 = Price.Create(value2, 5);

            // Act
            var result1 = price1.Add(price2);
            var result2 = price1 + price2;

            // Assert
            Assert.Equal(expected, result1.Value);
            Assert.Equal(expected, result2);
        }

        [Theory]
        [InlineData(2.2, 2, 0.2)]
        [InlineData(100.50, 0.50, 100)]
        [InlineData(25, 15, 10)]
        [InlineData(1, 0.00001, 0.99999)]
        internal void Subtract_VariousPrices_ReturnsExpectedPriceValues(decimal value1, decimal value2, decimal expected)
        {
            // Arrange
            var price1 = Price.Create(value1, 5);
            var price2 = Price.Create(value2, 5);

            // Act
            var result1 = price1.Subtract(price2);
            var result2 = price1 - price2;

            // Assert
            Assert.Equal(expected, result1.Value);
            Assert.Equal(expected, result2);
        }

        [Theory]
        [InlineData(1, 1, 0)]
        [InlineData(2, 1, 1)]
        [InlineData(1, 2, -1)]
        internal void CompareTo_VariousPrices_ReturnsExpectedResult(int value1, int value2, int expected)
        {
            // Arrange
            var price1 = Price.Create(value1, 5);
            var price2 = Price.Create(value2, 5);

            // Act
            var result = price1.CompareTo(price2);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(1, 0, "1")]
        [InlineData(0.1, 1, "0.1")]
        [InlineData(0.01, 2, "0.01")]
        [InlineData(0.01, 3, "0.010")]
        [InlineData(0.1, 4, "0.1000")]
        [InlineData(0.0020, 5, "0.00200")]
        [InlineData(10000, 2, "10000.00")]
        [InlineData(5000, 1, "5000.0")]
        [InlineData(100000, 0, "100000")]
        internal void ToString_VariousValues_ReturnsExpectedString(
            decimal value,
            int decimals,
            string expected)
        {
            // Arrange
            // Act
            var price = Price.Create(value, decimals);

            // Assert
            Assert.Equal(expected, price.ToString());
        }

        [Theory]
        [InlineData(0.1, 0.1, true)]
        [InlineData(0.00001, 0.00001, true)]
        [InlineData(1, 1, true)]
        [InlineData(3.142, 3.142, true)]
        [InlineData(2.20462, 2.20462, true)]
        [InlineData(1, 2, false)]
        [InlineData(0.11, 0.1, false)]
        [InlineData(0.0001, 0.00001, false)]
        [InlineData(10, 1, false)]
        [InlineData(3, 3.142, false)]
        [InlineData(2.20461, 2.20462, false)]
        internal void Equals_VariousValues_ReturnsExpectedResult(decimal value1, decimal value2, bool expected)
        {
            // Arrange
            // Act
            var price1 = Price.Create(value1, 5);
            var price2 = Price.Create(value2, 5);

            var result1 = price1.Equals(price2);
            var result2 = price1 == price2;

            // Assert
            Assert.Equal(expected, result1);
            Assert.Equal(expected, result2);
        }

        [Fact]
        internal void GetHashCode_ReturnsExpectedResult()
        {
            // Arrange
            // Act
            var price = Price.Create(1, 5);

            var result = price.GetHashCode();

            // Assert
            Assert.Equal(1072693741, result);
        }
    }
}
