//---------------------------------------------------------------------------------------------------------------------
// <copyright file="SymbolMapper.cs" company="Nautech Systems Pty Ltd">
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

using System.Collections.Generic;
using System.Linq;
using Nautilus.Core.Correctness;
using Nautilus.Core.Extensions;

namespace Nautilus.Fix
{
    /// <summary>
    /// Provides a converter between Nautilus symbols and broker symbols.
    /// </summary>
    public sealed class SymbolMapper
    {
        private readonly Dictionary<string, string> mapBrokerToNautilus;
        private readonly Dictionary<string, string> mapNautilusToBroker;

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolMapper"/> class.
        /// </summary>
        public SymbolMapper()
        {
            this.mapBrokerToNautilus = new Dictionary<string, string>();
            this.mapNautilusToBroker = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets all broker symbol codes.
        /// </summary>
        /// <returns>The collection of broker symbols.</returns>
        public IEnumerable<string> BrokerSymbolCodes => this.mapBrokerToNautilus.Keys.ToList();

        /// <summary>
        /// Maps the given broker code to a Nautilus code.
        /// </summary>
        /// <param name="brokerCode">The broker code.</param>
        public void MapBrokerCode(string brokerCode)
        {
            Debug.NotEmptyOrWhiteSpace(brokerCode, nameof(brokerCode));

            var nautilusCode = brokerCode.ToAlphaNumeric().ToUpper();

            this.mapBrokerToNautilus[brokerCode] = nautilusCode;
            this.mapNautilusToBroker[nautilusCode] = brokerCode;
        }

        /// <summary>
        /// Gets the broker code from the given Nautilus code (must be contained in the map).
        /// </summary>
        /// <param name="nautilusCode">The Nautilus code.</param>
        /// <returns>The broker code (if found).</returns>
        public string GetBrokerCode(string nautilusCode)
        {
            Debug.NotEmptyOrWhiteSpace(nautilusCode, nameof(nautilusCode));

            return this.mapNautilusToBroker[nautilusCode];
        }

        /// <summary>
        /// Gets the Nautilus code from the given broker code (must be contained in the map).
        /// </summary>
        /// <param name="brokerCode">The broker code.</param>
        /// <returns>The Nautilus code (if found).</returns>
        public string GetNautilusCode(string brokerCode)
        {
            Debug.NotEmptyOrWhiteSpace(brokerCode, nameof(brokerCode));

            return this.mapBrokerToNautilus[brokerCode];
        }
    }
}
