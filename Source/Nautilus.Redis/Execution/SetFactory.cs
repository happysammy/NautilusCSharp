// -------------------------------------------------------------------------------------------------
// <copyright file="SetFactory.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Redis.Execution
{
    using System.Collections.Generic;
    using Nautilus.Core.Annotations;
    using Nautilus.DomainModel.Identifiers;
    using StackExchange.Redis;

    /// <summary>
    /// Provides efficient mathematical set operations. Concrete classes avoid the overhead of LINQ
    /// and interface dispatching.
    /// </summary>
    internal static class SetFactory
    {
        /// <summary>
        /// Return a new set of identifiers converted from the given values.
        /// </summary>
        /// <param name="values">The values to convert.</param>
        /// <returns>The <see cref="HashSet{T}"/>.</returns>
        [PerformanceOptimized]
        internal static HashSet<TraderId> ConvertToTraderIds(RedisKey[] values)
        {
            var valuesLength = values.Length;
            var set = new HashSet<TraderId>(valuesLength);
            for (var i = 0; i < valuesLength; i++)
            {
                set.Add(TraderId.FromString(values[i]));
            }

            return set;
        }

        /// <summary>
        /// Return a new set of identifiers converted from the given values.
        /// </summary>
        /// <param name="values">The values to convert.</param>
        /// <returns>The <see cref="HashSet{T}"/>.</returns>
        [PerformanceOptimized]
        internal static HashSet<TraderId> ConvertToTraderIds(RedisValue[] values)
        {
            var valuesLength = values.Length;
            var set = new HashSet<TraderId>(valuesLength);
            for (var i = 0; i < valuesLength; i++)
            {
                set.Add(TraderId.FromString(values[i]));
            }

            return set;
        }

        /// <summary>
        /// Return a new set of identifiers converted from the given values.
        /// </summary>
        /// <param name="values">The values to convert.</param>
        /// <returns>The <see cref="HashSet{T}"/>.</returns>
        [PerformanceOptimized]
        internal static HashSet<StrategyId> ConvertToStrategyIds(RedisValue[] values)
        {
            var valuesLength = values.Length;
            var set = new HashSet<StrategyId>(valuesLength);
            for (var i = 0; i < valuesLength; i++)
            {
                set.Add(StrategyId.FromString(values[i]));
            }

            return set;
        }

        /// <summary>
        /// Return a new set of identifiers converted from the given values.
        /// </summary>
        /// <param name="values">The values to convert.</param>
        /// <returns>The <see cref="HashSet{T}"/>.</returns>
        [PerformanceOptimized]
        internal static HashSet<OrderId> ConvertToOrderIds(RedisValue[] values)
        {
            var valuesLength = values.Length;
            var set = new HashSet<OrderId>(valuesLength);
            for (var i = 0; i < valuesLength; i++)
            {
                set.Add(new OrderId(values[i]));
            }

            return set;
        }

        /// <summary>
        /// Return a new set of identifiers converted from the given values.
        /// </summary>
        /// <param name="values">The values to convert.</param>
        /// <returns>The <see cref="HashSet{T}"/>.</returns>
        [PerformanceOptimized]
        internal static HashSet<PositionId> ConvertToPositionIds(RedisValue[] values)
        {
            var valuesLength = values.Length;
            var set = new HashSet<PositionId>(valuesLength);
            for (var i = 0; i < valuesLength; i++)
            {
                set.Add(new PositionId(values[i]));
            }

            return set;
        }
    }
}
