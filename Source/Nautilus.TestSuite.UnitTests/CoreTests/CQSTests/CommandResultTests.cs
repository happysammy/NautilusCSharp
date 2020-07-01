//--------------------------------------------------------------------------------------------------
// <copyright file="CommandResultTests.cs" company="Nautech Systems Pty Ltd">
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

using System.Diagnostics.CodeAnalysis;
using Nautilus.Core.CQS;
using Xunit;

namespace Nautilus.TestSuite.UnitTests.CoreTests.CQSTests
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class CommandResultTests
    {
        [Fact]
        public void Ok_ReturnsOk()
        {
            // Arrange
            // Act
            var result = CommandResult.Ok();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailure);
        }

        [Fact]
        public void Ok_WithMessage_ReturnsOkWithMessage()
        {
            // Arrange
            var message = "The command was successful.";

            // Act
            var result = CommandResult.Ok(message);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(message, result.Message);
        }

        [Fact]
        public void Fail_WithValidInputs_ReturnsExpectedResult()
        {
            // Arrange
            // Act
            var result = CommandResult.Fail("error message");

            // Assert
            Assert.True(result.IsFailure);
            Assert.False(result.IsSuccess);
            Assert.Equal("error message", result.Message);
        }

        [Fact]
        public void FirstFailureOrSuccess_WithFailures_ReturnsFirstResult()
        {
            // Arrange
            var result1 = CommandResult.Ok();
            var result2 = CommandResult.Fail("failure 1");
            var result3 = CommandResult.Fail("failure 2");

            // Act
            var result = CommandResult.FirstFailureOrSuccess(result1, result2, result3);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("failure 1", result.Message);
        }

        [Fact]
        public void FirstFailureOrSuccess_WithNoFailures_ReturnsSuccess()
        {
            // Arrange
            var result1 = CommandResult.Ok();
            var result2 = CommandResult.Ok();
            var result3 = CommandResult.Ok();

            // Act
            var result = CommandResult.FirstFailureOrSuccess(result1, result2, result3);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void Combine_WithErrors_ReturnsExpectedResult()
        {
            // Arrange
            var result1 = CommandResult.Ok();
            var result2 = CommandResult.Fail("error 1");
            var result3 = CommandResult.Fail("error 2");

            // Act
            var result = CommandResult.Combine(result1, result2, result3);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("error 1; error 2", result.Message);
        }

        [Fact]
        public void Combine_AllOk_ReturnsExpectedResult()
        {
            // Arrange
            var result1 = CommandResult.Ok();
            var result2 = CommandResult.Ok();
            var result3 = CommandResult.Ok();

            // Act
            var result = CommandResult.Combine(result1, result2, result3);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void Combine_InArray_ReturnsExpectedResult()
        {
            // Arrange
            CommandResult[] commandResults = { CommandResult.Ok(), CommandResult.Ok() };

            // Act
            var result = CommandResult.Combine(commandResults);

            // Assert
            Assert.True(result.IsSuccess);
        }
    }
}
