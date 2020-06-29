//--------------------------------------------------------------------------------------------------
// <copyright file="EconomicEventFrame.cs" company="Nautech Systems Pty Ltd">
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

namespace Nautilus.DomainModel.Frames
{
    using System.Collections.Generic;
    using System.Linq;
    using Nautilus.Core.Annotations;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// A container for <see cref="EconomicEvent"/>s.
    /// </summary>
    [Immutable]
    public sealed class EconomicEventFrame
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EconomicEventFrame"/> class.
        /// </summary>
        /// <param name="events">The list of economic events.</param>
        public EconomicEventFrame(IEnumerable<EconomicEvent> events)
        {
            this.Events = events;
            this.CurrencySymbols = this.Events.Select(e => e.Currency).Distinct();
        }

        /// <summary>
        /// Gets the economic event frames currency symbols.
        /// </summary>
        public IEnumerable<Currency> CurrencySymbols { get; }

        /// <summary>
        /// Gets the economic event frames list of events.
        /// </summary>
        public IEnumerable<EconomicEvent> Events { get; }

        /// <summary>
        /// Gets the economic event frames first event time.
        /// </summary>
        public ZonedDateTime StartDateTime => this.Events.First().Time;

        /// <summary>
        /// Gets the economic event frames last event time.
        /// </summary>
        public ZonedDateTime EndDateTime => this.Events.Last().Time;
    }
}
