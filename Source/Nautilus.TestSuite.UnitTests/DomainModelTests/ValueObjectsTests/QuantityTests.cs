//--------------------------------------------------------------------------------------------------
// <copyright file="QuantityTests.cs" company="Nautech Systems Pty Ltd">
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
    public class QuantityTests
    {
        [Fact]
        internal void Zero_ReturnsExpectedQuantity()
        {
            // Arrange
            // Act
            var result = Quantity.Zero();

            // Assert
            Assert.Equal(0, result.Value);
        }

        [Fact]
        internal void Create_ValidAmount_ReturnsExpectedAmount()
        {
            // Arrange
            // Act
            var result = Quantity.Create(100);

            // Assert
            Assert.Equal(100, result.Value);
        }

        [Theory]
        [InlineData(1, 1, false)]
        [InlineData(2, 1, true)]
        [InlineData(1, 2, false)]
        internal void GreaterThan_VariousAmounts_ReturnsExpectedResult(int amount1, int amount2, bool expected)
        {
            // Arrange
            var quantity1 = Quantity.Create(amount1);
            var quantity2 = Quantity.Create(amount2);

            // Act
            var result = quantity1.Value > quantity2.Value;

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(1, 1, false)]
        [InlineData(1, 2, true)]
        [InlineData(2, 1, false)]
        internal void LessThan_VariousAmounts_ReturnsExpectedResult(int amount1, int amount2, bool expected)
        {
            // Arrange
            var quantity1 = Quantity.Create(amount1);
            var quantity2 = Quantity.Create(amount2);

            // Act
            var result = quantity1.Value < quantity2.Value;

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(1, 2, 3)]
        [InlineData(100, 200, 300)]
        [InlineData(100000, 100000, 200000)]
        internal void Add_VariousAmounts_ReturnsExpectedResult(int amount1, int amount2, int expected)
        {
            // Arrange
            var quantity1 = Quantity.Create(amount1);
            var quantity2 = Quantity.Create(amount2);

            // Act
            var result = quantity1.Add(quantity2);

            // Assert
            Assert.Equal(expected, result.Value);
        }

        [Theory]
        [InlineData(1, 1, 0)]
        [InlineData(200, 100, 100)]
        [InlineData(200000, 100000, 100000)]
        internal void Substract_VariousAmounts_ReturnsExpectedResult(int amount1, int amount2, int expected)
        {
            // Arrange
            var quantity1 = Quantity.Create(amount1);
            var quantity2 = Quantity.Create(amount2);

            // Act
            var result = quantity1.Subtract(quantity2);

            // Assert
            Assert.Equal(expected, result.Value);
        }

        [Theory]
        [InlineData(1, 1000)]
        [InlineData(200, 200000)]
        internal void MultiplyBy_VariousAmounts_ReturnsExpectedResult(int multiple, int expected)
        {
            // Arrange
            var quantity = Quantity.Create(1000);

            // Act
            var result = quantity.MultiplyBy(multiple);

            // Assert
            Assert.Equal(expected, result.Value);
        }

        [Theory]
        [InlineData(1, 1, 0)]
        [InlineData(1, 2, -1)]
        [InlineData(2, 1, 1)]
        internal void CompareTo_VariousQuantities_ReturnsExpectedResults(int left, int right, int expected)
        {
            // Arrange
            var quantity1 = Quantity.Create(left);
            var quantity2 = Quantity.Create(right);

            // Act
            var result = quantity1.CompareTo(quantity2);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        internal void Equals_QuantitiesZero_ReturnsExpectedResults()
        {
            // Arrange
            var quantity1 = Quantity.Zero();
            var quantity2 = Quantity.Zero();

            // Act
            var result1 = quantity1.Equals(quantity2);
            var result2 = quantity1 == quantity2;

            // Assert
            Assert.True(result1);
            Assert.True(result2);
        }

        [Theory]
        [InlineData(1, 1, true)]
        [InlineData(1000, 2000, false)]
        internal void Equals_VariousQuantities_ReturnsExpectedResults(int amount1, int amount2, bool expected)
        {
            // Arrange
            var quantity1 = Quantity.Create(amount1);
            var quantity2 = Quantity.Create(amount2);

            // Act
            var result1 = quantity1.Equals(quantity2);
            var result2 = quantity1 == quantity2;

            // Assert
            Assert.Equal(expected, result1);
            Assert.Equal(expected, result2);
        }

        [Fact]
        internal void GetHashCode_ReturnsExpectedResult()
        {
            // Arrange
            var quantity = Quantity.Create(100000);

            // Act
            var result = quantity.GetHashCode();

            // Assert
            Assert.Equal(100493, result);
        }
    }
}
