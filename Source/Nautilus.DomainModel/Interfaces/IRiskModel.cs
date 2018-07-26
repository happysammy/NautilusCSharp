//--------------------------------------------------------------------------------------------------
// <copyright file="IRiskModel.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The <see cref="IRiskModel"/> interface implements <see cref="IReadOnlyRiskModel"/>.
    /// Represents a systems model for quantifying financial market risk
    /// exposure.
    /// </summary>
    public interface IRiskModel : IReadOnlyRiskModel
    {
        /// <summary>
        /// Updates the global maximum risk per trade with the given input.
        /// </summary>
        /// <param name="maxRiskPerTrade">The maximum risk per trade.</param>
        /// <param name="timestamp">The update timestamp.</param>
        void UpdateGlobalMaxRiskPerTrade(Percentage maxRiskPerTrade, ZonedDateTime timestamp);

        /// <summary>
        /// Updates the position size hard limit with the given inputs.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="hardLimit">The quantity hard limit.</param>
        /// <param name="timestamp">The update timestamp.</param>
        void UpdatePositionSizeHardLimit(Symbol symbol, Quantity hardLimit, ZonedDateTime timestamp);

        /// <summary>
        /// Updates the maximum risk per trade type with the given inputs.
        /// </summary>
        /// <param name="tradeType">The trade type.</param>
        /// <param name="riskPerTrade">The risk per trade.</param>
        /// <param name="timestamp">The update timestamp.</param>
        void UpdateMaxRiskPerTradeType(TradeType tradeType, Percentage riskPerTrade, ZonedDateTime timestamp);

        /// <summary>
        /// Updates the maximum trades per type with the given inputs.
        /// </summary>
        /// <param name="tradeType">The trade type.</param>
        /// <param name="maxTrades">The maximum quantity of trades.</param>
        /// <param name="timestamp">The update timestamp.</param>
        void UpdateMaxTradesPerSymbolType(TradeType tradeType, Quantity maxTrades, ZonedDateTime timestamp);

        /// <summary>
        /// Returns a copy of the event log.
        /// </summary>
        /// <returns>A <see cref="IReadOnlyDictionary{TKey,TValue}"/>.</returns>
        IReadOnlyList<Tuple<ZonedDateTime, string>> GetEventLog();
    }
}