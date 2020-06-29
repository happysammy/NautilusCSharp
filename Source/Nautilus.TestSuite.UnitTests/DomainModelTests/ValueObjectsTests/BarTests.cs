// -------------------------------------------------------------------------------------------------
// <copyright file="BarTests.cs" company="Nautech Systems Pty Ltd">
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
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DomainModelTests.ValueObjectsTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.Fixtures;
    using Nautilus.TestSuite.TestKit.Stubs;
    using NodaTime;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class BarTests : TestBase
    {
        public BarTests(ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact]
        internal void FromString_WithValidString_ReturnsExpectedBar()
        {
            // Arrange
            var bar = new Bar(
                Price.Create(0.80000m, 5),
                Price.Create(0.80010m, 5),
                Price.Create(0.79990m, 5),
                Price.Create(0.80001m, 5),
                Volume.Create(1000000),
                StubZonedDateTime.UnixEpoch());

            // Act
            var barString = bar.ToString();
            var result = Bar.FromString(barString);

            // Assert
            Assert.Equal(bar, result);
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
                Volume.Create(1000000),
                StubZonedDateTime.UnixEpoch());

            var bar2 = new Bar(
                Price.Create(0.80000m, 5),
                Price.Create(0.80010m, 5),
                Price.Create(0.79990m, 5),
                Price.Create(0.80001m, 5),
                Volume.Create(1000000),
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
                Volume.Create(1000000),
                StubZonedDateTime.UnixEpoch());

            var bar2 = new Bar(
                Price.Create(0.80000m, 5),
                Price.Create(0.80010m, 5),
                Price.Create(0.79990m, 5),
                Price.Create(0.80001m, 5),
                Volume.Create(1000000),
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
                Volume.Create(1000000),
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
                Volume.Create(1000000),
                StubZonedDateTime.UnixEpoch());

            // Act
            var result = bar.ToString();

            // Assert
            Assert.Equal("0.80000,0.80010,0.79990,0.80001,1000000,1970-01-01T00:00:00.000Z", result);
        }
    }
}
