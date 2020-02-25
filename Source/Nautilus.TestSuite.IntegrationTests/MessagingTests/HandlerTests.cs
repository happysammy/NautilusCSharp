//--------------------------------------------------------------------------------------------------
// <copyright file="HandlerTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
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
            var handler = Handler.Create<string>(ThisWillBlowUp);

            // Act
            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle("BOOM!"));
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
