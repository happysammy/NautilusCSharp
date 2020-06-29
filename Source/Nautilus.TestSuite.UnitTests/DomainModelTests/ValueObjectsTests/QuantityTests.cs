//--------------------------------------------------------------------------------------------------
// <copyright file="QuantityTests.cs" company="Nautech Systems Pty Ltd">
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

namespace Nautilus.TestSuite.UnitTests.DomainModelTests.ValueObjectsTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.ValueObjects;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class QuantityTests
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
        internal void Subtract_VariousAmounts_ReturnsExpectedResult(int amount1, int amount2, int expected)
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
            var result = quantity * multiple;

            // Assert
            Assert.Equal(expected, result);
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
            Assert.Equal(100000, result);
        }

        [Fact]
        internal void ToStringFormatted_ReturnsExpectedResult()
        {
            // Arrange
            var quantity1 = Quantity.Create(10.05m, 2);
            var quantity2 = Quantity.Create(1000);
            var quantity3 = Quantity.Create(100000);
            var quantity4 = Quantity.Create(120100);
            var quantity5 = Quantity.Create(1000000);
            var quantity6 = Quantity.Create(2500000);
            var quantity7 = Quantity.Create(1111111);
            var quantity8 = Quantity.Create(2523000);
            var quantity9 = Quantity.Create(100000000);

            // Act
            var result1 = quantity1.ToStringFormatted();
            var result2 = quantity2.ToStringFormatted();
            var result3 = quantity3.ToStringFormatted();
            var result4 = quantity4.ToStringFormatted();
            var result5 = quantity5.ToStringFormatted();
            var result6 = quantity6.ToStringFormatted();
            var result7 = quantity7.ToStringFormatted();
            var result8 = quantity8.ToStringFormatted();
            var result9 = quantity9.ToStringFormatted();

            // Assert
            Assert.Equal("10.05", result1);
            Assert.Equal("1K", result2);
            Assert.Equal("100K", result3);
            Assert.Equal("120100", result4);
            Assert.Equal("1M", result5);
            Assert.Equal("2.5M", result6);
            Assert.Equal("1111111", result7);
            Assert.Equal("2.523M", result8);
            Assert.Equal("100M", result9);
        }
    }
}
