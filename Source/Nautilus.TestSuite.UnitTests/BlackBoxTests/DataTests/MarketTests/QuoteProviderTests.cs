// -------------------------------------------------------------------------------------------------
// <copyright file="QuoteProviderTests.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.BlackBoxTests.DataTests.MarketTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.BlackBox.Data.Market;
    using Nautilus.BlackBox.Core.Interfaces;
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
            this.quoteProvider = new QuoteProvider(Exchange.LMAX);

            this.audusd = new Symbol("AUDUSD", Exchange.LMAX);
            this.eurusd = new Symbol("EURUSD", Exchange.LMAX);
            this.eurgbp = new Symbol("EURGBP", Exchange.LMAX);
        }

        [Fact]
        internal void OnQuote_WhenNoPreviousQuote_AddsQuoteToLastQuotes()
        {
            // Arrange
            var quote = new Tick(
                this.audusd,
                Price.Create(0.80000m, 0.00001m),
                Price.Create(0.80005m, 0.00001m),
                StubDateTime.Now());

            // Act
            this.quoteProvider.OnQuote(quote);

            // Assert
            Assert.Equal(quote, this.quoteProvider.GetLastQuote(this.audusd));
        }

        [Fact]
        internal void OnQuote_WhenPreviousQuote_AddsQuoteToLastQuotes()
        {
            // Arrange
            var quote1 = new Tick(
                this.audusd,
                Price.Create(0.80000m, 0.00001m),
                Price.Create(0.80005m, 0.00001m),
                StubDateTime.Now());

            var quote2 = new Tick(
                this.audusd,
                Price.Create(0.80001m, 0.00001m),
                Price.Create(0.80006m, 0.00001m),
                StubDateTime.Now());

            // Act
            this.quoteProvider.OnQuote(quote1);
            this.quoteProvider.OnQuote(quote2);

            // Assert
            Assert.Equal(quote2, this.quoteProvider.GetLastQuote(this.audusd));
        }

        [Fact]
        internal void GetLastQuote_ReturnsTheLastQuote()
        {
            // Arrange
            var quote = new Tick(
                this.audusd,
                Price.Create(0.80001m, 0.00001m),
                Price.Create(0.80006m, 0.00001m),
                StubDateTime.Now());

            this.quoteProvider.OnQuote(quote);

            // Act
            var result = this.quoteProvider.GetLastQuote(this.audusd);

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
                StubDateTime.Now());

            var quote2 = new Tick(
                this.eurusd,
                Price.Create(1.20000m, 0.00001m),
                Price.Create(1.20005m, 0.00001m),
                StubDateTime.Now());

            this.quoteProvider.OnQuote(quote1);
            this.quoteProvider.OnQuote(quote2);

            // Act
            var result = this.quoteProvider.GetQuoteSymbolList();

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
                StubDateTime.Now());

            this.quoteProvider.OnQuote(quote);

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
                StubDateTime.Now());

            this.quoteProvider.OnQuote(quote);

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
                StubDateTime.Now());

            var quote2 = new Tick(
                this.eurusd,
                Price.Create(1.20000m, 0.00001m),
                Price.Create(1.20005m, 0.00001m),
                StubDateTime.Now());

            this.quoteProvider.OnQuote(quote1);
            this.quoteProvider.OnQuote(quote2);

            // Act
            var result = this.quoteProvider.GetExchangeRate(CurrencyCode.AUD, CurrencyCode.EUR);

            // Assert
            Assert.Equal(0.80000m / 1.20000m, result);
        }
    }
}