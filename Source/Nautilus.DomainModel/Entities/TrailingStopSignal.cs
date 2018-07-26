//--------------------------------------------------------------------------------------------------
// <copyright file="TrailingStopSignal.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Entities
{
    using System.Collections.Generic;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Represents a trade trailing stop signal.
    /// </summary>
    [Immutable]
    public sealed class TrailingStopSignal : Signal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrailingStopSignal"/> class.
        /// </summary>
        /// <param name="symbol">The trailing stop signal symbol.</param>
        /// <param name="signalId">The trailing stop signal identifier.</param>
        /// <param name="signalLabel">The trailing stop signal label.</param>
        /// <param name="tradeType">The trailing stop signal trade type.</param>
        /// <param name="forMarketPosition">The trailing stop signal market position type.</param>
        /// <param name="forUnitStopLossPrices">The trailing stop signal unit stop-loss prices dictionary.</param>
        /// <param name="signalTimestamp">The trailing stop signal timestamp.</param>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if any
        /// struct argument is the default value, or if the forMarketPosition is flat.</exception>
        public TrailingStopSignal(
            Symbol symbol,
            SignalId signalId,
            Label signalLabel,
            TradeType tradeType,
            MarketPosition forMarketPosition,
            Dictionary<int, Price> forUnitStopLossPrices,
            ZonedDateTime signalTimestamp)
            : base(
                  symbol,
                  signalId,
                  signalLabel,
                  tradeType,
                  signalTimestamp)
        {
            Debug.NotNull(symbol, nameof(symbol));
            Debug.NotNull(signalId, nameof(signalId));
            Debug.NotNull(signalLabel, nameof(signalLabel));
            Debug.NotNull(tradeType, nameof(tradeType));
            Debug.NotDefault(forMarketPosition, nameof(forMarketPosition));
            Debug.NotEqualTo(forMarketPosition, nameof(forMarketPosition), MarketPosition.Flat);
            Debug.CollectionNotNullOrEmpty(forUnitStopLossPrices, nameof(forUnitStopLossPrices));
            Debug.NotDefault(signalTimestamp, nameof(signalTimestamp));

            this.ForMarketPosition = forMarketPosition;
            this.ForUnitStopLossPrices = forUnitStopLossPrices;
        }

        /// <summary>
        /// Gets the trailing stop signals market position type.
        /// </summary>
        public MarketPosition ForMarketPosition { get; }

        /// <summary>
        /// Gets the trailing stop signals unit stop-loss prices read-only dictionary.
        /// </summary>
        public IReadOnlyDictionary<int, Price> ForUnitStopLossPrices { get; }
    }
}
