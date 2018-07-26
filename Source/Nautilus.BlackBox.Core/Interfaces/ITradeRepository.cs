//--------------------------------------------------------------------------------------------------
// <copyright file="ITradeRepository.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Interfaces
{
    using Nautilus.Core;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Identifiers;

    /// <summary>
    /// The <see cref="ITradeRepository"/> interface.
    /// </summary>
    public interface ITradeRepository
    {
        /// <summary>
        /// Queries the repository for a trade matching the given identifier (optional).
        /// </summary>
        /// <param name="tradeId">The trade identifier.</param>
        /// <returns>A <see cref="Option{Trade}"/>.</returns>
        Option<Trade> FindBy(TradeId tradeId);

        /// <summary>
        /// Adds the given trade to the repository.
        /// </summary>
        /// <param name="trade">The trade.</param>
        void Add(Trade trade);
    }
}
