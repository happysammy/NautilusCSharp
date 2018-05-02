// -------------------------------------------------------------------------------------------------
// <copyright file="BarSpecificationTests.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
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

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class BarSpecificationTests
    {
        private readonly ITestOutputHelper output;

        public BarSpecificationTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;
        }

        [Fact]
        internal void Equals_WithNullObject_ReturnsFalse()
        {
            // Arrange
            var barSpec = new BarSpecification(
                BarQuoteType.Bid,
                BarResolution.Minute,
                1);

            // Act
            var result1 = barSpec.Equals(null);
            var result2 = barSpec == null;

            // Assert
            Assert.False(result1);
            Assert.False(result2);
        }

        [Fact]
        internal void Equals_WithEqualObject_ReturnsTrue()
        {
            // Arrange
            var barSpec1 = new BarSpecification(
                BarQuoteType.Bid,
                BarResolution.Minute,
                1);

            var barSpec2 = new BarSpecification(
                BarQuoteType.Bid,
                BarResolution.Minute,
                1);

            // Act
            var result1 = barSpec1.Equals(barSpec2);
            var result2 = barSpec1 == barSpec2;

            // Assert
            Assert.True(result1);
            Assert.True(result2);
        }

        [Fact]
        internal void GetHashCode_ReturnsExpectedInt32()
        {
            // Arrange
            var barSpec = new BarSpecification(
                BarQuoteType.Bid,
                BarResolution.Minute,
                1);

            // Act
            var result = barSpec.GetHashCode();

            // Assert
            Assert.NotEqual(0, result);
        }

        [Fact]
        internal void ToString_ReturnsExpectedString()
        {
            // Arrange
            var barSpec = new BarSpecification(
                BarQuoteType.Bid,
                BarResolution.Minute,
                1);

            // Act
            var result = barSpec.ToString();

            // Assert
            Assert.Equal("1-Minute[Bid]", result);
        }
    }
}
