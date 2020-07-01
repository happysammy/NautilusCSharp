//--------------------------------------------------------------------------------------------------
// <copyright file="LabelTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
//--------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Nautilus.Core.Types;
using Xunit;

namespace Nautilus.TestSuite.UnitTests.DomainModelTests.ValueObjectsTests
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class LabelTests
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
            var label2 = new Label();

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
            var label2 = new Label();

            // Act
            // Assert
            Assert.True(label1.NotNone());
            Assert.False(label2.NotNone());
        }
    }
}
