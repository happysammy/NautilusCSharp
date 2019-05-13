﻿//--------------------------------------------------------------------------------------------------
// <copyright file="Instrument.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Entities
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Entities.Base;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Represents a tradeable financial market instrument.
    /// </summary>
    [Immutable]
    public sealed class Instrument : Entity<Instrument>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Instrument"/> class.
        /// </summary>
        /// <param name="symbol">The instruments symbol.</param>
        /// <param name="instrumentId">The instruments identifier.</param>
        /// <param name="brokerSymbol">The instruments broker symbol.</param>
        /// <param name="quoteCurrency">The instruments quote currency.</param>
        /// <param name="securityType">The instruments security type.</param>
        /// <param name="tickPrecision">The instruments tick decimal precision.</param>
        /// <param name="tickSize">The instruments tick size.</param>
        /// <param name="tickValue">The instruments tick value.</param>
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
        public Instrument(
            Symbol symbol,
            InstrumentId instrumentId,
            BrokerSymbol brokerSymbol,
            Currency quoteCurrency,
            SecurityType securityType,
            int tickPrecision,
            decimal tickSize,
            decimal tickValue,
            int roundLotSize,
            int minStopDistanceEntry,
            int minLimitDistanceEntry,
            int minStopDistance,
            int minLimitDistance,
            int minTradeSize,
            int maxTradeSize,
            decimal rolloverInterestBuy,
            decimal rolloverInterestSell,
            ZonedDateTime timestamp)
            : base(instrumentId, timestamp)
        {
            // Keep validation logic here.
            Precondition.NotNegativeInt32(tickPrecision, nameof(tickPrecision));
            Precondition.PositiveDecimal(tickSize, nameof(tickSize));
            Precondition.PositiveDecimal(tickValue, nameof(tickValue));
            Precondition.PositiveInt32(roundLotSize, nameof(roundLotSize));
            Precondition.NotNegativeInt32(minStopDistanceEntry, nameof(minStopDistanceEntry));
            Precondition.NotNegativeInt32(minLimitDistanceEntry, nameof(minLimitDistanceEntry));
            Precondition.NotNegativeInt32(minStopDistance, nameof(minStopDistance));
            Precondition.NotNegativeInt32(minLimitDistance, nameof(minLimitDistance));
            Precondition.PositiveInt32(minTradeSize, nameof(minTradeSize));
            Precondition.PositiveInt32(maxTradeSize, nameof(maxTradeSize));
            Precondition.NotDefault(timestamp, nameof(timestamp));

            this.Symbol = symbol;
            this.BrokerSymbol = brokerSymbol;
            this.QuoteCurrency = quoteCurrency;
            this.SecurityType = securityType;
            this.TickPrecision = tickPrecision;
            this.TickSize = tickSize;
            this.TickValue = tickValue;
            this.RoundLotSize = roundLotSize;
            this.MinStopDistanceEntry = minStopDistanceEntry;
            this.MinLimitDistanceEntry = minLimitDistanceEntry;
            this.MinStopDistance = minStopDistance;
            this.MinLimitDistance = minLimitDistance;
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
        /// Gets the instruments broker symbol.
        /// </summary>
        public BrokerSymbol BrokerSymbol { get; }

        /// <summary>
        /// Gets the instruments quote currency.
        /// </summary>
        public Currency QuoteCurrency { get; }

        /// <summary>
        /// Gets the instruments security type.
        /// </summary>
        public SecurityType SecurityType { get; }

        /// <summary>
        /// Gets the instruments tick decimal precision.
        /// </summary>
        public int TickPrecision { get; }

        /// <summary>
        /// Gets the instruments tick size.
        /// </summary>
        public decimal TickSize { get; }

        /// <summary>
        /// Gets the instruments tick value.
        /// </summary>
        public decimal TickValue { get; }

        /// <summary>
        /// Gets the instruments rounded lot size.
        /// </summary>
        public int RoundLotSize { get; }

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
        /// Gets the instruments minimum trade size.
        /// </summary>
        public int MinTradeSize { get; }

        /// <summary>
        /// Gets the instruments maximum trade size.
        /// </summary>
        public int MaxTradeSize { get; }

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
        /// <param name="obj">The other object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public override bool Equals(object obj) =>
            obj is Instrument otherInstrument && otherInstrument.Symbol.Equals(this.Symbol);

        /// <summary>
        /// Returns the hash code of the <see cref="Instrument"/>.
        /// </summary>
        /// <returns>A <see cref="int"/>.</returns>
        public override int GetHashCode() => this.Symbol.GetHashCode();

        /// <summary>
        /// Returns a string representation of the <see cref="Instrument"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => this.Symbol.ToString();
    }
}
