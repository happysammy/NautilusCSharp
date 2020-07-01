//--------------------------------------------------------------------------------------------------
// <copyright file="ObjectCache.cs" company="Nautech Systems Pty Ltd">
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

using System;
using System.Collections.Generic;
using System.Linq;

namespace Nautilus.Common.Componentry
{
    /// <summary>
    /// Provides an object cache and factory. Note this is not thread-safe.
    /// </summary>
    /// <typeparam name="TKey">The key type for the cache.</typeparam>
    /// <typeparam name="TValue">The value type for the cache.</typeparam>
    public sealed class ObjectCache<TKey, TValue>
        where TKey : class
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
