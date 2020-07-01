//--------------------------------------------------------------------------------------------------
// <copyright file="ExceptionFactoryTests.cs" company="Nautech Systems Pty Ltd">
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
//--------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Nautilus.Core.Correctness;
using Nautilus.DomainModel.Enums;
using Xunit;

namespace Nautilus.TestSuite.UnitTests.CoreTests.CorrectnessTests
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class ExceptionFactoryTests
    {
        [Fact]
        internal void InvalidSwitchArgument_WithObjectArgument_ReturnsExpectedException()
        {
            // Arrange
            var someArgument = "-1";

            // Act
            var exception = ExceptionFactory.InvalidSwitchArgument(someArgument, nameof(someArgument));

            // Assert
            Assert.Equal(typeof(ArgumentOutOfRangeException), exception.GetType());
            Assert.Equal(someArgument, exception.ActualValue);
            Assert.Equal("The value of argument 'someArgument' of type String was invalid out of range for this switch. (Parameter 'someArgument')\nActual value was -1.", exception.Message);
        }

        [Fact]
        internal void InvalidSwitchArgument_WithEnumArgument_ReturnsExpectedException()
        {
            // Arrange
            var someArgument = PriceType.Last;

            // Act
            var exception = ExceptionFactory.InvalidSwitchArgument(someArgument, nameof(someArgument));

            // Assert
            Assert.Equal(typeof(InvalidEnumArgumentException), exception.GetType());
            Assert.Equal("The value of argument 'someArgument' (4) is invalid for Enum type 'PriceType'. (Parameter 'someArgument')", exception.Message);
        }
    }
}
