// -------------------------------------------------------------------------------------------------
// <copyright file="BarWrangler.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Core.Wranglers
{
    using System.Collections.Generic;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.Database.Core.Extensions;
    using Nautilus.Database.Core.Keys;
    using Nautilus.Database.Core.Types;

    [Immutable]
    public static class BarWrangler
    {
        [PerformanceOptimized]
        public static List<BarData> ParseBars(string barsString)
        {
            Validate.NotNull(barsString, nameof(barsString));

            var splitStrings = barsString.Split('|');

            var barData = new List<BarData>();

            // TODO: Remove this whitespace string bug!
            for (var i = 0; i < splitStrings.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(splitStrings[i]))
                {
                    barData.Add(splitStrings[i].ToBarData());
                }
            }

            return barData;
        }

        [PerformanceOptimized]
        public static Dictionary<DateKey, List<BarData>> OrganizeBarsByDay(BarData[] bars)
        {
            Validate.CollectionNotNullOrEmpty(bars, nameof(bars));

            var barsDictionary = new Dictionary<DateKey, List<BarData>>();

            for (var i = 0; i < bars.Length; i++)
            {
                var dateKey = new DateKey(bars[i].Timestamp);

                if (!barsDictionary.ContainsKey(dateKey))
                {
                    barsDictionary.Add(dateKey, new List<BarData>());
                }

                barsDictionary[dateKey].Add(bars[i]);
            }

            return barsDictionary;
        }
    }
}
