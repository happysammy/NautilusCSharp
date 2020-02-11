// -------------------------------------------------------------------------------------------------
// <copyright file="FiniteStateMachineTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DomainModelTests.FiniteStateMachineTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Aggregates.Internal;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.FiniteStateMachine;
    using Xunit;

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
