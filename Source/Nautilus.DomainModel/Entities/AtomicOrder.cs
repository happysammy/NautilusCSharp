//--------------------------------------------------------------------------------------------------
// <copyright file="AtomicOrder.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Entities
{
    using Nautilus.Core.Annotations;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities.Base;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Represents a collection of orders being an entry, stop-loss and optional take-profit to
    /// be managed together.
    /// </summary>
    [Immutable]
    public sealed class AtomicOrder : Entity<AtomicOrder>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AtomicOrder"/> class.
        /// </summary>
        /// <param name="entry">The entry order.</param>
        /// <param name="stopLoss">The stop-loss order.</param>
        /// <param name="takeProfit">The take-profit order.</param>
        public AtomicOrder(
            Order entry,
            Order stopLoss,
            Order? takeProfit = null)
            : base(new AtomicOrderId("A" + entry.Id), entry.Timestamp)
        {
            this.Entry = entry;
            this.StopLoss = stopLoss;
            this.TakeProfit = takeProfit;
        }

        /// <summary>
        /// Gets the atomic orders symbol.
        /// </summary>
        public Symbol Symbol => this.Entry.Symbol;

        /// <summary>
        /// Gets the atomic orders entry order.
        /// </summary>
        public Order Entry { get; }

        /// <summary>
        /// Gets the atomic orders stop-loss order.
        /// </summary>
        public Order StopLoss { get; }

        /// <summary>
        /// Gets the atomic orders take-profit order (optional).
        /// </summary>
        public Order? TakeProfit { get; }
    }
}
