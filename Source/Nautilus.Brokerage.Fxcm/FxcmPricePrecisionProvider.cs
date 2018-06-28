//---------------------------------------------------------------------------------------------------------------------
// <copyright file="FxcmTickValueProvider.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//---------------------------------------------------------------------------------------------------------------------

namespace Nautilus.Brokerage.FXCM
{
    using System.Collections.Generic;
    using Nautilus.Core.Annotations;

    /// <summary>
    /// The immutable static <see cref="FxcmTickValueProvider"/> class.
    /// </summary>
    [Immutable]
    public static class FxcmPricePrecisionProvider
    {
        private static readonly Dictionary<string, int> TickSizeIndex =
                            new Dictionary<string, int>
        {
            { "AUD/CAD",  5 },
            { "AUD/CHF",  5 },
            { "AUD/JPY",  3 },
            { "AUD/NZD",  5 },
            { "AUD/USD",  5 },
            { "AUS200",   2 },
            { "Bund",     3 },
            { "CAD/CHF",  5 },
            { "CAD/JPY",  3 },
            { "CHF/JPY",  3 },
            { "CHN50",    2 },
            { "Copper",   4 },
            { "ESP35",    2 },
            { "EUR/AUD",  5 },
            { "EUR/CAD",  5 },
            { "EUR/CHF",  5 },
            { "EUR/CZK",  5 },
            { "EUR/GBP",  5 },
            { "EUR/JPY",  3 },
            { "EUR/NOK",  5 },
            { "EUR/NZD",  5 },
            { "EUR/SEK",  5 },
            { "EUR/TRY",  5 },
            { "EUR/USD",  5 },
            { "EUSTX50",  2 },
            { "FRA40",    2 },
            { "GBP/AUD",  5 },
            { "GBP/CAD",  5 },
            { "GBP/CHF",  5 },
            { "GBP/JPY",  3 },
            { "GBP/NZD",  5 },
            { "GBP/USD",  5 },
            { "GER30",    2 },
            { "HKG33",    2 },
            { "ITA40",    5 },
            { "JPN225",   2 },
            { "NAS100",   2 },
            { "NGAS",     4 },
            { "NZD/CAD",  5 },
            { "NZD/CHF",  5 },
            { "NZD/JPY",  3 },
            { "NZD/USD",  5 },
            { "SPX500",   2 },
            { "SUI20",    5 },
            { "SOYF",     2 },
            { "TRY/JPY",  3 },
            { "UK100",    2 },
            { "UKOil",    3 },
            { "US30",     2 },
            { "USD/MXN",  5 },
            { "USDOLLAR", 0 },
            { "USD/CAD",  5 },
            { "USD/CHF",  5 },
            { "USD/CNH",  5 },
            { "USD/CZK",  5 },
            { "USD/HKD",  5 },
            { "USD/JPY",  3 },
            { "USD/NOK",  5 },
            { "USD/SEK",  5 },
            { "USD/TRY",  5 },
            { "USD/ZAR",  5 },
            { "USOil",    3 },
            { "XAG/USD",  3 },
            { "XAU/USD",  2 },
            { "ZAR/JPY",  3 },
        };

        public static IReadOnlyDictionary<string, int> GetIndex() => TickSizeIndex;
    }
}
