//--------------------------------------------------------------------------------------------------
// <copyright file="StringExtensionsTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  https://github.com/nautechsystems/Nautilus.Core
//  the use of this source code is governed by the Apache 2.0 license
//  as found in the LICENSE.txt file.
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.CoreTests.ExtensionsTests
{
    using Nautilus.Core.Extensions;
    using Xunit;

    public class StringExtensionsTests
    {
        private enum TestEnum
        {
            Unknown = 0,
            Test = 1,
        }

        [Theory]
        [InlineData("A", "A")]
        [InlineData("123 123 1adc \n 222", "1231231adc222")]
        [InlineData("  123 123 1adc \n 222   ", "1231231adc222")]
        internal void RemoveWhiteSpace_WithVariousInputs_ReturnsStringWithNoWhiteSpace(string input, string expected)
        {
            // Arrange
            // Act
            var s = input.RemoveAllWhitespace();

            // Assert
            Assert.Equal(expected, s);
        }

        [Fact]
        internal void ToEnum_WhenWhiteSpaceString_ReturnsDefaultEnum()
        {
            // Arrange
            const string someString = " ";

            // Act
            var result = someString.ToEnum<TestEnum>();

            // Assert
            Assert.Equal(TestEnum.Unknown, result);
        }

        [Fact]
        internal void ToEnum_WhenString_ReturnsExpectedEnum()
        {
            // Arrange
            const string someString = "Test";

            // Act
            var result = someString.ToEnum<TestEnum>();

            // Assert
            Assert.Equal(TestEnum.Test, result);
        }
    }
}
