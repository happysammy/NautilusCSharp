// -------------------------------------------------------------------------------------------------
// <copyright file="FiniteStateMachineTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DomainModelTests.FiniteStateMachineTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.FiniteStateMachine;
    using Xunit;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class FiniteStateMachineTests
    {
        [Fact]
        public void CurrentState_WithNewStateMachine_EqualsStartingState()
        {
            // Arrange
            var stateMachine = ExampleOrderStateMachine.Create();

            // Act
            var result = stateMachine.CurrentState;

            // Assert
            Assert.Equal(new State(OrderStatus.Initialized), result);
        }

        [Fact]
        public void Process_WithValidTrigger_ReturnsExpectedState()
        {
            // Arrange
            var stateMachine = ExampleOrderStateMachine.Create();

            // Act
            var result = stateMachine.Process(new Trigger(nameof(OrderAccepted)));

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(new State(OrderStatus.Accepted), stateMachine.CurrentState);
        }

        [Fact]
        public void Process_WithInvalidTrigger_ReturnsFailure()
        {
            // Arrange
            var stateMachine = ExampleOrderStateMachine.Create();

            // Act
            var result = stateMachine.Process(new Trigger(nameof(OrderWorking)));

            // Assert
            Assert.True(result.IsFailure);
        }

        [Fact]
        public void Process_ThroughFullCycle_ReturnsExpectedState()
        {
            // Arrange
            var stateMachine = ExampleOrderStateMachine.Create();

            // Act
            stateMachine.Process(new Trigger(nameof(OrderAccepted)));
            stateMachine.Process(new Trigger(nameof(OrderWorking)));
            stateMachine.Process(new Trigger(nameof(OrderPartiallyFilled)));
            var result = stateMachine.Process(new Trigger(nameof(OrderFilled)));

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(new State(OrderStatus.Filled), stateMachine.CurrentState);
        }

        [Fact]
        public void Process_MultipleValidTriggersThenInvalidTrigger_ReturnsFailure()
        {
            // Arrange
            var stateMachine = ExampleOrderStateMachine.Create();

            // Act
            stateMachine.Process(new Trigger(nameof(OrderAccepted)));
            stateMachine.Process(new Trigger(nameof(OrderWorking)));
            stateMachine.Process(new Trigger(nameof(OrderPartiallyFilled)));
            var result = stateMachine.Process(new Trigger(nameof(OrderExpired)));

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(new State(OrderStatus.PartiallyFilled), stateMachine.CurrentState);
        }
    }
}
