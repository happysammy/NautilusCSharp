//--------------------------------------------------------------------------------------------------
// <copyright file="SafeConvertTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  https://github.com/nautechsystems/Nautilus.Core
//  the use of this source code is governed by the Apache 2.0 license
//  as found in the LICENSE.txt file.
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.CoreTests
{
    using Nautilus.Core;
    using Xunit;

    public class SafeConvertTests
    {
        [Theory]
        [InlineData(null, 0)]
        [InlineData("", 0)]
        [InlineData(" ", 0)]
        [InlineData("  ", 0)]
        [InlineData(".", 0)]
        [InlineData("1", 1)]
        [InlineData("  1", 1)]
        [InlineData("1  ", 1)]
        [InlineData("1. 618 ", 0)]
        [InlineData("1.618", 1.618)]
        [InlineData(" 1.618", 1.618)]
        [InlineData("1.618 ", 1.618)]
        [InlineData("1.618033988749", 1.618033988749)]
        internal void ToDecimal_WithVariousValues_ReturnsParsedDecimalOrZero(
            string toBeParsed,
            decimal expectedResult)
        {
            // Arrange
            // Act
            var result = SafeConvert.ToDecimalOr(toBeParsed, 0m);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData(null, 1, 1)]
        [InlineData("", 1.1, 1.1)]
        [InlineData(" ", 1, 1)]
        [InlineData("  ", 1, 1)]
        [InlineData(".", 2.2, 2.2)]
        [InlineData("1", 2, 1)]
        [InlineData("  1", 2, 1)]
        [InlineData("1  ", 2, 1)]
        [InlineData("1. 618 ", 0, 0)]
        [InlineData("1.618", 3.142, 1.618)]
        [InlineData(" 1.618", 3.142, 1.618)]
        [InlineData("1.618 ", 3.142, 1.618)]
        [InlineData("1.618033988749", 3.142, 1.618033988749)]
        internal void ToDecimalOr_WithVariousValues_ReturnsParsedDecimalOrTheAlternativeValue(
            string toBeParsed,
            decimal alternativeValue,
            decimal expectedResult)
        {
            // Arrange
            // Act
            var result = SafeConvert.ToDecimalOr(toBeParsed, alternativeValue);

            // Assert
            Assert.Equal(expectedResult, result);
        }
    }
}
