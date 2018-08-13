//--------------------------------------------------------------------------------------------------
// <copyright file="CommandExtensionsTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.CoreTests.ExtensionsTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core.CQS;
    using Nautilus.Core.Extensions;
    using Xunit;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
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
