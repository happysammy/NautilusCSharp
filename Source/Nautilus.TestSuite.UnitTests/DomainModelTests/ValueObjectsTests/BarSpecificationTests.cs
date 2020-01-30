// -------------------------------------------------------------------------------------------------
// <copyright file="BarSpecificationTests.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DomainModelTests.ValueObjectsTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class BarSpecificationTests
    {
        private readonly ITestOutputHelper output;

        public BarSpecificationTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;
        }

        [Fact]
        internal void Equals_WithEqualObject_ReturnsTrue()
        {
            // Arrange
            var barSpec1 = new BarSpecification(1, BarStructure.Minute, PriceType.Bid);
            var barSpec2 = new BarSpecification(1, BarStructure.Minute, PriceType.Bid);

            // Act
            var result1 = barSpec1.Equals(barSpec2);
            var result2 = barSpec1 == barSpec2;

            // Assert
            Assert.True(result1);
            Assert.True(result2);
        }

        [Fact]
        internal void Equals_WithUnequalObject_ReturnsTrue()
        {
            // Arrange
            var barSpec1 = new BarSpecification(1, BarStructure.Minute, PriceType.Bid);
            var barSpec2 = new BarSpecification(1, BarStructure.Hour, PriceType.Ask);

            // Act
            var result1 = barSpec1.Equals(barSpec2);
            var result2 = barSpec1 == barSpec2;

            // Assert
            Assert.False(result1);
            Assert.False(result2);
        }

        [Fact]
        internal void GetHashCode_ReturnsExpectedInt32()
        {
            // Arrange
            var barSpec = new BarSpecification(1, BarStructure.Minute, PriceType.Bid);

            // Act
            var result = barSpec.GetHashCode();

            // Assert
            Assert.NotEqual(0, result);
        }

        [Fact]
        internal void ToString_ReturnsExpectedString()
        {
            // Arrange
            var barSpec = new BarSpecification(1, BarStructure.Minute, PriceType.Bid);

            // Act
            var result = barSpec.ToString();

            // Assert
            Assert.Equal("1-MINUTE[BID]", result);
        }
    }
}
