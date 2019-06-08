//--------------------------------------------------------------------------------------------------
// <copyright file="Message.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using NodaTime;

    /// <summary>
    /// The base class for all <see cref="Message"/>s.
    /// </summary>
    [Immutable]
    public abstract class Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Message"/> class.
        /// </summary>
        /// <param name="type">The message type.</param>
        /// <param name="id">The message identifier.</param>
        /// <param name="timestamp">The message timestamp.</param>
        protected Message(
            Type type,
            Guid id,
            ZonedDateTime timestamp)
        {
            Debug.EqualTo(type, this.GetType(), nameof(type));
            Debug.NotDefault(id, nameof(id));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.Type = type;
            this.Id = id;
            this.Timestamp = timestamp;
        }

        /// <summary>
        /// Gets the message type.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets the message identifier.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Gets the message creation timestamp.
        /// </summary>
        public ZonedDateTime Timestamp { get; }

        /// <summary>
        /// Returns a value indicating whether the <see cref="Message"/>(s) are equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator ==(Message left, Message right)
        {
            if (left is null && right is null)
            {
                return true;
            }

            if (left is null || right is null)
            {
                return false;
            }

            return left.Equals(right);
        }

        /// <summary>
        /// Returns a value indicating whether the <see cref="Message"/>(s) are not equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator !=(Message left, Message right) => !(left == right);

        /// <summary>
        /// Returns a value indicating whether this <see cref="Message"/> is equal
        /// to the given <see cref="object"/>.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public override bool Equals(object other) => other is Message message && this.Equals(message);

        /// <summary>
        /// Returns a value indicating whether this <see cref="Message"/> is equal
        /// to the given <see cref="Message"/>.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>True if the message identifier equals the other identifier, otherwise false.</returns>
        public bool Equals(Message other) => this.Id == other.Id;

        /// <summary>
        /// Returns the hash code for this <see cref="Message"/>.
        /// </summary>
        /// <returns>The hash code <see cref="int"/>.</returns>
        public override int GetHashCode() => Hash.GetCode(this.Id);

        /// <summary>
        /// Returns a string representation of this <see cref="Message"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{this.Type.ExtractFormattedName()}(Id={this.Id})";
    }
}
