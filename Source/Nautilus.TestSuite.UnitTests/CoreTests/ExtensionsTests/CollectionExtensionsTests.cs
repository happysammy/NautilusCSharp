//--------------------------------------------------------------------------------------------------
// <copyright file="CollectionExtensionsTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.CoreTests.ExtensionsTests
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core.Extensions;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class CollectionExtensionsTests
    {
        [Fact]
        public void Count_WithEmptyEnumerable_ReturnsZero()
        {
            // Arrange
            var enumerable = new List<string>() as IEnumerable<string>;

            // Act
            var result = enumerable.Count();

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Count_WithNonZeroEnumerable_ReturnsCorrectCount()
        {
            // Arrange
            var enumerable = new List<string> { "hello", "world" } as IEnumerable<string>;

            // Act
            var result = enumerable.Count();

            // Assert
            Assert.Equal(2, result);
        }
    }
}
