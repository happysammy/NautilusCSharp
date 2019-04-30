//--------------------------------------------------------------------------------------------------
// <copyright file="RollingList{T}.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
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
    /// Provides a fixed-capacity list. The capacity of the internal collection is set at
    /// instantiation, elements can be added to the rolling list. When an element is added which
    /// would exceed the capacity of the rolling list, the element at index[0] is removed.
    /// </summary>
    /// <typeparam name="T">The list value type.</typeparam>
    [PerformanceOptimized]
    public class RollingList<T> : IList<T>
    {
        // Concrete list for performance reasons.
        private readonly List<T> internalList;
        private readonly int capacity;

        /// <summary>
        /// Initializes a new instance of the <see cref="RollingList{T}"/> class.
        /// </summary>
        /// <param name="capacity">The list capacity.</param>
        /// <exception cref="ValidationException">The capacity is less than or equal to zero.</exception>
        public RollingList(int capacity)
        {
            Precondition.PositiveInt32(capacity, nameof(capacity));

            this.internalList = new List<T>(capacity);
            this.capacity = capacity;
        }

        /// <summary>
        /// Gets the number of elements contained in the rolling list.
        /// </summary>
        public int Count => this.internalList.Count;

        /// <summary>
        /// Gets a value indicating whether the rolling list is read-only (always false).
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Returns the element at the given index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <exception cref="NotSupportedException">Throws if called.</exception>
        public T this[int index]
        {
            get => this.internalList[index];
            set => throw new NotSupportedException("Cannot insert at an index of a rolling list.");
        }

        /// <summary>
        /// If the count of elements held by the rolling list equals the capacity,
        /// then this method will first remove the element at index zero, before adding the new
        /// element to the collection.
        /// </summary>
        /// <param name="element">The element to be added.</param>
        public void Add(T element)
        {
            Debug.NotNull(element, nameof(element));

            this.internalList.Add(element);

            while (this.internalList.Count > this.capacity)
            {
                this.internalList.RemoveAt(0);
            }
        }

        /// <summary>
        /// Removes all items from the rolling list.
        /// </summary>
        public void Clear()
        {
            this.internalList.Clear();
        }

        /// <summary>
        /// Returns a result indicating whether the rolling list contains the item.
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
            throw new NotSupportedException("Cannot copy a rolling list to an array.");
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>Not supported.</returns>
        /// <exception cref="NotSupportedException">Throws if called.</exception>
        public bool Remove(T item)
        {
            throw new NotSupportedException("Cannot remove an element from a rolling list.");
        }

        /// <summary>
        /// Determines the index of a specific item in the rolling list.
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
            throw new NotSupportedException("Cannot insert at an index of a rolling list.");
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <exception cref="NotSupportedException">Throws if called.</exception>
        public void RemoveAt(int index)
        {
            throw new NotSupportedException("Cannot remove at an index from a rolling list.");
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
