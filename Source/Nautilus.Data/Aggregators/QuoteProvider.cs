//--------------------------------------------------------------------------------------------------
// <copyright file="QuoteProvider.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Aggregators
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Provides quotes for a particular exchange.
    /// </summary>
    public sealed class QuoteProvider : IQuoteProvider
    {
        private readonly IDictionary<Symbol, Tick> ticks;
        private readonly IDictionary<string, Symbol> symbolCodes;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuoteProvider"/> class.
        /// </summary>
        /// <param name="venue">The quote providers venue.</param>
        public QuoteProvider(Venue venue)
        {
            this.Venue = venue;
            this.ticks = new ConcurrentDictionary<Symbol, Tick>();
            this.symbolCodes = new ConcurrentDictionary<string, Symbol>();
        }

        /// <summary>
        /// Gets the quote providers exchange.
        /// </summary>
        public Venue Venue { get; }

        /// <summary>
        /// Updates the quote provider with the given quote.
        /// </summary>
        /// <param name="tick">The tick.</param>
        public void Update(Tick tick)
        {
            Debug.EqualTo(tick.Symbol.Venue, this.Venue, nameof(tick.Symbol));

            if (!this.ticks.ContainsKey(tick.Symbol))
            {
                this.ticks.Add(tick.Symbol, tick);
                this.symbolCodes.Add(tick.Symbol.Code, tick.Symbol);

                return;
            }

            this.ticks[tick.Symbol] = tick;
        }

        /// <summary>
        /// Returns the last quote corresponding to the given symbol (optional value).
        /// </summary>
        /// <param name="symbol">The quote symbol.</param>
        /// <returns>A <see cref="Tick"/>.</returns>
        public OptionRef<Tick> GetLastTick(Symbol symbol)
        {
            return this.ticks.ContainsKey(symbol)
                 ? this.ticks[symbol]
                 : OptionRef<Tick>.None();
        }

        /// <summary>
        /// Returns a collection of all quote symbols held by the quote provider.
        /// </summary>
        /// <returns>A read only collection.</returns>
        public IImmutableList<Symbol> GetSymbolList()
        {
            return this.ticks.Keys.ToImmutableList();
        }

        /// <summary>
        /// Returns the exchange rate of the given currencies (optional value).
        /// </summary>
        /// <param name="accountCurrency">The account currency.</param>
        /// <param name="quoteCurrency"> The quote currency.</param>
        /// <returns>A <see cref="OptionRef{T}"/>.</returns>
        public OptionVal<decimal> GetExchangeRate(
            CurrencyCode accountCurrency,
            CurrencyCode quoteCurrency)
        {
            if (accountCurrency.Equals(quoteCurrency))
            {
                return 1;
            }

            var quoteCurrencyString = quoteCurrency.ToString();
            var accountCurrencyString = accountCurrency.ToString();

            var rateSymbol = quoteCurrencyString + accountCurrencyString;
            var rateSymbolSwapped = accountCurrencyString + quoteCurrencyString;

            var exchangeRate = this.symbolCodes.ContainsKey(rateSymbol)
                               ? 1 / this.GetLastBid(rateSymbol)
                               : (this.symbolCodes.ContainsKey(rateSymbolSwapped)
                                      ? this.GetLastBid(rateSymbolSwapped).Value
                                      : this.ExchangeRateFromUsd(accountCurrencyString, quoteCurrencyString));

            return exchangeRate > 0
                 ? exchangeRate
                 : OptionVal<decimal>.None();
        }

        private decimal ExchangeRateFromUsd(
            string accountCurrency,
            string quoteCurrency)
        {
            Debug.NotEmptyOrWhiteSpace(accountCurrency, nameof(accountCurrency));
            Debug.NotEmptyOrWhiteSpace(quoteCurrency, nameof(quoteCurrency));

            var conversionRateAccountToUsd = decimal.Zero;
            var exchangeRateFromUsd = decimal.Zero;

            if (this.symbolCodes.ContainsKey(accountCurrency + "USD"))
            {
                conversionRateAccountToUsd = this.GetLastBid(accountCurrency + "USD").Value;
            }
            else if (this.symbolCodes.ContainsKey("USD" + accountCurrency))
            {
                conversionRateAccountToUsd = 1 / this.GetLastBid(accountCurrency + "USD");
            }

            if (this.symbolCodes.ContainsKey(quoteCurrency + "USD"))
            {
                exchangeRateFromUsd = conversionRateAccountToUsd / this.GetLastBid(quoteCurrency + "USD");
            }
            else if (this.symbolCodes.ContainsKey(CurrencyCode.USD + quoteCurrency))
            {
                exchangeRateFromUsd = conversionRateAccountToUsd / (1 / this.GetLastBid("USD" + quoteCurrency));
            }

            return exchangeRateFromUsd;
        }

        private Price GetLastBid(string symbol)
        {
            Debug.NotEmptyOrWhiteSpace(symbol, nameof(symbol));

            return this.ticks[this.symbolCodes[symbol]].Bid;
        }
    }
}
