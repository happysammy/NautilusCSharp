//--------------------------------------------------------------------------------------------------
// <copyright file="MessageProcessorTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusMQ.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using Xunit;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class MessageProcessorTests
    {
        [Fact]
        internal void RegisterHandler_WhenHandlerTypeAlreadyRegistered_Throws()
        {
            // Arrange
            var receiver = new List<object>();
            var processor = new MessageProcessor();
            processor.RegisterHandler<string>(receiver.Add);

            // Act

            // Assert
            Assert.Throws<ArgumentException>(() => processor.RegisterHandler<string>(receiver.Add));
        }

        [Fact]
        internal void RegisterHandler_WithValidHandlerMethod_Registers()
        {
            // Arrange
            var receiver = new List<object>();
            var processor = new MessageProcessor();

            // Act
            processor.RegisterHandler<string>(receiver.Add);

            // Assert
            Assert.Contains(typeof(string), processor.HandlerTypes);
        }

        [Fact]
        internal void GivenMessage_WhenNoHandlersRegistered_ThenStoresInUnhandledMessages()
        {
            // Arrange
            var processor = new MessageProcessor();

            // Act
            processor.Endpoint.Send("HI");

            Thread.Sleep(300);

            // Assert
            Assert.Contains("HI", processor.UnhandledMessages);
        }

        [Fact]
        internal void CanReceiveMessage()
        {
            // Arrange
            var testReceiver = new MockMessageReceiver();
            testReceiver.RegisterHandler<string>(testReceiver.OnMessage);

            // Act
            testReceiver.Endpoint.Send("HI");

            Thread.Sleep(300);

            // Assert
            Assert.Contains(typeof(object), testReceiver.HandlerTypes);
            Assert.Contains(typeof(string), testReceiver.HandlerTypes);
            Assert.Contains("HI", testReceiver.Messages);
        }
    }
}
