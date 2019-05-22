//---------------------------------------------------------------------------------------------------------------------
// <copyright file="Aggregate.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//---------------------------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Aggregates.Base
{
    using System.Collections.Generic;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Entities.Base;
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
            Identifier<T> identifier,
            ZonedDateTime timestamp)
            : base(identifier, timestamp)
        {
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
        public abstract void Apply(Event @event);
    }
}
