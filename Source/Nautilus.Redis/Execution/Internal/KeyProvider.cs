// -------------------------------------------------------------------------------------------------
// <copyright file="KeyProvider.cs" company="Nautech Systems Pty Ltd">
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

using System.Diagnostics.CodeAnalysis;
using Nautilus.DomainModel.Identifiers;

namespace Nautilus.Redis.Execution.Internal
{
    /// <summary>
    /// Provides key strings for a Redis execution database.
    /// </summary>
    [SuppressMessage("ReSharper", "SA1310", Justification = "Clearer separation of particles and keys")]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Clearer separation of particles and keys")]
    internal static class KeyProvider
    {
        // Define the key particle constants
        private const string NautilusExecutor = nameof(NautilusExecutor) + ":";
        private const string Index = NautilusExecutor + nameof(Index) + ":";
        private const string Trader = nameof(Trader);
        private const string Traders = nameof(Traders);
        private const string Account = nameof(Account);
        private const string Accounts = nameof(Accounts);
        private const string Order = nameof(Order);
        private const string Orders = nameof(Orders);
        private const string Position = nameof(Position);
        private const string Positions = nameof(Positions);
        private const string Strategy = nameof(Strategy);
        private const string Strategies = nameof(Strategies);
        private const string BrokerId = nameof(BrokerId);
        private const string Working = nameof(Working);
        private const string Completed = nameof(Completed);
        private const string Open = nameof(Open);
        private const string Closed = nameof(Closed);

        // Define the actual keys
        private const string KEY_TRADERS = NautilusExecutor + Traders + ":";                  // + TraderId    -> Set{TraderId}
        private const string KEY_ACCOUNTS = NautilusExecutor + Accounts + ":";                // + AccountId   -> List[AccountStateEvent]
        private const string KEY_ORDERS = NautilusExecutor + Orders + ":";                    // + OrderId     -> List[OrderEvent]
        private const string KEY_POSITIONS = NautilusExecutor + Positions + ":";              // + PositionId  -> List[OrderFillEvent]
        private const string KEY_INDEX_TRADERS = Index + Traders;                             // + TraderId -> Set{OrderId}
        private const string KEY_INDEX_TRADER_ORDERS = Index + Trader + Orders + ":";         // + TraderId -> Set{OrderId}
        private const string KEY_INDEX_TRADER_POSITIONS = Index + Trader + Positions + ":";   // + TraderId -> Set{PositionId}
        private const string KEY_INDEX_TRADER_STRATEGIES = Index + Trader + Strategies + ":"; // + TraderId -> Set{StrategyId}
        private const string KEY_INDEX_TRADER_STRATEGY_ORDERS = Index + Trader + Strategy + Orders + ":";        // + TraderId : StrategyId -> Set{OrderId}
        private const string KEY_INDEX_TRADER_STRATEGY_POSITIONS = Index + Trader + Strategy + Positions + ":";  // + TraderId : StrategyId -> Set{PositionId}
        private const string KEY_INDEX_ACCOUNT_ORDERS = Index + Account + Orders + ":";       // + AccountId -> Set{OrderId}
        private const string KEY_INDEX_ACCOUNT_POSITIONS = Index + Account + Positions + ":"; // + AccountId -> Set{PositionId}
        private const string KEY_INDEX_BROKER_POSITION = Index + BrokerId + Position + ":";   // + AccountId -> HashSet[PositionIdBroker, PositionId]
        private const string KEY_INDEX_ORDER_TRADER = Index + Order + Trader;                 // -> HashSet[OrderId, TraderId]
        private const string KEY_INDEX_ORDER_ACCOUNT = Index + Order + Account;               // -> HashSet[OrderId, AccountId]
        private const string KEY_INDEX_ORDER_POSITION = Index + Order + Position;             // -> HashSet[OrderId, PositionId]
        private const string KEY_INDEX_ORDER_STRATEGY = Index + Order + Strategy;             // -> HashSet[OrderId, StrategyId]
        private const string KEY_INDEX_POSITION_TRADER = Index + Position + Trader;           // -> HashSet[PositionId, TraderId]
        private const string KEY_INDEX_POSITION_ACCOUNT = Index + Position + Account;         // -> HashSet[PositionId, AccountId]
        private const string KEY_INDEX_POSITION_STRATEGY = Index + Position + Strategy;       // -> HashSet[PositionId, StrategyId]
        private const string KEY_INDEX_POSITION_BROKER = Index + Position + BrokerId;         // -> HashSet[PositionId, PositionIdBroker]
        private const string KEY_INDEX_POSITION_ORDERS = Index + Position + Orders + ":";     // + PositionId -> Set{OrderId}
        private const string KEY_INDEX_ORDERS = Index + Orders;                               // Set{OrderId}
        private const string KEY_INDEX_ORDERS_WORKING = Index + Orders + ":" + Working;       // Set{OrderId}
        private const string KEY_INDEX_ORDERS_COMPLETED = Index + Orders + ":" + Completed;   // Set{OrderId}
        private const string KEY_INDEX_POSITIONS = Index + Positions;                         // Set{PositionId}
        private const string KEY_INDEX_POSITIONS_OPEN = Index + Positions + ":" + Open;       // Set{PositionId}
        private const string KEY_INDEX_POSITIONS_CLOSED = Index + Positions + ":" + Closed;   // Set{PositionId}

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        /// <returns>The key string.</returns>
        internal static string TradersKey => KEY_TRADERS + "*";

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        /// <returns>The key string.</returns>
        internal static string AccountsKey => KEY_ACCOUNTS + "*";

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        /// <returns>The key string.</returns>
        internal static string OrdersKey => KEY_ORDERS + "*";

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        /// <returns>The key string.</returns>
        internal static string PositionsKey => KEY_POSITIONS + "*";

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        internal static string IndexTradersKey => KEY_INDEX_TRADERS;

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        internal static string IndexOrderTraderKey => KEY_INDEX_ORDER_TRADER;

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        internal static string IndexOrderAccountKey => KEY_INDEX_ORDER_ACCOUNT;

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        internal static string IndexOrderPositionKey => KEY_INDEX_ORDER_POSITION;

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        internal static string IndexOrderStrategyKey => KEY_INDEX_ORDER_STRATEGY;

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        internal static string IndexPositionTraderKey => KEY_INDEX_POSITION_TRADER;

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        internal static string IndexPositionAccountKey => KEY_INDEX_POSITION_ACCOUNT;

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        internal static string IndexPositionStrategyKey => KEY_INDEX_POSITION_STRATEGY;

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        internal static string IndexPositionBrokerIdKey => KEY_INDEX_POSITION_BROKER;

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        /// <returns>The key string.</returns>
        internal static string IndexOrdersKey => KEY_INDEX_ORDERS;

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        /// <returns>The key string.</returns>
        internal static string IndexOrdersWorkingKey => KEY_INDEX_ORDERS_WORKING;

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        /// <returns>The key string.</returns>
        internal static string IndexOrdersCompletedKey => KEY_INDEX_ORDERS_COMPLETED;

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        /// <returns>The key string.</returns>
        internal static string IndexPositionsKey => KEY_INDEX_POSITIONS;

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        /// <returns>The key string.</returns>
        internal static string IndexPositionsOpenKey => KEY_INDEX_POSITIONS_OPEN;

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        /// <returns>The key string.</returns>
        internal static string IndexPositionsClosedKey => KEY_INDEX_POSITIONS_CLOSED;

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        /// <param name="traderId">The trader identifier.</param>
        /// <returns>The key string.</returns>
        internal static string TraderKey(TraderId traderId) => KEY_TRADERS + traderId.Value;

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        /// <returns>The key string.</returns>
        internal static string AccountKey(AccountId accountId) => KEY_ACCOUNTS + accountId.Value;

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns>The key string.</returns>
        internal static string OrderKey(OrderId orderId) => KEY_ORDERS + orderId.Value;

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        /// <param name="positionId">The position identifier.</param>
        /// <returns>The key string.</returns>
        internal static string PositionKey(PositionId positionId) => KEY_POSITIONS + positionId.Value;

