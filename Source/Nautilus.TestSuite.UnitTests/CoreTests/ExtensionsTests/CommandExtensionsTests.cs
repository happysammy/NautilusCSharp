//--------------------------------------------------------------------------------------------------
// <copyright file="CommandExtensionsTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  https://github.com/nautechsystems/Nautilus.Core
//  the use of this source code is governed by the Apache 2.0 license
//  as found in the LICENSE.txt file.
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.CoreTests.ExtensionsTests
{
    using Nautilus.Core.CQS;
    using Nautilus.Core.Extensions;
    using Xunit;

    public class CommandExtensionsTests
    {
        private const string ErrorMessage = "this failed";

        [Fact]
        public void OnSuccess_WithAnonymousFunction_PerformsFunction()
        {
            // Arrange
            var testBool = false;

            // Act
            var command = CommandResult.Ok();
            command.OnSuccess(() => testBool = true);

            // Assert
            Assert.True(testBool);
        }

        [Fact]
        public void OnFailure_WithFailure_InvokesAction()
        {
            // Arrange
            var testBool = false;

            // Act
            var command = CommandResult.Fail(ErrorMessage);
            command.OnFailure(() => testBool = true);

            // Assert
            Assert.True(testBool);
        }
    }
}
