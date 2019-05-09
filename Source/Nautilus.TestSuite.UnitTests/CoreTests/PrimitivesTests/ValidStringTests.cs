//--------------------------------------------------------------------------------------------------
// <copyright file="ValidStringTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.CoreTests.PrimitivesTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core.Primitives;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class ValidStringTests
    {
        [Fact]
        internal void Value_WithValidValueGiven_ReturnsExpectedValue()
        {
            // Arrange
            var validString = new TestString("abc123");

            // Act
            var result = validString.Value;

            // Assert
            Assert.Equal("abc123", result);
        }

        [Fact]
        internal void EqualityOperators_ReturnExpectedValues()
        {
            // Arrange
            var validString1 = new TestString("abc123");
            var validString2 = new TestString("def999");
            var validString3 = new TestString("abc123");

            // Act
            // Assert
            Assert.True(validString1 == validString3);
            Assert.False(validString1 == validString2);
            Assert.False(validString1 != validString3);
            Assert.True(validString1 != validString2);

            Assert.True(validString1.Equals(validString1));
            Assert.False(validString1.Equals(validString2));
        }

        private class TestString : ValidString<TestString>
        {
            public TestString(string value)
                : base(value)
            {
            }
        }
    }
}
