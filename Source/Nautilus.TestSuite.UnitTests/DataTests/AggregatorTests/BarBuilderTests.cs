//--------------------------------------------------------------------------------------------------
// <copyright file="BarBuilderTests.cs" company="Nautech Systems Pty Ltd">
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

using System.Diagnostics.CodeAnalysis;
using Nautilus.Data.Aggregation;
using Nautilus.DomainModel.ValueObjects;
using Nautilus.TestSuite.TestKit.Stubs;
using NodaTime;
using Xunit;

namespace Nautilus.TestSuite.UnitTests.DataTests.AggregatorTests
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class BarBuilderTests
    {
        [Fact]
        internal void Build_WithOneQuotes_ReturnsExpectedBar()
        {
            // Arrange
            var quote = Price.Create(1.00000m, 5);

            var barBuilder = new BarBuilder(quote);

            // Act
            var bar = barBuilder.Build(StubZonedDateTime.UnixEpoch());

            // Assert
            Assert.Equal(quote, bar.Open);
            Assert.Equal(quote, bar.High);
            Assert.Equal(quote, bar.Low);
            Assert.Equal(quote, bar.Close);
            Assert.Equal(Volume.One(), bar.Volume);
            Assert.Equal(StubZonedDateTime.UnixEpoch(), bar.Timestamp);
        }

        [Fact]
        internal void Build_WithVariousQuotes1_ReturnsExpectedBar()
        {
            // Arrange
            var timestamp = StubZonedDateTime.UnixEpoch();
            var quote1 = Price.Create(1.00010m, 5);
            var quote2 = Price.Create(0.99980m, 5);
            var quote3 = Price.Create(0.99950m, 5);
            var quote4 = Price.Create(0.99950m, 5);

            var barBuilder = new BarBuilder(quote1);

            // Act
            barBuilder.Update(quote2);
            barBuilder.Update(quote3);
            barBuilder.Update(quote4);

            var bar = barBuilder.Build(timestamp + Period.FromSeconds(1).ToDuration());

            // Assert
            Assert.Equal(quote1, bar.Open);
            Assert.Equal(quote1, bar.High);
            Assert.Equal(quote4, bar.Low);
            Assert.Equal(quote4, bar.Close);
            Assert.Equal(Volume.Create(4), bar.Volume);
            Assert.Equal(timestamp + Period.FromSeconds(1).ToDuration(), bar.Timestamp);
        }

        [Fact]
        internal void Build_WithVariousQuotes2_ReturnsExpectedBar()
        {
            // Arrange
            var timestamp = StubZonedDateTime.UnixEpoch();
            var quote1 = Price.Create(1.00010m, 5);
            var quote2 = Price.Create(0.99980m, 5);
            var quote3 = Price.Create(1.00090m, 5);
            var quote4 = Price.Create(0.99800m, 5);

            var barBuilder = new BarBuilder(quote1);

            // Act
            barBuilder.Update(quote2);
            barBuilder.Update(quote3);
            barBuilder.Update(quote4);

            var bar = barBuilder.Build(timestamp + Period.FromMinutes(5).ToDuration());

            // Assert
            Assert.Equal(quote1, bar.Open);
            Assert.Equal(quote3, bar.High);
            Assert.Equal(quote4, bar.Low);
            Assert.Equal(quote4, bar.Close);
            Assert.Equal(Volume.Create(4), bar.Volume);
            Assert.Equal(timestamp + Period.FromMinutes(5).ToDuration(), bar.Timestamp);
        }

        [Fact]
        internal void Build_WithVariousQuotes3_ReturnsExpectedBar()
        {
            // Arrange
            var quote1 = Price.Create(0.99999m, 5);
            var quote2 = Price.Create(1.00001m, 5);
            var quote3 = Price.Create(1.00000m, 5);
            var quote4 = Price.Create(1.00000m, 5);

            var barBuilder = new BarBuilder(quote1);

            // Act
            barBuilder.Update(quote2);
            barBuilder.Update(quote3);
            barBuilder.Update(quote4);

            var bar = barBuilder.Build(StubZonedDateTime.UnixEpoch());

            // Assert
            Assert.Equal(quote1, bar.Open);
            Assert.Equal(quote2, bar.High);
            Assert.Equal(quote1, bar.Low);
            Assert.Equal(quote4, bar.Close);
            Assert.Equal(Volume.Create(4), bar.Volume);
            Assert.Equal(StubZonedDateTime.UnixEpoch(), bar.Timestamp);
        }
    }
}
