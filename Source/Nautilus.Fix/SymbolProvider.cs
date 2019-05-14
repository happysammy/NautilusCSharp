//---------------------------------------------------------------------------------------------------------------------
// <copyright file="SymbolProvider.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//---------------------------------------------------------------------------------------------------------------------

namespace Nautilus.Fix
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.CQS;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Provides data for tradeable instruments.
    /// </summary>
    public class SymbolProvider
    {
        private readonly ImmutableList<Symbol> symbols;
        private readonly ImmutableDictionary<string, string> symbolIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolProvider"/> class.
        /// </summary>
        /// <param name="venue">The instrument data providers venue.</param>
        /// <param name="symbolIndex">The CSV data file name.</param>
        public SymbolProvider(Venue venue, IReadOnlyDictionary<string, string> symbolIndex)
        {
            this.symbolIndex = symbolIndex.ToImmutableDictionary();
            this.symbols = this.symbolIndex
                .Values
                .Select(symbol => new Symbol(symbol, venue))
                .ToImmutableList();
        }

        /// <summary>
        /// Gets the Nautilus symbol from the given broker symbol, loaded from the brokerage
        /// instrument data CSV file (must be contained in the index).
        /// </summary>
        /// <param name="brokerSymbol">The broker symbol.</param>
        /// <returns>If successful returns the result, otherwise returns failure result.</returns>
        public QueryResult<string> GetNautilusSymbol(string brokerSymbol)
        {
            Debug.NotEmptyOrWhiteSpace(brokerSymbol, nameof(brokerSymbol));

            return this.symbolIndex.ContainsKey(brokerSymbol)
                ? QueryResult<string>.Ok(this.symbolIndex[brokerSymbol])
                : QueryResult<string>.Fail(
                    $"Cannot find the Nautilus symbol (index did not contain the given broker symbol {brokerSymbol}).");
        }

        /// <summary>
        /// Gets the broker symbol from the given Nautilus symbol, loaded from the brokerage
        /// instrument data CSV file (must be contained in the index).
        /// </summary>
        /// <param name="nautilusSymbol">The Nautilus symbol.</param>
        /// <returns>If successful returns the result, otherwise returns failure result.</returns>
        public QueryResult<string> GetBrokerSymbol(string nautilusSymbol)
        {
            Debug.NotEmptyOrWhiteSpace(nautilusSymbol, nameof(nautilusSymbol));

            return this.symbolIndex.ContainsValue(nautilusSymbol)
                ? QueryResult<string>.Ok(this.symbolIndex
                    .FirstOrDefault(x => x.Value == nautilusSymbol).Key)
                : QueryResult<string>.Fail(
                    $"Cannot find the broker symbol (index did not contain the given Nautilus symbol {nautilusSymbol}.");
        }

        /// <summary>
        /// Gets all Nautilus symbols, loaded from the brokerage instrument data CSV file.
        /// </summary>
        /// <returns>The list of broker symbols.</returns>
        public IEnumerable<Symbol> GetAllSymbols()
        {
            return this.symbols;
        }

        /// <summary>
        /// Gets all broker symbols, loaded from the brokerage instrument data CSV file.
        /// </summary>
        /// <returns>The list of broker symbols.</returns>
        public IEnumerable<string> GetAllBrokerSymbols()
        {
            return this.symbolIndex.Keys;
        }
    }
}
