//--------------------------------------------------------------------------------------------------
// <copyright file="TradeEvent.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.Core.Message;
    using Nautilus.DomainModel.Events.Base;
    using Nautilus.DomainModel.Identifiers;

    /// <summary>
    /// Represents an event where an order had been accepted by the broker.
    /// </summary>
    [Immutable]
    public sealed class TradeEvent : Event
    {
        private static readonly Type EventType = typeof(TradeEvent);

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeEvent"/> class.
        /// </summary>
        /// <param name="traderId">The event trader identifier.</param>
        /// <param name="orderEvent">The event order event.</param>
        public TradeEvent(TraderId traderId, OrderEvent orderEvent)
            : base(
                EventType,
                orderEvent.Id,
                orderEvent.Timestamp)
        {
            this.TraderId = traderId;
            this.Event = orderEvent;
        }

        /// <summary>
        /// Gets the events trader identifier.
        /// </summary>
        public TraderId TraderId { get; }

        /// <summary>
        /// Gets the trade event.
        /// </summary>
        public OrderEvent Event { get; }

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{this.Type.Name}(" +
                                             $"{this.Event.Type.Name}, " +
                                             $"TraderId={this.TraderId.Value}, " +
                                             $"OrderId={this.Event.OrderId.Value})";
    }
}
