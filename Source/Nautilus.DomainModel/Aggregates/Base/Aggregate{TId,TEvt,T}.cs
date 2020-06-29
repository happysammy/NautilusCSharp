//---------------------------------------------------------------------------------------------------------------------
// <copyright file="Aggregate{TId,TEvt,T}.cs" company="Nautech Systems Pty Ltd">
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
//---------------------------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Aggregates.Base
{
    using System.Collections.Generic;
    using Nautilus.Core.Message;
    using Nautilus.Core.Types;
    using Nautilus.DomainModel.Entities.Base;
    using NodaTime;

    /// <summary>
    /// The base class for all aggregates. Aggregates are uniquely identifiable domain objects which
    /// are made up of an aggregation of entities.
    /// </summary>
    /// <typeparam name="TId">The aggregates identifier type.</typeparam>
    /// <typeparam name="TEvt">The aggregates event type.</typeparam>
    /// <typeparam name="T">The aggregates base type.</typeparam>
    public abstract class Aggregate<TId, TEvt, T> : Entity<TId, T>
        where TId : Identifier<TId>
        where TEvt : Event
        where T : Entity<TId, T>
    {
        private readonly List<TEvt> events;

        /// <summary>
        /// Initializes a new instance of the <see cref="Aggregate{TId,TEvt,T}"/> class.
        /// </summary>
        /// <param name="identifier">The aggregate identifier.</param>
        /// <param name="initial">The initial event.</param>
        protected Aggregate(TId identifier, TEvt initial)
            : base(identifier, initial.Timestamp)
        {
            this.events = new List<TEvt> { initial };
        }

        /// <summary>
        /// Gets the initial event.
        /// </summary>
        public TEvt InitialEvent => this.events[0];

        /// <summary>
        /// Gets the last event applied.
        /// </summary>
        public TEvt LastEvent => this.events[^1];

        /// <summary>
        /// Gets the last updated time.
        /// </summary>
        public ZonedDateTime LastUpdated => this.LastEvent.Timestamp;

        /// <summary>
        /// Gets the event count.
        /// </summary>
        public int EventCount => this.events.Count;

        /// <summary>
        /// Apply the given event to update the state.
        /// </summary>
        /// <param name="event">The event to apply.</param>
        public void Apply(TEvt @event)
        {
            this.events.Add(@event);
            this.OnEvent(@event);
        }

        /// <summary>
        /// Called after an event is applied.
        /// </summary>
        /// <param name="event">The event to handle.</param>
        protected abstract void OnEvent(TEvt @event);

        /// <summary>
        /// Returns the internal events.
        /// </summary>
        /// <returns>The list of events.</returns>
        protected List<TEvt> Events() => this.events;
    }
}
