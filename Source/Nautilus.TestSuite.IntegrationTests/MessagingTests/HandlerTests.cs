//--------------------------------------------------------------------------------------------------
// <copyright file="HandlerTests.cs" company="Nautech Systems Pty Ltd">
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

namespace Nautilus.TestSuite.IntegrationTests.MessagingTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Messaging.Internal;
    using Nautilus.TestSuite.TestKit.Fixtures;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class HandlerTests : TestBase
    {
        public HandlerTests(ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact]
        internal async System.Threading.Tasks.Task Handle_WithExplodingFuncDelegate_PropagatesThrownException()
        {
            // Arrange
            // Act
            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => Handler.Create<string>(ThisWillBlowUp).Handle("BOOM!"));
        }

        [Fact]
        internal void Handle_WhenCorrectMessageReferenceType_ReturnsTrue()
        {
            // Arrange
            var receiver = new List<string>();
            var handler = Handler.Create<string>(receiver.Add);

            // Act
            var result = handler.Handle("test");

            // Assert
            Assert.Equal(typeof(string), handler.Type);
            Assert.True(result.IsCompletedSuccessfully);
            Assert.Contains("test", receiver);
        }

        [Fact]
        internal void Handle_WhenCorrectMessageValueType_ReturnsTrue()
        {
            // Arrange
            var receiver = new List<int>();
            var handler = Handler.Create<int>(receiver.Add);

            // Act
            var result = handler.Handle(1);

            // Assert
            Assert.Equal(typeof(int), handler.Type);
            Assert.True(result.IsCompletedSuccessfully);
            Assert.Contains(1, receiver);
        }

        [Fact]
        internal void Handle_WhenIncorrectMessageType_ReturnsFalse()
        {
            // Arrange
            var receiver = new List<int>();
            var handler = Handler.Create<int>(receiver.Add);

            // Act
            var result = handler.Handle("not an Int32");

            // Assert
            Assert.Equal(typeof(int), handler.Type);
            Assert.True(result.IsCompleted);
        }

        // Test function
        private static void ThisWillBlowUp(string input)
        {
            throw new InvalidOperationException(input);
        }
    }
}
