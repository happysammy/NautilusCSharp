//--------------------------------------------------------------------------------------------------
// <copyright file="OrderInvalid.cs" company="Nautech Systems Pty Ltd">
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

namespace Nautilus.DomainModel.Events
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Events.Base;
    using Nautilus.DomainModel.Identifiers;
    using NodaTime;

    /// <summary>
    /// Represents an event where an order had been invalidated by the system.
    /// </summary>
    [Immutable]
    public sealed class OrderInvalid : OrderEvent
    {
        private static readonly Type EventType = typeof(OrderInvalid);

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderInvalid"/> class.
        /// </summary>
        /// <param name="orderId">The event order identifier.</param>
        /// <param name="invalidReason">The event invalid reason.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="eventTimestamp">The event timestamp.</param>
        public OrderInvalid(
            OrderId orderId,
            string invalidReason,
            Guid eventId,
            ZonedDateTime eventTimestamp)
            : base(
                orderId,
                EventType,
                eventId,
                eventTimestamp)
        {
            Debug.NotDefault(eventId, nameof(eventId));
            Debug.NotDefault(eventTimestamp, nameof(eventTimestamp));

            this.InvalidReason = invalidReason;
        }

        /// <summary>
        /// Gets the events message.
        /// </summary>
        public string InvalidReason { get; }

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{this.Type.Name}(" +
                                             $"OrderId={this.OrderId.Value}, " +
                                             $"InvalidReason={this.InvalidReason})";
    }
}
