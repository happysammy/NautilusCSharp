//---------------------------------------------------------------------------------------------------------------------
// <copyright file="Aggregate.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//---------------------------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel
{
    using System.Collections.Generic;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.CQS;
    using Nautilus.Core.Validation;
    using NodaTime;

    /// <summary>
    /// The base class for all uniquely identifiable domain objects which are made up of an
    /// aggregation of entities.
    /// </summary>
    /// <typeparam name="T">The aggregate type.</typeparam>
    [PerformanceOptimized]
    public abstract class Aggregate<T> : Entity<T>
        where T : Entity<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Aggregate{T}"/> class.
        /// </summary>
        /// <param name="identifier">The aggregate identifier.</param>
        /// <param name="timestamp">The aggregate timestamp.</param>
        protected Aggregate(
            EntityId<T> identifier,
            ZonedDateTime timestamp)
            : base(identifier, timestamp)
        {
            Debug.NotNull(identifier, nameof(identifier));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.Events = new List<Event>();
        }

        /// <summary>
        /// Gets the aggregates event count.
        /// </summary>
        public int EventCount => this.Events.Count;

        /// <summary>
        /// Gets the aggregates events list.
        /// </summary>
        /// <returns>The <see cref="IList{Event}"/>.</returns>
        protected List<Event> Events { get; }

        /// <summary>
        /// Applies the given <see cref="Event"/> to the aggregate.
        /// </summary>
        /// <param name="event">The event.</param>
        /// <returns>A <see cref="CommandResult"/> result.</returns>
        public abstract CommandResult Apply(Event @event);
    }
}
