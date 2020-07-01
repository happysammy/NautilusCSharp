//--------------------------------------------------------------------------------------------------
// <copyright file="BarDataChecker.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
//--------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using Nautilus.Core.Annotations;
using Nautilus.Core.CQS;
using Nautilus.Core.Extensions;
using Nautilus.DomainModel.ValueObjects;
using NodaTime;

namespace Nautilus.Data.Integrity
{
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
                return QueryResult<List<string>>.Fail("no bars to check");
            }

            CheckDuplicateBars(barType, bars, anomalyList);
            CheckBarsInOrder(barType, bars, anomalyList);
            CheckBarsComplete(barType, bars, anomalyList);

            return anomalyList.Count == 0
                       ? PassResult(barType, anomalyList, bars[0].Timestamp, bars[^1].Timestamp)
                       : FailResult(barType, anomalyList, bars[0].Timestamp, bars[^1].Timestamp);
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
                        "DataAnomaly: Duplicate bars " +
                        $"at index {i} and {i + 1} for {bars[i].Timestamp.ToIso8601String()}");
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
                        "DataAnomaly: Bars are out of order " +
                        $"at index {i} at {bars[i].Timestamp.ToIso8601String()}, " +
                        $"next bar at {bars[i + 1].Timestamp.ToIso8601String()}");
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
                        "DataAnomaly: Missing bar "
                      + $"at index {i} at {bars[i].Timestamp.ToIso8601String()}, "
                      + $"next bar at {bars[i + 1].Timestamp.ToIso8601String()}");
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
                anomalyList, $"Data integrity check for {barType} from {fromDateTime.ToIso8601String()} to {toDateTime.ToIso8601String()} passed with no data anomalies");
        }

        private static QueryResult<List<string>> FailResult(
            BarType barType,
            List<string> anomalyList,
            ZonedDateTime fromDateTime,
            ZonedDateTime toDateTime)
        {
            return QueryResult<List<string>>.Ok(
                anomalyList, $"Data integrity check for {barType} from {fromDateTime.ToIso8601String()} to {toDateTime.ToIso8601String()} failed with {anomalyList.Count} anomalies");
        }
    }
}
