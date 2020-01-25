//--------------------------------------------------------------------------------------------------
// <copyright file="QueryExtensionsTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.CoreTests.ExtensionsTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core.CQS;
    using Nautilus.Core.Extensions;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("ReSharper", "SA1310", Justification = "Easier to read.")]
    public class QueryExtensionsTests
    {
        private const string ERROR_MESSAGE = "this failed";

        [Fact]
        public void OnFailure_WithQueryFailed_ExecutesChangeValueAction()
        {
            // Arrange
            var testBool = false;

            // Act
            var result = QueryResult<TestClass>.Fail(ERROR_MESSAGE);
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
            var result = QueryResult<TestClass>.Fail(ERROR_MESSAGE);
            result.OnFailure(error => testError = error);

            // Assert
            Assert.Equal(ERROR_MESSAGE, testError);
        }

        // Only instantiated within this class for testing purposes.
        private class TestClass
        {
        }
    }
}
