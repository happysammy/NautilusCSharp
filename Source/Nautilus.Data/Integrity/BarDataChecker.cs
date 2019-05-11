//--------------------------------------------------------------------------------------------------
// <copyright file="BarDataChecker.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Integrity
{
    using System.Collections.Generic;
    using System.Linq;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.CQS;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

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
        /// <param name="barType">The bar specification to check.</param>
        /// <param name="bars">The bars to check.</param>
        /// <returns>A result and anomaly list of <see cref="string"/>(s).</returns>
        public static QueryResult<List<string>> CheckBars(BarType barType, Bar[] bars)
        {
            var anomalyList = new List<string>();

            if (bars.Length == 0)
            {
                return QueryResult<List<string>>.Fail("No bars for BarDataChecker to check");
            }

            CheckDuplicateBars(barType, bars, anomalyList);
            CheckBarsInOrder(barType, bars, anomalyList);
            CheckBarsComplete(barType, bars, anomalyList);

            return anomalyList.Count == 0
                       ? PassResult(barType, anomalyList, bars.First().Timestamp, bars.Last().Timestamp)
                       : FailResult(barType, anomalyList, bars.First().Timestamp, bars.Last().Timestamp);
        }

        [PerformanceOptimized]
        private static void CheckDuplicateBars(
            BarType barType,
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
            BarType barType,
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
            BarType barType,
            Bar[] bars,
            List<string> anomalyList)
        {
            // Don't refactor Length - 1.
            for (var i = 0; i < bars.Length - 1; i++)
            {
                if (bars[i + 1].Timestamp - bars[i].Timestamp != barType.Specification.Duration)
                {
                    anomalyList.Add(
                        $"DataAnomaly: Missing bar "
                      + $"at index {i} at {bars[i].Timestamp.ToIsoString()}, "
                      + $"next bar at {bars[i + 1].Timestamp.ToIsoString()}");
                }
            }
        }

        private static QueryResult<List<string>> PassResult(
            BarType barType,
            List<string> anomalyList,
            ZonedDateTime fromDateTime,
            ZonedDateTime toDateTime)
        {
            return QueryResult<List<string>>.Ok(
                anomalyList, $"Data integrity check for {barType} from {fromDateTime.ToIsoString()} to {toDateTime.ToIsoString()} passed with no data anomalies");
        }

        private static QueryResult<List<string>> FailResult(
            BarType barType,
            List<string> anomalyList,
            ZonedDateTime fromDateTime,
            ZonedDateTime toDateTime)
        {
            return QueryResult<List<string>>.Ok(
                anomalyList, $"Data integrity check for {barType} from {fromDateTime.ToIsoString()} to {toDateTime.ToIsoString()} failed with {anomalyList.Count} anomalies");
        }
    }
}
