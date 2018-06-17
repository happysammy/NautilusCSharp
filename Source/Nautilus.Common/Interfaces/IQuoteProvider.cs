//--------------------------------------------------------------------------------------------------
// <copyright file="IQuoteProvider.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using System.Collections.Immutable;
    using Nautilus.Core;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The <see cref="IQuoteProvider"/> interface. Provides market quotes and exchange rate
    /// calculations.
    /// </summary>
    public interface IQuoteProvider
    {
        /// <summary>
        /// Updates the quote provider with the given tick.
        /// </summary>
        /// <param name="tick">The quote.</param>
        void OnTick(Tick tick);

        /// <summary>
        /// Returns the last quote corresponding to the given symbol if it is contained by the
        /// quote provider (optional).
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns>\A <see cref="Option{Tick}"/>.\</returns>
        Option<Tick> GetLastTick(Symbol symbol);

        /// <summary>
        /// Returns an immutable collection of the symbols held by the quote provider.
        /// </summary>
        /// <returns>The immutable list of symbols.</returns>
        IImmutableList<Symbol> GetSymbolList();

        /// <summary>
        /// Returns the exchange rate decimal if the quote provider contains the required
        /// information (optional value).
        /// </summary>
        /// <param name="accountCurrency">The account currency.</param>
        /// <param name="quoteCurrency">The quote currency.</param>
        /// <returns>A <see cref="Option{Decimal}"/>.\</returns>
        Option<decimal?> GetExchangeRate(CurrencyCode accountCurrency, CurrencyCode quoteCurrency);
    }
}
