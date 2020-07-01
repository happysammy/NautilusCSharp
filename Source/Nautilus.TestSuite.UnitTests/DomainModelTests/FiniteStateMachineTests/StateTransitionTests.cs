// -------------------------------------------------------------------------------------------------
// <copyright file="StateTransitionTests.cs" company="Nautech Systems Pty Ltd">
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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Nautilus.DomainModel.Enums;
using Nautilus.DomainModel.Events;
using Nautilus.DomainModel.FiniteStateMachine;
using Xunit;

namespace Nautilus.TestSuite.UnitTests.DomainModelTests.FiniteStateMachineTests
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class StateTransitionTests
    {
        [Fact]
        public void IndexDictionary_WithExampleStateTransition_ReturnsExpectedValue()
        {
            // Arrange
            var stateTransitionTable = new Dictionary<StateTransition<OrderState>, OrderState>
            {
                { new StateTransition<OrderState>(OrderState.Initialized, Trigger.Event(typeof(OrderAccepted))), OrderState.Accepted },
            };

            // Act
            var result = stateTransitionTable[new StateTransition<OrderState>(OrderState.Initialized, Trigger.Event(typeof(OrderAccepted)))];

            // Assert
            Assert.Equal(OrderState.Accepted, result);
        }

        [Fact]
        public void ContainsKey_WithExampleTransitionTable_ReturnsTrue()
        {
            // Arrange
            var stateTransitionTable = new Dictionary<StateTransition<OrderState>, OrderState>
            {
                { new StateTransition<OrderState>(OrderState.Initialized, Trigger.Event(typeof(OrderAccepted))), OrderState.Accepted },
            };

            // Act
            var result = stateTransitionTable.ContainsKey(new StateTransition<OrderState>(OrderState.Initialized, Trigger.Event(typeof(OrderAccepted))));

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_VariousStateTransitionsAndOperators_TestsCorrectly()
        {
            // Arrange
            var stateTransition1 = new StateTransition<OrderState>(OrderState.Initialized, Trigger.Event(typeof(OrderAccepted)));
            var stateTransition2 = new StateTransition<OrderState>(OrderState.Initialized, Trigger.Event(typeof(OrderAccepted)));
            var stateTransition3 = new StateTransition<OrderState>(OrderState.Accepted, Trigger.Event(typeof(OrderWorking)));

            // Act
            var result1 = stateTransition1.Equals(stateTransition2);
            var result2 = !stateTransition1.Equals(stateTransition3);
            var result3 = stateTransition1 == stateTransition2;
            var result4 = stateTransition1 != stateTransition3;
            var result5 = stateTransition1.GetHashCode() == stateTransition2.GetHashCode();

            // Assert
            Assert.True(result1);
            Assert.True(result2);
            Assert.True(result3);
            Assert.True(result4);
            Assert.True(result5);
        }

        [Fact]
        public void GetHashCode_ReturnsExpectedInt()
        {
            // Arrange
            var stateTransition = new StateTransition<OrderState>(OrderState.Initialized, Trigger.Event(typeof(OrderAccepted)));

            // Act
            var result = stateTransition.GetHashCode();

            // Assert
            Assert.Equal(typeof(int), result.GetType());
        }

        [Fact]
        public void ToString_ReturnsExpectedString()
        {
            // Arrange
            var stateTransition = new StateTransition<OrderState>(OrderState.Initialized, Trigger.Event(typeof(OrderAccepted)));

            // Act
            var result = stateTransition.ToString();

            // Assert
            Assert.Equal("StateTransition(Initialized -> OrderAccepted)", result);
        }

        [Fact]
        public void Description_ReturnsExpectedString()
        {
            // Arrange
            var stateTransition = new StateTransition<OrderState>(OrderState.Initialized, Trigger.Event(typeof(OrderAccepted)));

            // Act
            var result = stateTransition.Description();

            // Assert
            Assert.Equal("Initialized -> OrderAccepted", result);
        }
    }
}
