//--------------------------------------------------------------------------------------------------
// <copyright file="IZonedClock.cs" company="Nautech Systems Pty Ltd">
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

using NodaTime;

namespace Nautilus.Common.Interfaces
{
    /// <summary>
    /// Provides an adapter to a service clock with an embedded time zone.
    /// </summary>
    public interface IZonedClock
    {
        /// <summary>
        /// Returns the current time of the clock.
        /// </summary>
        /// <returns>A <see cref="ZonedDateTime"/>.</returns>
        ZonedDateTime TimeNow();

        /// <summary>
        /// Returns the current instant of the clock.
        /// </summary>
        /// <returns>A <see cref="Instant"/>.</returns>
        Instant InstantNow();

        /// <summary>
        /// Returns the time zone of the clock.
        /// </summary>
        /// <returns>A <see cref="DateTimeZone"/>.</returns>
        DateTimeZone GetTimeZone();
    }
}
