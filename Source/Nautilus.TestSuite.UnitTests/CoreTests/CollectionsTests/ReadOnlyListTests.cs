//--------------------------------------------------------------------------------------------------
// <copyright file="ReadOnlyListTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.CoreTests.CollectionsTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core.Collections;
    using Xunit;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class ReadOnlyListTests
    {
        [Fact]
        internal void Test_can_instantiate_with_a_list()
        {
            // Arrange
            var originalList = new List<string>
            {
                "one",
                "two",
            };

            var readOnlyList = new ReadOnlyList<string>(originalList);

            // Act
            var result1 = readOnlyList[0];
            var result2 = readOnlyList[1];

            // Assert
            Assert.Equal("one", result1);
            Assert.Equal("two", result2);
            Assert.Equal(2, readOnlyList.Count);
            Assert.True(readOnlyList.IsReadOnly);
        }

        [Fact]
        internal void Test_can_instantiate_with_a_single_element()
        {
            // Arrange
            var readOnlyList = new ReadOnlyList<string>("one");

            // Act
            var result = readOnlyList[0];

            // Assert
            Assert.Equal("one", result);
            Assert.Single(readOnlyList);
            Assert.True(readOnlyList.IsReadOnly);
        }

        [Fact]
        internal void Test_can_return_items_by_index()
        {
            // Arrange
            var originalList = new List<string>
            {
                "one",
                "two",
            };

            var readOnlyList = new ReadOnlyList<string>(originalList);

            // Act
            // Assert
            Assert.Equal("one", readOnlyList[0]);
            Assert.Equal("two", readOnlyList[1]);
        }

        [Fact]
        internal void Test_can_return_index_of()
        {
            // Arrange
            var originalList = new List<string>
            {
                "one",
                "two",
            };

            var readOnlyList = new ReadOnlyList<string>(originalList);

            // Act
            // Assert
            Assert.Equal(0, readOnlyList.IndexOf("one"));
            Assert.Equal(1, readOnlyList.IndexOf("two"));
        }

        [Fact]
        internal void Test_can_find_items_with_contains()
        {
            // Arrange
            var originalList = new List<string>
            {
                "one",
                "two",
            };

            var readOnlyList = new ReadOnlyList<string>(originalList);

            // Act
            // Assert
            Assert.Contains("one", readOnlyList);
            Assert.Contains("two", readOnlyList);
            Assert.DoesNotContain("three", readOnlyList);
        }

        [Fact]
        internal void Test_throws_on_not_implemented_operations()
        {
            // Arrange
            var readOnlyList = new ReadOnlyList<int>(0);

            // Act
            // Assert
            Assert.Throws<NotSupportedException>(() => readOnlyList.Add(1));
            Assert.Throws<NotSupportedException>(() => readOnlyList.Remove(0));
            Assert.Throws<NotSupportedException>(() => readOnlyList.RemoveAt(0));
            Assert.Throws<NotSupportedException>(() => readOnlyList.Clear());
        }
    }
}
