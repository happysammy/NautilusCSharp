//--------------------------------------------------------------------------------------------------
// <copyright file="TraderStrategyId.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Identifiers
{
    using Nautilus.Core.Types;

    /// <summary>
    /// Represents a trader-strategy identifier combination.
    /// </summary>
    public class TraderStrategyId : Identifier<TraderStrategyId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TraderStrategyId"/> class.
        /// </summary>
        /// <param name="traderId">The trader identifier.</param>
        /// <param name="strategyId">The strategy identifier.</param>
        public TraderStrategyId(TraderId traderId, StrategyId strategyId)
            : base($"{traderId.Value}:{strategyId.Value}")
        {
            this.TraderId = traderId;
            this.StrategyId = strategyId;
        }

        /// <summary>
        /// Gets the identifiers trader id.
        /// </summary>
        public TraderId TraderId { get; }

        /// <summary>
        /// Gets the identifiers strategy id.
        /// </summary>
        public StrategyId StrategyId { get; }
    }
}
