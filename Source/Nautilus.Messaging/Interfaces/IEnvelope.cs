//--------------------------------------------------------------------------------------------------
// <copyright file="IEnvelope.cs" company="Nautech Systems Pty Ltd">
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
using Nautilus.Core.Types;
using NodaTime;

namespace Nautilus.Messaging.Interfaces
{
    /// <summary>
    /// Represents a generic messaging envelope.
    /// </summary>
    public interface IEnvelope
    {
        /// <summary>
        /// Gets the envelopes message.
        /// </summary>
        Message Message { get; }

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
    }
}
