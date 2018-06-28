//---------------------------------------------------------------------------------------------------------------------
// <copyright file="FxcmTargetDirectSpreadProvider.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//---------------------------------------------------------------------------------------------------------------------

namespace Nautilus.Brokerage.FXCM
{
    using System.Collections.Generic;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.CQS;
    using Nautilus.Core.Validation;

    /// <summary>
    /// The immutable static <see cref="FxcmTargetDirectSpreadProvider"/> class.
    /// </summary>
    [Immutable]
    public static class FxcmTargetDirectSpreadProvider
    {
        private static readonly Dictionary<string, int> TargetDirectSpreadIndex =
                            new Dictionary<string, int>
        {
            { "AUD/CAD",  5 },
            { "AUD/CHF",  5 },
            { "AUD/JPY",  5 },
            { "AUD/NZD",  5 },
            { "AUD/USD",  5 },
            { "AUS200",   2 },
            { "Bund",     2 },
            { "CAD/CHF",  5 },
            { "CAD/JPY",  5 },
            { "CHF/JPY",  5 },
            { "CHN50",    5 },
            { "Copper",   2 },
            { "ESP35",    8 },
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
            { "EUSTX50",  1 },
            { "FRA40",    2 },
            { "GBP/AUD",  5 },
            { "GBP/CAD",  5 },
            { "GBP/CHF",  5 },
            { "GBP/JPY",  5 },
            { "GBP/NZD",  5 },
            { "GBP/USD",  5 },
            { "GER30",    2 },
            { "HKG33",    15 },
            { "ITA40",    5 },
            { "JPN225",   10 },
            { "NAS100",   1 },
            { "NGAS",     10 },
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
            { "US30",     2 },
            { "USD/MXN",  5 },
            { "USDOLLAR", 5 },
            { "USD/CAD",  5 },
            { "USD/CHF",  5 },
            { "USD/CNH",  5 },
            { "USD/CZK",  5 },
            { "USD/HKD",  5 },
            { "USD/JPY",  5 },
            { "USD/NOK",  5 },
            { "USD/SEK",  5 },
            { "USD/TRY",  5 },
            { "USD/ZAR",  5 },
            { "USOil",    5 },
            { "XAG/USD",  5 },
            { "XAU/USD", 50 },
            { "XPD/USD",  5 },
            { "XPT/USD",  5 },
            { "ZAR/JPY",  5 }
        };

        /// <summary>
        /// Returns the target direct spread of the given symbol if its contained in the index.
        /// </summary>
        /// <param name="fxcmSymbol">The FXCM symbol.</param>
        /// <returns>An <see cref="int"/>.</returns>
        public static QueryResult<int> GetTargetDirectSpread(string fxcmSymbol)
        {
            Validate.NotNull(fxcmSymbol, nameof(fxcmSymbol));

            return TargetDirectSpreadIndex.ContainsKey(fxcmSymbol)
                 ? QueryResult<int>.Ok(TargetDirectSpreadIndex[fxcmSymbol])
                 : QueryResult<int>.Fail($"Cannot find the target direct spread for {fxcmSymbol}");
        }
    }
}
