//--------------------------------------------------------------------------------------------------
// <copyright file="ValidateTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.CoreTests.ValidationTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core.Validation;
    using Xunit;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("ReSharper", "CollectionNeverUpdated.Local", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class ValidateTests
    {
        [Fact]
        internal void True_WhenFalse_Throws()
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Validate.True(false, "some_evaluation"));
        }

        [Fact]
        internal void True_WhenTrue_ReturnsNull()
        {
            // Arrange
            // Act
            // Assert
            Validate.True(true, "some_evaluation");
        }

        [Fact]
        internal void TrueIf_WhenConditionTrueAndPredicateFalse_Throws()
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Validate.TrueIf(true, false, "some_evaluation"));
        }

        [Fact]
        internal void TrueIf_WhenConditionTrueAndPredicateTrue_DoesNothing()
        {
            // Arrange
            // Act
            // Assert
            Validate.TrueIf(true, true, "some_evaluation");
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
            Assert.Throws<ValidationException>(() => Validate.NotNull(value, nameof(value)));
        }

        [Fact]
        internal void NotNull_WithAString_DoesNothing()
        {
            // Arrange
            var obj = "something";

            // Act
            // Assert
            Validate.NotNull(obj, nameof(obj));
        }

        [Fact]
        internal void NotNullTee_WithNullObject_Throws()
        {
            // Arrange
            object obj = null;

            // Act
            // Assert - Ignore expression is always null warning (the point of the test is to catch this condition).
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Throws<ValidationException>(() => Validate.NotNull(obj, nameof(obj)));
        }

        [Fact]
        internal void NotNullTee_WithAnObject_DoesNothing()
        {
            // Arrange
            object obj = new EventArgs();

            // Act
            // Assert
            Validate.NotNull(obj, nameof(obj));
        }

        [Fact]
        internal void NotDefault_WithNotDefaultStruct_DoesNothing()
        {
            // Arrange
            var obj = new TimeSpan(0, 0, 1);

            // Act
            // Assert
            Validate.NotDefault(obj, nameof(obj));
        }

        [Fact]
        internal void NotDefault_WithDefaultStruct_Throws()
        {
            // Arrange
            var point = default(TimeSpan);

            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Validate.NotDefault(point, nameof(point)));
        }

        [Fact]
        internal void Empty_WhenCollectionEmpty_DoesNothing()
        {
            // Arrange
            var collection = new List<string>();

            // Act
            // Assert
            Validate.Empty(collection, nameof(collection));
        }

        [Fact]
        internal void Empty_WhenCollectionNotEmpty_Throws()
        {
            // Arrange
            var collection = new List<string> { "anElement" };

            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Validate.Empty(collection, nameof(collection)));
        }

        [Fact]
        internal void Empty_WhenCollectionNull_Throws()
        {
            // Arrange
            List<string> collection = null;

            // Act
            // Assert - Ignore expression is always null warning (the point of the test is to catch this condition).
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Throws<ValidationException>(() => Validate.Empty(collection, nameof(collection)));
        }

        [Fact]
        internal void NotNullOrEmpty_WhenCollectionNotEmpty_DoesNothing()
        {
            // Arrange
            var collection = new List<string> { "foo" };

            // Act
            // Assert
            Validate.NotNullOrEmpty(collection, nameof(collection));
        }

        [Fact]
        internal void NotNullOrEmpty_WhenCollectionNull_Throws()
        {
            // Arrange
            List<string> collection = null;

            // Act
            // Assert - Ignore expression is always null warning (the point of the test is to catch this condition).
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Throws<ValidationException>(() => Validate.NotNullOrEmpty(collection, nameof(collection)));
        }

        [Fact]
        internal void NotNullOrEmpty_WhenCollectionEmpty_Throws()
        {
            // Arrange
            var collection = new List<string>();

            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Validate.NotNullOrEmpty(collection, nameof(collection)));
        }

        [Fact]
        internal void NotNullOrEmpty_WhenDictionaryNull_Throws()
        {
            // Arrange
            Dictionary<string, int> dictionary = null;

            // Act
            // Assert - Ignore expression is always null warning (the point of the test is to catch this condition).
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Throws<ValidationException>(() => Validate.NotNullOrEmpty(dictionary, nameof(dictionary)));
        }

        [Fact]
        internal void NotNullOrEmpty_WhenDictionaryEmpty_Throws()
        {
            // Arrange
            var dictionary = new Dictionary<string, int>();

            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Validate.NotNullOrEmpty(dictionary, nameof(dictionary)));
        }

        [Fact]
        internal void Contains_WhenCollectionDoesNotContainElement_Throws()
        {
            // Arrange
            var element = "the_fifth_element";
            var collection = new List<string>();

            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Validate.Contains(element, nameof(element), collection));
        }

        [Fact]
        internal void Contains_WhenCollectionContainsElement_DoesNothing()
        {
            // Arrange
            var element = "the_fifth_element";
            var collection = new List<string> { element };

            // Act
            // Assert
            Validate.Contains(element, nameof(element), collection);
        }

        [Fact]
        internal void DoesNotContain_WhenCollectionDoesNotContainElement_DoesNothing()
        {
            // Arrange
            var element = "the_fifth_element";
            var collection = new List<string>();

            // Act
            // Assert
            Validate.DoesNotContain(element, nameof(element), collection);
        }

        [Fact]
        internal void DoesNotContain_WhenCollectionContainsElement_Throws()
        {
            // Arrange
            var element = "the_fifth_element";
            var collection = new List<string> { element };

            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Validate.DoesNotContain(element, nameof(element), collection));
        }

        [Fact]
        internal void ContainsKey_WhenDictionaryDoesContainKey_DoesNothing()
        {
            // Arrange
            var key = "the_key";
            var dictionary = new Dictionary<string, int> { { key, 1 } };

            // Act
            // Assert
            Validate.ContainsKey(key, nameof(key), dictionary);
        }

        [Fact]
        internal void ContainsKey_WhenDictionaryContainsKey_DoesNothing()
        {
            // Arrange
            var key = "the_key";
            var dictionary = new Dictionary<string, int> { { key, 0 } };

            // Act
            // Assert
            Validate.ContainsKey(key, nameof(key), dictionary);
        }

        [Fact]
        internal void ContainsKey_WhenDictionaryDoesNotContainKey_Throws()
        {
            // Arrange
            var key = "the_key";
            var dictionary = new Dictionary<string, int>();

            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Validate.ContainsKey(key, nameof(key), dictionary));
        }

        [Fact]
        internal void DoesNotContainKey_WhenDictionaryDoesNotContainsKey_DoesNothing()
        {
            // Arrange
            var key = "the_key";
            var dictionary = new Dictionary<string, int> { { "another_key", 2 } };

            // Act
            // Assert
            Validate.DoesNotContainKey(key, nameof(key), dictionary);
        }

        [Fact]
        internal void DoesNotContainKey_WhenDictionaryContainsKey_Throws()
        {
            // Arrange
            var key = "the_key";
            var dictionary = new Dictionary<string, int> { { key, 1 } };

            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Validate.DoesNotContainKey(key, nameof(key), dictionary));
        }

        [Fact]
        internal void EqualTo_ValuesAreEqual_DoesNothing()
        {
            // Arrange
            var object1 = 1;
            var object2 = 1;

            // Act
            // Assert
            Validate.EqualTo(object1, nameof(object1), object2);
        }

        [Fact]
        internal void EqualTo_ObjectsAreEqual_DoesNothing()
        {
            // Arrange
            var object1 = "object";
            var object2 = "object";

            // Act
            // Assert
            Validate.EqualTo(object1, nameof(object1), object2);
        }

        [Fact]
        internal void EqualTo_ValuesAreNotEqual_Throws()
        {
            // Arrange
            var object1 = 1;
            var object2 = 2;

            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Validate.EqualTo(object1, nameof(object1), object2));
        }

        [Fact]
        internal void EqualTo_ObjectsAreNotEqual_Throws()
        {
            // Arrange
            var object1 = "object1";
            var object2 = "object2";

            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Validate.EqualTo(object1, nameof(object1), object2));
        }

        [Fact]
        internal void NotEqualTo_ValuesAreNotEqual_DoesNothing()
        {
            // Arrange
            var object1 = 1;
            var object2 = 2;

            // Act
            // Assert
            Validate.NotEqualTo(object1, nameof(object1), object2);
        }

        [Fact]
        internal void NotEqualTo_ObjectsAreNotEqual_DoesNothing()
        {
            // Arrange
            var object1 = "object1";
            var object2 = "object2";

            // Act
            // Assert
            Validate.NotEqualTo(object1, nameof(object1), object2);
        }

        [Fact]
        internal void NotEqualTo_ValuesAreEqual_Throws()
        {
            // Arrange
            var object1 = 1;
            var object2 = 1;

            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Validate.NotEqualTo(object1, nameof(object1), object2));
        }

        [Fact]
        internal void NotEqualTo_ObjectsAreEqual_Throws()
        {
            // Arrange
            var object1 = "object";
            var object2 = "object";

            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Validate.NotEqualTo(object1, nameof(object1), object2));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        internal void NotOutOfRangeInt32_VariousInInclusiveRangeValues_DoesNothing(int value)
        {
            // Arrange
            // Act
            // Assert
            Validate.NotOutOfRangeInt32(value, nameof(value), 0, 3);
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        [InlineData(-1)]
        [InlineData(2)]
        internal void NotOutOfRangeInt32_VariousOutOfInclusiveRangeValues_Throws(int value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Validate.NotOutOfRangeInt32(value, nameof(value), 0, 1));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        internal void NotOutOfRangeInt32_VariousInLowerExclusiveRangeValues_DoesNothing(int value)
        {
            // Arrange
            // Act
            // Assert
            Validate.NotOutOfRangeInt32(value, nameof(value), 0, 3, RangeEndPoints.LowerExclusive);
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        [InlineData(0)]
        [InlineData(2)]
        internal void NotOutOfRangeInt32_VariousOutOfLowerExclusiveRangeValues_Throws(int value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Validate.NotOutOfRangeInt32(value, nameof(value), 0, 1, RangeEndPoints.LowerExclusive));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        internal void NotOutOfRangeInt32_VariousInUpperExclusiveRangeValues_DoesNothing(int value)
        {
            // Arrange
            // Act
            // Assert
            Validate.NotOutOfRangeInt32(value, nameof(value), 0, 3, RangeEndPoints.UpperExclusive);
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        [InlineData(-1)]
        [InlineData(2)]
        internal void NotOutOfRangeInt32_VariousOutOfUpperExclusiveRangeValues_Throws(int value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Validate.NotOutOfRangeInt32(value, nameof(value), 0, 2, RangeEndPoints.UpperExclusive));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        internal void NotOutOfRangeInt32_VariousInExclusiveRangeValues_DoesNothing(int value)
        {
            // Arrange
            // Act
            // Assert
            Validate.NotOutOfRangeInt32(value, nameof(value), 0, 3, RangeEndPoints.Exclusive);
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        internal void NotOutOfRangeInt32_VariousOutOfExclusiveRangeValues_Throws(int value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Validate.NotOutOfRangeInt32(value, nameof(value), 0, 1, RangeEndPoints.Exclusive));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        internal void NotOutOfRangeInt64_VariousInInclusiveRangeValues_DoesNothing(int value)
        {
            // Arrange
            // Act
            // Assert
            Validate.NotOutOfRangeInt64(value, nameof(value), 0, 3);
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        [InlineData(-1)]
        [InlineData(2)]
        internal void NotOutOfRangeInt64_VariousOutOfInclusiveRangeValues_Throws(int value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Validate.NotOutOfRangeInt64(value, nameof(value), 0, 1));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        internal void NotOutOfRangeInt64_VariousInLowerExclusiveRangeValues_DoesNothing(int value)
        {
            // Arrange
            // Act
            // Assert
            Validate.NotOutOfRangeInt64(value, nameof(value), 0, 3, RangeEndPoints.LowerExclusive);
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        [InlineData(0)]
        [InlineData(2)]
        internal void NotOutOfRangeInt64_VariousOutOfLowerExclusiveRangeValues_Throws(int value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Validate.NotOutOfRangeInt64(value, nameof(value), 0, 1, RangeEndPoints.LowerExclusive));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        internal void NotOutOfRangeInt64_VariousInUpperExclusiveRangeValues_DoesNothing(int value)
        {
            // Arrange
            // Act
            // Assert
            Validate.NotOutOfRangeInt64(value, nameof(value), 0, 3, RangeEndPoints.UpperExclusive);
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        [InlineData(-1)]
        [InlineData(2)]
        internal void NotOutOfRangeInt64_VariousOutOfUpperExclusiveRangeValues_Throws(int value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Validate.NotOutOfRangeInt64(value, nameof(value), 0, 2, RangeEndPoints.UpperExclusive));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        internal void NotOutOfRangeInt64_VariousInExclusiveRangeValues_DoesNothing(int value)
        {
            // Arrange
            // Act
            // Assert
            Validate.NotOutOfRangeInt64(value, nameof(value), 0, 3, RangeEndPoints.Exclusive);
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        internal void NotOutOfRangeInt64_VariousOutOfExclusiveRangeValues_Throws(int value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Validate.NotOutOfRangeInt64(value, nameof(value), 0, 1, RangeEndPoints.Exclusive));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(1.1)]
        [InlineData(1.9)]
        [InlineData(2)]
        internal void NotOutOfRangeDouble_VariousInInclusiveRangeValues_DoesNothing(double value)
        {
            // Arrange
            // Act
            // Assert
            Validate.NotOutOfRangeDouble(value, nameof(value), 1, 2);
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
        internal void NotOutOfRangeDouble_VariousOutOfInclusiveRangeValues_Throws(double value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Validate.NotOutOfRangeDouble(value, nameof(value), 1, 2));
        }

        [Theory]
        [InlineData(1.1)]
        [InlineData(1.9)]
        [InlineData(2)]
        internal void NotOutOfRangeDouble_VariousInLowerExclusiveRangeValues_DoesNothing(double value)
        {
            // Arrange
            // Act
            // Assert
            Validate.NotOutOfRangeDouble(value, nameof(value), 1, 2, RangeEndPoints.LowerExclusive);
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
        internal void NotOutOfRangeDouble_VariousOutOfLowerExclusiveRangeValues_Throws(double value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Validate.NotOutOfRangeDouble(value, nameof(value), 1, 2, RangeEndPoints.LowerExclusive));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(1.9)]
        internal void NotOutOfRangeDouble_VariousInUpperExclusiveRangeValues_DoesNothing(double value)
        {
            // Arrange
            // Act
            // Assert
            Validate.NotOutOfRangeDouble(value, nameof(value), 1, 2, RangeEndPoints.UpperExclusive);
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
        internal void NotOutOfRangeDouble_VariousOutOfUpperExclusiveRangeValues_Throws(double value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Validate.NotOutOfRangeDouble(value, nameof(value), 1, 2, RangeEndPoints.UpperExclusive));
        }

        [Theory]
        [InlineData(1.0000000001)]
        [InlineData(1.9999999999)]
        internal void NotOutOfRangeDouble_VariousInExclusiveRangeValues_DoesNothing(double value)
        {
            // Arrange
            // Act
            // Assert
            Validate.NotOutOfRangeDouble(value, nameof(value), 1, 2, RangeEndPoints.Exclusive);
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
        internal void NotOutOfRangeDouble_VariousOutOfExclusiveRangeValues_Throws(double value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Validate.NotOutOfRangeDouble(value, nameof(value), 1, 2, RangeEndPoints.Exclusive));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        internal void NotInvalidNumber_ValueInBounds_DoesNothing(double value)
        {
            // Arrange
            // Act
            // Assert
            Validate.NotInvalidNumber(value, nameof(value));
        }

        [Theory]
        [InlineData(double.NaN)]
        [InlineData(double.PositiveInfinity)]
        [InlineData(double.NegativeInfinity)]
        internal void NotInvalidNumber_VariousOutOfBoundsValuesAndValueAtBounds_Throws(double value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Validate.NotInvalidNumber(value, nameof(value)));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(1.000000000000000000000000000000000001)]
        [InlineData(1.999999999999999999999999999999999999)]
        [InlineData(2)]
        internal void NotOutOfRangeDecimal_VariousInInclusiveRangeValues_DoesNothing(decimal value)
        {
            // Arrange
            // Act
            // Assert
            Validate.NotOutOfRangeDecimal(value, nameof(value), 1, 2);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(2.1)]
        internal void NotOutOfRangeDecimal_VariousOutOfInclusiveRangeValues_Throws(decimal value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Validate.NotOutOfRangeDecimal(value, nameof(value), 1, 2));
        }

        [Theory]
        [InlineData(1.1)]
        [InlineData(1.999999999999999999999999999999999999)]
        [InlineData(2)]
        internal void NotOutOfRangeDecimal_VariousInLowerExclusiveRangeValues_DoesNothing(decimal value)
        {
            // Arrange
            // Act
            // Assert
            Validate.NotOutOfRangeDecimal(value, nameof(value), 1, 2, RangeEndPoints.LowerExclusive);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2.1)]
        internal void NotOutOfRangeDecimal_VariousOutOfLowerExclusiveRangeValues_Throws(decimal value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Validate.NotOutOfRangeDecimal(value, nameof(value), 1, 2, RangeEndPoints.LowerExclusive));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(1.9)]
        internal void NotOutOfRangeDecimal_VariousInUpperExclusiveRangeValues_DoesNothing(decimal value)
        {
            // Arrange
            // Act
            // Assert
            Validate.NotOutOfRangeDecimal(value, nameof(value), 1, 2, RangeEndPoints.UpperExclusive);
        }

        [Theory]
        [InlineData(0.9)]
        [InlineData(2)]
        internal void NotOutOfRangeDecimal_VariousOutOfUpperExclusiveRangeValues_Throws(decimal value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Validate.NotOutOfRangeDecimal(value, nameof(value), 1, 2, RangeEndPoints.UpperExclusive));
        }

        [Theory]
        [InlineData(1.000000001)]
        [InlineData(1.999999999)]
        internal void NotOutOfRangeDecimal_VariousInExclusiveRangeValues_DoesNothing(decimal value)
        {
            // Arrange
            // Act
            // Assert
            Validate.NotOutOfRangeDecimal(value, nameof(value), 1, 2, RangeEndPoints.Exclusive);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(0.9)]
        [InlineData(2)]
        [InlineData(2.1)]
        internal void NotOutOfRangeDecimal_VariousOutOfExclusiveRangeValues_Throws(decimal value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Validate.NotOutOfRangeDecimal(value, nameof(value), 1, 2, RangeEndPoints.Exclusive));
        }
    }
}
