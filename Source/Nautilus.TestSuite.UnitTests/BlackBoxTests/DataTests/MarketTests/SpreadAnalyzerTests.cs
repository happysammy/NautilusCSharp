// -------------------------------------------------------------------------------------------------
// <copyright file="SpreadAnalyzerTests.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.BlackBoxTests.DataTests.MarketTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.BlackBox.Data.Market;
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
            this.spreadAnalyzer = new SpreadAnalyzer(0.00001m);
        }

        [Fact]
        internal void OnQuote_WhenNoPreviousQuote_UpdatesValuesAccordingly()
        {
            // Arrange
            var quote = new Tick(
                new Symbol("AUDUSD", Exchange.FXCM),
                Price.Create(0.80000m, 0.00001m),
                Price.Create(0.80005m, 0.00001m),
                StubDateTime.Now());

            // Act
            this.spreadAnalyzer.OnQuote(quote);

            // Assert
            Assert.Equal(0.80000m, this.spreadAnalyzer.CurrentBid.Value);
            Assert.Equal(0.80005m, this.spreadAnalyzer.CurrentAsk.Value);
            Assert.Equal(0.00005m, this.spreadAnalyzer.CurrentSpread);
            Assert.Equal(0.00005m, this.spreadAnalyzer.MaxSpread.Item2);
            Assert.Equal(0.00005m, this.spreadAnalyzer.MinSpread.Item2);
            Assert.Equal(0.00005m, this.spreadAnalyzer.AverageSpread);
        }

        [Fact]
        internal void AverageSpread_WhenNoPreviousBarUpdates_CalculatesWithEachQuote()
        {
            // Arrange
            var quote1 = new Tick(
                new Symbol("AUDUSD", Exchange.FXCM),
                Price.Create(0.80000m, 0.00001m),
                Price.Create(0.80005m, 0.00001m),
                StubDateTime.Now());

            var quote2 = new Tick(
                new Symbol("AUDUSD", Exchange.FXCM),
                Price.Create(0.80001m, 0.00001m),
                Price.Create(0.80006m, 0.00001m),
                StubDateTime.Now());

            var quote3 = new Tick(
                new Symbol("AUDUSD", Exchange.FXCM),
                Price.Create(0.80001m, 0.00001m),
                Price.Create(0.80004m, 0.00001m),
                StubDateTime.Now());

            // Act
            this.spreadAnalyzer.OnQuote(quote1);
            this.spreadAnalyzer.OnQuote(quote2);
            this.spreadAnalyzer.OnQuote(quote3);

            // Assert
            Assert.Equal(0.00004m, this.spreadAnalyzer.AverageSpread);
        }

        [Fact]
        internal void AverageSpread_WhenPreviousBarUpdates_ReturnsCalculatedValue()
        {
            // Arrange
            var quote1 = new Tick(
                new Symbol("AUDUSD", Exchange.FXCM),
                Price.Create(0.80000m, 0.00001m),
                Price.Create(0.80005m, 0.00001m),
                StubDateTime.Now());

            this.spreadAnalyzer.OnQuote(quote1);
            this.spreadAnalyzer.OnBarUpdate(quote1.Timestamp);

            var quote2 = new Tick(
                new Symbol("AUDUSD", Exchange.FXCM),
                Price.Create(0.80001m, 0.00001m),
                Price.Create(0.80006m, 0.00001m),
                StubDateTime.Now());

            var quote3 = new Tick(
                new Symbol("AUDUSD", Exchange.FXCM),
                Price.Create(0.80001m, 0.00001m),
                Price.Create(0.80004m, 0.00001m),
                StubDateTime.Now());

            this.spreadAnalyzer.OnQuote(quote2);
            this.spreadAnalyzer.OnQuote(quote3);

            // Act
            var result = this.spreadAnalyzer.AverageSpread;

            // Assert
            Assert.Equal(0.00005m, result);
        }
    }
}