//--------------------------------------------------------------------------------------------------
// <copyright file="OrderFsmFactory.cs" company="Nautech Systems Pty Ltd">
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

using System.Collections.Generic;
using System.Collections.Immutable;
using Nautilus.DomainModel.Enums;
using Nautilus.DomainModel.Events;
using Nautilus.DomainModel.FiniteStateMachine;

namespace Nautilus.DomainModel.Aggregates.Internal
{
    /// <summary>
    /// Provides a <see cref="FiniteStateMachine"/> for <see cref="Order"/>s.
    /// </summary>
    internal static class OrderFsmFactory
    {
        private static readonly ImmutableDictionary<StateTransition<OrderState>, OrderState> StateTransitionTable =
            new Dictionary<StateTransition<OrderState>, OrderState>
            {
                { new StateTransition<OrderState>(OrderState.Initialized, Trigger.Event(typeof(OrderCancelled))), OrderState.Cancelled },
                { new StateTransition<OrderState>(OrderState.Initialized, Trigger.Event(typeof(OrderInvalid))), OrderState.Invalid },
                { new StateTransition<OrderState>(OrderState.Initialized, Trigger.Event(typeof(OrderDenied))), OrderState.Denied },
                { new StateTransition<OrderState>(OrderState.Initialized, Trigger.Event(typeof(OrderSubmitted))), OrderState.Submitted },
                { new StateTransition<OrderState>(OrderState.Submitted, Trigger.Event(typeof(OrderCancelled))), OrderState.Cancelled },
                { new StateTransition<OrderState>(OrderState.Submitted, Trigger.Event(typeof(OrderRejected))), OrderState.Rejected },
                { new StateTransition<OrderState>(OrderState.Submitted, Trigger.Event(typeof(OrderAccepted))), OrderState.Accepted },
                { new StateTransition<OrderState>(OrderState.Submitted, Trigger.Event(typeof(OrderWorking))), OrderState.Working },
                { new StateTransition<OrderState>(OrderState.Rejected, Trigger.Event(typeof(OrderRejected))), OrderState.Rejected },
                { new StateTransition<OrderState>(OrderState.Accepted, Trigger.Event(typeof(OrderCancelled))), OrderState.Cancelled },
                { new StateTransition<OrderState>(OrderState.Accepted, Trigger.Event(typeof(OrderWorking))), OrderState.Working },
                { new StateTransition<OrderState>(OrderState.Accepted, Trigger.Event(typeof(OrderPartiallyFilled))), OrderState.PartiallyFilled },
                { new StateTransition<OrderState>(OrderState.Accepted, Trigger.Event(typeof(OrderFilled))), OrderState.Filled },
                { new StateTransition<OrderState>(OrderState.Working, Trigger.Event(typeof(OrderCancelled))), OrderState.Cancelled },
                { new StateTransition<OrderState>(OrderState.Working, Trigger.Event(typeof(OrderModified))), OrderState.Working },
                { new StateTransition<OrderState>(OrderState.Working, Trigger.Event(typeof(OrderExpired))), OrderState.Expired },
                { new StateTransition<OrderState>(OrderState.Working, Trigger.Event(typeof(OrderPartiallyFilled))), OrderState.PartiallyFilled },
                { new StateTransition<OrderState>(OrderState.Working, Trigger.Event(typeof(OrderFilled))), OrderState.Filled },
                { new StateTransition<OrderState>(OrderState.PartiallyFilled, Trigger.Event(typeof(OrderCancelled))), OrderState.Cancelled },
                { new StateTransition<OrderState>(OrderState.PartiallyFilled, Trigger.Event(typeof(OrderPartiallyFilled))), OrderState.PartiallyFilled },
                { new StateTransition<OrderState>(OrderState.PartiallyFilled, Trigger.Event(typeof(OrderFilled))), OrderState.Filled },
            }.ToImmutableDictionary();

        /// <summary>
        /// Return a new <see cref="Order"/> <see cref="FiniteStateMachine"/>.
        /// </summary>
        /// <returns>The finite state machine.</returns>
        internal static FiniteStateMachine<OrderState> Create()
        {
            return new FiniteStateMachine<OrderState>(StateTransitionTable, OrderState.Initialized);
        }
    }
}
