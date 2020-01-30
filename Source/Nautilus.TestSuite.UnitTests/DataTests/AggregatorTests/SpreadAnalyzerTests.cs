//--------------------------------------------------------------------------------------------------
// <copyright file="SpreadAnalyzerTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DataTests.AggregatorTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Data.Aggregation;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class SpreadAnalyzerTests
    {
        private readonly SpreadAnalyzer spreadAnalyzer;

        public SpreadAnalyzerTests()
        {
            this.spreadAnalyzer = new SpreadAnalyzer();
        }

        [Fact]
        internal void Update_WhenNoPreviousTick_UpdatesValuesAccordingly()
        {
            // Arrange
            var tick = new Tick(
                new Symbol("AUDUSD", new Venue("FXCM")),
                Price.Create(0.80000m, 5),
                Price.Create(0.80005m, 5),
                Volume.One(),
                Volume.One(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.spreadAnalyzer.Update(tick);

            // Assert
            Assert.Equal(0.80000m, this.spreadAnalyzer.CurrentBid?.Value);
            Assert.Equal(0.80005m, this.spreadAnalyzer.CurrentAsk?.Value);
            Assert.Equal(0.00005m, this.spreadAnalyzer.CurrentSpread);
            Assert.Equal(0.00005m, this.spreadAnalyzer.MaxSpread.Spread);
            Assert.Equal(0.00005m, this.spreadAnalyzer.MinSpread.Spread);
            Assert.Equal(0.00005m, this.spreadAnalyzer.AverageSpread);
        }

        [Fact]
        internal void AverageSpread_WhenNoPreviousBarUpdates_CalculatesWithEachTick()
        {
            // Arrange
            var tick1 = new Tick(
                new Symbol("AUDUSD", new Venue("FXCM")),
                Price.Create(0.80000m, 5),
                Price.Create(0.80005m, 5),
                Volume.One(),
                Volume.One(),
                StubZonedDateTime.UnixEpoch());

            var tick2 = new Tick(
                new Symbol("AUDUSD", new Venue("FXCM")),
                Price.Create(0.80001m, 5),
                Price.Create(0.80006m, 5),
                Volume.One(),
                Volume.One(),
                StubZonedDateTime.UnixEpoch());

            var tick3 = new Tick(
                new Symbol("AUDUSD", new Venue("FXCM")),
                Price.Create(0.80001m, 5),
                Price.Create(0.80004m, 5),
                Volume.One(),
                Volume.One(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.spreadAnalyzer.Update(tick1);
            this.spreadAnalyzer.Update(tick2);
            this.spreadAnalyzer.Update(tick3);

            // Assert
            Assert.Equal(0.00004m, this.spreadAnalyzer.AverageSpread);
        }

        [Fact]
        internal void AverageSpread_WhenPreviousBarUpdates_ReturnsCalculatedValue()
        {
            // Arrange
            var tick1 = new Tick(
                new Symbol("AUDUSD", new Venue("FXCM")),
                Price.Create(0.80000m, 5),
                Price.Create(0.80005m, 5),
                Volume.One(),
                Volume.One(),
                StubZonedDateTime.UnixEpoch());

            this.spreadAnalyzer.Update(tick1);
            this.spreadAnalyzer.OnBarUpdate(tick1.Timestamp);

            var tick2 = new Tick(
                new Symbol("AUDUSD", new Venue("FXCM")),
                Price.Create(0.80001m, 5),
                Price.Create(0.80006m, 5),
                Volume.One(),
                Volume.One(),
                StubZonedDateTime.UnixEpoch());

            var tick3 = new Tick(
                new Symbol("AUDUSD", new Venue("FXCM")),
                Price.Create(0.80001m, 5),
                Price.Create(0.80004m, 5),
                Volume.One(),
                Volume.One(),
                StubZonedDateTime.UnixEpoch());

            this.spreadAnalyzer.Update(tick2);
            this.spreadAnalyzer.Update(tick3);

            // Act
            var result = this.spreadAnalyzer.AverageSpread;

            // Assert
            Assert.Equal(0.00005m, result);
        }
    }
}
