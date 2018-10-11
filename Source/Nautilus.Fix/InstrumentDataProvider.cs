//---------------------------------------------------------------------------------------------------------------------
// <copyright file="InstrumentDataProvider.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//---------------------------------------------------------------------------------------------------------------------

namespace Nautilus.Fix
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using CsvHelper;
    using Nautilus.Core.Collections;
    using Nautilus.Core.CQS;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Provides data for tradeable instruments.
    /// </summary>
    public class InstrumentDataProvider
    {
        private readonly Venue venue;
        private readonly ReadOnlyDictionary<string, string> symbolIndex;
        private readonly ReadOnlyDictionary<string, int> priceDecimalPrecisionIndex;
        private readonly ReadOnlyDictionary<string, decimal> tickValueIndex;
        private readonly ReadOnlyDictionary<string, decimal> targetSpreadIndex;
        private readonly ReadOnlyDictionary<string, decimal> marginRequirementIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstrumentDataProvider"/> class.
        /// </summary>
        /// <param name="venue">The instrument data providers venue.</param>
        /// <param name="csvDataFileName">The CSV data file name.</param>
        public InstrumentDataProvider(Venue venue, string csvDataFileName)
        {
            this.venue = venue;
            var assemblyDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            var csvDataFilePath = Path.GetFullPath(Path.Combine(assemblyDirectory, csvDataFileName));

            this.symbolIndex = LoadData<string, string>(csvDataFilePath, 0, 1);
            this.priceDecimalPrecisionIndex = LoadData<string, int>(csvDataFilePath, 1, 2);
            this.tickValueIndex = LoadData<string, decimal>(csvDataFilePath, 0, 3);
            this.targetSpreadIndex = LoadData<string, decimal>(csvDataFilePath, 0, 4);
            this.marginRequirementIndex = LoadData<string, decimal>(csvDataFilePath, 0, 5);
        }

        /// <summary>
        /// Gets the Nautilus symbol from the given broker symbol, loaded from the brokerage
        /// instrument data CSV file (must be contained in the index).
        /// </summary>
        /// <param name="brokerSymbol">The broker symbol.</param>
        /// <returns>If successful returns the result, otherwise returns failure result.</returns>
        public QueryResult<string> GetNautilusSymbol(string brokerSymbol)
        {
            Debug.NotNull(brokerSymbol, nameof(brokerSymbol));

            return this.symbolIndex.ContainsKey(brokerSymbol)
                 ? QueryResult<string>.Ok(this.symbolIndex[brokerSymbol])
                 : QueryResult<string>.Fail($"Cannot find the Nautilus symbol (index did not contain the given broker symbol {brokerSymbol}).");
        }

        /// <summary>
        /// Gets the broker symbol from the given Nautilus symbol, loaded from the brokerage
        /// instrument data CSV file (must be contained in the index).
        /// </summary>
        /// <param name="nautilusSymbol">The Nautilus symbol.</param>
        /// <returns>If successful returns the result, otherwise returns failure result.</returns>
        public QueryResult<string> GetBrokerSymbol(string nautilusSymbol)
        {
            Debug.NotNull(nautilusSymbol, nameof(nautilusSymbol));

            return this.symbolIndex.ContainsValue(nautilusSymbol)
                 ? QueryResult<string>.Ok(this.symbolIndex.FirstOrDefault(x => x.Value == nautilusSymbol).Key)
                 : QueryResult<string>.Fail($"Cannot find the broker symbol (index did not contain the given Nautilus symbol {nautilusSymbol}.");
        }

        /// <summary>
        /// Gets all Nautilus symbols, loaded from the brokerage instrument data CSV file.
        /// </summary>
        /// <returns>The list of broker symbols.</returns>
        public ReadOnlyList<Symbol> GetAllSymbols()
        {
            var symbols = this.symbolIndex.Values
                .Select(symbol => new Symbol(symbol, this.venue))
                .ToList();

            return new ReadOnlyList<Symbol>(symbols);
        }

        /// <summary>
        /// Gets all broker symbols, loaded from the brokerage instrument data CSV file.
        /// </summary>
        /// <returns>The list of broker symbols.</returns>
        public ReadOnlyList<string> GetAllBrokerSymbols()
        {
            var brokerSymbols = this.symbolIndex.Keys.ToList();

            return new ReadOnlyList<string>(brokerSymbols);
        }

        /// <summary>
        /// Gets the price decimal precision index (key is Nautilus symbol), loaded from the brokerage instrument data CSV file.
        /// </summary>
        /// <returns>The tick decimals index.</returns>
        public ReadOnlyDictionary<string, int> GetPriceDecimalPrecisionIndex() => this.priceDecimalPrecisionIndex;

        /// <summary>
        /// Gets tick value of the given symbol, loaded from the brokerage instrument data CSV file
        /// (must be contained in the index).
        /// </summary>
        /// <param name="brokerSymbol">The broker symbol.</param>
        /// <returns>If successful returns the result, otherwise returns failure result.</returns>
        public QueryResult<decimal> GetTickValue(string brokerSymbol)
        {
            Debug.NotNull(brokerSymbol, nameof(brokerSymbol));

            return this.tickValueIndex.ContainsKey(brokerSymbol)
                ? QueryResult<decimal>.Ok(this.tickValueIndex[brokerSymbol])
                : QueryResult<decimal>.Fail($"Cannot get tick value (index did not contain the given broker symbol {brokerSymbol}).");
        }

        /// <summary>
        /// Gets the target direct spread of the given symbol, loaded from the brokerage instrument data CSV file
        /// (must be contained in the index).
        /// </summary>
        /// <param name="brokerSymbol">The broker symbol.</param>
        /// <returns>If successful returns the result, otherwise returns failure result.</returns>
        public QueryResult<decimal> GetTargetDirectSpread(string brokerSymbol)
        {
            Debug.NotNull(brokerSymbol, nameof(brokerSymbol));

            return this.targetSpreadIndex.ContainsKey(brokerSymbol)
                ? QueryResult<decimal>.Ok(this.targetSpreadIndex[brokerSymbol])
                : QueryResult<decimal>.Fail($"Cannot get target direct spread (index did not contain the given broker symbol {brokerSymbol}).");
        }

        /// <summary>
        /// Gets the margin requirement of the given symbol per minimum trade size, loaded from the
        /// brokerage instrument data CSV file (must be contained in the index).
        /// </summary>
        /// <param name="brokerSymbol">The broker symbol.</param>
        /// <returns>If successful returns the result, otherwise returns failure result.</returns>
        public QueryResult<decimal> GetMarginRequirement(string brokerSymbol)
        {
            Debug.NotNull(brokerSymbol, nameof(brokerSymbol));

            return this.marginRequirementIndex.ContainsKey(brokerSymbol)
                ? QueryResult<decimal>.Ok(this.marginRequirementIndex[brokerSymbol])
                : QueryResult<decimal>.Fail($"Cannot get margin requirement (index did not contain the given broker symbol {brokerSymbol}).");
        }

        private static ReadOnlyDictionary<TKey, TValue> LoadData<TKey, TValue>(
            string csvDataFilePath,
            int keyColumn,
            int valueColumn)
        {
            var data = new Dictionary<TKey, TValue>();

            using (var textReader = File.OpenText(csvDataFilePath))
            {
                var reader = new CsvReader(textReader);

                // Skip header.
                textReader.ReadLine();

                while (reader.Read())
                {
                    data.Add(reader.GetField<TKey>(keyColumn), reader.GetField<TValue>(valueColumn));
                }
            }

            return new ReadOnlyDictionary<TKey, TValue>(data);
        }
    }
}
