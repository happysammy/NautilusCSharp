// -------------------------------------------------------------------------------------------------
// <copyright file="BarSpecificationTests.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.Fixtures;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class BarSpecificationTests : TestBase
    {
        public BarSpecificationTests(ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact]
        internal void Create_WithValidString_ReturnsExpectedBarSpec()
        {
            // Arrange
            var barSpec1 = new BarSpecification(1, BarStructure.Minute, PriceType.Bid);
            var barSpec2 = new BarSpecification(1, BarStructure.Hour, PriceType.Mid);

            var string1 = barSpec1.ToString();
            var string2 = barSpec2.ToString();

            // Act
            var result1 = BarSpecification.FromString(string1);
            var result2 = BarSpecification.FromString(string2);

            // Assert
            Assert.Equal(barSpec1, result1);
            Assert.Equal(barSpec2, result2);
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
