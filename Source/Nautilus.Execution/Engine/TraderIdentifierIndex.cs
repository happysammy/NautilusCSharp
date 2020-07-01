// -------------------------------------------------------------------------------------------------
// <copyright file="TraderIdentifierIndex.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using Nautilus.DomainModel.Identifiers;

namespace Nautilus.Execution.Engine
{
    /// <summary>
    /// Provides an identifier index for a trader.
    /// </summary>
    public sealed class TraderIdentifierIndex
    {
        private readonly HashSet<OrderId> indexOrders;
        private readonly HashSet<PositionId> indexPositions;
        private readonly HashSet<StrategyId> indexStrategies;

        private readonly Dictionary<StrategyId, HashSet<OrderId>> indexStrategyOrders;
        private readonly Dictionary<StrategyId, HashSet<PositionId>> indexStrategyPositions;

        /// <summary>
        /// Initializes a new instance of the <see cref="TraderIdentifierIndex"/> class.
        /// </summary>
        public TraderIdentifierIndex()
        {
            this.indexOrders = new HashSet<OrderId>();
            this.indexPositions = new HashSet<PositionId>();
            this.indexStrategies = new HashSet<StrategyId>();
            this.indexStrategyOrders = new Dictionary<StrategyId, HashSet<OrderId>>();
            this.indexStrategyPositions = new Dictionary<StrategyId, HashSet<PositionId>>();
        }

        /// <summary>
        /// Gets the indexes position identifiers.
        /// </summary>
        public HashSet<StrategyId> StrategyIds => this.indexStrategies;

        /// <summary>
        /// Clears the trader index.
        /// </summary>
        public void Clear()
        {
            this.indexOrders.Clear();
            this.indexPositions.Clear();
            this.indexStrategies.Clear();
            this.indexStrategyOrders.Clear();
            this.indexStrategyPositions.Clear();
        }

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
