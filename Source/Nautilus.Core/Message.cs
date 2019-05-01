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
    using System.Diagnostics;
    using Nautilus.Core.Annotations;
    using NodaTime;

    /// <summary>
    /// The base class for all message types.
    /// </summary>
    [Immutable]
    public abstract class Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Message"/> class.
        /// </summary>
        /// <param name="id">The message identifier.</param>
        /// <param name="timestamp">The message timestamp.</param>
        protected Message(Guid id, ZonedDateTime timestamp)
        {
            Debug.Assert(id != default, AssertMsg.IsDefault(nameof(id)));
            Debug.Assert(timestamp != default, AssertMsg.IsDefault(nameof(timestamp)));

            this.Id = id;
            this.Timestamp = timestamp;
        }

        /// <summary>
        /// Gets the message identifier.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Gets the message timestamp.
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
        public override bool Equals(object other) => other != null && this.Equals(other);

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
        public override string ToString() => this.GetType().Name;
    }
}
