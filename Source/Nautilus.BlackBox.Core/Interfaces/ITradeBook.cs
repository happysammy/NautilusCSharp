//--------------------------------------------------------------------------------------------------
// <copyright file="ITradeBook.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Interfaces
{
    using System.Collections.Generic;
    using Nautilus.Core.Collections;
    using Nautilus.Core.CQS;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The <see cref="ITradeBook"/> interface. Represents a security portfolios trade book for
    /// managing active trades in the system.
    /// </summary>
    public interface ITradeBook
    {
        /// <summary>
        /// Returns a list of all <see cref="Trade"/>(s) which are of the given <see cref="TradeType"/>.
        /// </summary>
        /// <param name="tradeType">The trade type.</param>
        /// <returns>A <see cref="IList{Trade}"/>.</returns>
        ReadOnlyList<Trade> GetTradesByTradeType(TradeType tradeType);

        /// <summary>
        /// Returns a trade which matches the given order identifier (or a failure result).
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns>A <see cref="Trade"/>.</returns>
        QueryResult<Trade> GetTradeForOrder(OrderId orderId);

        /// <summary>
        /// Returns a list of all active order identifiers.
        /// </summary>
        /// <returns>A <see cref="IList{T}"/>.</returns>
        ReadOnlyList<OrderId> GetAllActiveOrderIds();
    }
}
