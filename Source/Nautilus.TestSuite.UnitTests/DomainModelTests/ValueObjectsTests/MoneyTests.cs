//--------------------------------------------------------------------------------------------------
// <copyright file="MoneyTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DomainModelTests.ValueObjectsTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class MoneyTests
    {
        [Fact]
        internal void Zero_ReturnsMoneyWithAValueOfZero()
        {
            // Arrange
            // Act
            var result = Money.Zero(CurrencyCode.AUD);

            // Assert
            Assert.Equal(0, result.Value);
            Assert.Equal(CurrencyCode.AUD, result.Currency);
        }

        [Theory]
        [InlineData(0.1)]
        [InlineData(1)]
        [InlineData(100)]
        internal void Create_VariousValidValues_ReturnsExpectedValue(decimal value)
        {
            // Arrange
            // Act
            var result = Money.Create(value, CurrencyCode.AUD);

            // Assert
            Assert.Equal(value, result.Value);
        }

        [Theory]
        [InlineData(1, 1, 2)]
        [InlineData(2.2, 2.2, 4.4)]
        [InlineData(100.50, 100, 200.50)]
        [InlineData(25, 15, 40)]
        internal void Add_VariousPrices_ReturnsExpectedResults(decimal value1, decimal value2, decimal expected)
        {
            // Arrange
            var money1 = Money.Create(value1, CurrencyCode.AUD);
            var money2 = Money.Create(value2, CurrencyCode.AUD);

            // Act
            var result = money1.Add(money2);

            // Assert
            Assert.Equal(expected, result.Value);
        }

        [Theory]
        [InlineData(2.2, 2, 0.2)]
        [InlineData(100.50, 0.50, 100)]
        [InlineData(25, 15, 10)]
        [InlineData(1, 0.01, 0.99)]
        internal void Subtract_VariousValues_ReturnsExpectedAmounts(decimal value1, decimal value2, decimal expected)
        {
            // Arrange
            var money1 = Money.Create(value1, CurrencyCode.AUD);
            var money2 = Money.Create(value2, CurrencyCode.AUD);

            // Act
            var result = money1.Subtract(money2);

            // Assert
            Assert.Equal(expected, result.Value);
        }

        [Theory]
        [InlineData(1, 1000)]
        [InlineData(200, 200000)]
        internal void MultiplyBy_VariousAmounts_ReturnsExpectedResult(int multiple, int expected)
        {
            // Arrange
            var money = Money.Create(1000, CurrencyCode.AUD);

            // Act
            var result = money.MultiplyBy(multiple);

            // Assert
            Assert.Equal(expected, result.Value);
        }

        [Fact]
        internal void MultiplyBy_Zero_Throws()
        {
            // Arrange
            var money = Money.Create(1000, CurrencyCode.AUD);

            // Act
            // Assert
            Assert.Throws<ValidationException>(() => money.DivideBy(0));
        }

        [Theory]
        [InlineData(1, 1000)]
        [InlineData(200, 5)]
        internal void DivideBy_VariousAmounts_ReturnsExpectedResult(int divisor, int expected)
        {
            // Arrange
            var money = Money.Create(1000, CurrencyCode.AUD);

            // Act
            var result = money.DivideBy(divisor);

            // Assert
            Assert.Equal(expected, result.Value);
        }

        [Theory]
        [InlineData(1, 1, 0)]
        [InlineData(2, 1, 1)]
        [InlineData(1, 2, -1)]
        internal void CompareTo_VariousValues_ReturnsExpectedResult(int value1, int value2, int expected)
        {
            // Arrange
            var money1 = Money.Create(value1, CurrencyCode.AUD);
            var money2 = Money.Create(value2, CurrencyCode.AUD);

            // Act
            var result = money1.CompareTo(money2);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        internal void ToString_UnknownSymbol()
        {
            // Arrange
            var money = Money.Zero(CurrencyCode.AUD);

            // Act
            var result = money.ToString();

            // Assert
            Assert.Equal("0.00(AUD)", result);
        }

        [Theory]
        [InlineData(1, "1.00(AUD)")]
        [InlineData(0.1, "0.10(AUD)")]
        [InlineData(0.01, "0.01(AUD)")]
        [InlineData(10, "10.00(AUD)")]
        [InlineData(100000, "100,000.00(AUD)")]
        internal void ToString_VariousValues_ReturnsExpectedString(decimal amount, string expected)
        {
            // Arrange
            // Act
            var result = Money.Create(amount, CurrencyCode.AUD);

            // Assert
            Assert.Equal(expected, result.ToString());
        }

        [Fact]
        internal void Equals_MoneyZeros_ReturnsTrue()
        {
            // Arrange
            // Act
            var money1 = Money.Zero(CurrencyCode.AUD);
            var money2 = Money.Zero(CurrencyCode.AUD);

            var result1 = money1.Equals(money2);
            var result2 = money1 == money2;

            // Assert
            Assert.True(result1);
            Assert.True(result2);
        }

        [Theory]
        [InlineData(0.01, 0.01, true)]
        [InlineData(1, 1, true)]
        [InlineData(3.14, 3.14, true)]
        [InlineData(1, 2, false)]
        [InlineData(0.11, 0.1, false)]
        [InlineData(10, 1, false)]
        [InlineData(3, 3.14, false)]
        internal void Equals_VariousValues_ReturnsExpectedResult(decimal value1, decimal value2, bool expected)
        {
            // Arrange
            // Act
            var money1 = Money.Create(value1, CurrencyCode.AUD);
            var money2 = Money.Create(value2, CurrencyCode.AUD);

            var result1 = money1.Equals(money2);
            var result2 = money1 == money2;

            // Assert
            Assert.Equal(expected, result1);
            Assert.Equal(expected, result2);
        }

        [Fact]
        internal void GetHashCode_ReturnsExpectedResult()
        {
            // Arrange
            // Act
            var result = Money.Create(1, CurrencyCode.AUD);

            // Assert
            Assert.Equal(-1106231307, result.GetHashCode());
        }
    }
}
