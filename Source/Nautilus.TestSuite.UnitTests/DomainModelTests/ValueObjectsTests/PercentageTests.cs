//--------------------------------------------------------------------------------------------------
// <copyright file="PercentageTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
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
    public class PercentageTests
    {
        [Fact]
        internal void Zero_ReturnsExpectedPercentage()
        {
            // Arrange
            // Act
            var result = Percentage.Zero();

            // Assert
            Assert.Equal(decimal.Zero, result.Value);
        }

        [Theory]
        [InlineData(0.1)]
        [InlineData(1)]
        [InlineData(100)]
        internal void Create_VariousValues_ReturnsExpectedResult(decimal value)
        {
            // Arrange
            // Act
            var result = Percentage.Create(value);

            // Assert
            Assert.Equal(value, result.Value);
        }

        [Theory]
        [InlineData(1, 1, 2)]
        [InlineData(2.2, 2.2, 4.4)]
        [InlineData(100.50, 100, 200.50)]
        [InlineData(25, 15, 40)]
        [InlineData(1, 0.00001, 1.00001)]
        internal void Add_VariousPrices_ReturnsExpectedResults(decimal value1, decimal value2, decimal expected)
        {
            // Arrange
            var price1 = Percentage.Create(value1);
            var price2 = Percentage.Create(value2);

            // Act
            var result = price1.Add(price2);

            // Assert
            Assert.Equal(expected, result.Value);
        }

        [Theory]
        [InlineData(1, 1, 0)]
        [InlineData(2.2, 2, 0.2)]
        [InlineData(100.50, 0.50, 100)]
        [InlineData(25, 15, 10)]
        [InlineData(1, 0.00001, 0.99999)]
        internal void Subtract_VariousValues_ReturnsExpectedValues(decimal value1, decimal value2, decimal expected)
        {
            // Arrange
            var percentage1 = Percentage.Create(value1);
            var percentage2 = Percentage.Create(value2);

            // Act
            var result = percentage1.Subtract(percentage2);

            // Assert
            Assert.Equal(expected, result.Value);
        }

        [Theory]
        [InlineData(1, 1, 1)]
        [InlineData(2, 2, 4)]
        [InlineData(0.1, 100, 10)]
        internal void Multiply_VariousValues_ReturnsExpectedValues(decimal value1, decimal value2, decimal expected)
        {
            // Arrange
            var percentage1 = Percentage.Create(value1);

            // Act
            var result = percentage1.MultiplyBy(value2);

            // Assert
            Assert.Equal(expected, result.Value);
        }

        [Theory]
        [InlineData(1, 1, 1)]
        [InlineData(20, 2, 10)]
        [InlineData(100, 0.1, 1000)]
        internal void DivideBy_VariousValues_ReturnsExpectedValues(decimal value1, decimal value2, decimal expected)
        {
            // Arrange
            var percentage1 = Percentage.Create(value1);

            // Act
            var result1 = percentage1.DivideBy(value2);

            // Assert
            Assert.Equal(expected, result1.Value);
        }

        [Theory]
        [InlineData(100, 1, 1)]
        [InlineData(50, 2, 1)]
        [InlineData(1, 1000, 10)]
        [InlineData(2.5, 1000, 25)]
        internal void PercentOf_VariousValues_ReturnsExpectedValues(decimal value1, decimal value2, decimal expected)
        {
            // Arrange
            var percentage1 = Percentage.Create(value1);

            // Act
            var result = percentage1.PercentOf(value2);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(1, 1, 0)]
        [InlineData(2, 1, 1)]
        [InlineData(1, 2, -1)]
        internal void CompareTo_VariousValues_ReturnsExpectedResult(int value1, int value2, int expected)
        {
            // Arrange
            var percentage1 = Percentage.Create(value1);
            var percentage2 = Percentage.Create(value2);

            // Act
            var result = percentage1.CompareTo(percentage2);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(1, "1%")]
        [InlineData(0.1, "0.1%")]
        [InlineData(0.01, "0.01%")]
        [InlineData(10, "10%")]
        [InlineData(100.000, "100%")]
        internal void ToString_VariousValues_ReturnsExpectedString(decimal value, string expected)
        {
            // Arrange
            // Act
            var result = Percentage.Create(value);

            // Assert
            Assert.Equal(expected, result.ToString());
        }

        [Fact]
        internal void Equals_PriceZeros_ReturnsTrue()
        {
            // Arrange
            // Act
            var percentage1 = Percentage.Zero();
            var percentage2 = Percentage.Zero();

            var result1 = percentage1.Equals(percentage2);
            var result2 = percentage1 == percentage2;

            // Assert
            Assert.True(result1);
            Assert.True(result2);
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
            var percentage1 = Percentage.Create(value1);
            var percentage2 = Percentage.Create(value2);

            var result1 = percentage1.Equals(percentage2);
            var result2 = percentage1 == percentage2;

            // Assert
            Assert.Equal(expected, result1);
            Assert.Equal(expected, result2);
        }

        [Fact]
        internal void GetHashCode_ReturnsExpectedResult()
        {
            // Arrange
            // Act
            var result = Percentage.Create(1);

            // Assert
            Assert.Equal(1072693741, result.GetHashCode());
        }
    }
}
