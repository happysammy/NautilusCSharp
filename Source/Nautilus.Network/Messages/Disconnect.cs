// -------------------------------------------------------------------------------------------------
// <copyright file="Disconnect.cs" company="Nautech Systems Pty Ltd">
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
using Nautilus.Network.Identifiers;
using NodaTime;

namespace Nautilus.Network.Messages
{
    /// <summary>
    /// Represents a request to disconnect from a session.
    /// </summary>
    [Immutable]
    public sealed class Disconnect : Request
    {
        private static readonly Type EventType = typeof(Disconnect);

        /// <summary>
        /// Initializes a new instance of the <see cref="Disconnect"/> class.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="sessionId">The session identifier to disconnect from.</param>
        /// <param name="id">The response identifier.</param>
        /// <param name="timestamp">The response timestamp.</param>
        public Disconnect(
            ClientId clientId,
            SessionId sessionId,
            Guid id,
            ZonedDateTime timestamp)
            : base(
                EventType,
                id,
                timestamp)
        {
            Debug.NotDefault(id, nameof(id));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.ClientId = clientId;
            this.SessionId = sessionId;
        }

        /// <summary>
        /// Gets the requests client identifier.
        /// </summary>
        public ClientId ClientId { get; }

        /// <summary>
        /// Gets the requests session identifier.
        /// </summary>
        public SessionId SessionId { get; }
    }
}
