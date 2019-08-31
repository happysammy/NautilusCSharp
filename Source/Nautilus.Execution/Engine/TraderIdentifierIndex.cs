// -------------------------------------------------------------------------------------------------
// <copyright file="TraderIdentifierIndex.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Execution.Engine
{
    using System.Collections.Generic;
    using Nautilus.DomainModel.Identifiers;

    /// <summary>
    /// Provides an identifier index for a trader.
    /// </summary>
    public class TraderIdentifierIndex
    {
        private readonly HashSet<OrderId> indexOrders;
        private readonly HashSet<PositionId> indexPositions;
        private readonly HashSet<StrategyId> indexStrategies;

        private readonly Dictionary<StrategyId, HashSet<OrderId>> indexStrategyOrders;
        private readonly Dictionary<StrategyId, HashSet<PositionId>> indexStrategyPositions;

        /// <summary>
        /// Initializes a new instance of the <see cref="TraderIdentifierIndex"/> class.
        /// </summary>
        /// <param name="traderId">The trader identifier.</param>
        public TraderIdentifierIndex(TraderId traderId)
        {
            this.indexOrders = new HashSet<OrderId>();
            this.indexPositions = new HashSet<PositionId>();
            this.indexStrategies = new HashSet<StrategyId>();
            this.indexStrategyOrders = new Dictionary<StrategyId, HashSet<OrderId>>();
            this.indexStrategyPositions = new Dictionary<StrategyId, HashSet<PositionId>>();

            this.TraderId = traderId;
        }

        /// <summary>
        /// Gets the indexes trader identifier.
        /// </summary>
        public TraderId TraderId { get; }

        /// <summary>
        /// Gets the indexes position identifiers.
        /// </summary>
        public HashSet<StrategyId> StrategyIds => this.indexStrategies;

        /// <summary>
        /// Add the given order identifier to the index.
        /// </summary>
        /// <param name="orderId">The order identifier to add.</param>
        /// <param name="positionId">The position identifier to add.</param>
        /// <param name="strategyId">The strategy identifier to add.</param>
        public void AddIdentifiers(OrderId orderId, PositionId positionId, StrategyId strategyId)
        {
            this.indexOrders.Add(orderId);
            this.indexPositions.Add(positionId);
            this.indexStrategies.Add(strategyId);

            if (this.indexStrategyOrders.TryGetValue(strategyId, out var orderIds))
            {
                orderIds.Add(orderId);
            }
            else
            {
                this.indexStrategyOrders[strategyId] = new HashSet<OrderId> { orderId };
            }

            if (this.indexStrategyPositions.TryGetValue(strategyId, out var positionIds))
            {
                positionIds.Add(positionId);
            }
            else
            {
                this.indexStrategyPositions[strategyId] = new HashSet<PositionId> { positionId };
            }
        }

        /// <summary>
        /// Returns the trader indexes order identifiers.
        /// </summary>
        /// <param name="strategyId">The optional strategy identifier filter.</param>
        /// <returns>The order identifiers.</returns>
        public HashSet<OrderId> OrderIds(StrategyId? strategyId)
        {
            if (strategyId is null)
            {
                return this.indexOrders;
            }

            return this.indexStrategyOrders.TryGetValue(strategyId, out var orderIds)
                ? orderIds
                : new HashSet<OrderId>();
        }

        /// <summary>
        /// Returns the trader indexes position identifiers.
        /// </summary>
        /// <param name="strategyId">The optional strategy identifier filter.</param>
        /// <returns>The position identifiers.</returns>
        public HashSet<PositionId> PositionIds(StrategyId? strategyId)
        {
            if (strategyId is null)
            {
                return this.indexPositions;
            }

            return this.indexStrategyPositions.TryGetValue(strategyId, out var positionIds)
                ? positionIds
                : new HashSet<PositionId>();
        }
    }
}
