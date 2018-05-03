//--------------------------------------------------------------------------------------------------
// <copyright file="DecimalExtensionsTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.CoreTests.ExtensionsTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core.Extensions;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class DecimalExtensionsTests
    {
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
