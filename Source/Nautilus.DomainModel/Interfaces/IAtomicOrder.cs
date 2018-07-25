//--------------------------------------------------------------------------------------------------
// <copyright file="IAtomicOrder.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Interfaces
{
    using Nautilus.Core;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Provides a read-only interface for atomic orders.
    /// </summary>
    public interface IAtomicOrder
    {
        /// <summary>
        /// Gets the atomic orders symbol.
        /// </summary>
        Symbol Symbol { get; }

        /// <summary>
        /// Gets the atomic orders trade type.
        /// </summary>
        TradeType TradeType { get; }

        /// <summary>
        /// Gets the atomic orders entry order.
        /// </summary>
        IOrder Entry { get; }

        /// <summary>
        /// Gets the atomic orders stop-loss order.
        /// </summary>
        IOrder StopLoss { get; }

        /// <summary>
        /// Gets the atomic orders profit target order (optional).
        /// </summary>
        Option<IOrder> ProfitTarget { get; }
    }
}
