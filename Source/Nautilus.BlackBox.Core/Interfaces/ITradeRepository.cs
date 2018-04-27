// -------------------------------------------------------------------------------------------------
// <copyright file="ITradeRepository.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Interfaces
{
    using NautechSystems.CSharp;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.Aggregates;

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
        Option<Trade> FindBy(EntityId tradeId);

        /// <summary>
        /// Adds the given trade to the repository.
        /// </summary>
        /// <param name="trade">The trade.</param>
        void Add(Trade trade);
    }
}
