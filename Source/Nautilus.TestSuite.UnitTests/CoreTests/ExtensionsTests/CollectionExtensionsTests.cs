//--------------------------------------------------------------------------------------------------
// <copyright file="CollectionExtensionsTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  https://github.com/nautechsystems/Nautilus.Core
//  the use of this source code is governed by the Apache 2.0 license
//  as found in the LICENSE.txt file.
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.ExtensionsTests
{
    using System.Collections.Generic;
    using Nautilus.Core.Extensions;
    using Xunit;

    public class CollectionExtensionsTests
    {
        [Fact]
        internal void LastIndex_WhenCollectionHasOneElement_ReturnsZero()
        {
            // Arrange
            var collection = new List<string> { "object" };

            // Act
            var result = collection.LastIndex();

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        internal void LastIndex_WhenCollectionHasThreeElements_ReturnsTwo()
        {
            // Arrange
            var collection = new List<string> { "object1", "object2", "object3" };

            // Act
            var result = collection.LastIndex();

            // Assert
            Assert.Equal(2, result);
        }

        [Theory]
        [InlineData(0, 9)]
        [InlineData(1, 8)]
        [InlineData(5, 4)]
        [InlineData(9, 0)]
        internal void GetByReverseIndex_WhenCollectionHasElements_ReturnsExpectedElement(
            int index,
            int expected)
        {
            // Arrange
            var collection = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            // Act
            var result = collection.GetByReverseIndex(index);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(0, 0, 9)]
        [InlineData(0, 1, 8)]
        [InlineData(0, 2, 7)]
        [InlineData(1, 1, 7)]
        [InlineData(1, 2, 6)]
        [InlineData(5, 0, 4)]
        [InlineData(4, 5, 0)]
        internal void GetByShiftedReverseIndex_WhenCollectionHasElements_ReturnsExpectedElement(
            int index,
            int shift,
            int expected)
        {
            // Arrange
            var collection = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            // Act
            var result = collection.GetByShiftedReverseIndex(index, shift);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        internal void ForEach_WhenCollectionEmpty()
        {
            // Arrange
            var collection = new List<string> { "action1", "action2" };

            // Act
            collection.ForEach(TestAction);

            // Assert

        }

        // Only used within this class for testing purposes.
        private static void TestAction(string input)
        {
            // An action which does nothing.
        }
    }
}
