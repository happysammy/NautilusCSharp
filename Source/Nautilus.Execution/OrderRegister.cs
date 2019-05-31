// -------------------------------------------------------------------------------------------------
// <copyright file="OrderRegister.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Execution
{
    using System.Collections.Generic;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.Execution.Identifiers;
    using Nautilus.Execution.Messages.Commands;

    /// <summary>
    /// Provides a register for <see cref="Order"/>s associated with Traders and Strategies.
    /// </summary>
    public class OrderRegister : Component
    {
        private readonly Dictionary<TraderId, Dictionary<StrategyId, List<OrderId>>> orderIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderRegister"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        public OrderRegister(IComponentryContainer container)
        : base(container)
        {
            this.orderIndex = new Dictionary<TraderId, Dictionary<StrategyId, List<OrderId>>>();
        }

        /// <summary>
        /// Gets the registered trader identifiers.
        /// </summary>
        public IEnumerable<TraderId> RegisteredTraders => this.orderIndex.Keys;

        /// <summary>
        /// Register the <see cref="TraderId"/>, <see cref="StrategyId"/> and <see cref="OrderId"/>
        /// with the register.
        /// </summary>
        /// <param name="command">The command to register.</param>
        public void Register(SubmitOrder command)
        {
            this.Register(
                command.TraderId,
                command.StrategyId,
                command.Order.Id);
        }

        /// <summary>
        /// Register the <see cref="TraderId"/>, <see cref="StrategyId"/> and <see cref="OrderId"/>
        /// with the register.
        /// </summary>
        /// <param name="command">The command to register.</param>
        public void Register(SubmitAtomicOrder command)
        {
            this.Register(
                command.TraderId,
                command.StrategyId,
                command.AtomicOrder.Entry.Id);

            this.Register(
                command.TraderId,
                command.StrategyId,
                command.AtomicOrder.StopLoss.Id);

            if (command.AtomicOrder.TakeProfit != null)
            {
                this.Register(
                    command.TraderId,
                    command.StrategyId,
                    command.AtomicOrder.TakeProfit.Id);
            }
        }

        private void Register(
            TraderId traderId,
            StrategyId strategyId,
            OrderId orderId)
        {
            if (!this.orderIndex.ContainsKey(traderId))
            {
                this.orderIndex[traderId] = new Dictionary<StrategyId, List<OrderId>>();
            }

            if (this.orderIndex[traderId].ContainsKey(strategyId))
            {
                this.orderIndex[traderId][strategyId] = new List<OrderId>();
            }

            if (this.orderIndex[traderId][strategyId].Contains(orderId))
            {
                this.Log.Error($"Cannot register order id {orderId} (duplicate order identifier).");
            }

            this.orderIndex[traderId][strategyId].Add(orderId);
        }
    }
}
