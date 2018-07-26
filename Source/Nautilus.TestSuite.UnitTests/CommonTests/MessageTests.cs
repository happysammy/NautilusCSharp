//--------------------------------------------------------------------------------------------------
// <copyright file="MessageTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.CommonTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Messages;
    using Nautilus.DomainModel.Factories;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class MessageTests
    {
        [Fact]
        internal void Equal_WithDifferentMessagesOfTheSameContent_CanEquateById()
        {
            // Arrange
            var message1 = new SystemStatusResponse(
                LabelFactory.Component(Messaging.CommandBus.ToString()),
                ComponentStatus.OK,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var message2 = new SystemStatusResponse(
                LabelFactory.Component(Messaging.CommandBus.ToString()),
                ComponentStatus.OK,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var result1 = message1 == message2;
            var result2 = message1.Equals(message2);
            // ReSharper disable once EqualExpressionComparison
            var result3 = message1 == message1;
            var result4 = message1.Equals(message1);

            // Assert
            Assert.False(result1);
            Assert.False(result2);
            Assert.True(result3);
            Assert.True(result4);
        }

        [Fact]
        internal void ToString_ReturnsExpectedString()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var message = new SystemStatusRequest(guid, StubZonedDateTime.UnixEpoch());

            // Act
            var result = message.ToString();

            // Assert
            Assert.Equal($"{nameof(SystemStatusRequest)}", result);
        }

        [Fact]
        internal void ToString_WhenOverridden_ReturnsExpectedString()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var message = new SystemStatusResponse(
                LabelFactory.Component(Messaging.CommandBus.ToString()),
                ComponentStatus.OK,
                guid,
                StubZonedDateTime.UnixEpoch());

            // Act
            var result = message.ToString();

            // Assert
            Assert.Equal($"{nameof(SystemStatusResponse)}-CommandBus=OK", result);
        }
    }
}
