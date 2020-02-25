// -------------------------------------------------------------------------------------------------
// <copyright file="BarDataFrameTests.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DomainModelTests.FramesTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Frames;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.Fixtures;
    using Nautilus.TestSuite.TestKit.Stubs;
    using Newtonsoft.Json;
    using NodaTime;
    using Xunit;
    using Xunit.Abstractions;

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

        [Fact]
        internal void Serialize_WithStubBar_ReturnsExpectedString()
        {
            // Arrange
            var bar = StubBarData.Create();
            var bars = new[] { bar };

            var barDataFrame = new BarDataFrame(
                this.stubBarType,
                bars);

            // Act
            var result = JsonConvert.SerializeObject(barDataFrame);

            // Assert
            this.Output.WriteLine(result);
        }

        [Fact]
        internal void Serialize_WithMultipleStubBars_ReturnsExpectedString()
        {
            // Arrange
            var bar1 = StubBarData.Create();
            var bar2 = StubBarData.Create(1);
            var bar3 = StubBarData.Create(2);
            var bar4 = StubBarData.Create(3);

            var bars = new[] { bar1, bar2, bar3, bar4 };

            var barDataFrame = new BarDataFrame(
                this.stubBarType,
                bars);

            // Act
            var result = JsonConvert.SerializeObject(barDataFrame);

            // Assert
            this.Output.WriteLine(result);
        }
    }
}
