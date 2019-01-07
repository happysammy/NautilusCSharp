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
    using Nautilus.DomainModel.Entities.Base;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.Interfaces;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Represents a collection of orders being an entry, stop-loss and profit target (optional) to
    /// be managed together.
    /// </summary>
    [Immutable]
    public sealed class AtomicOrder : Entity<AtomicOrder>, IAtomicOrder
    {
        private readonly Order entry;
        private readonly Order stopLoss;
        private readonly Option<Order> profitTarget;

        /// <summary>
        /// Initializes a new instance of the <see cref="AtomicOrder"/> class.
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
                  new AtomicOrderId(entry.Id.Value),
                  entry.Timestamp)
        {
            Debug.NotNull(tradeType, nameof(tradeType));
            Debug.NotNull(entry, nameof(entry));
            Debug.NotNull(stopLoss, nameof(stopLoss));
            Debug.NotNull(profitTarget, nameof(profitTarget));

            this.entry = entry;
            this.stopLoss = stopLoss;
            this.profitTarget = profitTarget;
            this.TradeType = tradeType;
            this.Entry = entry;
            this.StopLoss = stopLoss;

            this.ProfitTarget = profitTarget.HasValue
                ? Option<IOrder>.Some(profitTarget.Value)
                : Option<IOrder>.None();
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
        public IOrder Entry { get; }

        /// <summary>
        /// Gets the atomic orders stop-loss order.
        /// </summary>
        public IOrder StopLoss { get; }

        /// <summary>
        /// Gets the atomic orders profit target order (optional).
        /// </summary>
        public Option<IOrder> ProfitTarget { get; }

        /// <summary>
        /// Return the atomic orders concrete entry order.
        /// </summary>
        /// <returns>The entry order.</returns>
        public Order GetEntry() => this.entry;

        /// <summary>
        /// Return the atomic orders concrete stop-loss order.
        /// </summary>
        /// <returns>The entry order.</returns>
        public Order GetStopLoss() => this.stopLoss;

        /// <summary>
        /// Return the atomic orders concrete profit-target order.
        /// </summary>
        /// <returns>The entry order.</returns>
        public Option<Order> GetProfitTarget() => this.profitTarget;
    }
}
