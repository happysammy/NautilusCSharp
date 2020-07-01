//--------------------------------------------------------------------------------------------------
// <copyright file="SessionConnected.cs" company="Nautech Systems Pty Ltd">
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
using Nautilus.Core.Message;
using Nautilus.DomainModel.Identifiers;
using NodaTime;

namespace Nautilus.Common.Messages.Events
{
    /// <summary>
    /// Represents an event where a brokerage session has been connected.
    /// </summary>
    [Immutable]
    public sealed class SessionConnected : Event
    {
        private static readonly Type EventType = typeof(SessionConnected);

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionConnected"/> class.
        /// </summary>
        /// <param name="broker">The events brokerage connected to.</param>
        /// <param name="sessionId">The events session identifier.</param>
        /// <param name="id">The events identifier.</param>
        /// <param name="timestamp">The events timestamp.</param>
        public SessionConnected(
            Brokerage broker,
            string sessionId,
            Guid id,
            ZonedDateTime timestamp)
            : base(EventType, id, timestamp)
        {
            Debug.NotEmptyOrWhiteSpace(sessionId, nameof(sessionId));

            this.Broker = broker;
            this.SessionId = sessionId;
        }

        /// <summary>
        /// Gets the connection events broker.
        /// </summary>
        public Brokerage Broker { get; }

        /// <summary>
        /// Gets the connection events session identifier.
        /// </summary>
        public string SessionId { get; }
    }
}
