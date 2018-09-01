//--------------------------------------------------------------------------------------------------
// <copyright file="BarDataProviderConfig.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusDB.Configuration
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;

    /// <summary>
    /// Represents a bar data provider configuration.
    /// </summary>
    [Immutable]
    public sealed class BarDataProviderConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BarDataProviderConfig"/> class.
        /// </summary>
        /// <param name="run">The boolean flag indicating whether the provider should run.</param>
        /// <param name="csvDataDirectory">The CSV data directory.</param>
        /// <param name="symbols">The symbols to collect.</param>
        /// <param name="barResolutions">The bar resolutions to collect.</param>
        /// <param name="timestampParsePattern">The timestamp parse pattern.</param>
        /// <param name="volumeMultiple">The volume multiple.</param>
        /// <param name="isBarDataCheckOn">The boolean flag indicating whether bar data integrity checking is on.</param>
        public BarDataProviderConfig(
            bool run,
            string csvDataDirectory,
            string[] symbols,
            string[] barResolutions,
            string timestampParsePattern,
            int volumeMultiple,
            bool isBarDataCheckOn)
        {
            Validate.NotNull(csvDataDirectory, nameof(csvDataDirectory));
            Validate.NotNullOrEmpty(symbols, nameof(symbols));
            Validate.NotNullOrEmpty(barResolutions, nameof(barResolutions));
            Validate.NotNull(timestampParsePattern, nameof(timestampParsePattern));
            Validate.NotOutOfRangeInt32(volumeMultiple, nameof(volumeMultiple), 0, int.MaxValue);

            this.Run = run;
            this.CsvDataDirectory = csvDataDirectory;
            this.Symbols = symbols;
            this.BarResolutions = barResolutions;
            this.TimestampParsePattern = timestampParsePattern;
            this.VolumeMultiple = volumeMultiple;
            this.IsBarDataCheckOn = isBarDataCheckOn;
        }

        /// <summary>
        /// Gets a value indicating whether the provider should run.
        /// </summary>
        public bool Run { get; }

        /// <summary>
        /// Gets the CSV data directory.
        /// </summary>
        public string CsvDataDirectory { get; }

        /// <summary>
        /// Gets the symbols.
        /// </summary>
        public string[] Symbols { get; }

        /// <summary>
        /// Gets the bar resolutions.
        /// </summary>
        public string[] BarResolutions { get; }

        /// <summary>
        /// Gets the timestamp parse pattern.
        /// </summary>
        public string TimestampParsePattern { get; }

        /// <summary>
        /// Gets the volume multiple.
        /// </summary>
        public int VolumeMultiple { get; }

        /// <summary>
        /// Gets a value indicating whether bar data integrity checking is on.
        /// </summary>
        public bool IsBarDataCheckOn { get; }
    }
}
