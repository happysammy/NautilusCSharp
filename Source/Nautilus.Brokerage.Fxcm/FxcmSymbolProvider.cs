//---------------------------------------------------------------------------------------------------------------------
// <copyright file="FxcmSymbolProvider.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//---------------------------------------------------------------------------------------------------------------------

namespace Nautilus.Brokerage.FXCM
{
    using System.Collections.Generic;
    using System.Linq;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.CQS;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Maps a given symbol string to the brokers symbol.
    /// </summary>
    [Immutable]
    public static class FxcmSymbolProvider
    {
        private static readonly Dictionary<string, string> Symbols = new Dictionary<string, string>
        {
            { "AUD/CAD",  "AUDCAD" },
            { "AUD/CHF",  "AUDCHF" },
            { "AUD/JPY",  "AUDJPY" },
            { "AUD/NZD",  "AUDNZD" },
            { "AUD/USD",  "AUDUSD" },
            { "AUS200",   "AUS200" },
            { "Bund",     "DE10YB" },
            { "CAD/CHF",  "CADCHF" },
            { "CAD/JPY",  "CADJPY" },
            { "CHF/JPY",  "CHFJPY" },
            { "CHN50",    "CHN50" },
            { "COPPER",   "COPPER" },
            { "ESP35",    "ESP35" },
            { "EUR/AUD",  "EURAUD" },
            { "EUR/CAD",  "EURCAD" },
            { "EUR/CHF",  "EURCHF" },
            { "EUR/GBP",  "EURGBP" },
            { "EUR/JPY",  "EURJPY" },
            { "EUR/NOK",  "EURNOK" },
            { "EUR/NZD",  "EURNZD" },
            { "EUR/SEK",  "EURSEK" },
            { "EUR/TRY",  "EURTRY" },
            { "EUR/USD",  "EURUSD" },
            { "EUSTX50",  "EUSTX50" },
            { "FRA40",    "FRA40" },
            { "GBP/AUD",  "GBPAUD" },
            { "GBP/CAD",  "GBPCAD" },
            { "GBP/CHF",  "GBPCHF" },
            { "GBP/JPY",  "GBPJPY" },
            { "GBP/NZD",  "GBPNZD" },
            { "GBP/USD",  "GBPUSD" },
            { "GER30",    "DE30EUR" },
            { "HKG33",    "HKG33" },
            { "JPN225",   "JPN225" },
            { "NAS100",   "NAS100" },
            { "NGAS",     "NATGAS" },
            { "NZD/CAD",  "NZDCAD" },
            { "NZD/CHF",  "NZDCHF" },
            { "NZD/JPY",  "NZDJPY" },
            { "NZD/USD",  "NZDUSD" },
            { "SPX500",   "SPX500" },
            { "SOYF",     "SOYF" },
            { "TRY/JPY",  "TRYJPY" },
            { "UK100",    "UK100" },
            { "UKOil",    "BCOUSD" },
            { "US30",     "US30USD" },
            { "USD/MXN",  "USDMXN" },
            { "USDOLLAR", "DXYUSD" },
            { "USD/CAD",  "USDCAD" },
            { "USD/CHF",  "USDCHF" },
            { "USD/CNH",  "USDCNH" },
            { "USD/HKD",  "USDHKD" },
            { "USD/JPY",  "USDJPY" },
            { "USD/NOK",  "USDNOK" },
            { "USD/SEK",  "USDSEK" },
            { "USD/TRY",  "USDTRY" },
            { "USD/ZAR",  "USDZAR" },
            { "USOil",    "WTIUSD" },
            { "XAG/USD",  "XAGUSD" },
            { "XAU/USD",  "XAUUSD" },
            { "ZAR/JPY",  "ZARJPY" },
        };

        /// <summary>
        /// Returns the Nautilus symbol <see cref="string"/> from the given FXCM symbol.
        /// </summary>
        /// <param name="fxcmSymbol">The FXCM symbol.</param>
        /// <returns>The <see cref="Nautilus"/> symbol.</returns>
        public static QueryResult<string> GetNautilusSymbol(string fxcmSymbol)
        {
            Debug.NotNull(fxcmSymbol, nameof(fxcmSymbol));

            return Symbols.ContainsKey(fxcmSymbol)
                 ? QueryResult<string>.Ok(Symbols[fxcmSymbol])
                 : QueryResult<string>.Fail($"Cannot find the Nautilus symbol from the given broker symbol {fxcmSymbol}");
        }

        /// <summary>
        /// Returns the FXCM symbol <see cref="string"/> from the given Nautilus symbol.
        /// </summary>
        /// <param name="nautilusSymbol">The Nautilus symbol.</param>
        /// <returns>The FXCM symbol.</returns>
        public static QueryResult<string> GetBrokerSymbol(string nautilusSymbol)
        {
            Debug.NotNull(nautilusSymbol, nameof(nautilusSymbol));

            return Symbols.ContainsValue(nautilusSymbol)
                 ? QueryResult<string>.Ok(Symbols.FirstOrDefault(x => x.Value == nautilusSymbol).Key)
                 : QueryResult<string>.Fail($"Cannot find the broker symbol from the given Nautilus symbol {nautilusSymbol}");
        }

        /// <summary>
        /// Returns a read only list of all Nautilus symbols.
        /// </summary>
        /// <returns>The list of broker symbols.</returns>
        public static IReadOnlyList<Symbol> GetAllSymbols()
        {
            var symbols = new List<Symbol>();

            foreach (var symbol in Symbols.Values)
            {
                symbols.Add(new Symbol(symbol, Venue.FXCM));
            }

            return symbols;
        }

        /// <summary>
        /// Returns a read only list of all FXCM symbols.
        /// </summary>
        /// <returns>The list of broker symbols.</returns>
        public static IReadOnlyList<string> GetAllBrokerSymbols()
        {
            return Symbols.Keys.ToList().AsReadOnly();
        }
    }
}
