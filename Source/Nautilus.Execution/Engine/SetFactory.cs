// -------------------------------------------------------------------------------------------------
// <copyright file="SetFactory.cs" company="Nautech Systems Pty Ltd">
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

using System;
using System.Collections.Generic;
using Nautilus.Core.Annotations;

namespace Nautilus.Execution.Engine
{
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
                var input = values[i];

                if (input != null)
                {
                    set.Add(create(input.ToString()!));
                }
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

            switch (setsLength)
            {
                case 0:
                    return new HashSet<T>();
                case 1:
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

            switch (setsLength)
            {
                case 0:
                    return new SortedSet<T>();
                case 1:
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
