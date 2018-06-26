//---------------------------------------------------------------------------------------------------------------------
// <copyright file="FxcmTickValueProvider.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//---------------------------------------------------------------------------------------------------------------------

namespace Nautilus.Brokerage.FXCM
{
    using System;
    using System.Collections.Generic;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.CQS;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Enums;

    /// <summary>
    /// The immutable static <see cref="FxcmTickValueProvider"/> class.
    /// </summary>
    [Immutable]
    public static class FxcmTickValueProvider
    {
        private static readonly Dictionary<string, (decimal Value, CurrencyCode Currency)> TickValueIndex =
                            new Dictionary<string, (decimal Value, CurrencyCode Currency)>
        {
            { "AUD/CAD",  (1m, CurrencyCode.CAD) },
            { "AUD/CHF",  (1m, CurrencyCode.CHF) },
            { "AUD/JPY",  (1m, CurrencyCode.JPY) },
            { "AUD/NZD",  (1m, CurrencyCode.NZD) },
            { "AUD/USD",  (1m, CurrencyCode.USD) },
            { "AUS200",   (1m, CurrencyCode.AUD) },
            { "Bund",     (1m, CurrencyCode.EUR) },
            { "CAD/CHF",  (1m, CurrencyCode.CHF) },
            { "CAD/JPY",  (1m, CurrencyCode.JPY) },
            { "CHF/JPY",  (1m, CurrencyCode.JPY) },
            { "Copper",   (1m, CurrencyCode.USD) },
            { "ESP35",    (1m, CurrencyCode.USD) },
            { "EUR/AUD",  (1m, CurrencyCode.AUD) },
            { "EUR/CAD",  (1m, CurrencyCode.CAD) },
            { "EUR/CHF",  (1m, CurrencyCode.CHF) },
            { "EUR/CZK",  (1m, CurrencyCode.CZK) },
            { "EUR/GBP",  (1m, CurrencyCode.GBP) },
            { "EUR/JPY",  (1m, CurrencyCode.JPY) },
            { "EUR/NOK",  (1m, CurrencyCode.NOK) },
            { "EUR/NZD",  (1m, CurrencyCode.NZD) },
            { "EUR/SEK",  (1m, CurrencyCode.SEK) },
            { "EUR/TRY",  (1m, CurrencyCode.TRY) },
            { "EUR/USD",  (1m, CurrencyCode.USD) },
            { "EUSTX50",  (1m, CurrencyCode.EUR) },
            { "FRA40",    (1m, CurrencyCode.EUR) },
            { "GBP/AUD",  (1m, CurrencyCode.AUD) },
            { "GBP/CAD",  (1m, CurrencyCode.CAD) },
            { "GBP/CHF",  (1m, CurrencyCode.CHF) },
            { "GBP/JPY",  (1m, CurrencyCode.JPY) },
            { "GBP/NZD",  (1m, CurrencyCode.NZD) },
            { "GBP/USD",  (1m, CurrencyCode.USD) },
            { "GER30",    (1m, CurrencyCode.EUR) },
            { "HKG33",    (1m, CurrencyCode.HKD) },
            { "ITA40",    (1m, CurrencyCode.EUR) },
            { "JPN225",   (1m, CurrencyCode.JPY) },
            { "NAS100",   (1m, CurrencyCode.USD) },
            { "NGAS",     (1m, CurrencyCode.USD) },
            { "NZD/CAD",  (1m, CurrencyCode.CAD) },
            { "NZD/CHF",  (1m, CurrencyCode.CHF) },
            { "NZD/JPY",  (1m, CurrencyCode.JPY) },
            { "NZD/USD",  (1m, CurrencyCode.USD) },
            { "SPX500",   (1m, CurrencyCode.USD) },
            { "SUI20",    (1m, CurrencyCode.EUR) },
            { "TRY/JPY",  (1m, CurrencyCode.JPY) },
            { "UK100",    (1m, CurrencyCode.GBP) },
            { "UKOil",    (1m, CurrencyCode.USD) },
            { "US30",     (1m, CurrencyCode.USD) },
            { "USD/MXN",  (1m, CurrencyCode.MXN) },
            { "USDOLLAR", (1m, CurrencyCode.USD) },
            { "USD/CAD",  (1m, CurrencyCode.CAD) },
            { "USD/CHF",  (1m, CurrencyCode.CHF) },
            { "USD/CNH",  (1m, CurrencyCode.CNH) },
            { "USD/CZK",  (1m, CurrencyCode.CZK) },
            { "USD/HKD",  (1m, CurrencyCode.HKD) },
            { "USD/JPY",  (1m, CurrencyCode.JPY) },
            { "USD/NOK",  (1m, CurrencyCode.NOK) },
            { "USD/SEK",  (1m, CurrencyCode.SEK) },
            { "USD/TRY",  (1m, CurrencyCode.TRY) },
            { "USD/ZAR",  (1m, CurrencyCode.ZAR) },
            { "USOil",    (1m, CurrencyCode.USD) },
            { "XAG/USD",  (5m, CurrencyCode.USD) },
            { "XAU/USD",  (0.1m, CurrencyCode.USD) },
            { "XPD/USD",  (1m, CurrencyCode.USD) },
            { "XPT/USD",  (1m, CurrencyCode.USD) },
            { "ZAR/JPY",  (1m, CurrencyCode.JPY) },
        };

        /// <summary>
        /// Returns the <see cref="Decimal"/> tick value of the given symbol if its contained in the
        /// index.
        /// </summary>
        /// <param name="fxcmSymbol">The FXCM symbol.</param>
        /// <returns> A <see cref="decimal"/>.</returns>
        /// <exception cref="ValidationException">Throws if the argument is null.</exception>
        public static QueryResult<decimal> GetTickValue(string fxcmSymbol)
        {
            Validate.NotNull(fxcmSymbol, nameof(fxcmSymbol));

            return TickValueIndex.ContainsKey(fxcmSymbol)
                 ? QueryResult<decimal>.Ok(TickValueIndex[fxcmSymbol].Value)
                 : QueryResult<decimal>.Fail($"Cannot find tick value for {fxcmSymbol}");
        }

        /// <summary>
        /// Returns the <see cref="CurrencyCode"/> of the symbol if its contained in the index.
        /// </summary>
        /// <param name="fxcmSymbol">The FXCM symbol.</param>
        /// <returns>A <see cref="CurrencyCode"/>.</returns>
        /// <exception cref="ValidationException">Throws if the argument is null.</exception>
        public static QueryResult<CurrencyCode> GetQuoteCurrency(string fxcmSymbol)
        {
            Validate.NotNull(fxcmSymbol, nameof(fxcmSymbol));

            return TickValueIndex.ContainsKey(fxcmSymbol)
                 ? QueryResult<CurrencyCode>.Ok(TickValueIndex[fxcmSymbol].Currency)
                 : QueryResult<CurrencyCode>.Fail($"Cannot find the currency code for {fxcmSymbol}");
        }
    }
}
