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
    /// the internal list. If the clone option is used it is a shallow copy only.
    /// </summary>
    /// <typeparam name="T">The list value type.</typeparam>
    [Immutable]
    [PerformanceOptimized]
    public sealed class ReadOnlyList<T> : IList<T>, IReadOnlyList<T>
    {
        // Concrete list for performance reasons.
        private readonly List<T> internalList;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyList{T}"/> class.
        /// </summary>
        /// <param name="list">The original list.</param>
        /// <param name="clone">The optional flag to clone the internal list.</param>
        public ReadOnlyList(List<T> list, bool clone = false)
        {
            Debug.NotNull(list, nameof(list));

            this.internalList = clone
                ? new List<T>(list)
                : list;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyList{T}"/> class.
        /// </summary>
        /// <param name="list">The original list.</param>
        public ReadOnlyList(IList<T> list)
        {
            Debug.NotNull(list, nameof(list));

            this.internalList = new List<T>(list);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyList{T}"/> class.
        /// </summary>
        /// <param name="list">The original list.</param>
        public ReadOnlyList(IReadOnlyCollection<T> list)
        {
            Debug.NotNull(list, nameof(list));

            this.internalList = new List<T>(list);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyList{T}"/> class.
        /// </summary>
        /// <param name="element">The single element for the list to contain.</param>
        public ReadOnlyList(T element)
        {
            Debug.NotNull(element, nameof(element));

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
        /// <exception cref="NotSupportedException">Throws if set is called.</exception>
        public T this[int index]
        {
            get => this.internalList[index];
            set => throw new NotSupportedException("Cannot insert at an index of a read-only list.");
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
        /// Not implemented.
        /// </summary>
        /// <param name="array">\The array.</param>
        /// <param name="arrayIndex">\The array index.</param>
        /// <exception cref="NotSupportedException">Throws if called.</exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            Debug.NotNull(array, nameof(array));
            Debug.NotNegativeInt32(arrayIndex, nameof(arrayIndex));

            this.internalList.CopyTo(array, arrayIndex);
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
