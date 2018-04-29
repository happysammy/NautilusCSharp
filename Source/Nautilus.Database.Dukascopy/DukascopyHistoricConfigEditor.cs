// -------------------------------------------------------------------------------------------------
// <copyright file="DukascopyHistoricConfigEditor.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Dukascopy
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using NautechSystems.CSharp.CQS;
    using NautechSystems.CSharp.Validation;
    using NodaTime;
    using ServiceStack;
    using Nautilus.Core.Extensions;
    using Nautilus.Database.Core.Interfaces;

    public sealed class DukascopyHistoricConfigEditor : ICsvDataConfigEditor
    {
        private readonly string dateTimeParsePattern;
        private ZonedDateTime initialFromDate;

        /// <summary>
        /// Initializes a new instance of the <see cref="DukascopyHistoricConfigEditor"/> class.
        /// </summary>
        /// <param name="configCsvPath">The configuration CSV path.</param>
        /// <param name="dateTimeParsePattern">The date time parse pattern.</param>
        /// <param name="initialFromDateSpecified">Initial from date specified.</param>
        /// <param name="initialFromDateString">Initial from date string.</param>
        public DukascopyHistoricConfigEditor(
            string configCsvPath,
            string dateTimeParsePattern,
            bool initialFromDateSpecified,
            string initialFromDateString)
        {
            Validate.NotNull(configCsvPath, nameof(configCsvPath));
            Validate.NotNull(dateTimeParsePattern, nameof(dateTimeParsePattern));

            this.ConfigCsvPath = new FileInfo(configCsvPath);
            this.dateTimeParsePattern = dateTimeParsePattern;

            this.InitialFromDateSpecified = initialFromDateSpecified;

            if (this.InitialFromDateSpecified)
            {
                this.initialFromDate = initialFromDateString.ToZonedDateTime(this.dateTimeParsePattern);
            }
        }

        /// <summary>
        /// Gets the <see cref="Dukascopy"/> historic configuration CSV path.
        /// </summary>
        public FileInfo ConfigCsvPath { get; }

        /// <summary>
        /// Gets a value indicating whether there is an initial from <see cref="ZonedDateTime"/>
        /// specified for collection.
        /// </summary>
        public bool InitialFromDateSpecified { get; }

        public CommandResult InitialFromDateConfigCsv(
            IReadOnlyList<string> currencyPairs,
            ZonedDateTime toDateTime)
        {
            if (!this.ConfigCsvPath.Exists)
            {
                this.ConfigCsvPath.Delete();
            }

            var fromDateTimeDukas = this.initialFromDate.ToStringWithParsePattern(this.dateTimeParsePattern);

            var toDateTimeDukas = new ZonedDateTime(
                    new LocalDateTime(
                        toDateTime.Year,
                        toDateTime.Month,
                        toDateTime.Day,
                        23,
                        59),
                    DateTimeZone.Utc,
                    Offset.Zero).ToStringWithParsePattern(this.dateTimeParsePattern);

            var currencyPairsToWrite = currencyPairs.Select(cp => cp.Insert(3, "/")).ToList();

            var firstLine = fromDateTimeDukas + "," + toDateTimeDukas;
            var secondLine = currencyPairsToWrite.Join(",");

            using (var fileStream = File.Create(this.ConfigCsvPath.FullName))
            {
                // TODO: fileStream.Write($"{firstLine}{Environment.NewLine}{secondLine}");
            }

            return CommandResult.Ok();
        }

        public CommandResult UpdateConfigCsv(
            IReadOnlyList<string> currencyPairs,
            ZonedDateTime fromDateTime,
            ZonedDateTime toDateTime)
        {
            if (!this.ConfigCsvPath.Exists)
            {
                this.ConfigCsvPath.Delete();
            }

            var fromDateTimeDukas = new ZonedDateTime(
                new LocalDateTime(
                    fromDateTime.Year,
                    fromDateTime.Month,
                    fromDateTime.Day,
                    0,
                    0),
                DateTimeZone.Utc,
                Offset.Zero)
                .ToStringWithParsePattern(this.dateTimeParsePattern);

            var toDateTimeDukas = new ZonedDateTime(
                new LocalDateTime(
                    toDateTime.Year,
                    toDateTime.Month,
                    toDateTime.Day,
                    23,
                    59),
                DateTimeZone.Utc,
                Offset.Zero)
                .ToStringWithParsePattern(this.dateTimeParsePattern);

            var currencyPairsToWrite = currencyPairs.Select(cp => cp.Insert(3, "/")).ToList();

            var firstLine = fromDateTimeDukas + "," + toDateTimeDukas;
            var secondLine = currencyPairsToWrite.Join(",");

            using (var fileStream = File.Create(this.ConfigCsvPath.FullName))
            {
                // TODO: fileStream.Write($"{firstLine}{Environment.NewLine}{secondLine}");
            }

            return CommandResult.Ok();
        }
    }
}