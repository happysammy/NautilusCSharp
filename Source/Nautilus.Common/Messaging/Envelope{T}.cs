//--------------------------------------------------------------------------------------------------
// <copyright file="Envelope{T}.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messaging
{
    using System;
    using Nautilus.Common.Enums;
    using Nautilus.Core;
    using Nautilus.Core.Interfaces;
    using Nautilus.Core.Validation;
    using NodaTime;

    /// <summary>
    /// Provides an envelope wrapper for all messages sent via the messaging service.
    /// </summary>
    /// <typeparam name="T">The message type.</typeparam>
    public sealed class Envelope<T>
        where T : ISendable<Message>
    {
        private readonly T message;

        /// <summary>
        /// Initializes a new instance of the <see cref="Envelope{T}"/> class.
        /// </summary>
        /// <param name="receiver">The envelope receivers.</param>
        /// <param name="sender">The envelope sender.</param>
        /// <param name="message">The envelopes message.</param>
        /// <param name="envelopeId">The envelopes identifier.</param>
        /// <param name="timestamp">The envelopes timestamp.</param>
        public Envelope(
            NautilusService receiver,
            NautilusService sender,
            T message,
            Guid envelopeId,
            ZonedDateTime timestamp)
        {
            Debug.NotNull(receiver, nameof(receiver));
            Debug.NotNull(message, nameof(message));
            Debug.NotDefault(envelopeId, nameof(envelopeId));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.Receiver = receiver;
            this.Sender = sender;
            this.EnvelopeId = envelopeId;
            this.Timestamp = timestamp;
            this.message = message;
        }

        /// <summary>
        /// Gets the envelope receiver(s).
        /// </summary>
        public NautilusService Receiver { get; }

        /// <summary>
        /// Gets the envelope sender.
        /// </summary>
        public NautilusService Sender { get; }

        /// <summary>
        /// Gets the envelope identifier.
        /// </summary>
        public Guid EnvelopeId { get; }

        /// <summary>
        /// Gets the envelopes message type.
        /// </summary>
        public Type Type => this.message.GetType();

        /// <summary>
        /// Gets the envelope creation timestamp.
        /// </summary>
        public ZonedDateTime Timestamp { get; }

        /// <summary>
        /// Gets the envelopes opened time.
        /// </summary>
        public Option<ZonedDateTime?> OpenedTime { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the envelope has been opened.
        /// </summary>
        public bool IsOpened => this.OpenedTime.HasValue;

        /// <summary>
        /// Opens the envelope and returns the contained message (records the opened time if not
        /// already opened).
        /// </summary>
        /// <param name="currentTime">The current time (cannot be default).</param>
        /// <returns>The contained message of type T.</returns>
        public T Open(ZonedDateTime currentTime)
        {
            Debug.NotDefault(currentTime, nameof(currentTime));

            if (this.OpenedTime.HasNoValue)
            {
                this.OpenedTime = currentTime;
            }

            return this.message;
        }

        /// <summary>
        /// Returns the hash code for this <see cref="Envelope{T}"/>.
        /// </summary>
        /// <returns>The hash code <see cref="int"/>.</returns>
        public override int GetHashCode() => Hash.GetCode(this.EnvelopeId);

        /// <summary>
        /// Returns a string representation of this <see cref="Envelope{T}"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"[{this.message}]";
    }
}
