// -------------------------------------------------------------------------------------------------
// <copyright file="SetFactory.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Execution
{
    using System.Collections.Generic;
    using Nautilus.Core.Annotations;

    /// <summary>
    /// Provides efficient mathematical set operations. Concrete classes avoid the overhead of LINQ
    /// and interface dispatching.
    /// </summary>
    public class SetFactory
    {
        /// <summary>
        /// Return a new <see cref="HashSet{T}"/> representing the mathematical intersection of the
        /// given sets.
        /// </summary>
        /// <param name="setA">The first set.</param>
        /// <param name="setB">The second set.</param>
        /// <typeparam name="T">The hash set type.</typeparam>
        /// <returns>The intersection of the given sets as a new set.</returns>
        [PerformanceOptimized]
        public static HashSet<T> Intersection<T>(HashSet<T> setA, HashSet<T> setB)
        {
            HashSet<T> intersection;

            if (setA.Count <= setB.Count)
            {
                intersection = new HashSet<T>(setA);
                intersection.IntersectWith(setB);
            }
            else
            {
                intersection = new HashSet<T>(setB);
                intersection.IntersectWith(setA);
            }

            return intersection;
        }
    }
}
