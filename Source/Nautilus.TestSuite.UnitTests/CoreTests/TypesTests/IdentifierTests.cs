//--------------------------------------------------------------------------------------------------
// <copyright file="IdentifierTests.cs" company="Nautech Systems Pty Ltd">
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

namespace Nautilus.TestSuite.UnitTests.CoreTests.TypesTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core.Types;
    using Xunit;

    // Required warning suppression for tests
    // (do not remove even if compiler doesn't initially complain).
#pragma warning disable 8602
#pragma warning disable 8604
#pragma warning disable 8625
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class IdentifierTests
    {
        [Fact]
        internal void Equal_WithVariousValues_ReturnsExpectedResult()
        {
            // Arrange
            var identifier1 = new TestIdentifier("1");
            var identifier2 = new TestIdentifier("2");

            // Act
            // Assert
            Assert.False(identifier1 == null);
            Assert.False(identifier1 == identifier2);
            Assert.True(identifier1 != identifier2);
            Assert.True(identifier1.Equals(identifier1));
            Assert.False(identifier1.Equals(identifier2));
        }

        [Fact]
        internal void GetHashCode_ReturnsExpectedInteger()
        {
            // Arrange
            var identifier = new TestIdentifier("1");

            // Act
            // Assert
            Assert.IsType<int>(identifier.GetHashCode());
        }

        [Fact]
        internal void ToString_ReturnsExpectedString()
        {
            // Arrange
            var identifier = new TestIdentifier("1");

            // Act
            // Assert
            Assert.Equal("1", identifier.Value);
            Assert.Equal("TestIdentifier(1)", identifier.ToString());
        }

        private sealed class TestIdentifier : Identifier<TestIdentifier>
        {
            public TestIdentifier(string value)
                : base(value)
            {
            }
        }
    }
}
