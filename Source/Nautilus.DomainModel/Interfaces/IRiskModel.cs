//--------------------------------------------------------------------------------------------------
// <copyright file="IRiskModel.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Interfaces
{
    using Nautilus.Core;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Provides a read-only interface for risk models.
    /// </summary>
    public interface IRiskModel
    {
        /// <summary>
        /// Gets the risk models global maximum risk exposure percentage.
        /// </summary>
        Percentage GlobalMaxRiskExposure { get; }

        /// <summary>
        /// Gets the risk models global max risk per trade percentage.
        /// </summary>
        Percentage GlobalMaxRiskPerTrade { get; }

        /// <summary>
        /// Gets a value indicating whether there are position size hard limits.
        /// </summary>
        bool PositionSizeHardLimits { get; }

        /// <summary>
        /// Gets the risk models last event time.
        /// </summary>
        ZonedDateTime LastEventTime { get; }

        /// <summary>
        /// Gets the risk models event count.
        /// </summary>
        int EventCount { get; }

        /// <summary>
        /// Returns the quantity hard limit for the given symbol (optional).
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns>
        /// A <see cref="Option{Quantity}"/>.
        /// </returns>
        Option<Quantity> GetHardLimitQuantity(Symbol symbol);

        /// <summary>
        /// Returns the allowed risk per trade for the given trade type (returns global maximum
        /// risk per trade if not specified by trade type).
        /// </summary>
        /// <param name="tradeType">The trade type.</param>
        /// <returns>A <see cref="Percentage"/>.</returns>
        Percentage GetRiskPerTrade(TradeType tradeType);

        /// <summary>
        /// Returns the maximum number of trades allowed for the given trade type.
        /// </summary>
        /// <param name="tradeType">The trade type.</param>
        /// <returns>A <see cref="Quantity"/>.</returns>
        Quantity GetMaxTrades(TradeType tradeType);
    }
}
