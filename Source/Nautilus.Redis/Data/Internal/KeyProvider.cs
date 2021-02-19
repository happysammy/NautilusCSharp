// -------------------------------------------------------------------------------------------------
// <copyright file="KeyProvider.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Nautilus.DomainModel.Enums;
using Nautilus.DomainModel.Identifiers;
using Nautilus.DomainModel.ValueObjects;
using StackExchange.Redis;

namespace Nautilus.Redis.Data.Internal
{
    /// <summary>
    /// Provides database keys for market data.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Allows nameof.")]
    public static class KeyProvider
    {
        private const string NautilusData = nameof(NautilusData) + ":" + nameof(Nautilus.Data);
        private const string Prices = nameof(Prices);
        private const string Sizes = nameof(Sizes);
        private const string Bars = nameof(Bars);
        private const string Instruments = nameof(Instruments);

        private static readonly string PricesNamespace = $"{NautilusData}:{Prices}";
        private static readonly string SizesNamespace = $"{NautilusData}:{Sizes}";
        private static readonly string BarsNamespace = $"{NautilusData}:{Bars}";
        private static readonly string InstrumentsNamespace = $"{NautilusData}:{Instruments}";

        /// <summary>
        /// Returns a tick prices key from the given arguments.
        /// </summary>
        /// <param name="symbol">The symbol for the key.</param>
        /// <param name="priceType">The price type for the key.</param>
        /// <returns>A key string.</returns>
        internal static string GetPricesKey(Symbol symbol, PriceType priceType)
        {
            return $"{PricesNamespace}:{symbol.Venue.Value}:{symbol.Code}:{priceType.ToString()}";
        }

        /// <summary>
        /// Returns a tick volumes key from the given arguments.
        /// </summary>
        /// <param name="symbol">The symbol for the key.</param>
        /// <param name="priceType">The price type for the key.</param>
        /// <returns>A key string.</returns>
        internal static string GetSizesKey(Symbol symbol, PriceType priceType)
        {
            return $"{SizesNamespace}:{symbol.Venue.Value}:{symbol.Code}:{priceType.ToString()}";
        }

        /// <summary>
        /// Returns a opens key from the given arguments.
        /// </summary>
        /// <param name="barType">The bar type for the key.</param>
        /// <returns>A key string.</returns>
        internal static string GetBarOpensKey(BarType barType)
        {
            return GetBarOpensKey(
                barType.Symbol,
                barType.Specification.BarStructure,
                barType.Specification.PriceType);
        }

        // /// <summary>
        // /// Returns a highs key from the given arguments.
        // /// </summary>
        // /// <param name="barType">The bar type for the key.</param>
        // /// <returns>A key string.</returns>
        // internal static string GetBarHighsKey(BarType barType)
        // {
        //     return GetBarHighsKey(
        //         barType.Symbol,
        //         barType.Specification.BarStructure,
        //         barType.Specification.PriceType);
        // }
        //
        // /// <summary>
        // /// Returns a lows key from the given arguments.
        // /// </summary>
        // /// <param name="barType">The bar type for the key.</param>
        // /// <returns>A key string.</returns>
        // internal static string GetBarLowsKey(BarType barType)
        // {
        //     return GetBarLowsKey(
        //         barType.Symbol,
        //         barType.Specification.BarStructure,
        //         barType.Specification.PriceType);
        // }
        //
        // /// <summary>
        // /// Returns a closes key from the given arguments.
        // /// </summary>
        // /// <param name="barType">The bar type for the key.</param>
        // /// <returns>A key string.</returns>
        // internal static string GetBarClosesKey(BarType barType)
        // {
        //     return GetBarClosesKey(
        //         barType.Symbol,
        //         barType.Specification.BarStructure,
        //         barType.Specification.PriceType);
        // }
        //
        // /// <summary>
        // /// Returns a volumes key from the given arguments.
        // /// </summary>
        // /// <param name="barType">The bar type for the key.</param>
        // /// <returns>A key string.</returns>
        // internal static string GetBarVolumesKey(BarType barType)
        // {
        //     return GetBarVolumesKey(
        //         barType.Symbol,
        //         barType.Specification.BarStructure,
        //         barType.Specification.PriceType);
        // }

