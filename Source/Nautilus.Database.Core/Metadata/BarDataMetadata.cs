//--------------------------------------------------------------
// <copyright file="BarDataMetadata.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using NautechSystems.CSharp.Annotations;
using NautechSystems.CSharp.Extensions;
using NautechSystems.CSharp.Validation;
using NautilusDB.Core.Extensions;
using NodaTime;

namespace Nautilus.Database.Core.Metadata
{
    [Immutable]
    public struct BarDataMetadata
    {
        public BarDataMetadata(
            string symbol,
            ZonedDateTime lastUpdated,
            IReadOnlyCollection<BarResolution> resolutions,
            long barsCount)
        {
            this.Symbol = symbol;
            this.LastUpdated = lastUpdated;
            this.Resolutions = resolutions;
            this.BarsCount = barsCount;
        }

        public BarDataMetadata(string metadata)
        {
            var parsedMetadata = metadata.Split(",");

            this.Symbol = parsedMetadata[0];
            this.LastUpdated = parsedMetadata[1].ToZonedDateTimeFromIso();
            this.Resolutions = ParseResolutionsList(parsedMetadata[2]);
            this.BarsCount = Convert.ToInt64(parsedMetadata[3]);
        }

        public string Symbol { get; }

        public ZonedDateTime LastUpdated { get; }

        public IReadOnlyCollection<BarResolution> Resolutions { get; }

        public long BarsCount { get; }


        private static IReadOnlyCollection<BarResolution> ParseResolutionsList(string resolutionsString)
        {
            Debug.NotNull(resolutionsString, nameof(resolutionsString));

            return resolutionsString
                .Split("|")
                .Select(resolution => resolution.ToEnum<BarResolution>())
                .ToList();
        }
    }
}