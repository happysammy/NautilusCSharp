// -------------------------------------------------------------------------------------------------
// <copyright file="TriggerTests.cs" company="Nautech Systems Pty Ltd">
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

namespace Nautilus.TestSuite.UnitTests.DomainModelTests.FiniteStateMachineTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.FiniteStateMachine;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class TriggerTests
    {
        [Fact]
        public void Equals_VariousTriggerTransitionsAndOperators_TestsCorrectly()
        {
            // Arrange
            var trigger1 = Trigger.Event(typeof(OrderAccepted));
            var trigger2 = Trigger.Event(typeof(OrderAccepted));
            var trigger3 = Trigger.Event(typeof(OrderWorking));

            // Act
            var result1 = trigger1.Equals(trigger2);
            var result2 = trigger1.Equals((object)trigger2);
            var result3 = !trigger1.Equals(trigger3);
            var result4 = trigger1 == trigger2;
            var result5 = trigger1 != trigger3;
            var result6 = trigger1.GetHashCode() == trigger2.GetHashCode();

            // Assert
            Assert.True(result1);
            Assert.True(result2);
            Assert.True(result3);
            Assert.True(result4);
            Assert.True(result5);
            Assert.True(result6);
        }

        [Fact]
        public void GetHashCode_ReturnsExpectedInt()
        {
            // Arrange
            var trigger = Trigger.Event(typeof(OrderCancelled));

            // Act
            var result = trigger.GetHashCode();

            // Assert
            Assert.Equal(typeof(int), result.GetType());
        }

        [Fact]
        public void ToString_ReturnsExpectedString()
        {
            // Arrange
            var trigger = Trigger.Event(typeof(OrderWorking));

            // Act
            var result = trigger.ToString();

            // Assert
            Assert.Equal("OrderWorking", result);
        }
    }
}
