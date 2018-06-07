//--------------------------------------------------------------------------------------------------
// <copyright file="BarDataChecker.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Integrity.Checkers
{
    using System.Collections.Generic;
    using System.Linq;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.CQS;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Validation;
    using NodaTime;
    using Nautilus.Database.Types;
    using Nautilus.DomainModel.ValueObjects;
    using ServiceStack;

    /// <summary>
    /// Checks various data forms for known anomalies. Performance optimized.
    /// </summary>
    [Immutable]
    public static class BarDataChecker
    {
        /// <summary>
        /// Checks the given list of <see cref="Bar"/> for known data anomalies (duplicates,
        /// out of order or missing bars).
        /// </summary>
        /// <param name="barSpec">The bar specification to check.</param>
        /// <param name="bars">The bars to check.</param>
        /// <returns>A result and anomaly list of <see cref="string"/>(s).</returns>
        public static QueryResult<List<string>> CheckBars(SymbolBarSpec barSpec, Bar[] bars)
        {
            Validate.NotNull(barSpec, nameof(barSpec));
            Validate.NotNull(bars, nameof(bars));

            var anomalyList = new List<string>();

            if (bars.Length == 0)
            {
                return QueryResult<List<string>>.Fail("No bars for BarDataChecker to check");
            }

            CheckDuplicateBars(barSpec, bars, anomalyList);
            CheckBarsInOrder(barSpec, bars, anomalyList);
            CheckBarsComplete(barSpec, bars, anomalyList);

            return anomalyList.IsEmpty()
                       ? PassResult(barSpec, anomalyList, bars.First().Timestamp, bars.Last().Timestamp)
                       : FailResult(barSpec, anomalyList, bars.First().Timestamp, bars.Last().Timestamp);
        }

        [PerformanceOptimized]
        private static void CheckDuplicateBars(
            SymbolBarSpec barSpec,
            Bar[] bars,
            List<string> anomalyList)
        {
            // Don't refactor Length - 1.
            for (var i = 0; i < bars.Length - 1; i++)
            {
                if (bars[i + 1].Timestamp == bars[i].Timestamp)
                {
                    anomalyList.Add(
                        $"DataAnomaly: Duplicate bars " +
                        $"at index {i} and {i + 1} for {bars[i].Timestamp.ToIsoString()}");
                }
            }
        }

        [PerformanceOptimized]
        private static void CheckBarsInOrder(
            SymbolBarSpec barSpec,
            Bar[] bars,
            List<string> anomalyList)
        {
            // Don't refactor Length - 1.
            for (var i = 0; i < bars.Length - 1; i++)
            {
                if (bars[i + 1].Timestamp.IsLessThan(bars[i].Timestamp))
                {
                    anomalyList.Add(
                        $"DataAnomaly: Bars are out of order " +
                        $"at index {i} at {bars[i].Timestamp.ToIsoString()}, " +
                        $"next bar at {bars[i + 1].Timestamp.ToIsoString()}");
                }
            }
        }

        [PerformanceOptimized]
        private static void CheckBarsComplete(
            SymbolBarSpec barSpec,
            Bar[] bars,
            List<string> anomalyList)
        {
            // Don't refactor Length - 1.
            for (var i = 0; i < bars.Length - 1; i++)
            {
                if (bars[i + 1].Timestamp - bars[i].Timestamp != barSpec.BarSpecification.Duration)
                {
                    anomalyList.Add(
                        $"DataAnomaly: Missing bar "
                      + $"at index {i} at {bars[i].Timestamp.ToIsoString()}, "
                      + $"next bar at {bars[i + 1].Timestamp.ToIsoString()}");
                }
            }
        }

        private static QueryResult<List<string>> PassResult(
            SymbolBarSpec barSpec,
            List<string> anomalyList,
            ZonedDateTime fromDateTime,
            ZonedDateTime toDateTime)
        {
            return QueryResult<List<string>>.Ok(
                anomalyList,
                $"Data integrity check for {barSpec} from " +
                $"{fromDateTime.ToIsoString()} to {toDateTime.ToIsoString()} " +
                $"passed with no data anomalies");
        }

        private static QueryResult<List<string>> FailResult(
            SymbolBarSpec barSpec,
            List<string> anomalyList,
            ZonedDateTime fromDateTime,
            ZonedDateTime toDateTime)
        {
            return QueryResult<List<string>>.Ok(
                anomalyList,
                $"Data integrity check for {barSpec} from " +
                $"{fromDateTime.ToIsoString()} to {toDateTime.ToIsoString()} " +
                $"failed with {anomalyList.Count} anomalies");
        }
    }
}
