//--------------------------------------------------------------------------------------------------
// <copyright file="SpreadAnalyzerTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DatabaseTests.AggregatorTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Database.Aggregators;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class SpreadAnalyzerTests
    {
        private readonly SpreadAnalyzer spreadAnalyzer;

        public SpreadAnalyzerTests()
        {
            this.spreadAnalyzer = new SpreadAnalyzer();
        }

        [Fact]
        internal void Ontick_WhenNoPrevioustick_UpdatesValuesAccordingly()
        {
            // Arrange
            var tick = new Tick(
                new Symbol("AUDUSD", Venue.FXCM),
                Price.Create(0.80000m, 0.00001m),
                Price.Create(0.80005m, 0.00001m),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.spreadAnalyzer.Update(tick);

            // Assert
            Assert.Equal(0.80000m, this.spreadAnalyzer.CurrentBid.Value);
            Assert.Equal(0.80005m, this.spreadAnalyzer.CurrentAsk.Value);
            Assert.Equal(0.00005m, this.spreadAnalyzer.CurrentSpread);
            Assert.Equal(0.00005m, this.spreadAnalyzer.MaxSpread.Spread);
            Assert.Equal(0.00005m, this.spreadAnalyzer.MinSpread.Spread);
            Assert.Equal(0.00005m, this.spreadAnalyzer.AverageSpread);
        }

        [Fact]
        internal void AverageSpread_WhenNoPreviousBarUpdates_CalculatesWithEachtick()
        {
            // Arrange
            var tick1 = new Tick(
                new Symbol("AUDUSD", Venue.FXCM),
                Price.Create(0.80000m, 0.00001m),
                Price.Create(0.80005m, 0.00001m),
                StubZonedDateTime.UnixEpoch());

            var tick2 = new Tick(
                new Symbol("AUDUSD", Venue.FXCM),
                Price.Create(0.80001m, 0.00001m),
                Price.Create(0.80006m, 0.00001m),
                StubZonedDateTime.UnixEpoch());

            var tick3 = new Tick(
                new Symbol("AUDUSD", Venue.FXCM),
                Price.Create(0.80001m, 0.00001m),
                Price.Create(0.80004m, 0.00001m),
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
                new Symbol("AUDUSD", Venue.FXCM),
                Price.Create(0.80000m, 0.00001m),
                Price.Create(0.80005m, 0.00001m),
                StubZonedDateTime.UnixEpoch());

            this.spreadAnalyzer.Update(tick1);
            this.spreadAnalyzer.OnBarUpdate(tick1.Timestamp);

            var tick2 = new Tick(
                new Symbol("AUDUSD", Venue.FXCM),
                Price.Create(0.80001m, 0.00001m),
                Price.Create(0.80006m, 0.00001m),
                StubZonedDateTime.UnixEpoch());

            var tick3 = new Tick(
                new Symbol("AUDUSD", Venue.FXCM),
                Price.Create(0.80001m, 0.00001m),
                Price.Create(0.80004m, 0.00001m),
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
