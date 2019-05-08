//--------------------------------------------------------------------------------------------------
// <copyright file="HandlerTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.MessagingTests
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Messaging.Internal;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class HandlerTests
    {
        private readonly ITestOutputHelper output;

        public HandlerTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;
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
            Assert.True(result);
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
            Assert.True(result);
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
            Assert.False(result);
        }
    }
}
