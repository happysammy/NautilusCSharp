//--------------------------------------------------------------------------------------------------
// <copyright file="DecimalExtensionsTests.cs" company="Nautech Systems Pty Ltd">
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

    public class DecimalExtensionsTests
    {
        [Theory]
        [InlineData(0, 0)]
        [InlineData(0.1, 1)]
        [InlineData(0.01, 2)]
        [InlineData(0.001, 3)]
        [InlineData(0.0001, 4)]
        [InlineData(0.00001, 5)]
        [InlineData(0.000001, 6)]
        [InlineData(0.0000001, 7)]
        [InlineData(0.00000001, 8)]
        [InlineData(0.000000001, 9)]
        [InlineData(0.0000000001, 10)]
        internal void GetDecimalPlaces_VariousInputs_ReturnsExpectedInt(decimal value, int expected)
        {
            // Arrange
            var number = value;

            // Act
            var result = number.GetDecimalPlaces();

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 0.1)]
        [InlineData(3, 0.001)]
        [InlineData(5, 0.00001)]
        internal void GetTickSizeFromInt_VariousInputs_ReturnsExpectedDecimal(int fromInt, decimal expected)
        {
            // Arrange

            // Act
            var result = fromInt.ToTickSize();

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
