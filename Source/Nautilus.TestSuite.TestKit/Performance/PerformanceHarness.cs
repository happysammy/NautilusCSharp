//--------------------------------------------------------------------------------------------------
// <copyright file="PerformanceHarness.cs" company="Nautech Systems Pty Ltd">
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit.Abstractions;

namespace Nautilus.TestSuite.TestKit.Performance
{
    /// <summary>
    /// Provides a simple harness for performance benchmarking utilizing an internal stopwatch.
    /// </summary>
    public static class PerformanceHarness
    {
        /// <summary>
        /// Test the given delegate for the specified number of iterations and runs.
        /// The minimum duration is returned as it is more indicative of real performance unhampered
        /// by potential random background processes on the system.
        /// </summary>
        /// <param name="underTest">The action under test.</param>
        /// <param name="runs">The number of runs to perform.</param>
        /// <param name="iterations">The number of iterations per run.</param>
        /// <param name="output">The optional test output.</param>
        /// <returns>The minimum duration run.</returns>
        public static TimeSpan Test(Action underTest, int runs, int iterations, ITestOutputHelper? output = null)
        {
            var stopwatch = new Stopwatch();
            var results = new List<TimeSpan>();

            for (var i = 0; i < runs; i++)
            {
                stopwatch.Start();

                for (var j = 0; j < iterations; j++)
                {
                    underTest.Invoke();
                }

                stopwatch.Stop();
                results.Add(stopwatch.Elapsed);
                stopwatch.Reset();
            }

            var minimumTicks = Convert.ToInt64(results.Min(timeSpan => timeSpan.Ticks));
            var minimumTimesSpan = new TimeSpan(minimumTicks);

            output?.WriteLine($"Performance test: {underTest.Method.Name} " +
                              $"{minimumTimesSpan.Milliseconds}ms minimum " +
                              $"of {runs:N0} runs @ {iterations:N0} each run.");

            return minimumTimesSpan;
        }
    }
}
