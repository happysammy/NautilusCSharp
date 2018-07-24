//--------------------------------------------------------------------------------------------------
// <copyright file="Event.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using NodaTime;

    /// <summary>
    /// The base class for all events.
    /// </summary>
    [Immutable]
    public abstract class Event : ISendable<Event>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Event"/> class.
        /// </summary>
        /// <param name="id">The event identifier.</param>
        /// <param name="timestamp">The event timestamp.</param>
        protected Event(Guid id, ZonedDateTime timestamp)
        {
            Debug.NotDefault(id, nameof(id));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.Id = id;
            this.Timestamp = timestamp;
        }

        /// <summary>
        /// Gets the events identifier.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Gets the events timestamp.
        /// </summary>
        public ZonedDateTime Timestamp { get; }

        /// <summary>
        /// Returns a value indicating whether this event is equal to the given <see cref="Event"/>.
        /// </summary>
        /// <param name="other">The other event.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public bool Equals(Event other)
        {
            return other != null & this.Id.Equals(other?.Id);
        }

        /// <summary>
        /// Returns a value indicating whether this event is equal to the given <see cref="object"/>.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public override bool Equals([CanBeNull] object other)
        {
            // ReSharper disable once UsePatternMatching (causes compiler warning).
            var otherEvent = other as Event;
            return otherEvent != null & this.Id.Equals(otherEvent?.Id);
        }

        /// <summary>
        /// Returns the hash code for this event.
        /// </summary>
        /// <returns>An <see cref="int"/>.</returns>
        public override int GetHashCode() => this.Id.ToString().GetHashCode();

        /// <summary>
        /// Returns a string representation of this event.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => this.GetType().Name;
    }
}
