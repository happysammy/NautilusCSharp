//--------------------------------------------------------------
// <copyright file="AtomicOrder.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.DomainModel.Entities
{
    using NautechSystems.CSharp;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.DomainModel.Orders;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The immutable sealed <see cref="AtomicOrder"/> class. Represents a collection of orders being
    /// an entry, stop-loss and profit target (optional) to be managed together.
    /// </summary>
    [Immutable]
    public sealed class AtomicOrder : Entity<AtomicOrder>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AtomicOrder" /> class.
        /// </summary>
        /// <param name="tradeType">The trade type.</param>
        /// <param name="entryOrder">The entry order.</param>
        /// <param name="stopLossOrder">The stop-loss order.</param>
        /// <param name="profitTargetOrder">The profit target order.</param>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public AtomicOrder(
            TradeType tradeType,
            StopOrder entryOrder,
            StopMarketOrder stopLossOrder,
            Option<StopOrder> profitTargetOrder)
            : base(
                  entryOrder.OrderId,
                  entryOrder.OrderTimestamp)
        {
            Validate.NotNull(tradeType, nameof(tradeType));
            Validate.NotNull(entryOrder, nameof(entryOrder));
            Validate.NotNull(stopLossOrder, nameof(stopLossOrder));
            Validate.NotNull(profitTargetOrder, nameof(profitTargetOrder));

            this.TradeType = tradeType;
            this.EntryOrder = entryOrder;
            this.StopLossOrder = stopLossOrder;
            this.ProfitTargetOrder = profitTargetOrder;
        }

        /// <summary>
        /// Gets the atomic orders symbol.
        /// </summary>
        public Symbol Symbol => this.EntryOrder.Symbol;

        /// <summary>
        /// Gets the atomic orders identifier.
        /// </summary>
        public EntityId AtomicOrderId => this.EntityId;

        /// <summary>
        /// Gets the atomic orders trade type.
        /// </summary>
        public TradeType TradeType { get; }

        /// <summary>
        /// Gets the atomic orders entry order.
        /// </summary>
        public StopOrder EntryOrder { get; }

        /// <summary>
        /// Gets the atomic orders stop-loss order.
        /// </summary>
        public StopMarketOrder StopLossOrder { get; }

        /// <summary>
        /// Gets the atomic orders profit target order (optional).
        /// </summary>
        public Option<StopOrder> ProfitTargetOrder { get; }
    }
}