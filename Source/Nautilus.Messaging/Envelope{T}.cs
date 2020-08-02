//--------------------------------------------------------------------------------------------------
// <copyright file="Envelope{T}.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
//--------------------------------------------------------------------------------------------------

using System;
using Nautilus.Core.Annotations;
using Nautilus.Core.Correctness;
using Nautilus.Core.Extensions;
using Nautilus.Core.Types;
using Nautilus.Messaging.Interfaces;
using NodaTime;

namespace Nautilus.Messaging
{
    /// <summary>
    /// Represents an envelope with a wrapped message and optional sender and receiver addresses.
    /// </summary>
    /// <typeparam name="T">The message type.</typeparam>
    [Immutable]
    public sealed class Envelope<T> : IEnvelope, IEquatable<Envelope<T>>, IEquatable<IEnvelope>
        where T : Message
    {
        private readonly T message;

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

            this.message = message;
            this.Receiver = receiver;
            this.Sender = sender;
            this.Id = message.Id;
            this.Timestamp = timestamp;
        }

        /// <summary>
        /// Gets the envelopes message.
        /// </summary>
        public Message Message => this.message;

        /// <summary>
        /// Gets the envelopes message type.
        /// </summary>
        public Type MessageType => this.message.Type;

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

        // Due to the convention that an IEquatable<T> argument can be null the compiler now emits
        // a warning unless Equals is marked with [AllowNull] or takes a nullable param. We don't
        // want to allow null here for the sake of silencing the warning and so temporarily using
        // #pragma warning disable CS8767 until a better refactoring is determined.
#pragma warning disable CS8767
        /// <summary>
        /// Returns a value indicating whether this <see cref="Envelope{T}"/> is equal to the given
        /// <see cref="Envelope{T}"/>.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>True if the message identifier equals the other identifier, otherwise false.</returns>
        public bool Equals(Envelope<T> other) => other.Id == this.Id;

        // Due to the convention that an IEquatable<T> argument can be null the compiler now emits
        // a warning unless Equals is marked with [AllowNull] or takes a nullable param. We don't
        // want to allow null here for the sake of silencing the warning and so temporarily using
        // #pragma warning disable CS8767 until a better refactoring is determined.
#pragma warning disable CS8767
        /// <summary>
        /// Returns a value indicating whether this <see cref="IEnvelope"/> is equal to the given
        /// <see cref="IEnvelope"/>.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>True if the message identifier equals the other identifier, otherwise false.</returns>
        public bool Equals(IEnvelope other) => other.Id == this.Id;

        /// <summary>
        /// Returns a string representation of this <see cref="Envelope{T}"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{typeof(Envelope<T>).NameFormatted()}({this.Id})";
    }
}
