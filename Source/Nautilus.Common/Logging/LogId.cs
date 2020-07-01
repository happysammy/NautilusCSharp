//--------------------------------------------------------------------------------------------------
// <copyright file="LogId.cs" company="Nautech Systems Pty Ltd">
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

using Microsoft.Extensions.Logging;

namespace Nautilus.Common.Logging
{
    /// <summary>
    /// Represents a <see cref="Nautilus"/> specific log event identifier.
    /// </summary>
    public static class LogId
    {
        /// <summary>
        /// Gets the event identifier for component operation events.
        /// </summary>
        public static EventId Component { get; } = new EventId(0, nameof(Component));

        /// <summary>
        /// Gets the event identifier for networking events.
        /// </summary>
        public static EventId Networking { get; } = new EventId(1, nameof(Networking));

        /// <summary>
        /// Gets the event identifier for database events.
        /// </summary>
        public static EventId Database { get; } = new EventId(2, nameof(Database));

        /// <summary>
        /// Gets the event identifier for trading events.
        /// </summary>
        public static EventId Trading { get; } = new EventId(3, nameof(Trading));
    }
}
