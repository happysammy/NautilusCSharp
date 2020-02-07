//--------------------------------------------------------------------------------------------------
// <copyright file="MessageTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.CoreTests.TypesTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;

    // Required for equality testing
    #pragma warning disable 8602
    #pragma warning disable 8604
    #pragma warning disable 8625
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public sealed class MessageTests
    {
        [Fact]
        internal void Equal_WithDifferentMessagesOfTheSameContent_CanEquateById()
        {
            // Arrange
            var message1 = new Start(Guid.NewGuid(), StubZonedDateTime.UnixEpoch());
            var message2 = new Start(Guid.NewGuid(), StubZonedDateTime.UnixEpoch());

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
            var message = new Start(Guid.NewGuid(), StubZonedDateTime.UnixEpoch());

            // Act
            // Assert
            Assert.IsType<int>(message.GetHashCode());
        }

        [Fact]
        internal void ToString_ReturnsExpectedString()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var message = new Start(guid, StubZonedDateTime.UnixEpoch());

            // Act
            var result = message.ToString();

            // Assert
            Assert.Equal($"Start(Id={guid})", result);
        }
    }
}
