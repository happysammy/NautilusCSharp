//--------------------------------------------------------------------------------------------------
// <copyright file="CollectionExtensions.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;

    /// <summary>
    /// Provides useful generic collection extension methods.
    /// </summary>
    [Immutable]
    public static class CollectionExtensions
    {
        /// <summary>
        /// Returns the value of the last collection index.
        /// </summary>
        /// <param name="collection">The collection (cannot be null or empty).</param>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <exception cref="ValidationException">Throws if the collection is null or empty.</exception>
        /// <returns>An <see cref="int"/>.</returns>
        public static int LastIndex<T>(this ICollection<T> collection)
        {
            Debug.CollectionNotNullOrEmpty(collection, nameof(collection));

            return collection.Count - 1;
        }

        /// <summary>
        /// Returns the element at the given reversed index of a collection.
        /// </summary>
        /// <param name="collection">The collection (cannot be null or empty).</param>
        /// <param name="index">The index (cannot be negative).</param>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <exception cref="ValidationException">Throws if the collection is null or empty.</exception>
        /// <exception cref="ValidationException">Throws if the index is out of the specified range.</exception>
        /// <returns>The type.</returns>
        public static T GetByReverseIndex<T>(this ICollection<T> collection, int index)
        {
            Debug.CollectionNotNullOrEmpty(collection, nameof(collection));
            Debug.Int32NotOutOfRange(index, nameof(index), 0, collection.LastIndex());

            return collection.ElementAtOrDefault(collection.LastIndex() - index);
        }

        /// <summary>
        /// Returns the element at the given shifted reverse index of a collection.
        /// </summary>
        /// <param name="collection">The collection (cannot be null or empty).</param>
        /// <param name="index">The index (cannot be negative).</param>
        /// <param name="shift">The shift (cannot be negative).</param>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <returns>The type.</returns>
        /// <exception cref="ValidationException">Throws if the collection is null or empty.</exception>
        /// <exception cref="ValidationException">Throws if the index is out of the specified range.</exception>
        /// <exception cref="ValidationException">Throws if the shift is out of the specified range.</exception>
        public static T GetByShiftedReverseIndex<T>(this ICollection<T> collection, int index, int shift)
        {
            Debug.CollectionNotNullOrEmpty(collection, nameof(collection));
            Debug.Int32NotOutOfRange(index, nameof(index), 0, collection.LastIndex());
            Debug.Int32NotOutOfRange(shift, nameof(shift), 0, collection.LastIndex());
            Debug.Int32NotOutOfRange(index + shift, nameof(index) + nameof(shift), 0, collection.LastIndex());

            return collection.ElementAt(collection.LastIndex() - index - shift);
        }

        /// <summary>
        /// Performs the action on each element in the <see cref="IEnumerable{T}"/> source.
        /// </summary>
        /// <param name="source">The source (cannot be null).</param>
        /// <param name="action">The action (cannot be null).</param>
        /// <typeparam name="T">The type.</typeparam>
        /// <exception cref="ValidationException">Throws if either argument is null.</exception>
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration", Justification = "Reviewed. Multiple enumerations ok in Debug configuration.")]
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            // Multiple enumerations ok in Debug configuration.
            Debug.NotNull(source, nameof(source));
            Debug.NotNull(action, nameof(action));

            foreach (var element in source)
            {
                action(element);
            }
        }
    }
}
