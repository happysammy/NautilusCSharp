//--------------------------------------------------------------------------------------------------
// <copyright file="ObjectCache.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Componentry
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides an object cache and factory. Note this is not thread-safe.
    /// </summary>
    /// <typeparam name="TKey">The key type for the cache.</typeparam>
    /// <typeparam name="TValue">The value type for the cache.</typeparam>
    public class ObjectCache<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> index;
        private readonly Func<TKey, TValue> creator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectCache{TK, TV}"/> class.
        /// </summary>
        /// <param name="creator">The object value creator for the cache.</param>
        public ObjectCache(Func<TKey, TValue> creator)
        {
            this.index = new Dictionary<TKey, TValue>();
            this.creator = creator;
        }

        /// <summary>
        /// Gets the keys of the cache index.
        /// </summary>
        public ICollection<TKey> Keys => this.index.Keys.ToList();

        /// <summary>
        /// Returns the object from the cache if it exists otherwise create.
        /// </summary>
        /// <param name="key">The key for the cache index.</param>
        /// <returns>The value type.</returns>
        public TValue Get(TKey key)
        {
            if (this.index.TryGetValue(key, out var value))
            {
                return value;
            }

            var created = this.creator(key);
            this.index[key] = created;
            return created;
        }
    }
}
