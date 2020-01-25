// -------------------------------------------------------------------------------------------------
// <copyright file="StateTransitionTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DomainModelTests.FiniteStateMachineTests
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.FiniteStateMachine;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class StateTransitionTests
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
