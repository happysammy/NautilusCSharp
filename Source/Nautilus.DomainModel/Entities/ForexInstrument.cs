//--------------------------------------------------------------------------------------------------
// <copyright file="ForexInstrument.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Entities
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Identifiers;
    using NodaTime;

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
        /// <param name="brokerSymbol">The instruments broker symbol.</param>
        /// <param name="tickPrecision">The instruments tick decimal precision.</param>
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
            BrokerSymbol brokerSymbol,
            int tickPrecision,
            decimal tickSize,
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
            : base(
                symbol,
                brokerSymbol,
                symbol.Code.Substring(3, 3).ToEnum<Currency>(),
                SecurityType.Forex,
                tickPrecision,
                tickSize,
                roundLotSize,
                minStopDistanceEntry,
                minLimitDistanceEntry,
                minStopDistance,
                minLimitDistance,
                minTradeSize,
                maxTradeSize,
                rolloverInterestBuy,
                rolloverInterestSell,
                timestamp)
        {
            Condition.EqualTo(symbol.Code.Length, 6, nameof(symbol.Code.Length));

            this.BaseCurrency = symbol.Code.Substring(0, 3).ToEnum<Currency>();
        }

        /// <summary>
        /// Gets the instruments base currency.
        /// </summary>
        public Currency BaseCurrency { get; }
    }
}
