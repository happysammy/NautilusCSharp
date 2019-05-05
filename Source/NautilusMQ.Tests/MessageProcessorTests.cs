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
    using Xunit.Abstractions;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class MessageProcessorTests
    {
        private readonly ITestOutputHelper output;

        public MessageProcessorTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;
        }

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
            processor.Endpoint.Send("test");

            Thread.Sleep(300);

            // Assert
            Assert.Contains("test", processor.UnhandledMessages);
        }

        [Fact]
        internal void RegisterUnhandled_ThenStoresInNewUnhandledMessages()
        {
            // Arrange
            var receiver = new List<object>();
            var processor = new MessageProcessor();
            processor.RegisterUnhandled(receiver.Add);

            // Act
            processor.Endpoint.Send(1);

            Thread.Sleep(100);

            // Assert
            Assert.Contains(1, receiver);
            Assert.DoesNotContain(1, processor.UnhandledMessages);
        }

        [Fact]
        internal void CanReceiveSingleMessage()
        {
            // Arrange
            var receiver = new List<object>();
            var processor = new MessageProcessor();
            processor.RegisterHandler<string>(receiver.Add);

            // Act
            processor.Endpoint.Send("test");

            Thread.Sleep(100);

            // Assert
            Assert.Contains(typeof(string), processor.HandlerTypes);
            Assert.Contains("test", receiver);
            Assert.DoesNotContain("test", processor.UnhandledMessages);
        }

        [Fact]
        internal void CanReceiveDifferentMessageTypes()
        {
            // Arrange
            var receiver = new MockMessageReceiver();
            receiver.RegisterHandler<string>(receiver.OnMessage);
            receiver.RegisterHandler<int>(receiver.OnMessage);

            // Act
            receiver.Endpoint.Send("test");
            receiver.Endpoint.Send(2);

            Thread.Sleep(100);

            // Assert
            Assert.Contains(typeof(string), receiver.HandlerTypes);
            Assert.Contains(typeof(int), receiver.HandlerTypes);
            Assert.True(receiver.Messages[0].Equals("test"));
            Assert.True(receiver.Messages[1].Equals(2));
        }

        [Fact]
        internal void GivenManyMessages_WhenDifferentTypes_ReceivesInCorrectOrder()
        {
            // Arrange
            var receiver = new MockMessageReceiver();
            receiver.RegisterHandler<string>(receiver.OnMessage);
            receiver.RegisterHandler<int>(receiver.OnMessage);

            // Act
            receiver.Endpoint.Send("1");
            receiver.Endpoint.Send(1);
            receiver.Endpoint.Send("2");
            receiver.Endpoint.Send(2);
            receiver.Endpoint.Send("3");
            receiver.Endpoint.Send(3);
            receiver.Endpoint.Send("4");
            receiver.Endpoint.Send(4);

            Thread.Sleep(100);

            Assert.True(receiver.Messages[0].Equals("1"));
            Assert.True(receiver.Messages[1].Equals(1));
            Assert.True(receiver.Messages[2].Equals("2"));
            Assert.True(receiver.Messages[3].Equals(2));
            Assert.True(receiver.Messages[4].Equals("3"));
            Assert.True(receiver.Messages[5].Equals(3));
            Assert.True(receiver.Messages[6].Equals("4"));
            Assert.True(receiver.Messages[7].Equals(4));
        }
    }
}
