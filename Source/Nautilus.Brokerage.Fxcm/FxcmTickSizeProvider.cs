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
    public static class FxcmTickSizeProvider
    {
        private static readonly Dictionary<string, int> TickSizeIndex =
                            new Dictionary<string, int>
        {
            { "AUD/CAD",  5 },
            { "AUD/CHF",  5 },
            { "AUD/JPY",  5 },
            { "AUD/NZD",  5 },
            { "AUD/USD",  5 },
            { "AUS200",   2 },
            { "Bund",     5 },
            { "CAD/CHF",  5 },
            { "CAD/JPY",  5 },
            { "CHF/JPY",  5 },
            { "CHN50",    5 },
            { "Copper",   5 },
            { "ESP35",    5 },
            { "EUR/AUD",  5 },
            { "EUR/CAD",  5 },
            { "EUR/CHF",  5 },
            { "EUR/CZK",  5 },
            { "EUR/GBP",  5 },
            { "EUR/JPY",  5 },
            { "EUR/NOK",  5 },
            { "EUR/NZD",  5 },
            { "EUR/SEK",  5 },
            { "EUR/TRY",  5 },
            { "EUR/USD",  5 },
            { "EUSTX50",  5 },
            { "FRA40",    5 },
            { "GBP/AUD",  5 },
            { "GBP/CAD",  5 },
            { "GBP/CHF",  5 },
            { "GBP/JPY",  5 },
            { "GBP/NZD",  5 },
            { "GBP/USD",  5 },
            { "GER30",    5 },
            { "HKG33",    5 },
            { "ITA40",    5 },
            { "JPN225",   5 },
            { "NAS100",   5 },
            { "NGAS",     5 },
            { "NZD/CAD",  5 },
            { "NZD/CHF",  5 },
            { "NZD/JPY",  5 },
            { "NZD/USD",  5 },
            { "SPX500",   5 },
            { "SUI20",    5 },
            { "SOYF",     5 },
            { "TRY/JPY",  5 },
            { "UK100",    5 },
            { "UKOil",    5 },
            { "US30",     5 },
            { "USD/MXN",  5 },
            { "USDOLLAR", 5 },
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
            { "USOil",    5 },
            { "XAG/USD",  5 },
            { "XAU/USD",  5 },
            { "ZAR/JPY",  5 },
        };

        public static IReadOnlyDictionary<string, int> GetIndex() => TickSizeIndex;
    }
}
