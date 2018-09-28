﻿//--------------------------------------------------------------------------------------------------
// <copyright file="QueryResultTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.CoreTests.CQSTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core.CQS;
    using Nautilus.Core.Validation;
    using Xunit;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class QueryResultTests
    {
        [Fact]
        public void Ok_WithGenericResult_ReturnsOk()
        {
            // Arrange
            var testClass = new TestClass();

            // Act
            var result = QueryResult<TestClass>.Ok(testClass);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailure);
            Assert.Equal(testClass, result.Value);
        }

        [Fact]
        public void Ok_WithGenericResultAndMessage_ReturnsOkWithMessage()
        {
            // Arrange
            var message = "The query was successful.";
            var testClass = new TestClass();

            // Act
            var result = QueryResult<TestClass>.Ok(testClass, message);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailure);
            Assert.Equal(testClass, result.Value);
            Assert.Equal(message, result.Message);
        }

        [Fact]
        public void ActionInvoked_WithGenericNoValue_Throws()
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => ((Action)(() => { QueryResult<TestClass>.Ok(null); })).Invoke());
        }

        [Fact]
        public void Fail_GenericWithValueInputs_ReturnsExpectedResult()
        {
            // Arrange
            // Act
            var result = QueryResult<TestClass>.Fail("error message");

            // Assert
            Assert.True(result.IsFailure);
            Assert.False(result.IsSuccess);
            Assert.Equal("error message", result.Message);
        }

        [Fact]
        public void ActionInvoke_AttemptingToAccessValueWithNoValue_Throws()
        {
            // Arrange
            var result = QueryResult<TestClass>.Fail("error message");

            // Act
            // Assert
            Assert.True(result.IsFailure);
            Assert.Throws<InvalidOperationException>(() => ((Action)(() => { TestClass testClass = result.Value; })).Invoke());
        }

        [Fact]
        public void Fail_AllTypesWithNoErrorMessage_Throws()
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => ((Action)(() => { QueryResult<TestClass>.Fail(null); })).Invoke());
            Assert.Throws<ValidationException>(() => ((Action)(() => { QueryResult<TestClass>.Fail(string.Empty); })).Invoke());
        }

        // Only instantiated within this class for testing purposes.
        private class TestClass
        {
        }
    }
}
