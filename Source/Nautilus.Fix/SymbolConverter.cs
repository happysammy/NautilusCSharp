//---------------------------------------------------------------------------------------------------------------------
// <copyright file="SymbolConverter.cs" company="Nautech Systems Pty Ltd">
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
//---------------------------------------------------------------------------------------------------------------------

namespace Nautilus.Fix
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Nautilus.Core.Correctness;

    /// <summary>
    /// Provides a converter between Nautilus symbols and broker symbols.
    /// </summary>
    public sealed class SymbolConverter
    {
        private readonly ImmutableDictionary<string, string> indexBrokerNautilus;
        private readonly ImmutableDictionary<string, string> indexNautilusBroker;

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolConverter"/> class.
        /// </summary>
        /// <param name="indexBrokerNautilus">The symbol index broker to nautilus.</param>
        public SymbolConverter(ImmutableDictionary<string, string> indexBrokerNautilus)
        {
            this.indexBrokerNautilus = indexBrokerNautilus;

            var reversed = new Dictionary<string, string>();
            foreach (var (key, value) in indexBrokerNautilus)
            {
                if (value != null)
                {
                    reversed[value] = key;
                }
            }

            this.indexNautilusBroker = reversed.ToImmutableDictionary();

            this.BrokerSymbolCodes = this.indexBrokerNautilus.Keys.ToList();
        }

        /// <summary>
        /// Gets all broker symbol codes.
        /// </summary>
        /// <returns>The collection of broker symbols.</returns>
        public IEnumerable<string> BrokerSymbolCodes { get; }

        /// <summary>
        /// Gets the Nautilus symbol from the given broker symbol (must be contained in the index).
        /// </summary>
        /// <param name="brokerSymbolCode">The broker symbol.</param>
        /// <returns>If successful returns the result, otherwise returns failure result.</returns>
        public string? GetNautilusSymbolCode(string brokerSymbolCode)
        {
            Debug.NotEmptyOrWhiteSpace(brokerSymbolCode, nameof(brokerSymbolCode));

            return this.indexBrokerNautilus.TryGetValue(brokerSymbolCode, out var nautilusSymbolCode)
                ? nautilusSymbolCode
                : null;
        }

        /// <summary>
        /// Gets the broker symbol from the given Nautilus symbol (must be contained in the index).
        /// </summary>
        /// <param name="nautilusSymbolCode">The Nautilus symbol.</param>
        /// <returns>If successful returns the result, otherwise returns failure result.</returns>
        public string? GetBrokerSymbolCode(string nautilusSymbolCode)
        {
            Debug.NotEmptyOrWhiteSpace(nautilusSymbolCode, nameof(nautilusSymbolCode));

            return this.indexNautilusBroker.TryGetValue(nautilusSymbolCode, out var brokerSymbolCode)
                ? brokerSymbolCode
                : null;
        }
    }
}
