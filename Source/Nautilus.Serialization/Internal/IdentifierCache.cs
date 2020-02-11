// -------------------------------------------------------------------------------------------------
// <copyright file="IdentifierCache.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization.Internal
{
    using MsgPack;
    using Nautilus.Common.Componentry;
    using Nautilus.DomainModel.Identifiers;

    /// <summary>
    /// Provides an identifier cache.
    /// </summary>
    public sealed class IdentifierCache
    {
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
        /// Returns a TraderId extracted from the given <see cref="MessagePackObjectDictionary"/>.
        /// </summary>
        /// <param name="unpacked">The dictionary to extract from.</param>
        /// <returns>The extracted TraderId.</returns>
        internal TraderId TraderId(MessagePackObjectDictionary unpacked)
        {
            return this.cachedTraderIds.Get(unpacked[nameof(this.TraderId)].AsString());
        }

        /// <summary>
        /// Returns an AccountId extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted AccountId.</returns>
        internal AccountId AccountId(MessagePackObjectDictionary unpacked)
        {
            return this.cachedAccountIds.Get(unpacked[nameof(this.AccountId)].AsString());
        }

        /// <summary>
        /// Returns a StrategyId extracted from the given <see cref="MessagePackObjectDictionary"/>.
        /// </summary>
        /// <param name="unpacked">The dictionary to extract from.</param>
        /// <returns>The extracted StrategyId.</returns>
        internal StrategyId StrategyId(MessagePackObjectDictionary unpacked)
        {
            return this.cachedStrategyIds.Get(unpacked[nameof(this.StrategyId)].AsString());
        }

        /// <summary>
        /// Returns a Symbol extracted from the given <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="unpacked">The MessagePack object to extract from.</param>
        /// <returns>The extracted Symbol.</returns>
        internal Symbol Symbol(MessagePackObjectDictionary unpacked)
        {
            return this.cachedSymbols.Get(unpacked[nameof(this.Symbol)].AsString());
        }
    }
}
