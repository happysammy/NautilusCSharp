// -------------------------------------------------------------------------------------------------
// <copyright file="SetFactory.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Execution.Engine
{
    using System;
    using System.Collections.Generic;
    using Nautilus.Core.Annotations;

    /// <summary>
    /// Provides efficient mathematical set operations. Concrete classes avoid the overhead of LINQ
    /// and interface dispatching.
    /// </summary>
    public static class SetFactory
    {
        /// <summary>
        /// Return a new set of identifiers converted from the given values.
        /// </summary>
        /// <typeparam name="TInput">The input type.</typeparam>
        /// <typeparam name="TOutput">The output type.</typeparam>
        /// <param name="values">The values to convert.</param>
        /// <param name="create">The output creation function.</param>
        /// <returns>The <see cref="HashSet{T}"/>.</returns>
        [PerformanceOptimized]
        public static HashSet<TOutput> ConvertToSet<TInput, TOutput>(TInput[] values, Func<string, TOutput> create)
            where TOutput : class
        {
            var valuesLength = values.Length;
            var set = new HashSet<TOutput>(valuesLength);
            for (var i = 0; i < valuesLength; i++)
            {
                #pragma warning disable 8602
                // TODO: Fix this nullability warning
                set.Add(create(values[i].ToString()));
            }

            return set;
        }

        /// <summary>
        /// Return a new <see cref="HashSet{T}"/> representing the mathematical intersection of the
        /// given sets.
        /// </summary>
        /// <param name="sets">The sets to intersect.</param>
        /// <typeparam name="T">The hash set type.</typeparam>
        /// <returns>The intersection of the given sets as a new set.</returns>
        [PerformanceOptimized]
        public static HashSet<T> Intersection<T>(HashSet<T>[] sets)
        {
            var setsLength = sets.Length;

            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (setsLength == 0)
            {
                return new HashSet<T>();
            }

            if (setsLength == 1)
            {
                return sets[0];
            }

            var intersection = new HashSet<T>(sets[0]);
            for (var i = 1; i < setsLength; i++)
            {
                intersection.IntersectWith(sets[i]);
            }

            return intersection;
        }

        /// <summary>
        /// Return a new <see cref="HashSet{T}"/> representing the mathematical intersection of the
        /// given sets.
        /// </summary>
        /// <param name="sets">The sets to intersect.</param>
        /// <typeparam name="T">The hash set type.</typeparam>
        /// <returns>The intersection of the given sets as a new set.</returns>
        [PerformanceOptimized]
        public static SortedSet<T> IntersectionSorted<T>(HashSet<T>[] sets)
        {
            var setsLength = sets.Length;

            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (setsLength == 0)
            {
                return new SortedSet<T>();
            }

            if (setsLength == 1)
            {
                return new SortedSet<T>(sets[0]);
            }

            var intersection = new SortedSet<T>(sets[0]);
            for (var i = 1; i < setsLength; i++)
            {
                intersection.IntersectWith(sets[i]);
            }

            return intersection;
        }
    }
}
