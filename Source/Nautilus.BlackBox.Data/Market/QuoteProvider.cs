//--------------------------------------------------------------------------------------------------
// <copyright file="QuoteProvider.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Data.Market
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using NautechSystems.CSharp;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The sealed <see cref="QuoteProvider"/> class. Provides quotes for the <see cref="BlackBox"/>
    /// system.
    /// </summary>
    public sealed class QuoteProvider : IQuoteProvider
    {
        private readonly Exchange exchange;
        private readonly IDictionary<string, Tick> lastQuotes = new ConcurrentDictionary<string, Tick>();

        /// <summary>
        /// Initializes a new instance of the <see cref="QuoteProvider"/> class.
        /// </summary>
        /// <param name="exchange">The exchange.</param>
        public QuoteProvider(Exchange exchange)
        {
            this.exchange = exchange;
        }

        /// <summary>
        /// Updates the quote provider with the given quote.
        /// </summary>
        /// <param name="quote">]The quote.]</param>
        /// <exception cref="ValidationException">Throws if the quote is null.</exception>
        public void OnQuote(Tick quote)
        {
            Validate.NotNull(quote, nameof(quote));

            if (!this.lastQuotes.ContainsKey(quote.Symbol.Code))
            {
                this.lastQuotes.Add(quote.Symbol.Code, quote);

                return;
            }

            this.lastQuotes[quote.Symbol.Code] = quote;
        }

        /// <summary>
        /// Returns the last quote corresponding to the given symbol (optional value).
        /// </summary>
        /// <param name="symbol">The quote symbol.</param>
        /// <returns>A <see cref="Tick"/>.</returns>
        /// <exception cref="ValidationException">Throws if the symbol is null.</exception>
        public Option<Tick> GetLastQuote(Symbol symbol)
        {
            Validate.NotNull(symbol, nameof(symbol));

            return this.lastQuotes.ContainsKey(symbol.Code)
                 ? this.lastQuotes[symbol.Code]
                 : Option<Tick>.None();
        }

        /// <summary>
        /// Returns a collection of all quote symbols held by the quote provider.
        /// </summary>
        /// <returns>A read only collection.</returns>
        public IReadOnlyCollection<Symbol> GetQuoteSymbolList()
        {
            return this.lastQuotes
               .Keys
               .Select(symbolString => new Symbol(symbolString, this.exchange))
               .ToImmutableList();
        }

        /// <summary>
        /// Returns the exchange rate of the given currencies (optional value).
        /// </summary>
        /// <param name="accountCurrency">The account currency.</param>
        /// <param name="quoteCurrency"> The quote currency.</param>
        /// <returns>A <see cref="Option{Decimal}"/>.</returns>
        /// <exception cref="ValidationException">Throws if either currency is the default value
        /// (Unknown).</exception>
        public Option<decimal?> GetExchangeRate(
            CurrencyCode accountCurrency,
            CurrencyCode quoteCurrency)
        {
            Validate.NotDefault(accountCurrency, nameof(accountCurrency));
            Validate.NotDefault(quoteCurrency, nameof(quoteCurrency));

            if (accountCurrency.Equals(quoteCurrency))
            {
                return 1;
            }

            var quoteCurrencyString = quoteCurrency.ToString();
            var accountCurrencyString = accountCurrency.ToString();

            var rateSymbol = quoteCurrencyString + accountCurrencyString;
            var rateSymbolSwapped = accountCurrencyString + quoteCurrencyString;

            var exchangeRate = this.lastQuotes.ContainsKey(rateSymbol)
                               ? 1 / this.GetLastBid(rateSymbol)
                               : (this.lastQuotes.ContainsKey(rateSymbolSwapped)
                                      ? this.GetLastBid(rateSymbolSwapped).Value
                                      : this.ExchangeRateFromUsd(accountCurrencyString, quoteCurrencyString));

            return exchangeRate > 0
                 ? exchangeRate
                 : Option<decimal?>.None();
        }

        private decimal ExchangeRateFromUsd(
            string accountCurrency,
            string quoteCurrency)
        {
            Debug.NotNull(accountCurrency, nameof(accountCurrency));
            Debug.NotNull(quoteCurrency, nameof(quoteCurrency));

            var conversionRateAccountToUsd = decimal.Zero;
            var exchangeRateFromUsd = decimal.Zero;

            if (this.lastQuotes.ContainsKey(accountCurrency + "USD"))
            {
                conversionRateAccountToUsd = this.GetLastBid(accountCurrency + "USD").Value;
            }
            else if (this.lastQuotes.ContainsKey("USD" + accountCurrency))
            {
                conversionRateAccountToUsd = 1 / this.GetLastBid(accountCurrency + "USD");
            }

            if (this.lastQuotes.ContainsKey(quoteCurrency + "USD"))
            {
                exchangeRateFromUsd = conversionRateAccountToUsd / this.GetLastBid(quoteCurrency + "USD");
            }
            else if (this.lastQuotes.ContainsKey("USD" + quoteCurrency))
            {
                exchangeRateFromUsd = conversionRateAccountToUsd / (1 / this.GetLastBid("USD" + quoteCurrency));
            }

            return exchangeRateFromUsd;
        }

        private Price GetLastBid(string symbol)
        {
            Debug.NotNull(symbol, nameof(symbol));

            return this.lastQuotes
               .FirstOrDefault(q => q.Key == symbol)
               .Value.Bid;
        }
    }
}
