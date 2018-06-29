//--------------------------------------------------------------------------------------------------
// <copyright file="ReadOnlyList{T}.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;

    /// <summary>
    /// Provides a read-only list instantiated with a standard concrete list which then becomes
    /// the internal list. The original list cannot be null or empty.
    /// </summary>
    /// <typeparam name="T">The list value type.</typeparam>
    [Immutable]
    [PerformanceOptimized]
    public sealed class ReadOnlyList<T> : IList<T>
    {
        // Concrete list for performance reasons.
        private readonly List<T> internalList;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyList{T}"/> class.
        /// </summary>
        /// <param name="list">The original list.</param>
        /// <exception cref="ValidationException">Throws if the list is null or empty.</exception>
        public ReadOnlyList(List<T> list)
        {
            Validate.CollectionNotNullOrEmpty(list, nameof(list));

            this.internalList = list;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyList{T}"/> class.
        /// </summary>
        /// <param name="element">The element for the list to contain.</param>
        /// <exception cref="ValidationException">Throws if the element is null.</exception>
        public ReadOnlyList(T element)
        {
            Validate.NotNull(element, nameof(element));

            this.internalList = new List<T> { element };
        }

        /// <summary>
        /// Gets the number of elements contained in the read-only list.
        /// </summary>
        public int Count => this.internalList.Count;

        /// <summary>
        /// Gets a value indicating whether the read-only list is read-only (always true).
        /// </summary>
        public bool IsReadOnly => true;

        /// <summary>
        /// Returns the element at the given index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <exception cref="NotSupportedException">Throws if called.</exception>
        public T this[int index]
        {
            get => this.internalList[index];
            set => throw new NotSupportedException("Cannot insert at an index of a read-only list.");
        }

        /// <summary>
        /// Not implemented (cannot add to a read-only list).
        /// </summary>
        /// <param name="element">The element which cannot be added.</param>
        /// <exception cref="NotImplementedException">Throws if called.</exception>
        public void Add(T element)
        {
            throw new NotSupportedException("Cannot add to a read-only list.");
        }

        /// <summary>
        /// Not implemented (cannot clear a read-only list).
        /// </summary>
        /// <exception cref="NotImplementedException">Throws if called.</exception>
        public void Clear()
        {
            throw new NotSupportedException("Cannot clear a read-only list.");
        }

        /// <summary>
        /// Returns a result indicating whether the read-only contains the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public bool Contains(T item)
        {
            Debug.NotNull(item, nameof(item));

            return this.internalList.Contains(item);
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="array">\The array.</param>
        /// <param name="arrayIndex">\The array index.</param>
        /// <exception cref="NotSupportedException">Throws if called.</exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotSupportedException("Cannot copy a read-only list to an array.");
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>Not supported.</returns>
        /// <exception cref="NotSupportedException">Throws if called.</exception>
        public bool Remove(T item)
        {
            throw new NotSupportedException("Cannot remove an element from a read-only list.");
        }

        /// <summary>
        /// Determines the index of a specific item in the read-only list.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>An <see cref="int"/>.</returns>
        public int IndexOf(T item)
        {
            Debug.NotNull(item, nameof(item));

            return this.internalList.IndexOf(item);
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        /// <exception cref="NotSupportedException">Throws if called.</exception>
        public void Insert(int index, T item)
        {
            throw new NotSupportedException("Cannot insert at an index of a read-only list.");
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <exception cref="NotSupportedException">Throws if called.</exception>
        public void RemoveAt(int index)
        {
            throw new NotSupportedException("Cannot remove at an index from a read-only list.");
        }

        /// <summary>
        /// Returns an enumerator from the internal list, which iterates through the collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}"/>.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator which iterates through the collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/>.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
