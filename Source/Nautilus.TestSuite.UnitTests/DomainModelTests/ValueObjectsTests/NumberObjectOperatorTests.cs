//--------------------------------------------------------------------------------------------------
// <copyright file="NumberObjectOperatorTests.cs" company="Nautech Systems Pty Ltd">
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

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class NumberObjectOperatorTests
    {
        [Fact]
        internal void EqualToOperator()
        {
            // Arrange
            var valueObject1 = Quantity.Create(100);
            var valueObject2 = Quantity.Create(100);
            var valueObject3 = Quantity.Create(50);

            // Act
            var result1 = valueObject1 == valueObject2;
            var result2 = valueObject1 == valueObject3;

            // Assert
            Assert.True(result1);
            Assert.False(result2);
        }

        [Fact]
        internal void NotEqualToOperator()
        {
            // Arrange
            var valueObject1 = Quantity.Create(100);
            var valueObject2 = Quantity.Create(100);
            var valueObject3 = Quantity.Create(50);

            // Act
            var result1 = valueObject1 != valueObject2;
            var result2 = valueObject1 != valueObject3;

            // Assert
            Assert.False(result1);
            Assert.True(result2);
        }

        [Fact]
        internal void AdditionOperator()
        {
            // Arrange
            var valueObject1 = Quantity.Create(100);
            var valueObject2 = Quantity.Create(100);

            // Act
            var result1 = valueObject1 + valueObject2;
            var result2 = 50 + valueObject1;
            var result3 = valueObject1 + 50;

            // Assert
            Assert.Equal(200, result1);
            Assert.Equal(150, result2);
            Assert.Equal(150, result3);
        }

        [Fact]
        internal void SubtractionOperator()
        {
            // Arrange
            var valueObject1 = Quantity.Create(100);
            var valueObject2 = Quantity.Create(100);

            // Act
            var result1 = valueObject1 - valueObject2;
            var result2 = 200 - valueObject1;
            var result3 = valueObject1 - 50;

            // Assert
            Assert.Equal(0, result1);
            Assert.Equal(100, result2);
            Assert.Equal(50, result3);
        }

        [Fact]
        internal void GreaterThanOperator()
        {
            // Arrange
            var valueObject1 = Quantity.Create(100);
            var valueObject2 = Quantity.Create(100);

            // Act
            var result1 = valueObject1 > valueObject2;
            var result2 = valueObject1 < 200;
            var result3 = valueObject1 > 50;

            // Assert
            Assert.False(result1);
            Assert.True(result2);
            Assert.True(result3);
        }

        [Fact]
        internal void LessThanOperator()
        {
            // Arrange
            var valueObject1 = Quantity.Create(100);
            var valueObject2 = Quantity.Create(100);

            // Act
            var result1 = valueObject1 < valueObject2;
            var result2 = valueObject1 > 50;
            var result3 = valueObject1 < 200;

            // Assert
            Assert.False(result1);
            Assert.True(result2);
            Assert.True(result3);
        }
    }
}
