//--------------------------------------------------------------------------------------------------
// <copyright file="MarketOpened.cs" company="Nautech Systems Pty Ltd">
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
using Nautilus.Core.Annotations;
using Nautilus.Core.Correctness;
using Nautilus.Core.Message;
using Nautilus.DomainModel.Identifiers;
using NodaTime;

namespace Nautilus.DomainModel.Events
{
    /// <summary>
    /// Represents an event where a market has opened.
    /// </summary>
    [Immutable]
    public sealed class MarketOpened : Event
    {
        private static readonly Type EventType = typeof(MarketOpened);

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketOpened" /> class.
        /// </summary>
        /// <param name="symbol">The symbol of the market.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="eventTimestamp">The event timestamp.</param>
        public MarketOpened(
            Symbol symbol,
            Guid eventId,
            ZonedDateTime eventTimestamp)
            : base(
                EventType,
                eventId,
                eventTimestamp)
        {
            Debug.NotDefault(eventId, nameof(eventId));
            Debug.NotDefault(eventTimestamp, nameof(eventTimestamp));

            this.Symbol = symbol;
        }

        /// <summary>
        /// Gets the market events symbol.
        /// </summary>
        public Symbol Symbol { get; }
    }
}
