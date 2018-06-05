//--------------------------------------------------------------------------------------------------
// <copyright file="QueryExtensionsTests.cs" company="Nautech Systems Pty Ltd">
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

    public class QueryExtensionsTests
    {
        private const string ErrorMessage = "this failed";

        [Fact]
        public void OnFailure_WithQueryFailed_ExecutesChangeValueAction()
        {
            // Arrange
            var testBool = false;

            // Act
            var myResult = QueryResult<TestClass>.Fail(ErrorMessage);
            myResult.OnFailure(() => testBool = true);

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
