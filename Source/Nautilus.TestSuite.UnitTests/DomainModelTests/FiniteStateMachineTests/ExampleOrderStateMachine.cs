// -------------------------------------------------------------------------------------------------
// <copyright file="ExampleOrderStateMachine.cs" company="Nautech Systems Pty Ltd.">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DomainModelTests.FiniteStateMachineTests
{
    using System.Collections.Generic;
    using NautechSystems.CSharp.Annotations;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.FiniteStateMachine;

    [Immutable]
    public static class ExampleOrderStateMachine
    {
        /// <summary>
        /// Returns a <see cref="FiniteStateMachine"/> which represents a generic order.
        /// </summary>
        /// <returns>A <see cref="FiniteStateMachine"/>.</returns>
        public static FiniteStateMachine Create()
        {
            var stateTransitionTable = new Dictionary<StateTransition, State>
            {
                { new StateTransition(new State(OrderStatus.Initialized), new Trigger(nameof(OrderAccepted))), new State(OrderStatus.Accepted) },
                { new StateTransition(new State(OrderStatus.Initialized), new Trigger(nameof(OrderRejected))), new State(OrderStatus.Rejected) },
                { new StateTransition(new State(OrderStatus.Accepted), new Trigger(nameof(OrderWorking))), new State(OrderStatus.Working) },
                { new StateTransition(new State(OrderStatus.Working), new Trigger(nameof(OrderCancelled))), new State(OrderStatus.Cancelled) },
                { new StateTransition(new State(OrderStatus.Working), new Trigger(nameof(OrderExpired))), new State(OrderStatus.Expired) },
                { new StateTransition(new State(OrderStatus.Working), new Trigger(nameof(OrderFilled))), new State(OrderStatus.Filled) },
                { new StateTransition(new State(OrderStatus.Working), new Trigger(nameof(OrderPartiallyFilled))), new State(OrderStatus.PartiallyFilled) },
                { new StateTransition(new State(OrderStatus.PartiallyFilled), new Trigger(nameof(OrderPartiallyFilled))), new State(OrderStatus.PartiallyFilled) },
                { new StateTransition(new State(OrderStatus.PartiallyFilled), new Trigger(nameof(OrderFilled))), new State(OrderStatus.Filled) }
            };

            return new FiniteStateMachine(stateTransitionTable, new State(OrderStatus.Initialized));
        }
    }
}
