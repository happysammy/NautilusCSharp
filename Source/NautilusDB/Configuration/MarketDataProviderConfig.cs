//--------------------------------------------------------------------------------------------------
// <copyright file="MarketDataProviderConfig.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusDB.Configuration
{
    using Nautilus.Core.Validation;

    public sealed class MarketDataProviderConfig
    {
        public MarketDataProviderConfig(
            bool run,
            string csvDataDirectory,
            string[] currencyPairs,
            string[] barResolutions,
            string timestampParsePattern,
            int volumeMultiple,
            bool isBarDataCheckOn)
        {
            Validate.NotNull(csvDataDirectory, nameof(csvDataDirectory));
            Validate.CollectionNotNullOrEmpty(currencyPairs, nameof(currencyPairs));
            Validate.CollectionNotNullOrEmpty(barResolutions, nameof(barResolutions));
            Validate.NotNull(timestampParsePattern, nameof(timestampParsePattern));
            Validate.Int32NotOutOfRange(volumeMultiple, nameof(volumeMultiple), 0, int.MaxValue);

            this.Run = run;
            this.CsvDataDirectory = csvDataDirectory;
            this.CurrencyPairs = currencyPairs;
            this.BarResolutions = barResolutions;
            this.TimestampParsePattern = timestampParsePattern;
            this.VolumeMultiple = volumeMultiple;
            this.IsBarDataCheckOn = isBarDataCheckOn;
        }

        public bool Run { get; }

        public string CsvDataDirectory { get; }

        public string[] CurrencyPairs { get; }

        public string[] BarResolutions { get; }

        public string TimestampParsePattern { get; }

        public int VolumeMultiple { get; }

        public bool IsBarDataCheckOn { get; }
    }
}
