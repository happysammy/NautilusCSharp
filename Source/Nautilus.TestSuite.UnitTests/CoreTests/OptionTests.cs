//--------------------------------------------------------------------------------------------------
// <copyright file="OptionTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.CoreTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core;
    using Xunit;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class OptionTests
    {
        [Fact]
        internal void Create_WithImplicitOperator_HasValue()
        {
            // Arrange
            var instance = new TestClass();

            // Act
            OptionRef<TestClass> option = instance;

            // Assert
            Assert.True(option.HasValue);
            Assert.False(option.HasNoValue);
            Assert.Equal(instance, option.Value);
        }

        [Fact]
        internal void None_WithTestClass_ReturnsOptionWithNoValue()
        {
            // Arrange
            // Act
            var result = OptionRef<TestClass>.None();

            // Assert
            Assert.True(result.HasNoValue);
            Assert.False(result.HasValue);
        }

        [Fact]
        internal void None_WithDefaultStruct_ReturnsOptionWithValue()
        {
            // Arrange
            // Act
            var result = OptionVal<DateTime>.None();

            // Assert
            Assert.True(result.HasNoValue);
            Assert.False(result.HasValue);
        }

        [Fact]
        internal void None_WithNullableStruct_ReturnsOptionWithNoValue()
        {
            // Arrange
            // Act
            var result = OptionVal<DateTime>.None();

            // Assert
            Assert.True(result.HasNoValue);
            Assert.False(result.HasValue);
        }

        [Fact]
        internal void From_WithDateTime_ReturnsOptionWithTheValue()
        {
            // Arrange
            var time = new DateTime(2000, 1, 1);

            // Act
            var result = OptionVal<DateTime>.Some(time);

            // Assert
            Assert.Equal(time, result.Value);
        }

        [Fact]
        internal void Equals_OptionCreatedWithNone_ReturnsTrue()
        {
            // Arrange
            OptionRef<TestClass> option1 = OptionRef<TestClass>.None();
            OptionRef<TestClass> option2 = OptionRef<TestClass>.None();

            // Act
            var result = option1.Equals(option2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        internal void Equals_WhenOneOptionNull_ReturnsFalse()
        {
            // Arrange
            var option1 = OptionRef<TestClass>.Some(new TestClass());
            var option2 = OptionRef<TestClass>.None();

            // Act
            var result = option1.Equals(option2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        internal void Equals_WhenOptionsHaveSameValue_ReturnsTrue()
        {
            // Arrange
            var dateTime1 = new DateTime(2000, 1, 1);
            var dateTime2 = new DateTime(2000, 1, 1);

            var option1 = OptionVal<DateTime>.Some(dateTime1);
            var option2 = OptionVal<DateTime>.Some(dateTime2);

            // Act
            var result1 = option1.Equals((object)option2);
            var result2 = option1.Equals(option2);
            var result3 = option1.Equals((object)dateTime2);
            var result4 = option1.Equals(dateTime2);
            var result5 = option1 == option2;
            var result6 = option1 == dateTime2;

            // Assert
            Assert.True(result1);
            Assert.True(result2);
            Assert.True(result3);
            Assert.True(result4);
            Assert.True(result5);
            Assert.True(result6);
        }

        [Fact]
        internal void Equals_WhenOptionsHaveDifferentValues_ReturnsFalse()
        {
            // Arrange
            var dateTime1 = new DateTime(2000, 1, 1);
            var dateTime2 = new DateTime(2000, 1, 2);

            var option1 = OptionVal<DateTime>.Some(dateTime1);
            var option2 = OptionVal<DateTime>.Some(dateTime2);

            // Act
            var result1 = option1.Equals((object)option2);
            var result2 = option1.Equals(option2);
            var result3 = option1.Equals((object)dateTime2);
            var result4 = option1.Equals(dateTime2);
            var result5 = option1 != option2;
            var result6 = option1 != dateTime2;

            // Assert
            Assert.False(result1);
            Assert.False(result2);
            Assert.False(result3);
            Assert.False(result4);
            Assert.True(result5);
            Assert.True(result6);
        }

        [Fact]
        internal void Equals_WithString_ReturnsFalse()
        {
            // Arrange
            var option = OptionRef<TestClass>.Some(new TestClass());

            // Act
            var result = option.Equals("string");

            // Assert
            Assert.False(result);
        }

        [Fact]
        internal void GetHashCode_WithNoValue_ReturnsZero()
        {
            // Arrange
            var option = OptionRef<TestClass>.None();

            // Act
            var result = option.GetHashCode();

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        internal void GetHashCode_WithValue_ReturnsExpectedInt()
        {
            // Arrange
            var option = OptionRef<TestClass>.Some(new TestClass());

            // Act
            var result = option.GetHashCode();

            // Assert
            Assert.Equal(1028, result);
        }

        [Fact]
        internal void ToString_WithNoValue_ReturnsExpectedString()
        {
            // Arrange
            var option = OptionRef<TestClass>.None();

            // Act
            var result = option.ToString();

            // Assert
            Assert.Equal("NONE", result);
        }

        [Fact]
        internal void ToString_WithValue_ReturnsValueObjectsToString()
        {
            // Arrange
            OptionRef<TestClass> option = new TestClass();

            // Act
            var result = option.ToString();

            // Assert
            Assert.Equal("test_value", result);
        }

        // Only used within this class for testing purposes.
        private class TestClass
        {
            public override int GetHashCode() => 535;

            public override string ToString() => "test_value";
        }
    }
}
