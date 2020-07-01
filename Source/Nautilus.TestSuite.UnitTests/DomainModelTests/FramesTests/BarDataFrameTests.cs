// -------------------------------------------------------------------------------------------------
// <copyright file="BarDataFrameTests.cs" company="Nautech Systems Pty Ltd">
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

using System.Diagnostics.CodeAnalysis;
using Nautilus.DomainModel.Frames;
using Nautilus.DomainModel.ValueObjects;
using Nautilus.TestSuite.TestKit.Fixtures;
using Nautilus.TestSuite.TestKit.Stubs;
using NodaTime;
using Xunit;
using Xunit.Abstractions;

namespace Nautilus.TestSuite.UnitTests.DomainModelTests.FramesTests
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class BarDataFrameTests : TestBase
    {
        private readonly BarType stubBarType;

        public BarDataFrameTests(ITestOutputHelper output)
            : base(output)
        {
            // Fixture Setup
            this.stubBarType = StubBarType.AUDUSD_OneMinuteAsk();
        }

        [Fact]
        internal void Count_WithStubBars_ReturnsExpectedResult()
        {
            // Arrange
            var bar1 = StubBarData.Create();
            var bar2 = StubBarData.Create(1);
            var bars = new[] { bar1, bar2 };

            var barDataFrame = new BarDataFrame(
                this.stubBarType,
                bars);

            // Act
            var result = barDataFrame.Count;

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        internal void StartDateTime_WithStubBars_ReturnsExpectedResult()
        {
            // Arrange
            var bar1 = StubBarData.Create();
            var bar2 = StubBarData.Create(1);
            var bars = new[] { bar1, bar2 };

            var barDataFrame = new BarDataFrame(
                this.stubBarType,
                bars);

            // Act
            var result = barDataFrame.StartDateTime;

            // Assert
            Assert.Equal(StubZonedDateTime.UnixEpoch(), result);
        }

        [Fact]
        internal void EndDateTime_WithStubBars_ReturnsExpectedResult()
        {
            // Arrange
            var bar1 = StubBarData.Create();
            var bar2 = StubBarData.Create(1);
            var bars = new[] { bar1, bar2 };

            var barDataFrame = new BarDataFrame(
                this.stubBarType,
                bars);

            // Act
            var result = barDataFrame.EndDateTime;

            // Assert
            Assert.Equal(StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(1), result);
        }

        // [Fact]
        // internal void Serialize_WithStubBar_ReturnsExpectedString()
        // {
        //     // Arrange
        //     var bar = StubBarData.Create();
        //     var bars = new[] { bar };
        //
        //     var barDataFrame = new BarDataFrame(
        //         this.stubBarType,
        //         bars);
        //
        //     // Act
        //     var result = JsonConvert.SerializeObject(barDataFrame);
        //
        //     // Assert
        //     this.Output.WriteLine(result);
        // }
        //
        // [Fact]
        // internal void Serialize_WithMultipleStubBars_ReturnsExpectedString()
        // {
        //     // Arrange
        //     var bar1 = StubBarData.Create();
        //     var bar2 = StubBarData.Create(1);
        //     var bar3 = StubBarData.Create(2);
        //     var bar4 = StubBarData.Create(3);
        //
        //     var bars = new[] { bar1, bar2, bar3, bar4 };
        //
        //     var barDataFrame = new BarDataFrame(
        //         this.stubBarType,
        //         bars);
        //
        //     // Act
        //     var result = JsonConvert.SerializeObject(barDataFrame);
        //
        //     // Assert
        //     this.Output.WriteLine(result);
        // }
    }
}
