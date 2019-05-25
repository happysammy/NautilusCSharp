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
            stateMachine.Process(new Trigger(nameof(OrderSubmitted)));

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
            Assert.Throws<InvalidOperationException>(() => stateMachine.Process(new Trigger(nameof(OrderWorking))));
        }

        [Fact]
        public void Process_ThroughFullCycle_ReturnsExpectedState()
        {
            // Arrange
            var stateMachine = Order.CreateOrderFiniteStateMachine();

            // Act
            stateMachine.Process(new Trigger(nameof(OrderInitialized)));  // Redundant trigger doesn't throw.
            stateMachine.Process(new Trigger(nameof(OrderSubmitted)));
            stateMachine.Process(new Trigger(nameof(OrderAccepted)));
            stateMachine.Process(new Trigger(nameof(OrderWorking)));
            stateMachine.Process(new Trigger(nameof(OrderPartiallyFilled)));
            stateMachine.Process(new Trigger(nameof(OrderFilled)));

            // Assert
            Assert.Equal(new State(OrderStatus.Filled), stateMachine.State);
        }

        [Fact]
        public void Process_MultipleValidTriggersThenInvalidTrigger_ReturnsFailure()
        {
            // Arrange
            var stateMachine = Order.CreateOrderFiniteStateMachine();

            // Act
            stateMachine.Process(new Trigger(nameof(OrderSubmitted)));
            stateMachine.Process(new Trigger(nameof(OrderAccepted)));
            stateMachine.Process(new Trigger(nameof(OrderWorking)));
            stateMachine.Process(new Trigger(nameof(OrderPartiallyFilled)));

            // Assert
            Assert.Throws<InvalidOperationException>(() => stateMachine.Process(new Trigger(OrderStatus.Rejected)));
        }
    }
}
