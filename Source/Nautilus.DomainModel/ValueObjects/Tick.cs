//--------------------------------------------------------------------------------------------------
// <copyright file="Tick.cs" company="Nautech Systems Pty Ltd">
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

using Nautilus.Core.Annotations;
using Nautilus.DomainModel.Enums;
using Nautilus.DomainModel.Identifiers;
using NodaTime;

namespace Nautilus.DomainModel.ValueObjects
{
    /// <summary>
    /// Represents a financial market tick.
    /// </summary>
    [Immutable]
    public abstract class Tick
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Tick"/> class.
        /// </summary>
        /// <param name="symbol">The tick symbol.</param>
        /// <param name="tickSpec">The tick specification.</param>
        /// <param name="timestamp">The tick timestamp.</param>
        protected Tick(
            Symbol symbol,
            TickSpecification tickSpec,
            ZonedDateTime timestamp)
        {
            this.Symbol = symbol;
            this.Specification = tickSpec;
            this.Timestamp = timestamp;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tick"/> class.
        /// </summary>
        /// <param name="symbol">The tick symbol.</param>
        /// <param name="tickSpec">The tick specification.</param>
        /// <param name="unixTimestamp">The tick unix timestamp in milliseconds.</param>
        protected Tick(
            Symbol symbol,
            TickSpecification tickSpec,
            long unixTimestamp)
        {
            this.Symbol = symbol;
            this.Specification = tickSpec;
            this.Timestamp = Instant.FromUnixTimeMilliseconds(unixTimestamp).InUtc();
        }

        /// <summary>
        /// Gets the ticks symbol.
        /// </summary>
        public Symbol Symbol { get; }

        /// <summary>
        /// Gets the ticks specification.
        /// </summary>
        public TickSpecification Specification { get; }

        /// <summary>
        /// Gets the ticks timestamp.
        /// </summary>
        public ZonedDateTime Timestamp { get; }
    }
}
