//--------------------------------------------------------------------------------------------------
// <copyright file="Entity.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Entities.Base
{
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using NodaTime;

    /// <summary>
    /// The base class for all uniquely identifiable domain objects.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    [Immutable]
    public abstract class Entity<T>
        where T : Entity<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Entity{T}"/> class.
        /// </summary>
        /// <param name="identifier">The entity identifier.</param>
        /// <param name="timestamp">The entity timestamp.</param>
        protected Entity(
            Identifier<T> identifier,
            ZonedDateTime timestamp)
        {
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.Id = identifier;
            this.Timestamp = timestamp;
        }

        /// <summary>
        /// Gets the entity identifier.
        /// </summary>
        public Identifier<T> Id { get; }

        /// <summary>
        /// Gets the entity initialization timestamp.
        /// </summary>
        public ZonedDateTime Timestamp { get; }

        /// <summary>
        /// Returns a value indicating whether this entity is equal to the given <see cref="object"/>.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public override bool Equals(object other) => other is Entity<T> entity && this.Equals(entity);

        /// <summary>
        /// Returns a value indicating whether this entity is equal to the given entity.
        /// </summary>
        /// <param name="other">The other entity.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public bool Equals(Entity<T> other) => this.Id.Equals(other.Id);

        /// <summary>
        /// Returns the hash code of the wrapped object.
        /// </summary>
        /// <returns>An <see cref="int"/>.</returns>
        public override int GetHashCode() => Hash.GetCode(this.Id);
    }
}
