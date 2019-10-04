//--------------------------------------------------------------------------------------------------
// <copyright file="LabelTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DomainModelTests.ValueObjectsTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core.Types;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class LabelTests
    {
        [Fact]
        internal void Value_WithValidValueGiven_ReturnsExpectedValue()
        {
            // Arrange
            var label = new Label("abc123");

            // Act
            // Assert
            Assert.Equal("abc123", label.Value);
        }

        [Fact]
        internal void EqualityOperators_ReturnExpectedValues()
        {
            // Arrange
            var validString1 = new Label("abc123");
            var validString2 = new Label("def999");
            var validString3 = new Label("abc123");

            // Act
            // Assert
            Assert.True(validString1 == validString3);
            Assert.False(validString1 == validString2);
            Assert.False(validString1 != validString3);
            Assert.True(validString1 != validString2);

            Assert.True(validString1.Equals(validString1));
            Assert.False(validString1.Equals(validString2));
        }

        [Fact]
        internal void IsNone_WithVariousValues_ReturnsCorrectValue()
        {
            // Arrange
            var label1 = new Label("abc123");
            var label2 = new Label("NONE");

            // Act
            // Assert
            Assert.False(label1.IsNone());
            Assert.True(label2.IsNone());
        }

        [Fact]
        internal void NotNone_WithVariousValues_ReturnsCorrectValue()
        {
            // Arrange
            var label1 = new Label("abc123");
            var label2 = new Label("NONE");

            // Act
            // Assert
            Assert.True(label1.NotNone());
            Assert.False(label2.NotNone());
        }
    }
}
