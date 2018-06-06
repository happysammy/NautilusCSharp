//--------------------------------------------------------------------------------------------------
// <copyright file="DebugTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  https://github.com/nautechsystems/Nautilus.Core
//  the use of this source code is governed by the Apache 2.0 license
//  as found in the LICENSE.txt file.
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.CoreTests.ValidationTests
{
    using System;
    using System.Collections.Generic;
    using Nautilus.Core.Validation;
    using Xunit;

    public class DebugTests
    {
        [Fact]
        internal void True_WhenFalse_Throws()
        {
            // Arrange
            // Act
            // Assert
            var ex = Assert.Throws<ValidationException>(() => Debug.True(false, "some_evaluation"));

            Assert.Equal(typeof(ArgumentException), ex.InnerException.GetType());
        }

        [Fact]
        internal void True_WhenTrue_ReturnsNull()
        {
            // Arrange
            // Act
            // Assert
            Debug.True(true, "some_evaluation");
        }

        [Fact]
        internal void TrueIf_WhenConditionTrueAndPredicateFalse_Throws()
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.TrueIf(true, false, "some_evaluation"));
        }

        [Fact]
        internal void TrueIf_WhenConditionTrueAndPredicateTrue_DoesNothing()
        {
            // Arrange
            // Act
            // Assert
            Debug.TrueIf(true, true, "some_evaluation");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        internal void NotNull_WithVariousObjectAndInvalidStrings_Throws(string value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.NotNull(value, nameof(value)));
        }

        [Fact]
        internal void NotNull_WithAString_DoesNothing()
        {
            // Arrange
            const string obj = "something";

            // Act
            // Assert
            Debug.NotNull(obj, nameof(obj));
        }

        [Fact]
        internal void NotNullTee_WithNullObject_Throws()
        {
            // Arrange
            object obj = null;

            // Act
            // Assert - Ignore expression is always null warning (the point of the test is to catch this condition).
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Throws<ValidationException>(() => Debug.NotNull(obj, nameof(obj)));
        }

        [Fact]
        internal void NotNullTee_WithAnObject_DoesNothing()
        {
            // Arrange
            object obj = new EventArgs();

            // Act
            // Assert
            Debug.NotNull(obj, nameof(obj));
        }

        [Fact]
        internal void NotDefault_WithNotDefaultStruct_DoesNothing()
        {
            // Arrange
            var obj = new TimeSpan(0, 0, 1);

            // Act
            // Assert
            Debug.NotDefault(obj, nameof(obj));
        }

        [Fact]
        internal void NotDefault_WithDefaultStruct_Throws()
        {
            // Arrange
            var point = new TimeSpan();

            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.NotDefault(point, nameof(point)));
        }

        [Fact]
        internal void CollectionEmpty_WhenCollectionEmpty_DoesNothing()
        {
            // Arrange
            var collection = new List<string>();

            // Act
            // Assert
            Debug.CollectionEmpty(collection, nameof(collection));
        }

        [Fact]
        internal void ReadOnlyCollectionEmpty_WhenCollectionEmpty_DoesNothing()
        {
            // Arrange
            IReadOnlyCollection<string> collection = new List<string>().AsReadOnly();

            // Act
            // Assert
            Debug.ReadOnlyCollectionEmpty(collection, nameof(collection));
        }

        [Fact]
        internal void CollectionEmpty_WhenCollectionNotEmpty_Throws()
        {
            // Arrange
            List<string> collection = new List<string> { "anElement" };

            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.CollectionEmpty(collection, nameof(collection)));
        }

        [Fact]
        internal void ReadOnlyCollectionEmpty_WhenCollectionNotEmpty_Throws()
        {
            // Arrange
            IReadOnlyCollection<string> collection = new List<string> { "anElement" }.AsReadOnly();

            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.ReadOnlyCollectionEmpty(collection, nameof(collection)));
        }

        [Fact]
        internal void CollectionEmpty_WhenCollectionNull_Throws()
        {
            // Arrange
            List<string> collection = null;

            // Act
            // Assert - Ignore expression is always null warning (the point of the test is to catch this condition).
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Throws<ValidationException>(() => Debug.CollectionEmpty(collection, nameof(collection)));
        }

        [Fact]
        internal void ReadOnlyCollectionEmpty_WhenCollectionNull_Throws()
        {
            // Arrange
            IReadOnlyCollection<string> collection = null;

            // Act
            // Assert - Ignore expression is always null warning (the point of the test is to catch this condition).
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Throws<ValidationException>(() => Debug.ReadOnlyCollectionEmpty(collection, nameof(collection)));
        }

        [Fact]
        internal void CollectionNotNullOrEmpty_WhenCollectionNotEmpty_DoesNothing()
        {
            // Arrange
            var collection = new List<string> { "foo" };

            // Act
            // Assert
            Debug.CollectionNotNullOrEmpty(collection, nameof(collection));
        }

        [Fact]
        internal void ReadOnlyCollectionNotNullOrEmpty_WhenCollectionNotEmpty_DoesNothing()
        {
            // Arrange
            IReadOnlyCollection<string> collection = new List<string> { "foo" }.AsReadOnly();

            // Act
            // Assert
            Debug.ReadOnlyCollectionNotNullOrEmpty(collection, nameof(collection));
        }

        [Fact]
        internal void CollectionNotNullOrEmpty_WhenCollectionNull_Throws()
        {
            // Arrange
            List<string> collection = null;

            // Act
            // Assert - Ignore expression is always null warning (the point of the test is to catch this condition).
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Throws<ValidationException>(() => Debug.CollectionNotNullOrEmpty(collection, nameof(collection)));
        }

        [Fact]
        internal void ReadOnlyCollectionNotNullOrEmpty_WhenCollectionNull_Throws()
        {
            // Arrange
            IReadOnlyCollection<string> collection = null;

            // Act
            // Assert - Ignore expression is always null warning (the point of the test is to catch this condition).
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Throws<ValidationException>(() => Debug.ReadOnlyCollectionNotNullOrEmpty(collection, nameof(collection)));
        }

        [Fact]
        internal void CollectionNotNullOrEmpty_WhenCollectionEmpty_Throws()
        {
            // Arrange
            var collection = new List<string>();

            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.CollectionNotNullOrEmpty(collection, nameof(collection)));
        }

        [Fact]
        internal void ReadOnlyCollectionNotNullOrEmpty_WhenCollectionEmpty_Throws()
        {
            // Arrange
            IReadOnlyCollection<string> collection = new List<string>().AsReadOnly();

            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.ReadOnlyCollectionNotNullOrEmpty(collection, nameof(collection)));
        }

        [Fact]
        internal void CollectionNotNullOrEmpty_WhenDictionaryNull_Throws()
        {
            // Arrange
            Dictionary<string, int> dictionary = null;

            // Act
            // Assert - Ignore expression is always null warning (the point of the test is to catch this condition).
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Throws<ValidationException>(() => Debug.CollectionNotNullOrEmpty(dictionary, nameof(dictionary)));
        }

        [Fact]
        internal void CollectionNotNullOrEmpty_WhenDictionaryEmpty_Throws()
        {
            // Arrange
            var dictionary = new Dictionary<string, int>();

            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.CollectionNotNullOrEmpty(dictionary, nameof(dictionary)));
        }

        [Fact]
        internal void CollectionContains_WhenCollectionDoesNotContainElement_Throws()
        {
            // Arrange
            var element = "the_fifth_element";
            var collection = new List<string>();

            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.CollectionContains(element, nameof(element), collection));
        }

        [Fact]
        internal void ReadOnlyCollectionContains_WhenCollectionDoesNotContainElement_Throws()
        {
            // Arrange
            var element = "the_fifth_element";
            IReadOnlyCollection<string> collection = new List<string>().AsReadOnly();

            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.ReadOnlyCollectionContains(element, nameof(element), collection));
        }

        [Fact]
        internal void CollectionContains_WhenCollectionContainsElement_DoesNothing()
        {
            // Arrange
            var element = "the_fifth_element";
            var collection = new List<string> { element };

            // Act
            // Assert
            Debug.CollectionContains(element, nameof(element), collection);
        }

        [Fact]
        internal void ReadOnlyCollectionContains_WhenCollectionContainsElement_DoesNothing()
        {
            // Arrange
            var element = "the_fifth_element";
            IReadOnlyCollection<string> collection = new List<string> { element }.AsReadOnly();

            // Act
            // Assert
            Debug.ReadOnlyCollectionContains(element, nameof(element), collection);
        }

        [Fact]
        internal void CollectionDoesNotContain_WhenCollectionDoesNotContainElement_DoesNothing()
        {
            // Arrange
            var element = "the_fifth_element";
            var collection = new List<string>();

            // Act
            // Assert
            Debug.CollectionDoesNotContain(element, nameof(element), collection);
        }

        [Fact]
        internal void ReadOnlyCollectionDoesNotContain_WhenCollectionDoesNotContainElement_DoesNothing()
        {
            // Arrange
            var element = "the_fifth_element";
            IReadOnlyCollection<string> collection = new List<string>().AsReadOnly();

            // Act
            // Assert
            Debug.ReadOnlyCollectionDoesNotContain(element, nameof(element), collection);
        }

        [Fact]
        internal void CollectionDoesNotContain_WhenCollectionContainsElement_Throws()
        {
            // Arrange
            var element = "the_fifth_element";
            var collection = new List<string> { element };

            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.CollectionDoesNotContain(element, nameof(element), collection));
        }

        [Fact]
        internal void ReadOnlyCollectionDoesNotContain_WhenCollectionContainsElement_Throws()
        {
            // Arrange
            var element = "the_fifth_element";
            IReadOnlyCollection<string> collection = new List<string> { element }.AsReadOnly();

            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.ReadOnlyCollectionDoesNotContain(element, nameof(element), collection));
        }

        [Fact]
        internal void DictionaryContainsKey_WhenDictionaryDoesContainKey_DoesNothing()
        {
            // Arrange
            var key = "the_key";
            var collection = new Dictionary<string, int> { { key, 1 } };

            // Act
            // Assert
            Debug.DictionaryContainsKey(key, nameof(key), collection);
        }

        [Fact]
        internal void ReadOnlyDictionaryContainsKey_WhenDictionaryDoesContainKey_DoesNothing()
        {
            // Arrange
            var key = "the_key";
            IReadOnlyDictionary<string, int> dictionary = new Dictionary<string, int> { { key, 1 } };

            // Act
            // Assert
            Debug.ReadOnlyDictionaryContainsKey(key, nameof(key), dictionary);
        }

        [Fact]
        internal void DictionaryContainsKey_WhenDictionaryContainsKey_DoesNothing()
        {
            // Arrange
            var key = "the_key";
            var collection = new Dictionary<string, int> { { key, 0 } };

            // Act
            // Assert
            Debug.DictionaryContainsKey(key, nameof(key), collection);
        }

        [Fact]
        internal void ReadOnlyDictionaryContainsKey_WhenDictionaryContainsKey_DoesNothing()
        {
            // Arrange
            var key = "the_key";
            IReadOnlyDictionary<string, int> dictionary = new Dictionary<string, int> { { key, 0 } };

            // Act
            // Assert
            Debug.ReadOnlyDictionaryContainsKey(key, nameof(key), dictionary);
        }

        [Fact]
        internal void DictionaryContainsKey_WhenDictionaryDoesNotContainKey_Throws()
        {
            // Arrange
            var key = "the_key";
            var collection = new Dictionary<string, int>();

            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.DictionaryContainsKey(key, nameof(key), collection));
        }

        [Fact]
        internal void ReadOnlyDictionaryContainsKey_WhenDictionaryDoesNotContainKey_Throws()
        {
            // Arrange
            var key = "the_key";
            IReadOnlyDictionary<string, int> dictionary = new Dictionary<string, int>();

            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.ReadOnlyDictionaryContainsKey(key, nameof(key), dictionary));
        }

        [Fact]
        internal void DictionaryDoesNotContainKey_WhenDictionaryDoesNotContainsKey_DoesNothing()
        {
            // Arrange
            var key = "the_key";
            var collection = new Dictionary<string, int> { { "another_key", 2 } };

            // Act
            // Assert
            Debug.DictionaryDoesNotContainKey(key, nameof(key), collection);
        }

        [Fact]
        internal void ReadOnlyDictionaryDoesNotContainKey_WhenDictionaryDoesNotContainsKey_DoesNothing()
        {
            // Arrange
            var key = "the_key";
            IReadOnlyDictionary<string, int> dictionary = new Dictionary<string, int> { { "another_key", 2 } };

            // Act
            // Assert
            Debug.ReadOnlyDictionaryDoesNotContainKey(key, nameof(key), dictionary);
        }

        [Fact]
        internal void DictionaryDoesNotContainKey_WhenDictionaryContainsKey_Throws()
        {
            // Arrange
            var key = "the_key";
            var collection = new Dictionary<string, int> { { key, 1 } };

            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.DictionaryDoesNotContainKey(key, nameof(key), collection));
        }

        [Fact]
        internal void ReadOnlyDictionaryDoesNotContainKey_WhenDictionaryContainsKey_Throws()
        {
            // Arrange
            var key = "the_key";
            IReadOnlyDictionary<string, int> dictionary = new Dictionary<string, int> { { key, 1 } };

            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.ReadOnlyDictionaryDoesNotContainKey(key, nameof(key), dictionary));
        }

        [Fact]
        internal void EqualTo_ValuesAreEqual_DoesNothing()
        {
            // Arrange
            var object1 = 1;
            var object2 = 1;

            // Act
            // Assert
            Debug.EqualTo(object1, nameof(object1), object2);
        }

        [Fact]
        internal void EqualTo_ObjectsAreEqual_DoesNothing()
        {
            // Arrange
            var object1 = "object";
            var object2 = "object";

            // Act
            // Assert
            Debug.EqualTo(object1, nameof(object1), object2);
        }

        [Fact]
        internal void EqualTo_ValuesAreNotEqual_Throws()
        {
            // Arrange
            var object1 = 1;
            var object2 = 2;

            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.EqualTo(object1, nameof(object1), object2));
        }

        [Fact]
        internal void EqualTo_ObjectsAreNotEqual_Throws()
        {
            // Arrange
            var object1 = "object1";
            var object2 = "object2";

            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.EqualTo(object1, nameof(object1), object2));
        }

        [Fact]
        internal void NotEqualTo_ValuesAreNotEqual_DoesNothing()
        {
            // Arrange
            var object1 = 1;
            var object2 = 2;

            // Act
            // Assert
            Debug.NotEqualTo(object1, nameof(object1), object2);
        }

        [Fact]
        internal void NotEqualTo_ObjectsAreNotEqual_DoesNothing()
        {
            // Arrange
            var object1 = "object1";
            var object2 = "object2";

            // Act
            // Assert
            Debug.NotEqualTo(object1, nameof(object1), object2);
        }

        [Fact]
        internal void NotEqualTo_ValuesAreEqual_Throws()
        {
            // Arrange
            var object1 = 1;
            var object2 = 1;

            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.NotEqualTo(object1, nameof(object1), object2));
        }

        [Fact]
        internal void NotEqualTo_ObjectsAreEqual_Throws()
        {
            // Arrange
            var object1 = "object";
            var object2 = "object";

            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.NotEqualTo(object1, nameof(object1), object2));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        internal void Int32NotOutOfRange_VariousInInclusiveRangeValues_DoesNothing(int value)
        {
            // Arrange
            // Act
            // Assert
            Debug.Int32NotOutOfRange(value, nameof(value), 0, 3);
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        [InlineData(-1)]
        [InlineData(2)]
        internal void Int32NotOutOfRange_VariousOutOfInclusiveRangeValues_Throws(int value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.Int32NotOutOfRange(value, nameof(value), 0, 1));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        internal void Int32NotOutOfRange_VariousInLowerExclusiveRangeValues_DoesNothing(int value)
        {
            // Arrange
            // Act
            // Assert
            Debug.Int32NotOutOfRange(value, nameof(value), 0, 3, RangeEndPoints.LowerExclusive);
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        [InlineData(0)]
        [InlineData(2)]
        internal void Int32NotOutOfRange_VariousOutOfLowerExclusiveRangeValues_Throws(int value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.Int32NotOutOfRange(value, nameof(value), 0, 1, RangeEndPoints.LowerExclusive));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        internal void Int32NotOutOfRange_VariousInUpperExclusiveRangeValues_DoesNothing(int value)
        {
            // Arrange
            // Act
            // Assert
            Debug.Int32NotOutOfRange(value, nameof(value), 0, 3, RangeEndPoints.UpperExclusive);
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        [InlineData(-1)]
        [InlineData(2)]
        internal void Int32NotOutOfRange_VariousOutOfUpperExclusiveRangeValues_Throws(int value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.Int32NotOutOfRange(value, nameof(value), 0, 2, RangeEndPoints.UpperExclusive));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        internal void Int32NotOutOfRange_VariousInExclusiveRangeValues_DoesNothing(int value)
        {
            // Arrange
            // Act
            // Assert
            Debug.Int32NotOutOfRange(value, nameof(value), 0, 3, RangeEndPoints.Exclusive);
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        internal void Int32NotOutOfRange_VariousOutOfExclusiveRangeValues_Throws(int value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.Int32NotOutOfRange(value, nameof(value), 0, 1, RangeEndPoints.Exclusive));
        }

                [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        internal void LongNotOutOfRange_VariousInInclusiveRangeValues_DoesNothing(int value)
        {
            // Arrange
            // Act
            // Assert
            Validate.LongNotOutOfRange(value, nameof(value), 0, 3);
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        [InlineData(-1)]
        [InlineData(2)]
        internal void LongNotOutOfRange_VariousOutOfInclusiveRangeValues_Throws(int value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.LongNotOutOfRange(value, nameof(value), 0, 1));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        internal void LongNotOutOfRange_VariousInLowerExclusiveRangeValues_DoesNothing(int value)
        {
            // Arrange
            // Act
            // Assert
            Debug.LongNotOutOfRange(value, nameof(value), 0, 3, RangeEndPoints.LowerExclusive);
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        [InlineData(0)]
        [InlineData(2)]
        internal void LongNotOutOfRange_VariousOutOfLowerExclusiveRangeValues_Throws(int value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.LongNotOutOfRange(value, nameof(value), 0, 1, RangeEndPoints.LowerExclusive));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        internal void LongNotOutOfRange_VariousInUpperExclusiveRangeValues_DoesNothing(int value)
        {
            // Arrange
            // Act
            // Assert
            Debug.LongNotOutOfRange(value, nameof(value), 0, 3, RangeEndPoints.UpperExclusive);
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        [InlineData(-1)]
        [InlineData(2)]
        internal void LongNotOutOfRange_VariousOutOfUpperExclusiveRangeValues_Throws(int value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.LongNotOutOfRange(value, nameof(value), 0, 2, RangeEndPoints.UpperExclusive));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        internal void LongNotOutOfRange_VariousInExclusiveRangeValues_DoesNothing(int value)
        {
            // Arrange
            // Act
            // Assert
            Debug.LongNotOutOfRange(value, nameof(value), 0, 3, RangeEndPoints.Exclusive);
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        internal void LongNotOutOfRange_VariousOutOfExclusiveRangeValues_Throws(int value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.LongNotOutOfRange(value, nameof(value), 0, 1, RangeEndPoints.Exclusive));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(1.1)]
        [InlineData(1.9)]
        [InlineData(2)]
        internal void DoubleNotOutOfRange_VariousInInclusiveRangeValues_DoesNothing(double value)
        {
            // Arrange
            // Act
            // Assert
            Debug.DoubleNotOutOfRange(value, nameof(value), 1, 2);
        }

        [Theory]
        [InlineData(double.NaN)]
        [InlineData(double.PositiveInfinity)]
        [InlineData(double.NegativeInfinity)]
        [InlineData(double.Epsilon)]
        [InlineData(double.MinValue)]
        [InlineData(double.MaxValue)]
        [InlineData(0.99999999999)]
        [InlineData(2.00000000001)]
        internal void DoubleNotOutOfRange_VariousOutOfInclusiveRangeValues_Throws(double value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.DoubleNotOutOfRange(value, nameof(value), 1, 2));
        }

        [Theory]
        [InlineData(1.1)]
        [InlineData(1.9)]
        [InlineData(2)]
        internal void DoubleNotOutOfRange_VariousInLowerExclusiveRangeValues_DoesNothing(double value)
        {
            // Arrange
            // Act
            // Assert
            Debug.DoubleNotOutOfRange(value, nameof(value), 1, 2, RangeEndPoints.LowerExclusive);
        }

        [Theory]
        [InlineData(double.NaN)]
        [InlineData(double.PositiveInfinity)]
        [InlineData(double.NegativeInfinity)]
        [InlineData(double.Epsilon)]
        [InlineData(double.MinValue)]
        [InlineData(double.MaxValue)]
        [InlineData(1)]
        [InlineData(2.00000000001)]
        internal void DoubleNotOutOfRange_VariousOutOfLowerExclusiveRangeValues_Throws(double value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.DoubleNotOutOfRange(value, nameof(value), 1, 2, RangeEndPoints.LowerExclusive));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(1.9)]
        internal void DoubleNotOutOfRange_VariousInUpperExclusiveRangeValues_DoesNothing(double value)
        {
            // Arrange
            // Act
            // Assert
            Debug.DoubleNotOutOfRange(value, nameof(value), 1, 2, RangeEndPoints.UpperExclusive);
        }

        [Theory]
        [InlineData(double.NaN)]
        [InlineData(double.PositiveInfinity)]
        [InlineData(double.NegativeInfinity)]
        [InlineData(double.Epsilon)]
        [InlineData(double.MinValue)]
        [InlineData(double.MaxValue)]
        [InlineData(0)]
        [InlineData(2)]
        internal void DoubleNotOutOfRange_VariousOutOfUpperExclusiveRangeValues_Throws(double value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.DoubleNotOutOfRange(value, nameof(value), 1, 2, RangeEndPoints.UpperExclusive));
        }

        [Theory]
        [InlineData(1.0000000001)]
        [InlineData(1.9999999999)]
        internal void DoubleNotOutOfRange_VariousInExclusiveRangeValues_DoesNothing(double value)
        {
            // Arrange
            // Act
            // Assert
            Debug.DoubleNotOutOfRange(value, nameof(value), 1, 2, RangeEndPoints.Exclusive);
        }

        [Theory]
        [InlineData(double.NaN)]
        [InlineData(double.PositiveInfinity)]
        [InlineData(double.NegativeInfinity)]
        [InlineData(double.Epsilon)]
        [InlineData(double.MinValue)]
        [InlineData(double.MaxValue)]
        [InlineData(0.9)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(2.1)]
        internal void DoubleNotOutOfRange_VariousOutOfExclusiveRangeValues_Throws(double value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.DoubleNotOutOfRange(value, nameof(value), 1, 2, RangeEndPoints.Exclusive));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        internal void DoubleNotOutOfBounds_ValueInBounds_DoesNothing(double value)
        {
            // Arrange
            // Act
            // Assert
            Debug.DoubleNotInvalidNumber(value, nameof(value));
        }

        [Theory]
        [InlineData(double.NaN)]
        [InlineData(double.PositiveInfinity)]
        [InlineData(double.NegativeInfinity)]
        internal void DoubleNotInvalidNumber_VariousOutOfBoundsValuesAndValueAtBounds_Throws(double value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.DoubleNotInvalidNumber(value, nameof(value)));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(1.000000000000000000000000000000000001)]
        [InlineData(1.999999999999999999999999999999999999)]
        [InlineData(2)]
        internal void DecimalNotOutOfRange_VariousInInclusiveRangeValues_DoesNothing(decimal value)
        {
            // Arrange
            // Act
            // Assert
            Debug.DecimalNotOutOfRange(value, nameof(value), 1, 2);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(2.1)]
        internal void DecimalNotOutOfRange_VariousOutOfInclusiveRangeValues_Throws(decimal value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.DecimalNotOutOfRange(value, nameof(value), 1, 2));
        }

        [Theory]
        [InlineData(1.1)]
        [InlineData(1.999999999999999999999999999999999999)]
        [InlineData(2)]
        internal void DecimalNotOutOfRange_VariousInLowerExclusiveRangeValues_DoesNothing(decimal value)
        {
            // Arrange
            // Act
            // Assert
            Debug.DecimalNotOutOfRange(value, nameof(value), 1, 2, RangeEndPoints.LowerExclusive);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2.1)]
        internal void DecimalNotOutOfRange_VariousOutOfLowerExclusiveRangeValues_Throws(decimal value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.DecimalNotOutOfRange(value, nameof(value), 1, 2, RangeEndPoints.LowerExclusive));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(1.9)]
        internal void DecimalNotOutOfRange_VariousInUpperExclusiveRangeValues_DoesNothing(decimal value)
        {
            // Arrange
            // Act
            // Assert
            Debug.DecimalNotOutOfRange(value, nameof(value), 1, 2, RangeEndPoints.UpperExclusive);
        }

        [Theory]
        [InlineData(0.9)]
        [InlineData(2)]
        internal void DecimalNotOutOfRange_VariousOutOfUpperExclusiveRangeValues_Throws(decimal value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.DecimalNotOutOfRange(value, nameof(value), 1, 2, RangeEndPoints.UpperExclusive));
        }

        [Theory]
        [InlineData(1.000000001)]
        [InlineData(1.999999999)]
        internal void DecimalNotOutOfRange_VariousInExclusiveRangeValues_DoesNothing(decimal value)
        {
            // Arrange
            // Act
            // Assert
            Debug.DecimalNotOutOfRange(value, nameof(value), 1, 2, RangeEndPoints.Exclusive);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(0.9)]
        [InlineData(2)]
        [InlineData(2.1)]
        internal void DecimalNotOutOfRange_VariousOutOfExclusiveRangeValues_Throws(decimal value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.DecimalNotOutOfRange(value, nameof(value), 1, 2, RangeEndPoints.Exclusive));
        }
    }
}
