//--------------------------------------------------------------------------------------------------
// <copyright file="AtomicOrder.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Entities
{
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Interfaces;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Represents a collection of orders being an entry, stop-loss and profit target (optional) to
    /// be managed together.
    /// </summary>
    [Immutable]
    public sealed class AtomicOrder : Entity<AtomicOrder>, IAtomicOrder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AtomicOrder" /> class.
        /// </summary>
        /// <param name="tradeType">The trade type.</param>
        /// <param name="entry">The entry order.</param>
        /// <param name="stopLoss">The stop-loss order.</param>
        /// <param name="profitTarget">The profit target order.</param>
        public AtomicOrder(
            TradeType tradeType,
            Order entry,
            Order stopLoss,
            Option<Order> profitTarget)
            : base(
                  entry.Id,
                  entry.Timestamp)
        {
            Debug.NotNull(tradeType, nameof(tradeType));
            Debug.NotNull(entry, nameof(entry));
            Debug.NotNull(stopLoss, nameof(stopLoss));
            Debug.NotNull(profitTarget, nameof(profitTarget));

            this.TradeType = tradeType;
            this.Entry = entry;
            this.StopLoss = stopLoss;
            this.ProfitTarget = profitTarget;
        }

        /// <summary>
        /// Gets the atomic orders symbol.
        /// </summary>
        public Symbol Symbol => this.Entry.Symbol;

        /// <summary>
        /// Gets the atomic orders trade type.
        /// </summary>
        public TradeType TradeType { get; }

        /// <summary>
        /// Gets the atomic orders entry order.
        /// </summary>
        public Order Entry { get; }

        /// <summary>
        /// Gets the atomic orders stop-loss order.
        /// </summary>
        public Order StopLoss { get; }

        /// <summary>
        /// Gets the atomic orders profit target order (optional).
        /// </summary>
        public Option<Order> ProfitTarget { get; }
    }
}
