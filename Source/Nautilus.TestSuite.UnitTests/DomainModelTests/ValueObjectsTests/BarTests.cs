// -------------------------------------------------------------------------------------------------
// <copyright file="BarTests.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DomainModelTests.ValueObjectsTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NodaTime;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class BarTests
    {
        private readonly ITestOutputHelper output;

        public BarTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;
        }

        [Fact]
        internal void Equals_WithEqualObject_ReturnsTrue()
        {
            // Arrange
            var bar1 = new Bar(
                Price.Create(0.80000m, 5),
                Price.Create(0.80010m, 5),
                Price.Create(0.79990m, 5),
                Price.Create(0.80001m, 5),
                Quantity.Create(1000000),
                StubZonedDateTime.UnixEpoch());

            var bar2 = new Bar(
                Price.Create(0.80000m, 5),
                Price.Create(0.80010m, 5),
                Price.Create(0.79990m, 5),
                Price.Create(0.80001m, 5),
                Quantity.Create(1000000),
                StubZonedDateTime.UnixEpoch());

            // Act
            var result1 = bar1.Equals(bar2);
            var result2 = bar1 == bar2;

            // Assert
            Assert.True(result1);
            Assert.True(result2);
        }

        [Fact]
        internal void Equals_WithUnequalObject_ReturnsFalse()
        {
            // Arrange
            var bar1 = new Bar(
                Price.Create(0.80000m, 5),
                Price.Create(0.80010m, 5),
                Price.Create(0.79990m, 5),
                Price.Create(0.80001m, 5),
                Quantity.Create(1000000),
                StubZonedDateTime.UnixEpoch());

            var bar2 = new Bar(
                Price.Create(0.80000m, 5),
                Price.Create(0.80010m, 5),
                Price.Create(0.79990m, 5),
                Price.Create(0.80001m, 5),
                Quantity.Create(1000000),
                StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(1));

            // Act
            var result1 = bar1.Equals(bar2);
            var result2 = bar1 == bar2;

            // Assert
            Assert.False(result1);
            Assert.False(result2);
        }

        [Fact]
        internal void GetHashCode_ReturnsExpectedInt32()
        {
            // Arrange
            var bar = new Bar(
                Price.Create(0.80000m, 5),
                Price.Create(0.80010m, 5),
                Price.Create(0.79990m, 5),
                Price.Create(0.80001m, 5),
                Quantity.Create(1000000),
                StubZonedDateTime.UnixEpoch());

            // Act
            var result = bar.GetHashCode();

            // Assert
            Assert.Equal(typeof(int), result.GetType());
            Assert.NotEqual(0, result);
        }

        [Fact]
        internal void ToString_ReturnsExpectedString()
        {
            // Arrange
            var bar = new Bar(
                Price.Create(0.80000m, 5),
                Price.Create(0.80010m, 5),
                Price.Create(0.79990m, 5),
                Price.Create(0.80001m, 5),
                Quantity.Create(1000000),
                StubZonedDateTime.UnixEpoch());

            // Act
            var result = bar.ToString();

            // Assert
            Assert.Equal("0.80000,0.80010,0.79990,0.80001,1000000,1970-01-01T00:00:00.000Z", result);
        }
    }
}
