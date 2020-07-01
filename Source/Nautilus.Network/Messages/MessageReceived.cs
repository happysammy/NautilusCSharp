// -------------------------------------------------------------------------------------------------
// <copyright file="MessageReceived.cs" company="Nautech Systems Pty Ltd">
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
// -------------------------------------------------------------------------------------------------

using System;
using Nautilus.Core.Annotations;
using Nautilus.Core.Correctness;
using Nautilus.Core.Message;
using NodaTime;

namespace Nautilus.Network.Messages
{
    /// <summary>
    /// Represents a response acknowledging receipt of a message.
    /// </summary>
    [Immutable]
    public sealed class MessageReceived : Response
    {
        private static readonly Type EventType = typeof(MessageReceived);

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageReceived"/> class.
        /// </summary>
        /// <param name="receivedType">The received message type.</param>
        /// <param name="correlationId">The response correlation identifier.</param>
        /// <param name="id">The response identifier.</param>
        /// <param name="timestamp">The response timestamp.</param>
        public MessageReceived(
            string receivedType,
            Guid correlationId,
            Guid id,
            ZonedDateTime timestamp)
            : base(
                EventType,
                correlationId,
                id,
                timestamp)
        {
            Debug.NotEmptyOrWhiteSpace(receivedType, nameof(receivedType));
            Debug.NotDefault(id, nameof(id));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.ReceivedType = receivedType;
        }

        /// <summary>
        /// Gets the responses received message type.
        /// </summary>
        public string ReceivedType { get; }

        /// <summary>
        /// Returns the string representation of this object.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{this.Type.Name}(ReceivedType={this.ReceivedType}, CorrelationId={this.CorrelationId})";
    }
}
