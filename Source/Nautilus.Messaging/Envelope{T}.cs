//--------------------------------------------------------------------------------------------------
// <copyright file="Envelope{T}.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Messaging
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Correctness;
    using NodaTime;

    /// <summary>
    /// Provides an envelope wrapper for all messages sent via the messaging service.
    /// </summary>
    /// <typeparam name="T">The message type.</typeparam>
    public sealed class Envelope<T>
        where T : Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Envelope{T}"/> class.
        /// </summary>
        /// <param name="message">The message payload.</param>
        /// <param name="receiver">The envelope receiver.</param>
        /// <param name="sender">The envelope sender.</param>
        /// <param name="timestamp">The envelopes timestamp.</param>
        public Envelope(
            T message,
            Address? receiver,
            Address? sender,
            ZonedDateTime timestamp)
        {
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.Message = message;
            this.Receiver = receiver;
            this.Sender = sender;
            this.Id = message.Id;
            this.Timestamp = timestamp;
        }

        /// <summary>
        /// Gets the envelopes message.
        /// </summary>
        public T Message { get; }

        /// <summary>
        /// Gets the envelopes receiver.
        /// </summary>
        public Address? Receiver { get; }

        /// <summary>
        /// Gets the envelopes sender.
        /// </summary>
        public Address? Sender { get; }

        /// <summary>
        /// Gets the envelope identifier.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Gets the envelope creation timestamp.
        /// </summary>
        public ZonedDateTime Timestamp { get; }

        /// <summary>
        /// Returns the hash code for this <see cref="Envelope{T}"/>.
        /// </summary>
        /// <returns>The hash code <see cref="int"/>.</returns>
        public override int GetHashCode() => Hash.GetCode(this.Id);

        /// <summary>
        /// Returns a string representation of this <see cref="Envelope{T}"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"Envelope<{typeof(T).Name}>[{this.Message}]";
    }
}
