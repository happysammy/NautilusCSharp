//--------------------------------------------------------------------------------------------------
// <copyright file="IEnvelope.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Messaging.Interfaces
{
    using System;
    using Nautilus.Core;
    using NodaTime;

    /// <summary>
    /// Represents a generic messaging envelope.
    /// </summary>
    public interface IEnvelope
    {
        /// <summary>
        /// Gets the envelopes message.
        /// </summary>
        Message MessageBase { get; }

        /// <summary>
        /// Gets the envelopes message type.
        /// </summary>
        Type MessageType { get; }

        /// <summary>
        /// Gets the envelopes receiver (optional).
        /// </summary>
        Address? Receiver { get; }

        /// <summary>
        /// Gets the envelopes sender (optional).
        /// </summary>
        Address? Sender { get; }

        /// <summary>
        /// Gets the envelope identifier.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the envelope creation timestamp.
        /// </summary>
        ZonedDateTime Timestamp { get; }

        /// <summary>
        /// Returns a value indicating whether this <see cref="IEnvelope"/> is equal to the given
        /// <see cref="IEnvelope"/>.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>True if the message identifier equals the other identifier, otherwise false.</returns>
        bool Equals(IEnvelope other);

        /// <summary>
        /// Returns the hash code for this <see cref="IEnvelope"/>.
        /// </summary>
        /// <returns>The hash code <see cref="int"/>.</returns>
        int GetHashCode();

        /// <summary>
        /// Returns a string representation of this <see cref="IEnvelope"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        string ToString();
    }
}
