//--------------------------------------------------------------------------------------------------
// <copyright file="OrderAccepted.cs" company="Nautech Systems Pty Ltd">
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
using Nautilus.Core.Types;
using Nautilus.DomainModel.Events.Base;
using Nautilus.DomainModel.Identifiers;
using NodaTime;

namespace Nautilus.DomainModel.Events
{
    /// <summary>
    /// Represents an event where an order has been accepted by the broker.
    /// </summary>
    [Immutable]
    public sealed class OrderAccepted : OrderEvent
    {
        private static readonly Type EventType = typeof(OrderAccepted);

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderAccepted"/> class.
        /// </summary>
        /// <param name="accountId">The event account identifier.</param>
        /// <param name="orderId">The event order identifier.</param>
        /// <param name="orderIdBroker">The event order identifier from the broker.</param>
        /// <param name="label">The event order label. </param>
        /// <param name="acceptedTime">The event accepted time.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="eventTimestamp">The event timestamp.</param>
        public OrderAccepted(
            AccountId accountId,
            OrderId orderId,
            OrderIdBroker orderIdBroker,
            Label label,
            ZonedDateTime acceptedTime,
            Guid eventId,
            ZonedDateTime eventTimestamp)
            : base(
                orderId,
                EventType,
                eventId,
                eventTimestamp)
        {
            Debug.NotDefault(acceptedTime, nameof(acceptedTime));
            Debug.NotDefault(eventId, nameof(eventId));
            Debug.NotDefault(eventTimestamp, nameof(eventTimestamp));

            this.OrderIdBroker = orderIdBroker;
            this.AccountId = accountId;
            this.Label = label;
            this.AcceptedTime = acceptedTime;
        }

        /// <summary>
        /// Gets the events account identifier.
        /// </summary>
        public AccountId AccountId { get; }

        /// <summary>
        /// Gets the events order identifier from the broker.
        /// </summary>
        public OrderIdBroker OrderIdBroker { get; }

        /// <summary>
        /// Gets the events order label.
        /// </summary>
        public Label Label { get; }

        /// <summary>
        /// Gets the events order accepted time.
        /// </summary>
        public ZonedDateTime AcceptedTime { get; }

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{this.Type.Name}(" +
                                             $"AccountId={this.AccountId.Value}, " +
                                             $"OrderId={this.OrderId.Value}, " +
                                             $"OrderIdBroker={this.OrderIdBroker.Value}, " +
                                             $"Label={this.Label.Value})";
    }
}
