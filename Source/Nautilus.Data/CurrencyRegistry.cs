//--------------------------------------------------------------------------------------------------
// <copyright file="CurrencyRegistry.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data
{
    using System.Collections.Generic;
    using Nautilus.DomainModel.Enums;

    /// <summary>
    /// Provides a registry of country strings mapped to currency codes.
    /// </summary>
    internal static class CurrencyRegistry
    {
        private static readonly Dictionary<string, Currency> CountryToCurrency =
            new Dictionary<string, Currency>
                {
                    { "New Zealand", Currency.NZD },
                    { "Australia", Currency.AUD },
                    { "China", Currency.CNY },
                    { "Japan", Currency.JPY },
                    { "Switzerland", Currency.CHF },
                    { "France", Currency.EUR },
                    { "Spain", Currency.EUR },
                    { "Italy", Currency.EUR },
                    { "Portugal", Currency.EUR },
                    { "Germany", Currency.EUR },
                    { "Greece", Currency.EUR },
                    { "United Kingdom", Currency.GBP },
                    { "European Monetary Union", Currency.EUR },
                    { "Canada", Currency.CAD },
                    { "United States", Currency.USD },
                };

        /// <summary>
        /// Returns the currency code for the given country string.
        /// </summary>
        /// <param name="country">The country string.</param>
        /// <returns>The currency code.</returns>
        internal static Currency ForCountry(string country)
        {
            return CountryToCurrency[country];
        }
    }
}
