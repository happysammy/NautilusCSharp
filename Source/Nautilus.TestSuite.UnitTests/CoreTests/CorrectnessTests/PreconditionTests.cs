//--------------------------------------------------------------------------------------------------
// <copyright file="PreconditionTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.CoreTests.CorrectnessTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core.Correctness;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class PreconditionTests
    {
        [Fact]
        internal void True_WhenFalse_Throws()
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => Condition.True(false, "some_evaluation"));
        }

        [Fact]
        internal void True_WhenTrue_ReturnsNull()
        {
            // Arrange
            // Act
            // Assert
            Condition.True(true, "some_evaluation");
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
            Assert.Throws<ArgumentNullException>(() => Condition.NotEmptyOrWhiteSpace(value, nameof(value)));
        }

        [Fact]
        internal void NotNull_WithAString_DoesNothing()
        {
            // Arrange
            var obj = "something";

            // Act
            // Assert
            Condition.NotEmptyOrWhiteSpace(obj, nameof(obj));
        }

        [Fact]
        internal void NotNullTee_WithAnObject_DoesNothing()
        {
            // Arrange
            object obj = new EventArgs();

            // Act
            // Assert
            Condition.NotNull(obj, nameof(obj));
        }

        [Fact]
        internal void NotDefault_WithNotDefaultStruct_DoesNothing()
        {
            // Arrange
            var obj = new TimeSpan(0, 0, 1);

            // Act
            // Assert
            Condition.NotDefault(obj, nameof(obj));
        }

        [Fact]
        internal void NotDefault_WithDefaultStruct_Throws()
        {
            // Arrange
            var point = default(TimeSpan);

            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => Condition.NotDefault(point, nameof(point)));
        }

        [Fact]
        internal void Empty_WhenCollectionEmpty_DoesNothing()
        {
            // Arrange
            // ReSharper disable once CollectionNeverUpdated.Local (this is the point of the test)
            var collection = new List<string>();

            // Act
            // Assert
            Condition.Empty(collection, nameof(collection));
        }

        [Fact]
        internal void Empty_WhenCollectionNotEmpty_Throws()
        {
            // Arrange
            var collection = new List<string> { "anElement" };

            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => Condition.Empty(collection, nameof(collection)));
        }

        [Fact]
        internal void NotEmpty_WhenCollectionNotEmpty_DoesNothing()
        {
            // Arrange
            var collection = new List<string> { "foo" };

            // Act
            // Assert
            Condition.NotEmpty(collection, nameof(collection));
        }

        [Fact]
        internal void NotEmpty_WhenCollectionEmpty_Throws()
        {
            // Arrange
            // ReSharper disable once CollectionNeverUpdated.Local (this is the point of the test)
            var collection = new List<string>();

            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => Condition.NotEmpty(collection, nameof(collection)));
        }

        [Fact]
        internal void NotEmpty_WhenDictionaryEmpty_Throws()
        {
            // Arrange
            var dictionary = new Dictionary<string, int>();

            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => Condition.NotEmpty(dictionary, nameof(dictionary)));
        }

        [Fact]
        internal void IsIn_WhenCollectionDoesNotContainElement_Throws()
        {
            // Arrange
            var element = "the_fifth_element";
            var collection = new List<string>();

            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => Condition.IsIn(element,  collection, nameof(element), nameof(collection)));
        }

        [Fact]
        internal void IsIn_WhenCollectionContainsElement_DoesNothing()
        {
            // Arrange
            var element = "the_fifth_element";
            var collection = new List<string> { element };

            // Act
            // Assert
            Condition.IsIn(element, collection, nameof(element), nameof(collection));
        }

        [Fact]
        internal void NotIn_WhenCollectionDoesNotContainElement_DoesNothing()
        {
            // Arrange
            var element = "the_fifth_element";
            var collection = new List<string>();

            // Act
            // Assert
            Condition.NotIn(element, collection, nameof(element), nameof(collection));
        }

        [Fact]
        internal void NotIn_WhenCollectionContainsElement_Throws()
        {
            // Arrange
            var element = "the_fifth_element";
            var collection = new List<string> { element };

            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => Condition.NotIn(element, collection, nameof(element), nameof(collection)));
        }

        [Fact]
        internal void KeyIn_WhenDictionaryDoesContainKey_DoesNothing()
        {
            // Arrange
            var key = "the_key";
            var dictionary = new Dictionary<string, int> { { key, 1 } };

            // Act
            // Assert
            Condition.KeyIn(key, dictionary, nameof(key), nameof(dictionary));
        }

        [Fact]
        internal void KeyIn_WhenDictionaryContainsKey_DoesNothing()
        {
            // Arrange
            var key = "the_key";
            var dictionary = new Dictionary<string, int> { { key, 0 } };

            // Act
            // Assert
            Condition.KeyIn(key, dictionary, nameof(key), nameof(dictionary));
        }

        [Fact]
        internal void KeyIn_WhenDictionaryDoesNotContainKey_Throws()
        {
            // Arrange
            var key = "the_key";
            var dictionary = new Dictionary<string, int>();

            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => Condition.KeyIn(key, dictionary, nameof(key), nameof(dictionary)));
        }

        [Fact]
        internal void KeyNotIn_WhenDictionaryDoesNotContainsKey_DoesNothing()
        {
            // Arrange
            var key = "the_key";
            var dictionary = new Dictionary<string, int> { { "another_key", 2 } };

            // Act
            // Assert
            Condition.KeyNotIn(key, dictionary, nameof(key), nameof(dictionary));
        }

        [Fact]
        internal void KeyNotIn_WhenDictionaryContainsKey_Throws()
        {
            // Arrange
            var key = "the_key";
            var dictionary = new Dictionary<string, int> { { key, 1 } };

            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => Condition.KeyNotIn(key, dictionary, nameof(key), nameof(dictionary)));
        }

        [Fact]
        internal void EqualTo_ValuesAreEqual_DoesNothing()
        {
            // Arrange
            var object1 = 1;
            var object2 = 1;

            // Act
            // Assert
            Condition.EqualTo(object1, object2, nameof(object1));
        }

        [Fact]
        internal void EqualTo_ObjectsAreEqual_DoesNothing()
        {
            // Arrange
            var object1 = "object";
            var object2 = "object";

            // Act
            // Assert
            Condition.EqualTo(object1, object2, nameof(object1));
        }

        [Fact]
        internal void EqualTo_ValuesAreNotEqual_Throws()
        {
            // Arrange
            var object1 = 1;
            var object2 = 2;

            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => Condition.EqualTo(object1, object2, nameof(object1)));
        }

        [Fact]
        internal void EqualTo_ObjectsAreNotEqual_Throws()
        {
            // Arrange
            var object1 = "object1";
            var object2 = "object2";

            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => Condition.EqualTo(object1, object2, nameof(object1)));
        }

        [Fact]
        internal void NotEqualTo_ValuesAreNotEqual_DoesNothing()
        {
            // Arrange
            var object1 = 1;
            var object2 = 2;

            // Act
            // Assert
            Condition.NotEqualTo(object1, object2, nameof(object1));
        }

        [Fact]
        internal void NotEqualTo_ObjectsAreNotEqual_DoesNothing()
        {
            // Arrange
            var object1 = "object1";
            var object2 = "object2";

            // Act
            // Assert
            Condition.NotEqualTo(object1, object2, nameof(object1));
        }

        [Fact]
        internal void NotEqualTo_ValuesAreEqual_Throws()
        {
            // Arrange
            var object1 = 1;
            var object2 = 1;

            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => Condition.NotEqualTo(object1, object2, nameof(object1)));
        }

        [Fact]
        internal void NotEqualTo_ObjectsAreEqual_Throws()
        {
            // Arrange
            var object1 = "object";
            var object2 = "object";

            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => Condition.NotEqualTo(object1, object2, nameof(object1)));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(int.MaxValue)]
        internal void PositiveInt32_VariousPositiveValues_DoesNothing(int value)
        {
            // Arrange
            // Act
            // Assert
            Condition.PositiveInt32(value, nameof(value));
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(0)]
        internal void PositiveInt32_VariousNotPositiveValues_Throws(int value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => Condition.PositiveInt32(value, nameof(value)));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(int.MaxValue)]
        internal void NotNegativeInt32_VariousNotNegativeValues_DoesNothing(int value)
        {
            // Arrange
            // Act
            // Assert
            Condition.NotNegativeInt32(value, nameof(value));
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        internal void NotNegativeInt32_VariousNegativeValues_Throws(int value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => Condition.NotNegativeInt32(value, nameof(value)));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(int.MaxValue)]
        internal void PositiveInt64_VariousPositiveValues_DoesNothing(int value)
        {
            // Arrange
            // Act
            // Assert
            Condition.PositiveInt64(value, nameof(value));
        }

        [Theory]
        [InlineData(long.MinValue)]
        [InlineData(-1)]
        [InlineData(0)]
        internal void PositiveInt64_VariousNotPositiveValues_Throws(long value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => Condition.PositiveInt64(value, nameof(value)));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(long.MaxValue)]
        internal void NotNegativeInt64_VariousNotNegativeValues_DoesNothing(long value)
        {
            // Arrange
            // Act
            // Assert
            Condition.NotNegativeInt64(value, nameof(value));
        }

        [Theory]
        [InlineData(long.MinValue)]
        [InlineData(-1)]
        internal void NotNegativeInt64_VariousNegativeValues_Throws(long value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => Condition.NotNegativeInt64(value, nameof(value)));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(double.MaxValue)]
        internal void PositiveDouble_VariousPositiveValues_DoesNothing(double value)
        {
            // Arrange
            // Act
            // Assert
            Condition.PositiveDouble(value, nameof(value));
        }

        [Theory]
        [InlineData(double.MinValue)]
        [InlineData(-1)]
        [InlineData(0)]
        internal void PositiveDouble_VariousNotPositiveValues_Throws(double value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => Condition.PositiveDouble(value, nameof(value)));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(double.MaxValue)]
        internal void NotNegativeDouble_VariousNotNegativeValues_DoesNothing(double value)
        {
            // Arrange
            // Act
            // Assert
            Condition.NotNegativeDouble(value, nameof(value));
        }

        [Theory]
        [InlineData(double.MinValue)]
        [InlineData(-1)]
        internal void NotNegativeDouble_VariousNegativeValues_Throws(double value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => Condition.NotNegativeDouble(value, nameof(value)));
        }

        [Theory]
        [InlineData(0.1)]
        [InlineData(1.0)]
        internal void PositiveDecimal_VariousPositiveValues_DoesNothing(decimal value)
        {
            // Arrange
            // Act
            // Assert
            Condition.PositiveDecimal(value, nameof(value));
        }

        [Theory]
        [InlineData(0.0)]
        [InlineData(-1.0)]
        internal void PositiveDecimal_VariousNotPositiveValues_Throws(decimal value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => Condition.PositiveDecimal(value, nameof(value)));
        }

        [Theory]
        [InlineData(0.0)]
        [InlineData(1)]
        internal void NotNegativeDecimal_VariousNotNegativeValues_DoesNothing(decimal value)
        {
            // Arrange
            // Act
            // Assert
            Condition.NotNegativeDecimal(value, nameof(value));
        }

        [Theory]
        [InlineData(-0.1)]
        [InlineData(-1.0)]
        internal void NotNegativeDecimal_VariousNegativeValues_Throws(decimal value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => Condition.NotNegativeDecimal(value, nameof(value)));
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
            Condition.NotOutOfRangeInt32(value,  0, 3, nameof(value));
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
            Assert.Throws<ArgumentOutOfRangeException>(() => Condition.NotOutOfRangeInt32(value, 0, 1, nameof(value)));
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
            Condition.NotOutOfRangeInt64(value, 0, 3, nameof(value));
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
            Assert.Throws<ArgumentOutOfRangeException>(() => Condition.NotOutOfRangeInt64(value, 0, 1, nameof(value)));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        internal void NotOutOfRangeDouble_VariousInInclusiveRangeValues_DoesNothing(double value)
        {
            // Arrange
            // Act
            // Assert
            Condition.NotOutOfRangeDouble(value, 1, 2, nameof(value));
        }

        [Theory]
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
            Assert.Throws<ArgumentOutOfRangeException>(() => Condition.NotOutOfRangeDouble(value, 1, 2, nameof(value)));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        internal void NotOutOfRangeDecimal_VariousInInclusiveRangeValues_DoesNothing(decimal value)
        {
            // Arrange
            // Act
            // Assert
            Condition.NotOutOfRangeDecimal(value, 1, 2, nameof(value));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(2.1)]
        internal void NotOutOfRangeDecimal_VariousOutOfInclusiveRangeValues_Throws(decimal value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => Condition.NotOutOfRangeDecimal(value, 1, 2, nameof(value)));
        }
    }
}
