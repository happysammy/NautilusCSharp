//--------------------------------------------------------------------------------------------------
// <copyright file="QueryExtensionsTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
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

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class QueryExtensionsTests
    {
        private const string ErrorMessage = "this failed";

        [Fact]
        public void OnFailure_WithQueryFailed_ExecutesChangeValueAction()
        {
            // Arrange
            var testBool = false;

            // Act
            var result = QueryResult<TestClass>.Fail(ErrorMessage);
            result.OnFailure(() => testBool = true);

            // Assert
            Assert.True(testBool);
        }

        [Fact]
        public void OnFailure_WithQueryFailed_ExecutesChangeStringAction()
        {
            // Arrange
            var testError = string.Empty;

            // Act
            var result = QueryResult<TestClass>.Fail(ErrorMessage);
            result.OnFailure(error => testError = error);

            // Assert
            Assert.Equal(ErrorMessage, testError);
        }

        // Only instantiated within this class for testing purposes.
        private class TestClass
        {
        }
    }
}
