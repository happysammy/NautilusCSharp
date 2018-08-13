//--------------------------------------------------------------------------------------------------
// <copyright file="ReadOnlyDictionaryTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
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
    public class ReadOnlyDictionaryTests
    {
        [Fact]
        internal void Test_can_instantiate_with_a_dictionary()
        {
            // Arrange
            var originalDictionary = new Dictionary<int, string>
            {
                { 1, "one" },
                { 2, "two" },
            };

            var readOnlyDictionary = new ReadOnlyDictionary<int, string>(originalDictionary);

            // Act
            var result1 = readOnlyDictionary[1];
            var result2 = readOnlyDictionary[2];

            // Assert
            Assert.Equal("one", result1);
            Assert.Equal("two", result2);
            Assert.Equal(2, readOnlyDictionary.Count);
            Assert.True(readOnlyDictionary.IsReadOnly);
        }

        [Fact]
        internal void Test_can_return_items_by_index()
        {
            // Arrange
            var originalDictionary = new Dictionary<int, string>
            {
                { 1, "one" },
                { 2, "two" },
            };

            var readOnlyDictionary = new ReadOnlyDictionary<int, string>(originalDictionary);

            // Act
            // Assert
            Assert.Equal("one", readOnlyDictionary[1]);
            Assert.Equal("two", readOnlyDictionary[2]);
        }

        [Fact]
        internal void Test_can_try_get_value()
        {
            // Arrange
            var originalDictionary = new Dictionary<int, string>
            {
                { 1, "one" },
                { 2, "two" },
            };

            var readOnlyDictionary = new ReadOnlyDictionary<int, string>(originalDictionary);

            // Act
            readOnlyDictionary.TryGetValue(0, out var result1);
            readOnlyDictionary.TryGetValue(1, out var result2);

            // Assert
            Assert.Null(result1);
            Assert.Equal("one", result2);
        }

        [Fact]
        internal void Test_can_find_items_with_contains()
        {
            // Arrange
            var originalDictionary = new Dictionary<int, string>
            {
                { 1, "one" },
                { 2, "two" },
            };

            var readOnlyDictionary = new ReadOnlyDictionary<int, string>(originalDictionary);

            // Act
            // Assert
            Assert.Contains(new KeyValuePair<int, string>(1, "one"), readOnlyDictionary);
            Assert.Contains(new KeyValuePair<int, string>(2, "two"), readOnlyDictionary);
            Assert.DoesNotContain(new KeyValuePair<int, string>(3, "three"), readOnlyDictionary);
            Assert.Contains(1, readOnlyDictionary.Keys);
            Assert.False(readOnlyDictionary.ContainsKey(0));
            Assert.True(readOnlyDictionary.ContainsValue("two"));
            Assert.False(readOnlyDictionary.ContainsValue("three"));
        }

        [Fact]
        internal void Test_can_return_expected_keys_and_values()
        {
            // Arrange
            var originalDictionary = new Dictionary<int, string>
            {
                { 1, "one" },
                { 2, "two" },
                { 3, "three" },
            };

            var readOnlyDictionary = new ReadOnlyDictionary<int, string>(originalDictionary);

            // Act
            // Assert
            Assert.Equal(new List<int> { 1, 2, 3 }, readOnlyDictionary.Keys);
            Assert.Equal(new List<string> { "one", "two", "three" }, readOnlyDictionary.Values);
        }

        [Fact]
        internal void Test_throws_on_not_implemented_operations()
        {
            // Arrange
            var originalDictionary = new Dictionary<int, string>
            {
                { 1, "one" },
                { 2, "two" },
                { 3, "three" },
            };

            var readOnlyDictionary = new ReadOnlyDictionary<int, string>(originalDictionary);

            // Act
            // Assert
            Assert.Throws<NotSupportedException>(() => readOnlyDictionary.Add(new KeyValuePair<int, string>(0, "zero")));
            Assert.Throws<NotSupportedException>(() => readOnlyDictionary.Add(0, "zero"));
            Assert.Throws<NotSupportedException>(() => readOnlyDictionary.Remove(new KeyValuePair<int, string>(0, "zero")));
            Assert.Throws<NotSupportedException>(() => readOnlyDictionary.Remove(0));
            Assert.Throws<NotSupportedException>(() => readOnlyDictionary.Clear());
            Assert.Throws<NotSupportedException>(() => readOnlyDictionary.CopyTo(new KeyValuePair<int, string>[1], 0));
        }
    }
}
