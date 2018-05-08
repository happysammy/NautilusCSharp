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
    using NautechSystems.CSharp;
    using NautechSystems.CSharp.CQS;
    using NautechSystems.CSharp.Validation;
    using NodaTime;

    /// <summary>
    /// The base class for all uniquely identifiable domain objects which are made up of an
    /// aggregation of entities.
    /// </summary>
    /// <typeparam name="T">The aggregate type.</typeparam>
    public abstract class Aggregate<T> : Entity<T> where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Aggregate{T}"/> class.
        /// </summary>
        /// <param name="entityId">The aggregate identifier.</param>
        /// <param name="entityTimestamp">The aggregate timestamp.</param>
        protected Aggregate(
            EntityId entityId,
            ZonedDateTime entityTimestamp)
            : base(entityId, entityTimestamp)
        {
            Debug.NotNull(entityId, nameof(entityId));
        }

        /// <summary>
        /// Returns the aggregates event count.
        /// </summary>
        public int EventCount => this.Events.Count;

        /// <summary>
        /// Gets the aggregates events list.
        /// </summary>
        /// <returns>The <see cref="IList{Event}"/>.</returns>
        protected IList<Event> Events { get; } = new List<Event>();

        /// <summary>
        /// Applies the given <see cref="Event"/> to the aggregate.
        /// </summary>
        /// <param name="event">The event.</param>
        /// <returns>A <see cref="CommandResult"/> result.</returns>
        public abstract CommandResult Apply(Event @event);
    }
}