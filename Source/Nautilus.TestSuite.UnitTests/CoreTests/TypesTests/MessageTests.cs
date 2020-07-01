//--------------------------------------------------------------------------------------------------
// <copyright file="MessageTests.cs" company="Nautech Systems Pty Ltd">
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

using System;
using System.Diagnostics.CodeAnalysis;
using Nautilus.Core.Message;
using Nautilus.TestSuite.TestKit.Stubs;
using NodaTime;
using Xunit;

namespace Nautilus.TestSuite.UnitTests.CoreTests.TypesTests
{
    // Required warning suppression for tests
    // (do not remove even if compiler doesn't initially complain).
#pragma warning disable 8602
#pragma warning disable 8604
#pragma warning disable 8625
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class MessageTests
    {
        [Fact]
        internal void Equal_WithDifferentMessagesOfTheSameContent_CanEquateById()
        {
            // Arrange
            var message1 = new TestMessage(Guid.NewGuid(), StubZonedDateTime.UnixEpoch());
            var message2 = new TestMessage(Guid.NewGuid(), StubZonedDateTime.UnixEpoch());

            // Act
            // Assert
            Assert.NotNull(message1);
            Assert.False(message1 == null);
            Assert.False(message1 == message2);
            Assert.True(message1 != message2);
            Assert.True(message1.Equals(message1));
            Assert.False(message1.Equals(message2));
        }

        [Fact]
        internal void GetHashCode_ReturnsExpectedInteger()
        {
            // Arrange
            var message = new TestMessage(Guid.NewGuid(), StubZonedDateTime.UnixEpoch());

            // Act
            // Assert
            Assert.IsType<int>(message.GetHashCode());
        }

        [Fact]
        internal void ToString_ReturnsExpectedString()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var message = new TestMessage(guid, StubZonedDateTime.UnixEpoch());

            // Act
            var result = message.ToString();

            // Assert
            Assert.Equal($"TestMessage(Id={guid})", result);
        }

        private sealed class TestMessage : Document
        {
            public TestMessage(Guid id, ZonedDateTime timestamp)
                : base(typeof(TestMessage), id, timestamp)
            {
            }
        }
    }
}
