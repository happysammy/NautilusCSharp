// -------------------------------------------------------------------------------------------------
// <copyright file="Key.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Redis.Execution
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Identifiers;

    /// <summary>
    /// Provides key strings for a Redis execution database.
    /// </summary>
    [SuppressMessage("ReSharper", "SA1310", Justification = "Easier to read.")]
    internal static class Key
    {
        private const string NAUTILUS_EXECUTOR = "NautilusExecutor:";
        private const string INDEX = NAUTILUS_EXECUTOR + "Index:";
        private const string TRADER = "Trader";
        private const string TRADERS = "Traders";
        private const string ACCOUNT = "Account";
        private const string ACCOUNTS = "Accounts";
        private const string ORDER = "Order";
        private const string ORDERS = "Orders";
        private const string POSITION = "Position";
        private const string POSITIONS = "Positions";
        private const string STRATEGY = "Strategy";
        private const string STRATEGIES = "Strategies";
        private const string BROKER_ID = "BrokerId";
        private const string WORKING = "Working";
        private const string COMPLETED = "Completed";
        private const string OPEN = "Open";
        private const string CLOSED = "Closed";

        private const string KEY_TRADERS = NAUTILUS_EXECUTOR + TRADERS + ":";                  // + TraderId    -> Set{TraderId}
        private const string KEY_ACCOUNTS = NAUTILUS_EXECUTOR + ACCOUNTS + ":";                // + AccountId   -> List[AccountStateEvent]
        private const string KEY_ORDERS = NAUTILUS_EXECUTOR + ORDERS + ":";                    // + OrderId     -> List[OrderEvent]
        private const string KEY_POSITIONS = NAUTILUS_EXECUTOR + POSITIONS + ":";              // + PositionId  -> List[OrderFillEvent]

        private const string KEY_INDEX_TRADERS = INDEX + TRADERS;                              // + TraderId -> Set{OrderId}
        private const string KEY_INDEX_TRADER_ORDERS = INDEX + TRADER + ORDERS + ":";          // + TraderId -> Set{OrderId}
        private const string KEY_INDEX_TRADER_POSITIONS = INDEX + TRADER + POSITIONS + ":";    // + TraderId -> Set{PositionId}
        private const string KEY_INDEX_TRADER_STRATEGIES = INDEX + TRADER + STRATEGIES + ":";  // + TraderId -> Set{StrategyId}
        private const string KEY_INDEX_TRADER_STRATEGY_ORDERS = INDEX + TRADER + STRATEGY + ORDERS + ":";        // + TraderId : StrategyId -> Set{OrderId}
        private const string KEY_INDEX_TRADER_STRATEGY_POSITIONS = INDEX + TRADER + STRATEGY + POSITIONS + ":";  // + TraderId : StrategyId -> Set{PositionId}
        private const string KEY_INDEX_ACCOUNT_ORDERS = INDEX + ACCOUNT + ORDERS + ":";        // + AccountId -> Set{OrderId}
        private const string KEY_INDEX_ACCOUNT_POSITIONS = INDEX + ACCOUNT + POSITIONS + ":";  // + AccountId -> Set{PositionId}
        private const string KEY_INDEX_BROKER_POSITION = INDEX + BROKER_ID + POSITION + ":";   // + AccountId -> HashSet[PositionIdBroker, PositionId]
        private const string KEY_INDEX_ORDER_TRADER = INDEX + ORDER + TRADER;                  // -> HashSet[OrderId, TraderId]
        private const string KEY_INDEX_ORDER_ACCOUNT = INDEX + ORDER + ACCOUNT;                // -> HashSet[OrderId, AccountId]
        private const string KEY_INDEX_ORDER_POSITION = INDEX + ORDER + POSITION;              // -> HashSet[OrderId, PositionId]
        private const string KEY_INDEX_ORDER_STRATEGY = INDEX + ORDER + STRATEGY;              // -> HashSet[OrderId, StrategyId]
        private const string KEY_INDEX_POSITION_TRADER = INDEX + POSITION + TRADER;            // -> HashSet[PositionId, TraderId]
        private const string KEY_INDEX_POSITION_ACCOUNT = INDEX + POSITION + ACCOUNT;          // -> HashSet[PositionId, AccountId]
        private const string KEY_INDEX_POSITION_STRATEGY = INDEX + POSITION + STRATEGY;        // -> HashSet[PositionId, StrategyId]
        private const string KEY_INDEX_POSITION_BROKER = INDEX + POSITION + BROKER_ID;         // -> HashSet[PositionId, PositionIdBroker]
        private const string KEY_INDEX_POSITION_ORDERS = INDEX + POSITION + ORDERS + ":";      // + PositionId -> Set{OrderId}

        private const string KEY_INDEX_ORDERS = INDEX + ORDERS;                                // Set{OrderId}
        private const string KEY_INDEX_ORDERS_WORKING = INDEX + ORDERS + ":" + WORKING;        // Set{OrderId}
        private const string KEY_INDEX_ORDERS_COMPLETED = INDEX + ORDERS + ":" + COMPLETED;    // Set{OrderId}
        private const string KEY_INDEX_POSITIONS = INDEX + POSITIONS;                          // Set{PositionId}
        private const string KEY_INDEX_POSITIONS_OPEN = INDEX + POSITIONS + ":" + OPEN;        // Set{PositionId}
        private const string KEY_INDEX_POSITIONS_CLOSED = INDEX + POSITIONS + ":" + CLOSED;    // Set{PositionId}

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        /// <returns>The key string.</returns>
        internal static string Traders => KEY_TRADERS + "*";

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        /// <returns>The key string.</returns>
        internal static string Accounts => KEY_ACCOUNTS + "*";

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        /// <returns>The key string.</returns>
        internal static string Orders => KEY_ORDERS + "*";

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        /// <returns>The key string.</returns>
        internal static string Positions => KEY_POSITIONS + "*";

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        internal static string IndexTraders => KEY_INDEX_TRADERS;

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        internal static string IndexOrderTrader => KEY_INDEX_ORDER_TRADER;

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        internal static string IndexOrderAccount => KEY_INDEX_ORDER_ACCOUNT;

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        internal static string IndexOrderPosition => KEY_INDEX_ORDER_POSITION;

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        internal static string IndexOrderStrategy => KEY_INDEX_ORDER_STRATEGY;

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        internal static string IndexPositionTrader => KEY_INDEX_POSITION_TRADER;

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        internal static string IndexPositionAccount => KEY_INDEX_POSITION_ACCOUNT;

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        internal static string IndexPositionStrategy => KEY_INDEX_POSITION_STRATEGY;

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        internal static string IndexPositionBrokerId => KEY_INDEX_POSITION_BROKER;

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        /// <returns>The key string.</returns>
        internal static string IndexOrders => KEY_INDEX_ORDERS;

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        /// <returns>The key string.</returns>
        internal static string IndexOrdersWorking => KEY_INDEX_ORDERS_WORKING;

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        /// <returns>The key string.</returns>
        internal static string IndexOrdersCompleted => KEY_INDEX_ORDERS_COMPLETED;

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        /// <returns>The key string.</returns>
        internal static string IndexPositions => KEY_INDEX_POSITIONS;

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        /// <returns>The key string.</returns>
        internal static string IndexPositionsOpen => KEY_INDEX_POSITIONS_OPEN;

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        /// <returns>The key string.</returns>
        internal static string IndexPositionsClosed => KEY_INDEX_POSITIONS_CLOSED;

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        /// <param name="traderId">The trader identifier.</param>
        /// <returns>The key string.</returns>
        internal static string Trader(TraderId traderId) => KEY_TRADERS + traderId.Value;

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        /// <returns>The key string.</returns>
        internal static string Account(AccountId accountId) => KEY_ACCOUNTS + accountId.Value;

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns>The key string.</returns>
        internal static string Order(OrderId orderId) => KEY_ORDERS + orderId.Value;

        /// <summary>
        /// Gets the Redis key.
        /// </summary>
        /// <param name="positionId">The position identifier.</param>
        /// <returns>The key string.</returns>
        internal static string Position(PositionId positionId) => KEY_POSITIONS + positionId.Value;

        /// <summary>
        /// Returns the Redis key.
        /// </summary>
        /// <param name="traderId">The trader identifier.</param>
        /// <returns>The key string.</returns>
        internal static string IndexTraderOrders(TraderId traderId) => KEY_INDEX_TRADER_ORDERS + traderId.Value;

        /// <summary>
        /// Returns the Redis key.
        /// </summary>
        /// <param name="traderId">The trader identifier.</param>
        /// <returns>The key string.</returns>
        internal static string IndexTraderPositions(TraderId traderId) => KEY_INDEX_TRADER_POSITIONS + traderId.Value;

        /// <summary>
        /// Returns the Redis key.
        /// </summary>
        /// <param name="traderId">The trader identifier.</param>
        /// <returns>The key string.</returns>
        internal static string IndexTraderStrategies(TraderId traderId) => KEY_INDEX_TRADER_STRATEGIES + traderId.Value;

        /// <summary>
        /// Returns the Redis key.
        /// </summary>
        /// <param name="traderId">The trader identifier.</param>
        /// <param name="strategyId">The strategy identifier.</param>
        /// <returns>The key string.</returns>
        internal static string IndexTraderStrategyOrders(TraderId traderId, StrategyId strategyId) => KEY_INDEX_TRADER_STRATEGY_ORDERS + traderId.Value + ":" + strategyId.Value;

        /// <summary>
        /// Returns the Redis key.
        /// </summary>
        /// <param name="traderId">The trader identifier.</param>
        /// <param name="strategyId">The strategy identifier.</param>
        /// <returns>The key string.</returns>
        internal static string IndexTraderStrategyPositions(TraderId traderId, StrategyId strategyId) => KEY_INDEX_TRADER_STRATEGY_POSITIONS + traderId.Value + ":" + strategyId.Value;

        /// <summary>
        /// Returns the Redis key.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        /// <returns>The key string.</returns>
        internal static string IndexAccountOrders(AccountId accountId) => KEY_INDEX_ACCOUNT_ORDERS + accountId.Value;

        /// <summary>
        /// Returns the Redis key.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        /// <returns>The key string.</returns>
        internal static string IndexAccountPositions(AccountId accountId) => KEY_INDEX_ACCOUNT_POSITIONS + accountId.Value;

        /// <summary>
        /// Returns the Redis key.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        /// <returns>The key string.</returns>
        internal static string IndexBrokerIdPosition(AccountId accountId) => KEY_INDEX_BROKER_POSITION + accountId.Value;

        /// <summary>
        /// Returns the Redis key.
        /// </summary>
        /// <param name="positionId">The position identifier.</param>
        /// <returns>The key string.</returns>
        internal static string IndexPositionOrders(PositionId positionId) => KEY_INDEX_POSITION_ORDERS + positionId.Value;
    }
}
