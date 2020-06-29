//--------------------------------------------------------------------------------------------------
// <copyright file="AccountEvent.cs" company="Nautech Systems Pty Ltd">
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

namespace Nautilus.DomainModel.Events.Base
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Message;
    using Nautilus.DomainModel.Identifiers;
    using NodaTime;

    /// <summary>
    /// The base class for all account events.
    /// </summary>
    [Immutable]
    public abstract class AccountEvent : Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountEvent"/> class.
        /// </summary>
        /// <param name="accountId">The event order identifier.</param>
        /// <param name="eventType">The event type.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="eventTimestamp">The event timestamp.</param>
        protected AccountEvent(
            AccountId accountId,
            Type eventType,
            Guid eventId,
            ZonedDateTime eventTimestamp)
            : base(
                eventType,
                eventId,
                eventTimestamp)
        {
            Debug.NotDefault(eventId, nameof(eventId));
            Debug.NotDefault(eventTimestamp, nameof(eventTimestamp));

            this.AccountId = accountId;
        }

        /// <summary>
        /// Gets the events order identifier.
        /// </summary>
        public AccountId AccountId { get; }

        /// <summary>
        /// Returns a string representation of this <see cref="OrderEvent"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{this.Type.Name}({this.AccountId.Value})";
    }
}
