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

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class CollectionExtensionsTests
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

        [Fact]
        public void SliceToLimitFromEnd_EmptyArray_ReturnsEmptyArray()
        {
            // Arrange
            var array = new int[0];

            // Act
            var result = array.SliceToLimitFromEnd(1);

            // Assert
            Assert.Equal(array, result);
        }

        [Fact]
        public void SliceToLimitFromEnd_WithZeroLimit_ReturnsEmptyArray()
        {
            // Arrange
            var array = new[] { 0, 1, 2, 3 };

            // Act
            var result = array.SliceToLimitFromEnd(0);

            // Assert
            Assert.Equal(new int[0], result);
        }

        [Fact]
        public void SliceToLimitFromEnd_WithLimit_ReturnsCorrectlySlicedArray()
        {
            // Arrange
            var array = new[] { 0, 1, 2, 3 };

            // Act
            var result = array.SliceToLimitFromEnd(2);

            // Assert
            Assert.Equal(new[] { 2, 3 }, result);
        }

        [Fact]
        public void Print_WithVariousDictionaries_ReturnsExpectedString()
        {
            // Arrange
            var dict0 = new Dictionary<string, string>();
            var dict1 = new Dictionary<string, string> { { "element", "1" } };
            var dict2 = new Dictionary<string, string> { { "element1", "1" }, { "element2", "2" } };

            // Act
            var result0 = dict0.Print();
            var result1 = dict1.Print();
            var result2 = dict2.Print();

            // Assert
            Assert.Equal("{ }", result0);
            Assert.Equal("{ element: 1 }", result1);
            Assert.Equal("{ element1: 1, element2: 2 }", result2);
        }
    }
}