        /// <summary>
        /// Returns a opens key from the given arguments.
        /// </summary>
        /// <param name="symbol">The symbol for the key.</param>
        /// <param name="barStructure">The bar structure for the key.</param>
        /// <param name="priceType">The price type for the key.</param>
        /// <returns>A key string.</returns>
        internal static string GetBarOpensKey(Symbol symbol, BarStructure barStructure, PriceType priceType)
        {
            return $"{BarsNamespace}:{symbol.Venue.Value}:{symbol.Code}:{barStructure.ToString()}:{priceType.ToString()}:Opens";
        }

        /// <summary>
        /// Returns a highs key from the given arguments.
        /// </summary>
        /// <param name="symbol">The symbol for the key.</param>
        /// <param name="barStructure">The bar structure for the key.</param>
        /// <param name="priceType">The price type for the key.</param>
        /// <returns>A key string.</returns>
        internal static string GetBarHighsKey(Symbol symbol, BarStructure barStructure, PriceType priceType)
        {
            return $"{BarsNamespace}:{symbol.Venue.Value}:{symbol.Code}:{barStructure.ToString()}:{priceType.ToString()}:Highs";
        }

        /// <summary>
        /// Returns a highs key from the given arguments.
        /// </summary>
        /// <param name="symbol">The symbol for the key.</param>
        /// <param name="barStructure">The bar structure for the key.</param>
        /// <param name="priceType">The price type for the key.</param>
        /// <returns>A key string.</returns>
        internal static string GetBarLowsKey(Symbol symbol, BarStructure barStructure, PriceType priceType)
        {
            return $"{BarsNamespace}:{symbol.Venue.Value}:{symbol.Code}:{barStructure.ToString()}:{priceType.ToString()}:Lows";
        }

        /// <summary>
        /// Returns a closes key from the given arguments.
        /// </summary>
        /// <param name="symbol">The symbol for the key.</param>
        /// <param name="barStructure">The bar structure for the key.</param>
        /// <param name="priceType">The price type for the key.</param>
        /// <returns>A key string.</returns>
        internal static string GetBarClosesKey(Symbol symbol, BarStructure barStructure, PriceType priceType)
        {
            return $"{BarsNamespace}:{symbol.Venue.Value}:{symbol.Code}:{barStructure.ToString()}:{priceType.ToString()}:Closes";
        }

        /// <summary>
        /// Returns a bar volumes key from the given arguments.
        /// </summary>
        /// <param name="symbol">The symbol for the key.</param>
        /// <param name="barStructure">The bar structure for the key.</param>
        /// <param name="priceType">The price type for the key.</param>
        /// <returns>A key string.</returns>
        internal static string GetBarVolumesKey(Symbol symbol, BarStructure barStructure, PriceType priceType)
        {
            return $"{BarsNamespace}:{symbol.Venue.Value}:{symbol.Code}:{barStructure.ToString()}:{priceType.ToString()}:Volumes";
        }

        /// <summary>
        /// Returns a wildcard key string for all instruments.
        /// </summary>
        /// <returns>The key <see cref="string"/>.</returns>
        internal static RedisValue GetInstrumentsPattern()
        {
            return InstrumentsNamespace + "*";
        }

        /// <summary>
        /// Returns the instruments key.
        /// </summary>
        /// <param name="symbol">The instruments symbol.</param>
        /// <returns>The key <see cref="string"/>.</returns>
        internal static RedisKey GetInstrumentsKey(Symbol symbol)
        {
            return InstrumentsNamespace + ":" + symbol.Value;
        }
    }
}
