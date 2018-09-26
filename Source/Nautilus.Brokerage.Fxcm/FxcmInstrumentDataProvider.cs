//---------------------------------------------------------------------------------------------------------------------
// <copyright file="FxcmInstrumentDataProvider.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//---------------------------------------------------------------------------------------------------------------------

namespace Nautilus.Brokerage.FXCM
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
    /// Provides data for FXCM tradeable instruments.
    /// </summary>
    public static class FxcmInstrumentDataProvider
    {
        // All keys are ISO convention broker symbols.
        private const string CsvDataFileName = "brokerage_fxcm_instruments.csv";
        private static readonly string AssemblyDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        private static readonly string CsvDataFilePath = Path.GetFullPath(Path.Combine(AssemblyDirectory, CsvDataFileName));
        private static readonly ReadOnlyDictionary<string, string> SymbolIndex;
        private static readonly ReadOnlyDictionary<string, decimal> TickValueIndex;
        private static readonly ReadOnlyDictionary<string, decimal> TargetSpreadIndex;
        private static readonly ReadOnlyDictionary<string, decimal> MarginRequirementIndex;

        static FxcmInstrumentDataProvider()
        {
            SymbolIndex = LoadSymbols();
            TickValueIndex = LoadTickValues();
            TargetSpreadIndex = LoadTargetSpreads();
            MarginRequirementIndex = LoadMarginRequirements();
        }

        /// <summary>
        /// Returns the Nautilus symbol <see cref="string"/> from the given FXCM symbol.
        /// </summary>
        /// <param name="fxcmSymbol">The FXCM symbol.</param>
        /// <returns>The <see cref="Nautilus"/> symbol.</returns>
        public static QueryResult<string> GetNautilusSymbol(string fxcmSymbol)
        {
            Debug.NotNull(fxcmSymbol, nameof(fxcmSymbol));

            return SymbolIndex.ContainsKey(fxcmSymbol)
                 ? QueryResult<string>.Ok(SymbolIndex[fxcmSymbol])
                 : QueryResult<string>.Fail($"Cannot find the Nautilus symbol from the given broker symbol {fxcmSymbol}");
        }

        /// <summary>
        /// Returns the FXCM symbol <see cref="string"/> from the given Nautilus symbol.
        /// </summary>
        /// <param name="nautilusSymbol">The Nautilus symbol.</param>
        /// <returns>The FXCM symbol.</returns>
        public static QueryResult<string> GetBrokerSymbol(string nautilusSymbol)
        {
            Debug.NotNull(nautilusSymbol, nameof(nautilusSymbol));

            return SymbolIndex.ContainsValue(nautilusSymbol)
                 ? QueryResult<string>.Ok(SymbolIndex.FirstOrDefault(x => x.Value == nautilusSymbol).Key)
                 : QueryResult<string>.Fail($"Cannot find the broker symbol from the given Nautilus symbol {nautilusSymbol}");
        }

        /// <summary>
        /// Returns a read only list of all Nautilus symbols.
        /// </summary>
        /// <returns>The list of broker symbols.</returns>
        public static ReadOnlyList<Symbol> GetAllSymbols()
        {
            var symbols = SymbolIndex.Values
                .Select(symbol => new Symbol(symbol, Venue.FXCM))
                .ToList();

            return new ReadOnlyList<Symbol>(symbols);
        }

        /// <summary>
        /// Returns a read only list of all FXCM symbols.
        /// </summary>
        /// <returns>The list of broker symbols.</returns>
        public static ReadOnlyList<string> GetAllBrokerSymbols()
        {
            var brokerSymbols = SymbolIndex.Keys.ToList();

            return new ReadOnlyList<string>(brokerSymbols);
        }

        /// <summary>
        /// Returns the <see cref="decimal"/> tick value of the given symbol if its contained in the
        /// index.
        /// </summary>
        /// <param name="brokerSymbol">The broker symbol.</param>
        /// <returns> A <see cref="decimal"/>.</returns>
        public static QueryResult<decimal> GetTickValue(string brokerSymbol)
        {
            Debug.NotNull(brokerSymbol, nameof(brokerSymbol));

            return TickValueIndex.ContainsKey(brokerSymbol)
                ? QueryResult<decimal>.Ok(TickValueIndex[brokerSymbol])
                : QueryResult<decimal>.Fail($"Cannot find tick value for {brokerSymbol}");
        }

        /// <summary>
        /// Returns the target direct spread of the given symbol if its contained in the index.
        /// </summary>
        /// <param name="brokerSymbol">The broker symbol.</param>
        /// <returns>An <see cref="int"/>.</returns>
        public static QueryResult<decimal> GetTargetDirectSpread(string brokerSymbol)
        {
            Validate.NotNull(brokerSymbol, nameof(brokerSymbol));

            return TargetSpreadIndex.ContainsKey(brokerSymbol)
                ? QueryResult<decimal>.Ok(TargetSpreadIndex[brokerSymbol])
                : QueryResult<decimal>.Fail($"Cannot find the target direct spread for {brokerSymbol}");
        }

        private static ReadOnlyDictionary<string, string> LoadSymbols()
        {
            var symbols = new Dictionary<string, string>();

            using (var textReader = File.OpenText(CsvDataFilePath))
            {
                var reader = new CsvReader(textReader);

                // Skip header.
                textReader.ReadLine();

                while (reader.Read())
                {
                    symbols.Add(reader.GetField<string>(0), reader.GetField<string>(1));
                }
            }

            return new ReadOnlyDictionary<string, string>(symbols);
        }

        private static ReadOnlyDictionary<string, decimal> LoadTickValues()
        {
            var values = new Dictionary<string, decimal>();

            using (var textReader = File.OpenText(CsvDataFilePath))
            {
                var reader = new CsvReader(textReader);

                // Skip header.
                textReader.ReadLine();

                while (reader.Read())
                {
                    values.Add(reader.GetField<string>(0), reader.GetField<decimal>(2));
                }
            }

            return new ReadOnlyDictionary<string, decimal>(values);
        }

        private static ReadOnlyDictionary<string, decimal> LoadTargetSpreads()
        {
            var spreads = new Dictionary<string, decimal>();

            using (var textReader = File.OpenText(CsvDataFilePath))
            {
                var reader = new CsvReader(textReader);

                // Skip header.
                textReader.ReadLine();

                while (reader.Read())
                {
                    spreads.Add(reader.GetField<string>(0), reader.GetField<decimal>(3));
                }
            }

            return new ReadOnlyDictionary<string, decimal>(spreads);
        }

        private static ReadOnlyDictionary<string, decimal> LoadMarginRequirements()
        {
            var margins = new Dictionary<string, decimal>();

            using (var textReader = File.OpenText(CsvDataFilePath))
            {
                var reader = new CsvReader(textReader);

                // Skip header.
                textReader.ReadLine();

                while (reader.Read())
                {
                    margins.Add(reader.GetField<string>(0), reader.GetField<decimal>(4));
                }
            }

            return new ReadOnlyDictionary<string, decimal>(margins);
        }
    }
}
