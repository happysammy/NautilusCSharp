//--------------------------------------------------------------
// <copyright file="DukascopyBarDataProvider.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.Database.Dukascopy
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using NautechSystems.CSharp.CQS;
    using NautechSystems.CSharp.Extensions;
    using NautechSystems.CSharp.Validation;
    using Nautilus.Database.Core.Configuration;
    using Nautilus.Database.Core.Interfaces;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Provides the <see cref="Dukascopy"/> meta-data.
    /// </summary>
    public class DukascopyBarDataProvider : IBarDataProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DukascopyBarDataProvider"/> class.
        /// </summary>
        /// <param name="config">The market data provider configuration.</param>
        /// <param name="configCsvPath">The historic config CSV path.</param>
        /// <param name="initialFromDateSpecified">Is the initial from date specified flag.</param>
        /// <param name="initialFromDateString">The initial from date string.</param>
        /// <param name="collectionOffsetMinutes">The collection offset minutes.</param>
        public DukascopyBarDataProvider(
            MarketDataProviderConfig config,
            string configCsvPath,
            bool initialFromDateSpecified,
            string initialFromDateString,
            int collectionOffsetMinutes)
        {
            Validate.NotNull(config, nameof(config));
            Validate.NotNull(initialFromDateString, nameof(initialFromDateString));
            Validate.Int32NotOutOfRange(collectionOffsetMinutes, nameof(collectionOffsetMinutes), 0, int.MaxValue);

            this.BarSpecifications = BuildBarSpecifications(config.CurrencyPairs, config.BarResolutions);
            this.DataPath = new DirectoryInfo(config.CsvDataDirectory);

            this.HistoricConfigEditor = new DukascopyHistoricConfigEditor(
                configCsvPath,
                config.TimestampParsePattern,
                initialFromDateSpecified,
                initialFromDateString);

            this.TimestampParsePattern = config.TimestampParsePattern;
            this.VolumeMultiple = config.VolumeMultiple;
            this.IsBarDataCheckOn = config.IsBarDataCheckOn;
        }

        /// <summary>
        /// Gets the <see cref="Dukascopy"/> bar specifications.
        /// </summary>
        public IReadOnlyCollection<BarSpecification> BarSpecifications { get; }

        /// <summary>
        /// Gets the <see cref="Dukascopy"/> CSV data path.
        /// </summary>
        public DirectoryInfo DataPath { get; }

        /// <summary>
        /// Gets the <see cref="Dukascopy"/> historic config CSV file editor;
        /// </summary>
        public DukascopyHistoricConfigEditor HistoricConfigEditor { get; }

        /// <summary>
        /// Gets the <see cref="Dukascopy"/> <see cref="ZonedDateTime"/> parse pattern.
        /// </summary>
        public string TimestampParsePattern { get; }

        /// <summary>
        /// Gets the <see cref="Dukascopy"/> volume multiple for bars data.
        /// </summary>
        public int VolumeMultiple { get; }

        /// <summary>
        /// Gets a value indicating whether the bar data integrity check is on.
        /// </summary>
        public bool IsBarDataCheckOn { get; }

        // TODO: Temporary property to provide a hook into the historic config csv editor.
        public bool InitialFromDateSpecified => this.HistoricConfigEditor.InitialFromDateSpecified;

        /// <summary>
        /// Returns the <see cref="Dukascopy"/> label for the given <see cref="BarResolution"/>.
        /// </summary>
        /// <param name="resolution">The bar resolution.</param>
        /// <returns>The data providers label for the given <see cref="BarResolution"/>.</returns>
        public string GetResolutionLabel(BarResolution resolution)
        {
            switch (resolution)
            {
                case BarResolution.Second:
                    return "Second";

                case BarResolution.Minute:
                    return "Min";

                case BarResolution.Hour:
                    return "Hourly";

                case BarResolution.Day:
                    return "Day";

                default:
                    throw new ArgumentOutOfRangeException(nameof(resolution), resolution, null);
            }
        }

        // TODO: Temporary method to provide a hook into the historic config csv editor.
        public CommandResult InitialFromDateConfigCsv(
            IReadOnlyList<string> currencyPairs,
            ZonedDateTime toDateTime)
        {
            return this.HistoricConfigEditor.InitialFromDateConfigCsv(currencyPairs, toDateTime);
        }

        public CommandResult UpdateConfigCsv(
            IReadOnlyList<string> currencyPairs,
            ZonedDateTime fromDateTime,
            ZonedDateTime toDateTime)
        {
            return this.HistoricConfigEditor.UpdateConfigCsv(currencyPairs, fromDateTime, toDateTime);
        }

        private static IReadOnlyCollection<BarSpecification> BuildBarSpecifications(
            IReadOnlyCollection<string> currencyPairs,
            IReadOnlyCollection<string> barResolutions)
        {
            var barSpecs = new List<BarSpecification>();

            foreach (var symbol in currencyPairs.Distinct())
            {
                foreach (var resolution in barResolutions)
                {
                    barSpecs.Add(new BarSpecification(new Symbol(symbol, Exchange.Dukascopy), BarQuoteType.Bid, resolution.ToEnum<BarResolution>(), 1));
                    barSpecs.Add(new BarSpecification(new Symbol(symbol, Exchange.Dukascopy), BarQuoteType.Ask, resolution.ToEnum<BarResolution>(), 1));
                }
            }

            return barSpecs;
        }
    }
}
