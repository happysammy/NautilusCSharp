//--------------------------------------------------------------------------------------------------
// <copyright file="Instrument.cs" company="Nautech Systems Pty Ltd">
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

using System;
using Nautilus.Core.Annotations;
using Nautilus.Core.Correctness;
using Nautilus.DomainModel.Entities.Base;
using Nautilus.DomainModel.Enums;
using Nautilus.DomainModel.Identifiers;
using Nautilus.DomainModel.ValueObjects;
using NodaTime;

namespace Nautilus.DomainModel.Entities
{
    /// <summary>
    /// Represents a tradeable financial market instrument.
    /// </summary>
    [Immutable]
    public class Instrument : Entity<InstrumentId, Instrument>, ICloneable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Instrument"/> class.
        /// </summary>
        /// <param name="symbol">The instruments symbol.</param>
        /// <param name="quoteCurrency">The instruments quote currency.</param>
        /// <param name="securityType">The instruments security type.</param>
        /// <param name="pricePrecision">The instruments tick decimal precision.</param>
        /// <param name="sizePrecision">The instruments quantity size precision.</param>
        /// <param name="minStopDistanceEntry">The instruments minimum stop distance for entry.</param>
        /// <param name="minLimitDistanceEntry">The instruments minimum limit distance for entry.</param>
        /// <param name="minStopDistance">The instruments minimum stop distance.</param>
        /// <param name="minLimitDistance">The instruments minimum limit distance.</param>
        /// <param name="tickSize">The instruments tick size.</param>
        /// <param name="roundLotSize">The instruments rounded lot size.</param>
        /// <param name="minTradeSize">The instruments minimum trade size.</param>
        /// <param name="maxTradeSize">The instruments maximum trade size.</param>
        /// <param name="rolloverInterestBuy">The instruments rollover interest for long positions.</param>
        /// <param name="rolloverInterestSell">The instruments rollover interest for short positions.</param>
        /// <param name="timestamp"> The instruments initialization timestamp.</param>
        public Instrument(
            Symbol symbol,
            Currency quoteCurrency,
            SecurityType securityType,
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
            : base(new InstrumentId(symbol.Value), timestamp)
        {
            Condition.NotNegativeInt32(pricePrecision, nameof(pricePrecision));
            Condition.NotNegativeInt32(sizePrecision, nameof(pricePrecision));
            Condition.NotNegativeInt32(minStopDistanceEntry, nameof(minStopDistanceEntry));
            Condition.NotNegativeInt32(minLimitDistanceEntry, nameof(minLimitDistanceEntry));
            Condition.NotNegativeInt32(minStopDistance, nameof(minStopDistance));
            Condition.NotNegativeInt32(minLimitDistance, nameof(minLimitDistance));
            Condition.NotDefault(timestamp, nameof(timestamp));

            this.Symbol = symbol;
            this.QuoteCurrency = quoteCurrency;
            this.SecurityType = securityType;
            this.PricePrecision = pricePrecision;
            this.SizePrecision = sizePrecision;
            this.MinStopDistanceEntry = minStopDistanceEntry;
            this.MinLimitDistanceEntry = minLimitDistanceEntry;
            this.MinStopDistance = minStopDistance;
            this.MinLimitDistance = minLimitDistance;
            this.TickSize = tickSize;
            this.RoundLotSize = roundLotSize;
            this.MinTradeSize = minTradeSize;
            this.MaxTradeSize = maxTradeSize;
            this.RolloverInterestBuy = rolloverInterestBuy;
            this.RolloverInterestSell = rolloverInterestSell;
        }

        /// <summary>
        /// Gets the instruments symbol.
        /// </summary>
        public Symbol Symbol { get; }

        /// <summary>
        /// Gets the instruments quote currency.
        /// </summary>
        public Currency QuoteCurrency { get; }

        /// <summary>
        /// Gets the instruments security type.
        /// </summary>
        public SecurityType SecurityType { get; }

        /// <summary>
        /// Gets the instruments price decimal precision.
        /// </summary>
        public int PricePrecision { get; }

        /// <summary>
        /// Gets the instruments quantity decimal precision.
        /// </summary>
        public int SizePrecision { get; }

        /// <summary>
        /// Gets the instruments minimum stop distance for entry.
        /// </summary>
        public int MinStopDistanceEntry { get; }

        /// <summary>
        /// Gets the instruments minimum limit distance for entry.
        /// </summary>
        public int MinLimitDistanceEntry { get; }

        /// <summary>
        /// Gets the instruments minimum stop distance.
        /// </summary>
        public int MinStopDistance { get; }

        /// <summary>
        /// Gets the instruments minimum limit distance.
        /// </summary>
        public int MinLimitDistance { get; }

        /// <summary>
        /// Gets the instruments tick size.
        /// </summary>
        public Price TickSize { get; }

        /// <summary>
        /// Gets the instruments rounded lot size.
        /// </summary>
        public Quantity RoundLotSize { get; }

        /// <summary>
        /// Gets the instruments minimum trade size.
        /// </summary>
        public Quantity MinTradeSize { get; }

        /// <summary>
        /// Gets the instruments maximum trade size.
        /// </summary>
        public Quantity MaxTradeSize { get; }

        /// <summary>
        /// Gets the instruments rollover interest for long positions.
        /// </summary>
        public decimal RolloverInterestBuy { get; }

        /// <summary>
        /// Gets the instruments rollover interest for short positions.
        /// </summary>
        public decimal RolloverInterestSell { get; }

        /// <summary>
        /// Returns a value indicating whether this <see cref="Instrument"/> is equal to the
        /// specified <see cref="object"/>.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public override bool Equals(object? other) => other is Instrument instrument && this.Equals(instrument);

        /// <summary>
        /// Returns the hash code of the <see cref="Instrument"/>.
        /// </summary>
        /// <returns>A <see cref="int"/>.</returns>
        public override int GetHashCode() => this.Id.GetHashCode();

        /// <summary>
        /// Returns a string representation of the <see cref="Instrument"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => this.Id.Value;

        /// <inheritdoc />
        public object Clone()
        {
            return this;  // Immutable type
        }
    }
}
