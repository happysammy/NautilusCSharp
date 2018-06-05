//--------------------------------------------------------------------------------------------------
// <copyright file="CommandResultTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  https://github.com/nautechsystems/Nautilus.Core
//  the use of this source code is governed by the Apache 2.0 license
//  as found in the LICENSE.txt file.
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.CoreTests.CQSTests
{
    using System;
    using Nautilus.Core.CQS;
    using Nautilus.Core.Validation;
    using Xunit;

    public class CommandResultTests
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
            Assert.Equal("CommandResult Failure (error message).", result.FullMessage());
        }

        [Fact]
        public void Fail_WithNoErrorMessage_Throws()
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => ((Action)(() => { CommandResult.Fail(null); })).Invoke());
            Assert.Throws<ValidationException>(() => ((Action)(() => { CommandResult.Fail(string.Empty); })).Invoke());
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
            Assert.Equal("CommandResult Failure (failure 1).", result.FullMessage());
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
            Assert.Equal("CommandResult Failure (error 1; error 2).", result.FullMessage());
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
