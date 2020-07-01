// -------------------------------------------------------------------------------------------------
// <copyright file="IdentifierCache.cs" company="Nautech Systems Pty Ltd">
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
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using Nautilus.Common.Componentry;
using Nautilus.DomainModel.Identifiers;

namespace Nautilus.Serialization.MessageSerializers.Internal
{
    /// <summary>
    /// Provides an identifier cache.
    /// </summary>
    public sealed class IdentifierCache
    {
        private static readonly Func<byte[], string> Decode = Encoding.UTF8.GetString;

        private readonly ObjectCache<string, TraderId> cachedTraderIds;
        private readonly ObjectCache<string, AccountId> cachedAccountIds;
        private readonly ObjectCache<string, StrategyId> cachedStrategyIds;
        private readonly ObjectCache<string, Symbol> cachedSymbols;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentifierCache"/> class.
        /// </summary>
        public IdentifierCache()
        {
            this.cachedTraderIds = new ObjectCache<string, TraderId>(Nautilus.DomainModel.Identifiers.TraderId.FromString);
            this.cachedAccountIds = new ObjectCache<string, AccountId>(Nautilus.DomainModel.Identifiers.AccountId.FromString);
            this.cachedStrategyIds = new ObjectCache<string, StrategyId>(Nautilus.DomainModel.Identifiers.StrategyId.FromString);
            this.cachedSymbols = new ObjectCache<string, Symbol>(Nautilus.DomainModel.Identifiers.Symbol.FromString);
        }

        /// <summary>
        /// Returns a TraderId extracted from the given unpacked dictionary.
        /// </summary>
        /// <param name="unpacked">The dictionary to extract from.</param>
        /// <returns>The extracted TraderId.</returns>
        internal TraderId TraderId(Dictionary<string, byte[]> unpacked)
        {
            return this.cachedTraderIds.Get(Decode(unpacked[nameof(this.TraderId)]));
        }

        /// <summary>
        /// Returns an AccountId extracted from the given unpacked dictionary.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted AccountId.</returns>
        internal AccountId AccountId(Dictionary<string, byte[]> unpacked)
        {
            return this.cachedAccountIds.Get(Decode(unpacked[nameof(this.AccountId)]));
        }

        /// <summary>
        /// Returns a StrategyId extracted from the given unpacked dictionary.
        /// </summary>
        /// <param name="unpacked">The dictionary to extract from.</param>
        /// <returns>The extracted StrategyId.</returns>
        internal StrategyId StrategyId(Dictionary<string, byte[]> unpacked)
        {
            return this.cachedStrategyIds.Get(Decode(unpacked[nameof(this.StrategyId)]));
        }

        /// <summary>
        /// Returns a Symbol extracted from the given unpacked dictionary.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted Symbol.</returns>
        internal Symbol Symbol(Dictionary<string, byte[]> unpacked)
        {
            return this.cachedSymbols.Get(Decode(unpacked[nameof(this.Symbol)]));
        }
    }
}
