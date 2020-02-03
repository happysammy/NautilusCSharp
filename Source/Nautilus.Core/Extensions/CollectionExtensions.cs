//--------------------------------------------------------------------------------------------------
// <copyright file="CollectionExtensions.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Extensions
{
    using System.Collections.Generic;

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
    }
}
