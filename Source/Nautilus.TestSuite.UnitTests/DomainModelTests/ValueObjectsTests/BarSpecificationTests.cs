// -------------------------------------------------------------------------------------------------
// <copyright file="BarSpecificationTests.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
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
            var barType1 = new BarSpecification(
                QuoteType.Bid,
                Resolution.Minute,
                1);

            var barType2 = new BarSpecification(
                QuoteType.Bid,
                Resolution.Minute,
                1);

            // Act
            var result1 = barType1.Equals(barType2);
            var result2 = barType1 == barType2;

            // Assert
            Assert.True(result1);
            Assert.True(result2);
        }

        [Fact]
        internal void GetHashCode_ReturnsExpectedInt32()
        {
            // Arrange
            var barType = new BarSpecification(
                QuoteType.Bid,
                Resolution.Minute,
                1);

            // Act
            var result = barType.GetHashCode();

            // Assert
            Assert.NotEqual(0, result);
        }

        [Fact]
        internal void ToString_ReturnsExpectedString()
        {
            // Arrange
            var barType = new BarSpecification(
                QuoteType.Bid,
                Resolution.Minute,
                1);

            // Act
            var result = barType.ToString();

            // Assert
            Assert.Equal("1-Minute[Bid]", result);
        }
    }
}
