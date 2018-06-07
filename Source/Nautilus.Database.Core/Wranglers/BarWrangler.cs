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
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.Database.Core.Keys;
    using Nautilus.DomainModel.ValueObjects;

    [Immutable]
    public static class BarWrangler
    {
        [PerformanceOptimized]
        public static List<Bar> ParseBars(string barsString)
        {
            Validate.NotNull(barsString, nameof(barsString));

            var splitStrings = barsString.Split('|');

            var barData = new List<Bar>();

            // TODO: Remove this whitespace string bug!
            for (var i = 0; i < splitStrings.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(splitStrings[i]))
                {
                    barData.Add(Bar.GetFromString(splitStrings[i]));
                }
            }

            return barData;
        }

        [PerformanceOptimized]
        public static Dictionary<DateKey, List<Bar>> OrganizeBarsByDay(Bar[] bars)
        {
            Validate.CollectionNotNullOrEmpty(bars, nameof(bars));

            var barsDictionary = new Dictionary<DateKey, List<Bar>>();

            for (var i = 0; i < bars.Length; i++)
            {
                var dateKey = new DateKey(bars[i].Timestamp);

                if (!barsDictionary.ContainsKey(dateKey))
                {
                    barsDictionary.Add(dateKey, new List<Bar>());
                }

                barsDictionary[dateKey].Add(bars[i]);
            }

            return barsDictionary;
        }
    }
}
