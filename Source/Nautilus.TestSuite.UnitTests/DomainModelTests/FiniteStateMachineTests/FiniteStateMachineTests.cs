// -------------------------------------------------------------------------------------------------
// <copyright file="FiniteStateMachineTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DomainModelTests.FiniteStateMachineTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.FiniteStateMachine;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class FiniteStateMachineTests
    {
        [Fact]
        public void State_WithNewStateMachine_EqualsStartingState()
        {
            // Arrange
            var stateMachine = Order.CreateOrderFiniteStateMachine();

            // Act
            var result = stateMachine.State;

            // Assert
            Assert.Equal(new State(OrderStatus.Initialized), result);
        }

        [Fact]
        public void Process_WithValidTrigger_ReturnsExpectedState()
        {
            // Arrange
            var stateMachine = Order.CreateOrderFiniteStateMachine();

            // Act
            stateMachine.Process(Trigger.Event(typeof(OrderSubmitted)));

            // Assert
            Assert.Equal(new State(OrderStatus.Submitted), stateMachine.State);
        }

        [Fact]
        public void Process_WithInvalidTrigger_ReturnsFailure()
        {
            // Arrange
            var stateMachine = Order.CreateOrderFiniteStateMachine();

            // Act
            // Assert
            Assert.Throws<InvalidOperationException>(() => stateMachine.Process(Trigger.Event(typeof(OrderWorking))));
        }

        [Fact]
        public void Process_ThroughFullCycle_ReturnsExpectedState()
        {
            // Arrange
            var stateMachine = Order.CreateOrderFiniteStateMachine();

            // Act
            stateMachine.Process(Trigger.Event(typeof(OrderInitialized)));  // Redundant trigger doesn't throw.
            stateMachine.Process(Trigger.Event(typeof(OrderSubmitted)));
            stateMachine.Process(Trigger.Event(typeof(OrderAccepted)));
            stateMachine.Process(Trigger.Event(typeof(OrderWorking)));
            stateMachine.Process(Trigger.Event(typeof(OrderPartiallyFilled)));
            stateMachine.Process(Trigger.Event(typeof(OrderFilled)));

            // Assert
            Assert.Equal(new State(OrderStatus.Filled), stateMachine.State);
        }

        [Fact]
        public void Process_MultipleValidTriggersThenInvalidTrigger_ReturnsFailure()
        {
            // Arrange
            var stateMachine = Order.CreateOrderFiniteStateMachine();

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
