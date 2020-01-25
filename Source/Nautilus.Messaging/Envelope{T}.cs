//--------------------------------------------------------------------------------------------------
// <copyright file="Envelope{T}.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Messaging
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Types;
    using Nautilus.Messaging.Interfaces;
    using NodaTime;

    /// <summary>
    /// Represents an envelope with a wrapped message and optional sender and receiver addresses.
    /// </summary>
    /// <typeparam name="T">The message type.</typeparam>
    [Immutable]
    public sealed class Envelope<T> : IEnvelope, IEquatable<Envelope<T>>, IEquatable<IEnvelope>
        where T : Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Envelope{T}"/> class.
        /// </summary>
        /// <param name="message">The message to wrap.</param>
        /// <param name="receiver">The envelope receiver.</param>
        /// <param name="sender">The envelope sender.</param>
        /// <param name="timestamp">The envelope creation timestamp.</param>
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
        /// Gets the envelopes message as a type of <see cref="Message"/>.
        /// </summary>
        public Message MessageBase => this.Message;

        /// <summary>
        /// Gets the envelopes message type.
        /// </summary>
        public Type MessageType => this.Message.Type;

        /// <summary>
        /// Gets the envelopes receiver (optional).
        /// </summary>
        public Address? Receiver { get; }

        /// <summary>
        /// Gets the envelopes sender (optional).
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
        /// Returns a value indicating whether this <see cref="Envelope{T}"/> is equal to the given
        /// <see cref="Envelope{T}"/>.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>True if the message identifier equals the other identifier, otherwise false.</returns>
        public bool Equals(Envelope<T> other) => other != null && other.Id == this.Id;

        /// <summary>
        /// Returns a value indicating whether this <see cref="IEnvelope"/> is equal to the given
        /// <see cref="IEnvelope"/>.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>True if the message identifier equals the other identifier, otherwise false.</returns>
        public bool Equals(IEnvelope other) => other != null && other.Id == this.Id;

        /// <summary>
        /// Returns the hash code for this <see cref="Envelope{T}"/>.
        /// </summary>
        /// <returns>The hash code <see cref="int"/>.</returns>
        public override int GetHashCode() => Hash.GetCode(this.Id);

        /// <summary>
        /// Returns a string representation of this <see cref="Envelope{T}"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{typeof(Envelope<T>).ExtractFormattedName()}({this.Id})";
    }
}
