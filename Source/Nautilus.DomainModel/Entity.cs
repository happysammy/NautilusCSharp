//--------------------------------------------------------------------------------------------------
// <copyright file="Entity.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel
{
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using NodaTime;

    /// <summary>
    /// The immutable abstract <see cref="Entity{T}"/> class. The base class for all uniquely
    /// identifiable domain objects.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    [Immutable]
    public abstract class Entity<T> where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Entity{T}"/> class.
        /// </summary>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="entityTimestamp">The entity timestamp.</param>
        protected Entity(
            EntityId entityId,
            ZonedDateTime entityTimestamp)
        {
            Debug.NotNull(entityId, nameof(entityId));
            Debug.NotDefault(entityTimestamp, nameof(entityTimestamp));

            this.EntityId = entityId;
            this.EntityTimestamp = entityTimestamp;
        }

        /// <summary>
        /// Gets the entity identifier.
        /// </summary>
        protected EntityId EntityId { get; }

        /// <summary>
        /// Gets the entity timestamp.
        /// </summary>
        protected ZonedDateTime EntityTimestamp { get; }

        /// <summary>
        /// Returns a value indicating whether this entity is equal to the given <see cref="object"/>.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public override bool Equals([CanBeNull] object other) => this.Equals(other as T);

        /// <summary>
        /// Returns a value indicating whether this entity is equal to the given entity.
        /// </summary>
        /// <param name="other">The other entity</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public bool Equals([CanBeNull] T other)
        {
            var otherEntity = other as Entity<T>;
            return otherEntity != null & this.EntityId.Equals(otherEntity?.EntityId);
        }

        /// <summary>
        /// Returns the hash code for this entity.
        /// </summary>
        /// <returns>An <see cref="int"/>.</returns>
        public override int GetHashCode() => this.EntityId.ToString().GetHashCode();
    }
}