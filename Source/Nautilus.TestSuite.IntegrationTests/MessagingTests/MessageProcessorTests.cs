//--------------------------------------------------------------------------------------------------
// <copyright file="MessageProcessorTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.IntegrationTests.MessagingTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Nautilus.Messaging.Internal;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;
    using Xunit.Abstractions;

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
        internal void RegisterUnhandled_ThenStoresInReceiver()
        {
            // Arrange
            var receiver = new List<object>();
            var processor = new MessageProcessor();
            processor.RegisterUnhandled(receiver.Add);

            // Act
            processor.Endpoint.Send(1);

            Task.Delay(200).Wait();

            // Assert
            Assert.Contains(1, receiver);
            Assert.DoesNotContain(1, processor.UnhandledMessages);
            Assert.Equal(1, processor.ProcessedCount);
        }

        [Fact]
        internal void RegisterHandleAny_ThenStoresInReceiver()
        {
            // Arrange
            var receiver = new List<object>();
            var processor = new MessageProcessor();
            processor.RegisterHandler<object>(receiver.Add);

            // Act
            processor.Endpoint.Send(1);

            Task.Delay(200).Wait();

            // Assert
            Assert.Contains(1, receiver);
            Assert.DoesNotContain(1, processor.UnhandledMessages);
            Assert.Equal(1, processor.ProcessedCount);
        }

        [Fact]
        internal void GivenMessage_WhenNoHandlersRegistered_ThenStoresInUnhandledMessages()
        {
            // Arrange
            var processor = new MessageProcessor();

            // Act
            processor.Endpoint.Send("test");

            Task.Delay(200).Wait();

            // Assert
            Assert.Contains("test", processor.UnhandledMessages);
            Assert.Equal(0, processor.InputCount);
            Assert.Equal(1, processor.ProcessedCount);
        }

        [Fact]
        internal void GivenMessage_WhenHandlerRegistered_ThenStoresInReceiver()
        {
            // Arrange
            var receiver = new List<object>();
            var processor = new MessageProcessor();
            processor.RegisterHandler<string>(receiver.Add);

            // Act
            processor.Endpoint.Send("test");

            Task.Delay(200).Wait();

            // Assert
            Assert.Single(processor.HandlerTypes);
            Assert.Contains(typeof(string), processor.HandlerTypes);
            Assert.Contains("test", receiver);
            Assert.DoesNotContain("test", processor.UnhandledMessages);
            Assert.Equal(0, processor.InputCount);
            Assert.Equal(1, processor.ProcessedCount);
        }

        [Fact]
        internal void GivenMessagesOfDifferentTypes_WhenHandlersRegistered_ThenStoresInReceiver()
        {
            // Arrange
            var receiver = new MockMessagingAgent();
            receiver.RegisterHandler<string>(receiver.OnMessage);
            receiver.RegisterHandler<int>(receiver.OnMessage);

            // Act
            receiver.Endpoint.Send("test");
            receiver.Endpoint.Send(2);

            Task.Delay(200).Wait();

            // Assert
            Assert.Contains(typeof(string), receiver.HandlerTypes);
            Assert.Contains(typeof(int), receiver.HandlerTypes);
            Assert.True(receiver.Messages[0].Equals("test"));
            Assert.True(receiver.Messages[1].Equals(2));
            Assert.Equal(0, receiver.InputCount);
            Assert.Equal(2, receiver.ProcessedCount);
            Assert.Equal(2, receiver.Messages.Count);
        }

        [Fact]
        internal void GivenManyMessagesOfDifferentTypes_WhenHandlersRegistered_ThenStoresInReceiver()
        {
            // Arrange
            var receiver = new MockMessagingAgent();
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

            Task.Delay(200).Wait();

            Assert.True(receiver.Messages[0].Equals("1"));
            Assert.True(receiver.Messages[1].Equals(1));
            Assert.True(receiver.Messages[2].Equals("2"));
            Assert.True(receiver.Messages[3].Equals(2));
            Assert.True(receiver.Messages[4].Equals("3"));
            Assert.True(receiver.Messages[5].Equals(3));
            Assert.True(receiver.Messages[6].Equals("4"));
            Assert.True(receiver.Messages[7].Equals(4));
            Assert.Equal(0, receiver.InputCount);
            Assert.Equal(8, receiver.ProcessedCount);
            Assert.Equal(8, receiver.Messages.Count);
        }

        [Fact]
        internal void GivenMessagesOfDifferentTypes_WithWorkDelay_ProcessesSynchronously()
        {
            // Arrange
            var receiver = new MockMessagingAgent();
            receiver.RegisterHandler<string>(receiver.OnMessageWithWorkDelay);
            receiver.RegisterHandler<int>(receiver.OnMessage);

            // Act
            receiver.Endpoint.Send("test");
            receiver.Endpoint.Send(2);

            Task.Delay(200).Wait();

            // Assert
            Assert.Contains("test", receiver.Messages);
            Assert.Single(receiver.Messages);
            Assert.Equal(1, receiver.InputCount);
            Assert.Equal(1, receiver.ProcessedCount);
        }

        [Fact]
        internal void GivenManyMessages_WithWorkDelay_ProcessesSynchronously()
        {
            // Arrange
            var receiver = new MockMessagingAgent();
            receiver.RegisterHandler<string>(receiver.OnMessageWithWorkDelay);

            // Act
            receiver.Endpoint.Send("1");
            receiver.Endpoint.Send("2");
            receiver.Endpoint.Send("3");
            receiver.Endpoint.Send("4");

            Task.Delay(200).Wait();

            // Assert
            Assert.Contains("1", receiver.Messages);
            Assert.Single(receiver.Messages);
            Assert.Equal(3, receiver.InputCount);
            Assert.Equal(1, receiver.ProcessedCount);
        }

        [Fact]
        internal void MessagingPerformanceTest()
        {
            // Arrange
            var receiver = new MockMessagingAgent();
            receiver.RegisterHandler<int>(receiver.OnMessage);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // Act
            for (var i = 0; i < 1000000; i++)
            {
                receiver.Endpoint.Send(i);
                receiver.Endpoint.Send(i);
            }

            while (receiver.ProcessedCount < 2000000)
            {
                // Wait
            }

            stopwatch.Stop();

            this.output.WriteLine(stopwatch.ElapsedMilliseconds.ToString());
            this.output.WriteLine(receiver.Messages.Count.ToString());

            // ~1350 ms for 1000000 messages with Task<bool> being returned by handler
            // ~1250 ms for 1000000 messages with bool being returned by handler

            // Assert
            Assert.Equal(0, receiver.InputCount);
            Assert.Equal(2000000, receiver.ProcessedCount);
        }

        // Test function
        private static void ThisWillBlowUp(string input)
        {
            throw new InvalidOperationException(input);
        }
    }
}
