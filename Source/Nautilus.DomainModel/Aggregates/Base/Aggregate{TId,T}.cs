//---------------------------------------------------------------------------------------------------------------------
// <copyright file="Aggregate{TId,T}.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//---------------------------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Aggregates.Base
{
    using System.Collections.Generic;
    using System.Linq;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Message;
    using Nautilus.Core.Types;
    using Nautilus.DomainModel.Entities.Base;
    using NodaTime;

    /// <summary>
    /// The base class for all uniquely identifiable domain objects which are made up of an
    /// aggregation of entities.
    /// </summary>
    /// <typeparam name="TId">The identifier type.</typeparam>
    /// <typeparam name="T">The aggregate type.</typeparam>
    public abstract class Aggregate<TId, T> : Entity<TId, T>
        where TId : Identifier<TId>
        where T : Entity<TId, T>
    {
        private readonly List<Event> events;

        /// <summary>
        /// Initializes a new instance of the <see cref="Aggregate{TId,T}"/> class.
        /// </summary>
        /// <param name="identifier">The aggregate identifier.</param>
        /// <param name="timestamp">The aggregate timestamp.</param>
        protected Aggregate(
            TId identifier,
            ZonedDateTime timestamp)
            : base(identifier, timestamp)
        {
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.events = new List<Event>();
        }

        /// <summary>
        /// Gets the last event applied to the position.
        /// </summary>
        public Event? LastEvent => this.events.LastOrDefault();

        /// <summary>
        /// Gets the timestamp of the last event update.
        /// </summary>
        public ZonedDateTime? LastUpdated => this.events.LastOrDefault()?.Timestamp;

        /// <summary>
        /// Gets the aggregates event count.
        /// </summary>
        public int EventCount => this.events.Count;

        /// <summary>
        /// Append the given event to the aggregates events.
        /// </summary>
        /// <param name="event">The event to append.</param>
        protected void AppendEvent(Event @event)
        {
            this.events.Add(@event);
        }

        /// <summary>
        /// Returns a copy of the list of events.
        /// </summary>
        /// <returns>The events.</returns>
        protected List<Event> GetEvents() => this.events;
    }
}
