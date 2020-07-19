//--------------------------------------------------------------------------------------------------
// <copyright file="StringExtensionsTests.cs" company="Nautech Systems Pty Ltd">
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
using System.Diagnostics.CodeAnalysis;
using Nautilus.Core.Extensions;
using Xunit;

namespace Nautilus.TestSuite.UnitTests.CoreTests.ExtensionsTests
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class StringExtensionsTests
    {
        private enum TestEnum
        {
            Undefined = -1,
            Success = 0,
            SomethingElse = 1,
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

        [Theory]
        [InlineData("easy", false)]
        [InlineData("camelCase", false)]
        [InlineData("YES", true)]
        [InlineData("YES1", true)]
        [InlineData("@!YES1", true)]
        [InlineData("@!no1", false)]
        internal void IsAllUpperCase_WhenValidStrings_ReturnsExpectedResult(string input, bool result)
        {
            // Arrange
            // Act
            // Assert
            Assert.Equal(result, input.IsAllUpperCase());
        }
    }
}
