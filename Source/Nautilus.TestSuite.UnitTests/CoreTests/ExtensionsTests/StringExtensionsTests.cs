//--------------------------------------------------------------------------------------------------
// <copyright file="StringExtensionsTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.CoreTests.ExtensionsTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core.Extensions;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class StringExtensionsTests
    {
        private enum TestEnum
        {
            Unknown = 0,
            Test = 1,
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
