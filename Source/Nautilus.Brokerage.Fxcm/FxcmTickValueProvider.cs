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
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.CQS;
    using NautechSystems.CSharp.Validation;
    using Nautilus.DomainModel.Enums;

    /// <summary>
    /// The immutable static <see cref="FxcmTickValueProvider"/> class.
    /// </summary>
    [Immutable]
    public static class FxcmTickValueProvider
    {
        private static readonly Dictionary<string, Tuple<decimal, CurrencyCode>> TickValueIndex =
                            new Dictionary<string, Tuple<decimal, CurrencyCode>>
        {
            { "AUD/CAD",  Tuple.Create(1m, CurrencyCode.CAD) },
            { "AUD/CHF",  Tuple.Create(1m, CurrencyCode.CHF) },
            { "AUD/JPY",  Tuple.Create(1m, CurrencyCode.JPY) },
            { "AUD/NZD",  Tuple.Create(1m, CurrencyCode.NZD) },
            { "AUD/USD",  Tuple.Create(1m, CurrencyCode.USD) },
            { "AUS200",   Tuple.Create(1m, CurrencyCode.AUD) },
            { "Bund",     Tuple.Create(1m, CurrencyCode.EUR) },
            { "CAD/CHF",  Tuple.Create(1m, CurrencyCode.CHF) },
            { "CAD/JPY",  Tuple.Create(1m, CurrencyCode.JPY) },
            { "CHF/JPY",  Tuple.Create(1m, CurrencyCode.JPY) },
            { "Copper",   Tuple.Create(1m, CurrencyCode.USD) },
            { "ESP35",    Tuple.Create(1m, CurrencyCode.USD) },
            { "EUR/AUD",  Tuple.Create(1m, CurrencyCode.AUD) },
            { "EUR/CAD",  Tuple.Create(1m, CurrencyCode.CAD) },
            { "EUR/CHF",  Tuple.Create(1m, CurrencyCode.CHF) },
            { "EUR/CZK",  Tuple.Create(1m, CurrencyCode.CZK) },
            { "EUR/GBP",  Tuple.Create(1m, CurrencyCode.GBP) },
            { "EUR/JPY",  Tuple.Create(1m, CurrencyCode.JPY) },
            { "EUR/NOK",  Tuple.Create(1m, CurrencyCode.NOK) },
            { "EUR/NZD",  Tuple.Create(1m, CurrencyCode.NZD) },
            { "EUR/SEK",  Tuple.Create(1m, CurrencyCode.SEK) },
            { "EUR/TRY",  Tuple.Create(1m, CurrencyCode.TRY) },
            { "EUR/USD",  Tuple.Create(1m, CurrencyCode.USD) },
            { "EUSTX50",  Tuple.Create(1m, CurrencyCode.EUR) },
            { "FRA40",    Tuple.Create(1m, CurrencyCode.EUR) },
            { "GBP/AUD",  Tuple.Create(1m, CurrencyCode.AUD) },
            { "GBP/CAD",  Tuple.Create(1m, CurrencyCode.CAD) },
            { "GBP/CHF",  Tuple.Create(1m, CurrencyCode.CHF) },
            { "GBP/JPY",  Tuple.Create(1m, CurrencyCode.JPY) },
            { "GBP/NZD",  Tuple.Create(1m, CurrencyCode.NZD) },
            { "GBP/USD",  Tuple.Create(1m, CurrencyCode.USD) },
            { "GER30",    Tuple.Create(1m, CurrencyCode.EUR) },
            { "HKG33",    Tuple.Create(1m, CurrencyCode.HKD) },
            { "ITA40",    Tuple.Create(1m, CurrencyCode.EUR) },
            { "JPN225",   Tuple.Create(1m, CurrencyCode.JPY) },
            { "NAS100",   Tuple.Create(1m, CurrencyCode.USD) },
            { "NGAS",     Tuple.Create(1m, CurrencyCode.USD) },
            { "NZD/CAD",  Tuple.Create(1m, CurrencyCode.CAD) },
            { "NZD/CHF",  Tuple.Create(1m, CurrencyCode.CHF) },
            { "NZD/JPY",  Tuple.Create(1m, CurrencyCode.JPY) },
            { "NZD/USD",  Tuple.Create(1m, CurrencyCode.USD) },
            { "SPX500",   Tuple.Create(1m, CurrencyCode.USD) },
            { "SUI20",    Tuple.Create(1m, CurrencyCode.EUR) },
            { "TRY/JPY",  Tuple.Create(1m, CurrencyCode.JPY) },
            { "UK100",    Tuple.Create(1m, CurrencyCode.GBP) },
            { "UKOil",    Tuple.Create(1m, CurrencyCode.USD) },
            { "US30",     Tuple.Create(1m, CurrencyCode.USD) },
            { "USD/MXN",  Tuple.Create(1m, CurrencyCode.MXN) },
            { "USDOLLAR", Tuple.Create(1m, CurrencyCode.USD) },
            { "USD/CAD",  Tuple.Create(1m, CurrencyCode.CAD) },
            { "USD/CHF",  Tuple.Create(1m, CurrencyCode.CHF) },
            { "USD/CNH",  Tuple.Create(1m, CurrencyCode.CNH) },
            { "USD/CZK",  Tuple.Create(1m, CurrencyCode.CZK) },
            { "USD/HKD",  Tuple.Create(1m, CurrencyCode.HKD) },
            { "USD/JPY",  Tuple.Create(1m, CurrencyCode.JPY) },
            { "USD/NOK",  Tuple.Create(1m, CurrencyCode.NOK) },
            { "USD/SEK",  Tuple.Create(1m, CurrencyCode.SEK) },
            { "USD/TRY",  Tuple.Create(1m, CurrencyCode.TRY) },
            { "USD/ZAR",  Tuple.Create(1m, CurrencyCode.ZAR) },
            { "USOil",    Tuple.Create(1m, CurrencyCode.USD) },
            { "XAG/USD",  Tuple.Create(5m, CurrencyCode.USD) },
            { "XAU/USD",  Tuple.Create(0.1m, CurrencyCode.USD) },
            { "XPD/USD",  Tuple.Create(1m, CurrencyCode.JPY) },
            { "XPT/USD",  Tuple.Create(1m, CurrencyCode.JPY) },
            { "ZAR/JPY",  Tuple.Create(1m, CurrencyCode.JPY) },
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
                 ? QueryResult<decimal>.Ok(TickValueIndex[fxcmSymbol].Item1)
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
                 ? QueryResult<CurrencyCode>.Ok(TickValueIndex[fxcmSymbol].Item2)
                 : QueryResult<CurrencyCode>.Fail($"Cannot find the currency code for {fxcmSymbol}");
        }
    }
}
