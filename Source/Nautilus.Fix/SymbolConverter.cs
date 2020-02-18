//---------------------------------------------------------------------------------------------------------------------
// <copyright file="SymbolConverter.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
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
