//--------------------------------------------------------------------------------------------------
// <copyright file="StringExtensionsTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.CoreTests.ExtensionsTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core.Extensions;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class StringExtensionsTests
    {
        private enum TestEnum
        {
            Undefined = -1,
            Success = 0,
            Failure = 1,
            SomethingElse = 2,
        }

        [Theory]
        [InlineData("", TestEnum.Undefined)]
        [InlineData(" ", TestEnum.Undefined)]
        [InlineData("_", TestEnum.Undefined)]
        [InlineData("Other", TestEnum.Undefined)]
        internal void ToEnum_WhenDegenerateStrings_ReturnsUndefinedEnum(string input, Enum result)
        {
            // Arrange
            // Act
            // Assert
            Assert.Equal(result, input.ToEnum<TestEnum>());
        }

        [Theory]
        [InlineData("Undefined", TestEnum.Undefined)]
        [InlineData("UNDEFINED", TestEnum.Undefined)]
        [InlineData("Success", TestEnum.Success)]
        [InlineData("SUCCESS", TestEnum.Success)]
        [InlineData("0", TestEnum.Success)]
        [InlineData("SomethingElse", TestEnum.SomethingElse)]
        [InlineData("SOMETHING_ELSE", TestEnum.SomethingElse)]
        internal void ToEnum_WhenValidStrings_ReturnsExpectedEnum(string input, Enum result)
        {
            // Arrange
            // Act
            // Assert
            Assert.Equal(result, input.ToEnum<TestEnum>());
        }

        [Theory]
        [InlineData("easy", "EASY")]
        [InlineData("camelCase", "CAMEL_CASE")]
        [InlineData("CamelCase", "CAMEL_CASE")]
        internal void ToSnakeCase_WhenValidStrings_ReturnsExpectedString(string input, string result)
        {
            // Arrange
            // Act
            // Assert
            Assert.Equal(result, input.ToSnakeCase());
        }

        [Theory]
        [InlineData("easy", "easy")]
        [InlineData("camelCase", "camel_case")]
        [InlineData("CamelCase", "camel_case")]
        internal void ToSnakeCaseWithLowerCase_WhenValidStrings_ReturnsExpectedString(string input, string result)
        {
            // Arrange
            // Act
            // Assert
            Assert.Equal(result, input.ToSnakeCase(false));
        }
    }
}
