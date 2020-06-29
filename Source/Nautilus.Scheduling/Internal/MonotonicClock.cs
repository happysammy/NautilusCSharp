// -------------------------------------------------------------------------------------------------
// <copyright file="MonotonicClock.cs" company="Nautech Systems Pty Ltd">
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
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Scheduling.Internal
{
    using System.Diagnostics;
    using NodaTime;
    using NodaTime.Extensions;

    /// <summary>
    /// Provides a monotonic clock implementation based on total uptime.
    /// Used for keeping accurate time internally.
    /// </summary>
    internal static class MonotonicClock
    {
        private static readonly Stopwatch Stopwatch = Stopwatch.StartNew();

        /// <summary>
        /// Gets the elapsed time as determined by a <see cref="Stopwatch"/>
        /// running continuously in the background.
        /// </summary>
        internal static Duration Elapsed => Stopwatch.Elapsed.ToDuration();
    }
}
