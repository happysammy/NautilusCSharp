//--------------------------------------------------------------------------------------------------
// <copyright file="RollingListTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.CoreTests.CollectionsTests
{
    using System;
    using System.Collections;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Nautilus.Core.Collections;
    using Xunit;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class RollingListTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        internal void GivenNewInstantiation_VariousCapacities_ReturnsCountOfZero(int capacity)
        {
            // Arrange
            // Act
            var rollingList = new RollingList<int>(capacity);

            // Assert
            Assert.Empty(rollingList);
            Assert.False(rollingList.IsReadOnly);
        }

        [Fact]
        internal void Add_SingleInteger_ReturnsCountOfOne()
        {
            // Arrange

            // Act
            var rollingList = new RollingList<int>(20) { 1 };

            // Act

            // Assert
            Assert.Single(rollingList);
        }

        [Fact]
        internal void Add_IntegersBeyondCapacity_ReturnsCountOfCapacity()
        {
            // Arrange
            var capacity = 10;

            // Act
            var rollingList = new RollingList<int>(capacity) { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

            // Act

            // Assert
            Assert.Equal(capacity, rollingList.Count);
            Assert.Equal(3, rollingList[0]);
            Assert.Equal(12, rollingList[rollingList.Count - 1]);
        }

        [Fact]
        internal void Clear_AfterOneElementAdded_ReturnsCountToZero()
        {
            // Arrange
            var rollingList = new RollingList<int>(20) { 1 };
            var result1 = rollingList.Count;

            // Act
            rollingList.Clear();

            var result2 = rollingList.Count;

            // Assert
            Assert.Equal(1, result1);
            Assert.Equal(0, result2);
        }

        [Fact]
        internal void Contains_VariousDecimals_ReturnsExpectedResults()
        {
            // Arrange

            // Act
            var rollingList = new RollingList<decimal>(10) { 1.1m };

            // Assert
            Assert.Contains(1.1m, rollingList);
            Assert.DoesNotContain(decimal.Zero, rollingList);
        }

        [Fact]
        internal void Contains_VariousStrings_ReturnsExpectedResults()
        {
            // Arrange

            // Act
            var rollingList = new RollingList<string>(10) { "Foo" };

            // Assert
            Assert.Contains("Foo", rollingList);
            Assert.DoesNotContain("Bar", rollingList);
        }

        [Fact]
        internal void Indexer_VariousElementsAdded_ReturnsExpectedResults()
        {
            // Arrange

            // Act
            var rollingList = new RollingList<int>(10) { 1, 3, 9 };

            // Assert
            Assert.Equal(1, rollingList[0]);
            Assert.Equal(9, rollingList[rollingList.Count - 1]);
        }

        [Fact]
        internal void IndexOf_WhenTwoIntegersAdded_ReturnsExpectedIndex()
        {
            // Arrange
            var rollingList = new RollingList<int>(10) { 2, 4 };

            // Act
            var result = rollingList.IndexOf(2);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        internal void Indexer_AttemptToInsertElementAtIndex_Throws()
        {
            // Arrange
            var rollingList = new RollingList<int>(10) { 42 };

            // Act
            // Assert
            Assert.Single(rollingList);
            Assert.Throws<NotSupportedException>(() => rollingList[1] = 42);
        }

        [Fact]
        internal void GetEnumerator_ReturnsAnEnumerator()
        {
            // Arrange
            var rollingList = new RollingList<int>(10) { 10, 11 };

            // Act
            var result = rollingList.GetEnumerator();
            result.MoveNext();
            result.MoveNext();

            // Assert
            Assert.Equal(11, result.Current);
            result.Dispose();
        }

        [Fact]
        internal void CopyTo_AttemptToCopyTheRollingList_Throws()
        {
            // Arrange
            var rollingList = new RollingList<int>(10) { 3 };
            var array = new int[10];

            // Act
            // Assert
            Assert.Throws<NotSupportedException>(() => rollingList.CopyTo(array, 1));
        }

        [Fact]
        internal void RemoveT_AttemptToRemoveAnElement_Throws()
        {
            // Arrange
            var rollingList = new RollingList<int>(10) { 3 };

            // Act
            // Assert
            Assert.Throws<NotSupportedException>(() => rollingList.Remove(3));
        }

        [Fact]
        internal void Insert_AttemptToInsertElementAtIndex_Throws()
        {
            // Arrange
            var rollingList = new RollingList<int>(10) { 3 };

            // Act
            // Assert
            Assert.Throws<NotSupportedException>(() => rollingList.Insert(0, 42));
        }

        [Fact]
        internal void RemoveAt_AttemptToRemoveAtIndex_Throws()
        {
            // Arrange
            var rollingList = new RollingList<int>(10) { 1 };

            // Act
            // Assert
            Assert.Throws<NotSupportedException>(() => rollingList.RemoveAt(0));
        }

        [Fact]
        internal void RollingList_StressTest()
        {
            // Arrange
            var rollingList = new RollingList<int>(1000);

            // Act
            for (var i = 0; i < 1000000; i++)
            {
                rollingList.Add(i);
            }

            // Assert
            Assert.Equal(1000, rollingList.Count);
            Assert.Equal(999000, rollingList[0]);
            Assert.Equal(999999, rollingList[rollingList.Count - 1]);
        }

        // This test method exists to test the non generic GetEnumerator of the rolling list.
        [Fact]
        internal void GetEnumerator_FirstFifteenNumbers_AreCorrect()
        {
            // Arrange

            // Act
            IEnumerable strong = new RollingList<int>(10) { 1 };
            var weak = AsWeakEnumerable(strong);
            var enumerable = weak as object[] ?? weak.Cast<object>().ToArray();
            var result = enumerable.Cast<int>().Take(1);
            enumerable.GetEnumerator().MoveNext();

            // Asser
            Assert.Equal(0, result.GetEnumerator().Current);
        }

        private static IEnumerable AsWeakEnumerable(IEnumerable source)
        {
            return source.Cast<object>();
        }
    }
}