        /// <summary>
        /// Returns the Redis key.
        /// </summary>
        /// <param name="traderId">The trader identifier.</param>
        /// <returns>The key string.</returns>
        internal static string IndexTraderOrdersKey(TraderId traderId) => KEY_INDEX_TRADER_ORDERS + traderId.Value;

        /// <summary>
        /// Returns the Redis key.
        /// </summary>
        /// <param name="traderId">The trader identifier.</param>
        /// <returns>The key string.</returns>
        internal static string IndexTraderPositionsKey(TraderId traderId) => KEY_INDEX_TRADER_POSITIONS + traderId.Value;

        /// <summary>
        /// Returns the Redis key.
        /// </summary>
        /// <param name="traderId">The trader identifier.</param>
        /// <returns>The key string.</returns>
        internal static string IndexTraderStrategiesKey(TraderId traderId) => KEY_INDEX_TRADER_STRATEGIES + traderId.Value;

        /// <summary>
        /// Returns the Redis key.
        /// </summary>
        /// <param name="traderId">The trader identifier.</param>
        /// <param name="strategyId">The strategy identifier.</param>
        /// <returns>The key string.</returns>
        internal static string IndexTraderStrategyOrdersKey(TraderId traderId, StrategyId strategyId) => KEY_INDEX_TRADER_STRATEGY_ORDERS + traderId.Value + ":" + strategyId.Value;

        /// <summary>
        /// Returns the Redis key.
        /// </summary>
        /// <param name="traderId">The trader identifier.</param>
        /// <param name="strategyId">The strategy identifier.</param>
        /// <returns>The key string.</returns>
        internal static string IndexTraderStrategyPositionsKey(TraderId traderId, StrategyId strategyId) => KEY_INDEX_TRADER_STRATEGY_POSITIONS + traderId.Value + ":" + strategyId.Value;

        /// <summary>
        /// Returns the Redis key.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        /// <returns>The key string.</returns>
        internal static string IndexAccountOrdersKey(AccountId accountId) => KEY_INDEX_ACCOUNT_ORDERS + accountId.Value;

        /// <summary>
        /// Returns the Redis key.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        /// <returns>The key string.</returns>
        internal static string IndexAccountPositionsKey(AccountId accountId) => KEY_INDEX_ACCOUNT_POSITIONS + accountId.Value;

        /// <summary>
        /// Returns the Redis key.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        /// <returns>The key string.</returns>
        internal static string IndexBrokerIdPositionKey(AccountId accountId) => KEY_INDEX_BROKER_POSITION + accountId.Value;

        /// <summary>
        /// Returns the Redis key.
        /// </summary>
        /// <param name="positionId">The position identifier.</param>
        /// <returns>The key string.</returns>
        internal static string IndexPositionOrdersKey(PositionId positionId) => KEY_INDEX_POSITION_ORDERS + positionId.Value;
    }
}
