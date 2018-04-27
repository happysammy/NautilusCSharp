// -------------------------------------------------------------------------------------------------
// <copyright file="IQuoteProvider.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Interfaces
{
    using System.Collections.Generic;
    using NautechSystems.CSharp;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The <see cref="IQuoteProvider"/> interface. Provides market quotes and exchange rate 
    /// calculations.
    /// </summary>
    public interface IQuoteProvider
    {
        /// <summary>
        /// Updates the quote provider with the given quote.
        /// </summary>
        /// <param name="quote">The quote.</param>
        void OnQuote(Tick quote);

        /// <summary>
        /// Returns the last quote corresponding to the given symbol if it is contained by the
        /// quote provider (optional).
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns>\A <see cref="Option{Tick}"/>.\</returns>
        Option<Tick> GetLastQuote(Symbol symbol);

        /// <summary>
        /// Returns a collection of the symbol elements held by the quote provider.
        /// </summary>
        /// <returns>A <see cref="IReadOnlyCollection{Symbol}"/>.</returns>
        IReadOnlyCollection<Symbol> GetQuoteSymbolList();

        /// <summary>
        /// Returns the exchange rate decimal if the quote provider contains the required 
        /// information (optional value).
        /// </summary>
        /// <param name="accountCurrency">The account currency.</param>
        /// <param name="quoteCurrency">The quote currency.</param>
        /// <returns>\A <see cref="Option{Decimal}"/>.\</returns>
        Option<decimal?> GetExchangeRate(CurrencyCode accountCurrency, CurrencyCode quoteCurrency);
    }
}