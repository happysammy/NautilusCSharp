//--------------------------------------------------------------------------------------------------
// <copyright file="CollectionExtensions.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using Nautilus.Core.Correctness;

    /// <summary>
    /// Provides useful generic collection extension methods.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Returns the count of the give enumerable source.
        /// </summary>
        /// <param name="source">The enumerable source.</param>
        /// <returns>The count.</returns>
        /// <typeparam name="T">The enumerable element type.</typeparam>
        public static int Count<T>(this IEnumerable<T> source)
        {
            if (source is ICollection<T> cast)
            {
                return cast.Count;
            }

            var result = 0;
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    result++;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the given array sliced to the given limit.
        /// </summary>
        /// <param name="array">The array to slice.</param>
        /// <param name="limit">The limit for the array slice (>= 0).</param>
        /// <typeparam name="T">The array elements type.</typeparam>
        /// <returns>The sliced array.</returns>
        public static T[] SliceToLimitFromEnd<T>(this T[] array, int limit)
        {
            Debug.NotNegativeInt32(limit, nameof(limit));

            var startIndex = Math.Max(0, array.Length - limit);
            return array[startIndex..];
        }
    }
}
