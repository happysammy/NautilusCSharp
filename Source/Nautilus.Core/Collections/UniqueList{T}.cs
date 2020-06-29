//--------------------------------------------------------------------------------------------------
// <copyright file="UniqueList{T}.cs" company="Nautech Systems Pty Ltd">
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
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Collections
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a strongly typed list of unique objects which preserves insertion order.
    /// </summary>
    /// <typeparam name="T">The type of the objects.</typeparam>
    public sealed class UniqueList<T> : List<T>
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UniqueList{T}"/> class.
        /// </summary>
        public UniqueList()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UniqueList{T}"/> class.
        /// </summary>
        /// <param name="capacity">The initial capacity for the list.</param>
        public UniqueList(int capacity)
            : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UniqueList{T}"/> class.
        /// </summary>
        /// <param name="initial">The initial element for the list.</param>
        public UniqueList(T initial)
        {
            base.Add(initial);
        }

        private UniqueList(IEnumerable<T> elements)
            : base(elements)
        {
        }

        /// <summary>
        /// Returns the element at the given index or sets if the value is unique.
        /// </summary>
        /// <param name="index">The index.</param>
        public new T this[int index]
        {
            get => base[index];
            set => throw new InvalidOperationException("Cannot set an element of a UniqueList<T> by index.");
        }

        /// <summary>
        /// Add the given element to the end of the list (if the element is unique to the list).
        /// </summary>
        /// <param name="element">The element to add (if unique).</param>
        public new void Add(T element)
        {
            if (!this.Contains(element))
            {
                base.Add(element);
            }
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the list (for the elements
        /// which are unique to the list).
        /// </summary>
        /// <param name="collection">The collection to add.</param>
        public new void AddRange(IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                if (!this.Contains(item))
                {
                    base.Add(item);
                }
            }
        }

        /// <summary>
        /// Inserts the given element into the list at the specified index (if the element is unique
        /// to the list).
        /// </summary>
        /// <param name="index">The index of the item to insert.</param>
        /// <param name="element">The element to add (if unique).</param>
        public new void Insert(int index, T element)
        {
            if (!this.Contains(element))
            {
                base.Insert(index, element);
            }
        }

        /// <summary>
        /// Inserts the given elements of the specified collection into the list at the specified
        /// index (for the elements which are unique to the list).
        /// </summary>
        /// <param name="index">The index of the item to insert.</param>
        /// <param name="collection">The collection to insert.</param>
        public new void InsertRange(int index, IEnumerable<T> collection)
        {
            var counter = 0;
            foreach (var item in collection)
            {
                if (!this.Contains(item))
                {
                    base.Insert(index + counter, item);
                    counter++;
                }
            }
        }

        /// <summary>
        /// Return the first element of the list if not empty (otherwise throws exception).
        /// </summary>
        /// <returns>The first element.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the list is empty.</exception>
        public T First()
        {
            return base[0];
        }

        /// <summary>
        /// Return the first element of the list if not empty (otherwise returns null).
        /// </summary>
        /// <returns>The first element or null.</returns>
        public T? FirstOrNull()
        {
            return this.Count == 0
                ? null
                : base[0];
        }

        /// <summary>
        /// Return the last element of the list if not empty (otherwise throws exception).
        /// </summary>
        /// <returns>The last element.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the list is empty.</exception>
        public T Last()
        {
            return base[^1];
        }

        /// <summary>
        /// Return the last element of the list if not empty (otherwise returns null).
        /// </summary>
        /// <returns>The last element or null.</returns>
        public T? LastOrNull()
        {
            return this.Count == 0
                ? null
                : base[^1];
        }

        /// <summary>
        /// Returns a copy of this list.
        /// </summary>
        /// <returns>The copy.</returns>
        public UniqueList<T> Copy()
        {
            return new UniqueList<T>(this);
        }
    }
}
