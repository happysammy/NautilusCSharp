//--------------------------------------------------------------------------------------------------
// <copyright file="ForexInstrument.cs" company="Nautech Systems Pty Ltd">
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
//--------------------------------------------------------------------------------------------------

using Nautilus.Core.Annotations;
using Nautilus.Core.Extensions;
using Nautilus.DomainModel.Enums;
using Nautilus.DomainModel.Identifiers;
using Nautilus.DomainModel.ValueObjects;
using NodaTime;

namespace Nautilus.DomainModel.Entities
{
    /// <summary>
    /// Represents a tradeable FOREX currency pair.
    /// </summary>
    [Immutable]
    public sealed class ForexInstrument : Instrument
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForexInstrument"/> class.
        /// </summary>
        /// <param name="symbol">The instruments symbol.</param>
        /// <param name="pricePrecision">The instruments tick decimal precision.</param>
        /// <param name="sizePrecision">The instruments quantity size precision.</param>
        /// <param name="tickSize">The instruments tick size.</param>
        /// <param name="roundLotSize">The instruments rounded lot size.</param>
        /// <param name="minStopDistanceEntry">The instruments minimum stop distance for entry.</param>
        /// <param name="minLimitDistanceEntry">The instruments minimum limit distance for entry.</param>
        /// <param name="minStopDistance">The instruments minimum stop distance.</param>
        /// <param name="minLimitDistance">The instruments minimum limit distance.</param>
        /// <param name="minTradeSize">The instruments minimum trade size.</param>
        /// <param name="maxTradeSize">The instruments maximum trade size.</param>
        /// <param name="rolloverInterestBuy">The instruments rollover interest for long positions.</param>
        /// <param name="rolloverInterestSell">The instruments rollover interest for short positions.</param>
        /// <param name="timestamp"> The instruments initialization timestamp.</param>
        public ForexInstrument(
            Symbol symbol,
            int pricePrecision,
            int sizePrecision,
            int minStopDistanceEntry,
            int minLimitDistanceEntry,
            int minStopDistance,
            int minLimitDistance,
            Price tickSize,
            Quantity roundLotSize,
            Quantity minTradeSize,
            Quantity maxTradeSize,
            decimal rolloverInterestBuy,
            decimal rolloverInterestSell,
            ZonedDateTime timestamp)
            : base(
                symbol,
                GetQuoteCurrency(symbol),
                SecurityType.Forex,
                pricePrecision,
                sizePrecision,
                minStopDistanceEntry,
                minLimitDistanceEntry,
                minStopDistance,
                minLimitDistance,
                tickSize,
                roundLotSize,
                minTradeSize,
                maxTradeSize,
                rolloverInterestBuy,
                rolloverInterestSell,
                timestamp)
        {
            this.BaseCurrency = GetBaseCurrency(symbol);
        }

        /// <summary>
        /// Gets the instruments base currency.
        /// </summary>
        public Currency BaseCurrency { get; }

        /// <summary>
        /// Get the quote currency for the given FX symbol.
        /// </summary>
        /// <param name="symbol">The FX symbol to parse.</param>
        /// <returns>The quote currency.</returns>
        public static Currency GetBaseCurrency(Symbol symbol)
        {
            return symbol.Code.Substring(0, 3).ToEnum<Currency>();
        }

        /// <summary>
        /// Get the quote currency for the given FX symbol.
        /// </summary>
        /// <param name="symbol">The FX symbol to parse.</param>
        /// <returns>The quote currency.</returns>
        public static Currency GetQuoteCurrency(Symbol symbol)
        {
            return symbol.Code.Substring(symbol.Code.Length - 3).ToEnum<Currency>();
        }
    }
}
