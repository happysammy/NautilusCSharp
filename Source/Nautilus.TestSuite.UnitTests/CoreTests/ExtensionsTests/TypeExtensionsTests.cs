//--------------------------------------------------------------------------------------------------
// <copyright file="TypeExtensionsTests.cs" company="Nautech Systems Pty Ltd">
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

namespace Nautilus.TestSuite.UnitTests.CoreTests.ExtensionsTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Types;
    using Nautilus.Messaging;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class TypeExtensionsTests
    {
        [Fact]
        internal void NameFormatted_FromReferenceType_ReturnsExpectedName()
        {
            // Arrange
            // Act
            var result = typeof(Console).NameFormatted();

            // Assert
            Assert.Equal("Console", result);
        }

        [Fact]
        internal void NameFormatted_FromSingleGenericType_ReturnsExpectedName()
        {
            // Arrange
            // Act
            var result1 = typeof(Envelope<Message>).Name;
            var result2 = typeof(Envelope<Message>).NameFormatted();

            // Assert
            Assert.Equal("Envelope`1", result1);
            Assert.Equal("Envelope<Message>", result2);
        }

        [Fact]
        internal void NameFormatted_FromMultipleGenericType_ReturnsExpectedName()
        {
            // Arrange
            // Act
            var result1 = typeof(Func<int, int, string>).Name;
            var result2 = typeof(Func<int, int, string>).NameFormatted();

            // Assert
            Assert.Equal("Func`3", result1);
            Assert.Equal("Func<Int32,Int32,String>", result2);
        }
    }
}
