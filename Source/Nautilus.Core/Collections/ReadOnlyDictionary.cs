//--------------------------------------------------------------------------------------------------
// <copyright file="ReadOnlyDictionary.cs" company="Nautech Systems Pty Ltd">
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
    using System.Linq;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;

    /// <summary>
    /// Provides a read-only dictionary instantiated with a standard concrete dictionary which then
    /// becomes the internal dictionary.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    [Immutable]
    [PerformanceOptimized]
    public sealed class ReadOnlyDictionary<TKey, TValue>
        : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    {
        // Concrete dictionary for performance reasons.
        private readonly Dictionary<TKey, TValue> internalDictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyDictionary{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="dictionary">The original dictionary.</param>
        /// <exception cref="ValidationException">Throws if the dictionary is null.</exception>
        public ReadOnlyDictionary(Dictionary<TKey, TValue> dictionary)
        {
            Debug.NotNull(dictionary, nameof(dictionary));

            this.internalDictionary = dictionary;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyDictionary{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="dictionary">The original read-only dictionary.</param>
        /// <exception cref="ValidationException">Throws if the dictionary is null.</exception>
        public ReadOnlyDictionary(IReadOnlyDictionary<TKey, TValue> dictionary)
        {
            Debug.NotNull(dictionary, nameof(dictionary));

            this.internalDictionary = new Dictionary<TKey, TValue>(dictionary);
        }

        /// <summary>
        /// Gets the read-only dictionaries keys.
        /// </summary>
        public ICollection<TKey> Keys => this.internalDictionary.Keys;

        /// <summary>
        /// Gets the read-only dictionaries values.
        /// </summary>
        public ICollection<TValue> Values => this.internalDictionary.Values;

        /// <summary>
        /// Gets the enumerable for the read-only dictionaries keys.
        /// </summary>
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => this.Keys;

        /// <summary>
        /// Gets the enumerable for the read-only dictionaries values.
        /// </summary>
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => this.Values;

        /// <summary>
        /// Gets the count of the read-only dictionary.
        /// </summary>
        public int Count => this.internalDictionary.Count;

        /// <summary>
        /// Gets a value indicating whether the read-only dictionary is read-only (always true).
        /// </summary>
        public bool IsReadOnly => true;

        /// <summary>
        /// Returns the value associated with the given key.
        /// </summary>
        /// <param name="key">The key.</param>
        public TValue this[TKey key]
        {
            get => this.internalDictionary[key];
            set => throw new NotSupportedException("Cannot set an index in read-only dictionary.");
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key to get.</param>
        /// <param name="value">The value to return to.</param>
        /// <returns>The value found for the given key.</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            Debug.NotNull(key, nameof(key));

            return this.internalDictionary.TryGetValue(key, out value);
        }

        /// <summary>
        /// Returns a value indicating whether the read-only dictionary contains the given item.
        /// </summary>
        /// <param name="item">The item to check if the read-only dictionary contains.</param>
        /// <returns>True if the read-only dictionary contains the given item, otherwise
        /// false.</returns>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            Debug.NotNull(item, nameof(item));

            return this.internalDictionary.Contains(item);
        }

        /// <summary>
        /// Returns a value indicating whether the read-only dictionary contains the given key.
        /// </summary>
        /// <param name="key">The key to check if the read-only dictionary contains.</param>
        /// <returns>True if the read-only dictionary contains the given key, otherwise
        /// false.</returns>
        public bool ContainsKey(TKey key)
        {
            Debug.NotNull(key, nameof(key));

            return this.internalDictionary.ContainsKey(key);
        }

        /// <summary>
        /// Returns a value indicating whether the read-only dictionary contains the given value.
        /// </summary>
        /// <param name="value">The value to check if the read-only dictionary contains.</param>
        /// <returns>True if the read-only dictionary contains the given value, otherwise
        /// false.</returns>
        public bool ContainsValue(TValue value)
        {
            Debug.NotNull(value, nameof(value));

            return this.internalDictionary.ContainsValue(value);
        }

        /// <summary>
        /// Not implemented (cannot add to a read-only dictionary).
        /// </summary>
        /// <param name="item">The item which cannot be added.</param>
        /// <exception cref="NotImplementedException">Throws if called.</exception>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException("Cannot add to a read-only dictionary.");
        }

        /// <summary>
        /// Not implemented (cannot add to a read-only dictionary).
        /// </summary>
        /// <param name="key">The key which cannot be added.</param>
        /// <param name="value">The value which cannot be added.</param>
        /// <exception cref="NotImplementedException">Throws if called.</exception>
        public void Add(TKey key, TValue value)
        {
            throw new NotSupportedException("Cannot add to a read-only dictionary.");
        }

        /// <summary>
        /// Not implemented (cannot remove from a read-only dictionary).
        /// </summary>
        /// <param name="item">The item which cannot be removed.</param>
        /// <returns>Not implemented.</returns>
        /// <exception cref="NotImplementedException">Throws if called.</exception>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException("Cannot remove an item from a read-only dictionary.");
        }

        /// <summary>
        /// Not implemented (cannot remove from a read-only dictionary).
        /// </summary>
        /// <param name="key">The key which cannot be removed.</param>
        /// <returns>Not supported.</returns>
        /// <exception cref="NotSupportedException">Throws if called.</exception>
        public bool Remove(TKey key)
        {
            throw new NotSupportedException("Cannot remove a key from a read-only dictionary.");
        }

        /// <summary>
        /// Not implemented (cannot clear a read-only dictionary).
        /// </summary>
        /// <exception cref="NotImplementedException">Throws if called.</exception>
        public void Clear()
        {
            throw new NotSupportedException("Cannot clear a read-only dictionary.");
        }

        /// <summary>
        /// Not implemented (cannot copy to a read-only dictionary).
        /// </summary>
        /// <param name="array">The array which cannot be copied.</param>
        /// <param name="arrayIndex">The array index.</param>
        /// <exception cref="NotImplementedException">Throws if called.</exception>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotSupportedException("Cannot copy to array from a read-only dictionary.");
        }

        /// <summary>
        /// Returns an enumerator which iterates through the read-only dictionary.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return this.internalDictionary.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator which iterates through the read-only dictionary.
        /// </summary>
        /// <returns>The enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
