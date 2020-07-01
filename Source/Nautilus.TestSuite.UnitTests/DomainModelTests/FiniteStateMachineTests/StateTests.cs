// -------------------------------------------------------------------------------------------------
// <copyright file="StateTests.cs" company="Nautech Systems Pty Ltd">
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

using System.Diagnostics.CodeAnalysis;
using Nautilus.DomainModel.Enums;
using Nautilus.DomainModel.FiniteStateMachine;
using Xunit;

namespace Nautilus.TestSuite.UnitTests.DomainModelTests.FiniteStateMachineTests
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class StateTests
    {
        [Fact]
        public void Equals_VariousStateTransitionsAndOperators_TestsCorrectly()
        {
            // Arrange
            var state1 = new State(OrderState.Initialized);
            var state2 = new State(OrderState.Initialized);
            var state3 = new State(OrderState.Accepted);

            // Act
            var result1 = state1.Equals(state2);
            var result2 = !state1.Equals(state3);
            var result3 = !state1.Equals((object)state3);
            var result4 = state1 == state2;
            var result5 = state1 != state3;
            var result6 = state1.GetHashCode() == state2.GetHashCode();

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
            var state = new State(OrderState.Initialized);

            // Act
            var result = state.GetHashCode();

            // Assert
            Assert.Equal(typeof(int), result.GetType());
        }

        [Fact]
        public void ToString_ReturnsExpectedString()
        {
            // Arrange
            var state = new State(OrderState.Initialized);

            // Act
            var result = state.ToString();

            // Assert
            Assert.Equal("Initialized", result);
        }
    }
}
