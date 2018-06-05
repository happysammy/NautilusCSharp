//--------------------------------------------------------------------------------------------------
// <copyright file="RollingList.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the Apache 2.0 license
//  as found in the LICENSE.txt file.
//  https://github.com/nautechsystems/Nautilus.Core
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Nautilus.Core.Validation;

    /// <summary>
    /// Implements a fixed-capacity List, which uses a collection of objects to store the
    /// elements. The capacity of the internal collection is set at instantiation,
    /// elements can be added to the RollingList. When an element is added which
    /// would exceed the capacity of the RollingList, the element at index[0] is removed.
    /// </summary>
    /// <typeparam name="T">The list type.</typeparam>
    public class RollingList<T> : IList<T>
    {
        private readonly int capacity;
        private readonly IList<T> internalList;

        /// <summary>
        /// Initializes a new instance of the <see cref="RollingList{T}"/> class.
        /// </summary>
        /// <param name="capacity">The list capacity.</param>
        /// <exception cref="ValidationException">The capacity is less than or equal to zero.</exception>
        public RollingList(int capacity)
        {
            Validate.Int32NotOutOfRange(capacity, nameof(capacity), 0, int.MaxValue, RangeEndPoints.LowerExclusive);

            this.capacity = capacity;

            this.internalList = new List<T>(capacity);
        }

        /// <summary>
        /// Gets the number of elements contained in the RollingList.
        /// </summary>
        public int Count => this.internalList.Count;

        /// <summary>
        /// Gets a value indicating whether the RollingList is read-only.
        /// </summary>
        public bool IsReadOnly => this.internalList.IsReadOnly;

        /// <summary>
        /// Returns the element at the given index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <exception cref="NotSupportedException">Throws if there is an attempt to set an element
        /// of the <see cref="RollingList{T}"/> by index.</exception>
        /// <exception cref="ValidationException" accessor="get">Throws if the value is out of the
        /// specified range.</exception>
        public T this[int index]
        {
            get
            {
                Debug.Int32NotOutOfRange(index, nameof(index), 0, int.MaxValue);

                lock (this.internalList)
                {
                    return this.internalList[index];
                }
            }

            set => throw new NotSupportedException("Cannot insert at an index of a rolling list.");
        }

        /// <summary>
        /// If the count of elements held by the <see cref="RollingList{T}"/> equals the capacity,
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
        /// Removes all items from the RollingList.
        /// </summary>
        public void Clear()
        {
            this.internalList.Clear();

            Debug.CollectionEmpty(this.internalList, nameof(this.internalList));
        }

        /// <summary>
        /// Returns a result indicating whether the RollingList contains the item.
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
        /// <exception cref="NotSupportedException">Throws if there is an attempt to copy the
        /// <see cref="RollingList{T}"/> to an <see cref="Array"/>.</exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotSupportedException("Cannot copy a RollingList to an array.");
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <exception cref="NotSupportedException">Throws if there is an attempt to remove an
        /// element from the rolling list.</exception>
        /// <returns>A <see cref="bool"/>.</returns>
        public bool Remove(T item)
        {
            throw new NotSupportedException("Cannot remove an element from a RollingList.");
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

        /// <summary>
        /// Determines the index of a specific item in the RollingList.
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
        /// <exception cref="NotSupportedException">Throws if there is an attempt to insert an
        /// element at a specified index.</exception>
        public void Insert(int index, T item)
        {
            throw new NotSupportedException("Cannot insert at an index of a RollingList.");
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <exception cref="NotSupportedException">Throws if there is an attempt to remove an
        /// element at a specified index.</exception>
        public void RemoveAt(int index)
        {
            throw new NotSupportedException("Cannot remove at an index from a RollingList.");
        }
    }
}
