//--------------------------------------------------------------------------------------------------
// <copyright file="QuoteProviderTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.BlackBoxTests.DataTests.MarketTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Common.Interfaces;
    using Nautilus.Database.Aggregators;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class QuoteProviderTests
    {
        private readonly ITestOutputHelper output;
        private readonly IQuoteProvider quoteProvider;
        private readonly Symbol audusd;
        private readonly Symbol eurusd;
        private readonly Symbol eurgbp;

        public QuoteProviderTests(ITestOutputHelper output)
        {
            // Fixture;
            this.output = output;
            this.quoteProvider = new QuoteProvider(Venue.LMAX);

            this.audusd = new Symbol("AUDUSD", Venue.LMAX);
            this.eurusd = new Symbol("EURUSD", Venue.LMAX);
            this.eurgbp = new Symbol("EURGBP", Venue.LMAX);
        }

        [Fact]
        internal void OnQuote_WhenNoPreviousQuote_AddsQuoteToLastQuotes()
        {
            // Arrange
            var quote = new Tick(
                this.audusd,
                Price.Create(0.80000m, 0.00001m),
                Price.Create(0.80005m, 0.00001m),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.quoteProvider.OnTick(quote);

            // Assert
            Assert.Equal(quote, this.quoteProvider.GetLastTick(this.audusd));
        }

        [Fact]
        internal void OnQuote_WhenPreviousQuote_AddsQuoteToLastQuotes()
        {
            // Arrange
            var quote1 = new Tick(
                this.audusd,
                Price.Create(0.80000m, 0.00001m),
                Price.Create(0.80005m, 0.00001m),
                StubZonedDateTime.UnixEpoch());

            var quote2 = new Tick(
                this.audusd,
                Price.Create(0.80001m, 0.00001m),
                Price.Create(0.80006m, 0.00001m),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.quoteProvider.OnTick(quote1);
            this.quoteProvider.OnTick(quote2);

            // Assert
            Assert.Equal(quote2, this.quoteProvider.GetLastTick(this.audusd));
        }

        [Fact]
        internal void GetLastQuote_ReturnsTheLastQuote()
        {
            // Arrange
            var quote = new Tick(
                this.audusd,
                Price.Create(0.80001m, 0.00001m),
                Price.Create(0.80006m, 0.00001m),
                StubZonedDateTime.UnixEpoch());

            this.quoteProvider.OnTick(quote);

            // Act
            var result = this.quoteProvider.GetLastTick(this.audusd);

            // Assert
            Assert.Equal(quote, result);
        }

        [Fact]
        internal void GetQuoteSymbolList_ReturnsTheExpectedListOfSymbols()
        {
            // Arrange
            var quote1 = new Tick(
                this.audusd,
                Price.Create(0.80000m, 0.00001m),
                Price.Create(0.80005m, 0.00001m),
                StubZonedDateTime.UnixEpoch());

            var quote2 = new Tick(
                this.eurusd,
                Price.Create(1.20000m, 0.00001m),
                Price.Create(1.20005m, 0.00001m),
                StubZonedDateTime.UnixEpoch());

            this.quoteProvider.OnTick(quote1);
            this.quoteProvider.OnTick(quote2);

            // Act
            var result = this.quoteProvider.GetSymbolList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(this.audusd, result);
            Assert.Contains(this.eurusd, result);
        }

        [Fact]
        internal void GetExchangeRate_WhenNoQuotesReceived_ReturnsOptionWithNoValue()
        {
            // Arrange

            // Act
            var result = this.quoteProvider.GetExchangeRate(CurrencyCode.AUD, CurrencyCode.USD);

            // Assert
            Assert.True(result.HasNoValue);
        }

        [Fact]
        internal void GetExchangeRate_WhenAccountCurrencyTheSameAsQuoteCurrency()
        {
            // Arrange

            // Act
            var result = this.quoteProvider.GetExchangeRate(CurrencyCode.AUD, CurrencyCode.AUD);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        internal void GetExchangeRate_WhenLastQuotesContainExchangeRate()
        {
            // Arrange
            var quote = new Tick(
                this.audusd,
                Price.Create(0.80000m, 0.00001m),
                Price.Create(0.80005m, 0.00001m),
                StubZonedDateTime.UnixEpoch());

            this.quoteProvider.OnTick(quote);

            // Act
            var result = this.quoteProvider.GetExchangeRate(CurrencyCode.AUD, CurrencyCode.USD);

            // Assert
            Assert.Equal(0.80000m, result);
        }

        [Fact]
        internal void GetExchangeRate_WhenLastQuotesContainSwappedExchangeRate()
        {
            // Arrange
            var quote = new Tick(
                this.eurgbp,
                Price.Create(0.88000m, 0.00001m),
                Price.Create(0.88005m, 0.00001m),
                StubZonedDateTime.UnixEpoch());

            this.quoteProvider.OnTick(quote);

            // Act
            var result = this.quoteProvider.GetExchangeRate(CurrencyCode.GBP, CurrencyCode.EUR);

            // Assert
            Assert.Equal(1 / 0.88000m, result);
        }

        [Fact]
        internal void GetExchangeRate_WhenAccountCurrencyDiffersFromQuoteCurrency()
        {
            // Arrange
            var quote1 = new Tick(
                this.audusd,
                Price.Create(0.80000m, 0.00001m),
                Price.Create(0.80005m, 0.00001m),
                StubZonedDateTime.UnixEpoch());

            var quote2 = new Tick(
                this.eurusd,
                Price.Create(1.20000m, 0.00001m),
                Price.Create(1.20005m, 0.00001m),
                StubZonedDateTime.UnixEpoch());

            this.quoteProvider.OnTick(quote1);
            this.quoteProvider.OnTick(quote2);

            // Act
            var result = this.quoteProvider.GetExchangeRate(CurrencyCode.AUD, CurrencyCode.EUR);

            // Assert
            Assert.Equal(0.80000m / 1.20000m, result);
        }
    }
}