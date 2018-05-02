//--------------------------------------------------------------------------------------------------
// <copyright file="FxStreetCurrencyRegistry.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Nautilus.Database.FxStreet
{
    using Nautilus.DomainModel.Enums;

    /// <summary>
    /// Provides a registry of country strings mapped to currency codes.
    /// </summary>
    internal static class FxStreetCurrencyRegistry
    {
        private static readonly Dictionary<string, CurrencyCode> CountryToCurrency =
            new Dictionary<string, CurrencyCode>
                {
                    { "New Zealand", CurrencyCode.NZD },
                    { "Australia", CurrencyCode.AUD },
                    { "China", CurrencyCode.CNY },
                    { "Japan", CurrencyCode.JPY },
                    { "Switzerland", CurrencyCode.CHF },
                    { "France", CurrencyCode.EUR },
                    { "Spain", CurrencyCode.EUR },
                    { "Italy", CurrencyCode.EUR },
                    { "Portugal", CurrencyCode.EUR },
                    { "Germany", CurrencyCode.EUR },
                    { "Greece", CurrencyCode.EUR },
                    { "United Kingdom", CurrencyCode.GBP },
                    { "European Monetary Union", CurrencyCode.EUR },
                    { "Canada", CurrencyCode.CAD },
                    { "United States", CurrencyCode.USD }
                };

        internal static CurrencyCode ForCountry(string country)
        {
            return CountryToCurrency[country];
        }
    }
}
