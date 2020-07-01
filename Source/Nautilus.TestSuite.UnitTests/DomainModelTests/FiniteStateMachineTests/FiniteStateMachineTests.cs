// -------------------------------------------------------------------------------------------------
// <copyright file="FiniteStateMachineTests.cs" company="Nautech Systems Pty Ltd">
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
using System.Diagnostics.CodeAnalysis;
using Nautilus.DomainModel.Aggregates.Internal;
using Nautilus.DomainModel.Enums;
using Nautilus.DomainModel.Events;
using Nautilus.DomainModel.FiniteStateMachine;
using Xunit;

namespace Nautilus.TestSuite.UnitTests.DomainModelTests.FiniteStateMachineTests
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class FiniteStateMachineTests
    {
        [Fact]
        public void State_WithNewStateMachine_EqualsStartingState()
        {
            // Arrange
            var stateMachine = OrderFsmFactory.Create();

            // Act
            var result = stateMachine.State;

            // Assert
            Assert.Equal(OrderState.Initialized, result);
        }

        [Fact]
        public void Process_WithValidTrigger_ReturnsExpectedState()
        {
            // Arrange
            var stateMachine = OrderFsmFactory.Create();

            // Act
            stateMachine.Process(Trigger.Event(typeof(OrderSubmitted)));

            // Assert
            Assert.Equal(OrderState.Submitted, stateMachine.State);
        }

        [Fact]
        public void Process_WithInvalidTrigger_ReturnsFailure()
        {
            // Arrange
            var stateMachine = OrderFsmFactory.Create();

            // Act
            // Assert
            Assert.Throws<InvalidOperationException>(() => stateMachine.Process(Trigger.Event(typeof(OrderExpired))));
        }

        [Fact]
        public void Process_ThroughFullCycle_ReturnsExpectedState()
        {
            // Arrange
            var stateMachine = OrderFsmFactory.Create();

            // Act
            stateMachine.Process(Trigger.Event(typeof(OrderSubmitted)));
            stateMachine.Process(Trigger.Event(typeof(OrderAccepted)));
            stateMachine.Process(Trigger.Event(typeof(OrderWorking)));
            stateMachine.Process(Trigger.Event(typeof(OrderPartiallyFilled)));
            stateMachine.Process(Trigger.Event(typeof(OrderFilled)));

            // Assert
            Assert.Equal(OrderState.Filled, stateMachine.State);
        }

        [Fact]
        public void Process_MultipleValidTriggersThenInvalidTrigger_ReturnsFailure()
        {
            // Arrange
            var stateMachine = OrderFsmFactory.Create();

            // Act
            stateMachine.Process(Trigger.Event(typeof(OrderSubmitted)));
            stateMachine.Process(Trigger.Event(typeof(OrderAccepted)));
            stateMachine.Process(Trigger.Event(typeof(OrderWorking)));
            stateMachine.Process(Trigger.Event(typeof(OrderPartiallyFilled)));

            // Assert
            Assert.Throws<InvalidOperationException>(() => stateMachine.Process(Trigger.Event(typeof(OrderRejected))));
        }
    }
}
