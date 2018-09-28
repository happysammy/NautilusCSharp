//--------------------------------------------------------------------------------------------------
// <copyright file="OptionExtensionsTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.CoreTests.ExtensionsTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core;
    using Nautilus.Core.Extensions;
    using Xunit;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class OptionExtensionsTests
    {
        [Fact]
        public void Unwrap_WithValue_ReturnsExpectedValue()
        {
            // Arrange
            var instance = new TestClass();
            Option<TestClass> option = instance;

            // Act
            var testClass = option.Unwrap();

            // Assert
            Assert.Equal(instance, testClass);
        }

        [Fact]
        public void Unwrap_WithNoValue_ReturnsNull()
        {
            // Arrange
            Option<TestClass> option = null;

            // Act
            var testClass = option.Unwrap();

            // Assert
            Assert.Null(testClass);
        }

        [Fact]
        public void Unwrap_WithFilter_ReturnsExpectedResult()
        {
            // Arrange
            Option<TestClass> option = new TestClass { Label = "Some value" };

            // Act
            var result = option.Unwrap(x => x.Label);

            // Assert
            Assert.Equal("Some value", result);
        }

        [Fact]
        public void Unwrap_WithDefaultEmptyString_ReturnsEmptyString()
        {
            // Arrange
            Option<string> option = null;

            // Act
            var result = option.Unwrap(string.Empty);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void ToResult_WithValue_IsSuccess()
        {
            // Arrange
            var instance = new TestClass();
            Option<TestClass> option = instance;

            // Act
            var result = option.ToResult("Ok");

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(instance, result.Value);
        }

        [Fact]
        public void ToResult_WithNoValue_ReturnsError()
        {
            // Arrange
            Option<TestClass> option = null;

            // Act
            var result = option.ToResult("error");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("error", result.Message);
        }

        [Fact]
        public void Where_WithPredicateTrue_ReturnsOption()
        {
            // Arrange
            var instance = new TestClass { Label = "Some value" };
            Option<TestClass> option = instance;

            // Act
            var option2 = option.Where(x => x.Label == "Some value");

            // Assert
            Assert.True(option2.HasValue);
            Assert.Equal(instance, option2.Value);
        }

        [Fact]
        public void Where_WithPredicateFalse_ReturnsNoValue()
        {
            // Arrange
            var instance = new TestClass { Label = "Some value" };
            Option<TestClass> option = instance;

            // Act
            var option2 = option.Where(x => x.Label == "Different value");

            // Assert
            Assert.False(option2.HasValue);
        }

        [Fact]
        public void Where_WithNullOption_HasNoValue()
        {
            // Arrange
            Option<TestClass> option = null;

            // Act
            var option2 = option.Where(x => x.Label == "Some value");

            // Assert
            Assert.False(option2.HasValue);
        }

        [Fact]
        public void Select_WithEqualValue_ReturnsNewOption()
        {
            // Arrange
            Option<TestClass> option = new TestClass { Label = "Some value" };

            // Act
            var option2 = option.Select(x => x.Label);

            // Assert
            Assert.True(option2.HasValue);
            Assert.Equal("Some value", option2.Value);
        }

        [Fact]
        public void Select_WithNoValue_ReturnsNoValue()
        {
            // Arrange
            Option<TestClass> option = null;

            // Act
            var option2 = option.Select(x => x.Label);

            // Assert
            Assert.False(option2.HasValue);
        }

        [Fact]
        public void Bind_WithEqualValue_ReturnsNewOption()
        {
            // Arrange
            Option<TestClass> option = new TestClass { Label = "Some value" };

            // Act
            var option2 = option.Select(GetLabel);

            // Assert
            Assert.True(option2.HasValue);
            Assert.Equal("Some value", option2.Value);
        }

        [Fact]
        public void Bind_WhenInternalMethodReturnsNoValue_ReturnsNoValue()
        {
            // Arrange
            Option<TestClass> option = new TestClass { Label = null };

            // Act
            var option2 = option.Select(GetLabel);

            // Assert
            Assert.False(option2.HasValue);
        }

        [Fact]
        public void Execute_WithAction_ExecutesTheAction()
        {
            // Arrange
            string property = null;
            Option<TestClass> option = new TestClass { Label = "Some value" };

            // Act
            option.Execute(x => property = x.Label);

            // Assert
            Assert.Equal("Some value", property);
            Assert.Equal("Some value", option.Value.Label);
        }

        [Fact]
        public void Execute_WithNoValue_DoesNothing()
        {
            // Arrange
            string property = string.Empty;
            var option = Option<TestClass>.None();

            // Act
            option.Execute(x => property = x.Label);

            // Assert
            Assert.Equal(string.Empty, property);
        }

        [Fact]
        public void Unwrap_WithValueType_ReturnsTheCorrectValue()
        {
            // Arrange
            Option<TestClass> option = new TestClass { Value = 42 };

            // Act
            var integer = option.Select(x => x.Value).Unwrap();

            // Assert
            Assert.Equal(42, integer);
        }

        [Fact]
        public void Unwrap_WithNullValue_ReturnsDefaultValue()
        {
            // Arrange
            Option<TestClass> option = null;

            // Act
            var result = option.Select(x => x.Value).Unwrap();

            // Assert
            Assert.Equal(0, result);
        }

        private static Option<string> GetLabel(TestClass testClass)
        {
            return testClass.Label;
        }

        // Only instantiated within this class for testing purposes.
        private class TestClass
        {
            public string Label { get; set; }

            public int Value { get; set; }
        }
    }
}
