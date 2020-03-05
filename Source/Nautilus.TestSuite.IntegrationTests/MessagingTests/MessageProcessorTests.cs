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
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Common.Interfaces;
    using Nautilus.Messaging;
    using Nautilus.TestSuite.TestKit.Components;
    using Nautilus.TestSuite.TestKit.Fixtures;
    using Nautilus.TestSuite.TestKit.Mocks;
    using Nautilus.TestSuite.TestKit.Performance;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class MessageProcessorTests : TestBase
    {
        private readonly IComponentryContainer container;

        public MessageProcessorTests(ITestOutputHelper output)
            : base(output)
        {
            // Fixture Setup
            this.container = TestComponentryContainer.Create(this.Output);
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
            processor.Endpoint.SendAsync(1).Wait();
            processor.GracefulStop().Wait();

            // Assert
            Assert.Contains(1, receiver);
            Assert.DoesNotContain(1, processor.UnhandledMessages);
            Assert.Equal(1, processor.CountProcessed);
        }

        [Fact]
        internal void RegisterHandleAny_ThenStoresInReceiver()
        {
            // Arrange
            var receiver = new List<object>();
            var processor = new MessageProcessor();
            processor.RegisterHandler<object>(receiver.Add);

            // Act
            processor.Endpoint.SendAsync(1).Wait();
            processor.GracefulStop().Wait();

            // Assert
            Assert.Contains(1, receiver);
            Assert.DoesNotContain(1, processor.UnhandledMessages);
            Assert.Equal(1, processor.CountProcessed);
        }

        [Fact]
        internal void GivenMessage_WithExplodingHandler_PropagatesException()
        {
            // Arrange
            var processor = new MessageProcessor();
            processor.RegisterHandler<string>(ThisWillBlowUp);

            // Act
            processor.Endpoint.SendAsync("BOOM!").Wait();
            processor.GracefulStop().Wait();

            // Assert
            Assert.Single(processor.Exceptions);
            Assert.Contains("BOOM!", processor.UnhandledMessages);
            Assert.Equal(0, processor.CountInput);
            Assert.Equal(1, processor.CountProcessed);
        }

        [Fact]
        internal void GivenMessage_WhenNoHandlersRegistered_ThenStoresInUnhandledMessages()
        {
            // Arrange
            var processor = new MessageProcessor();

            // Act
            processor.Endpoint.SendAsync("test").Wait();
            processor.GracefulStop().Wait();

            // Assert
            Assert.Contains("test", processor.UnhandledMessages);
            Assert.Equal(0, processor.CountInput);
            Assert.Equal(1, processor.CountProcessed);
        }

        [Fact]
        internal void GivenMessage_WhenHandlerRegistered_ThenStoresInReceiver()
        {
            // Arrange
            var receiver = new List<object>();
            var processor = new MessageProcessor();
            processor.RegisterHandler<string>(receiver.Add);

            // Act
            processor.Endpoint.SendAsync("test").Wait();
            processor.GracefulStop().Wait();

            // Assert
            Assert.Single(processor.HandlerTypes);
            Assert.Contains(typeof(string), processor.HandlerTypes);
            Assert.Contains("test", receiver);
            Assert.DoesNotContain("test", processor.UnhandledMessages);
            Assert.Equal(0, processor.CountInput);
            Assert.Equal(1, processor.CountProcessed);
        }

        [Fact]
        internal void GivenMessagesOfDifferentTypes_WhenHandlersRegistered_ThenStoresInReceiver()
        {
            // Arrange
            var receiver = new MockComponent(this.container);
            receiver.RegisterHandler<string>(receiver.OnMessage);
            receiver.RegisterHandler<int>(receiver.OnMessage);

            // Act
            receiver.Endpoint.SendAsync("test");
            receiver.Endpoint.SendAsync(2).Wait();
            receiver.Stop().Wait();

            // Assert
            Assert.Contains(typeof(string), receiver.HandlerTypes);
            Assert.Contains(typeof(int), receiver.HandlerTypes);
            Assert.True(receiver.Messages[0].Equals("test"));
            Assert.True(receiver.Messages[1].Equals(2));
            Assert.Equal(0, receiver.InputCount);
            Assert.Equal(3, receiver.ProcessedCount);
            Assert.Equal(2, receiver.Messages.Count);
        }

        [Fact]
        internal void GivenManyMessagesOfDifferentTypes_WhenHandlersRegistered_ThenStoresInReceiver()
        {
            // Arrange
            var receiver = new MockComponent(this.container);
            receiver.RegisterHandler<string>(receiver.OnMessage);
            receiver.RegisterHandler<int>(receiver.OnMessage);

            // Act
            receiver.Endpoint.SendAsync("1");
            receiver.Endpoint.SendAsync(1);
            receiver.Endpoint.SendAsync("2");
            receiver.Endpoint.SendAsync(2);
            receiver.Endpoint.SendAsync("3");
            receiver.Endpoint.SendAsync(3);
            receiver.Endpoint.SendAsync("4");
            receiver.Endpoint.SendAsync(4).Wait();
            receiver.Stop().Wait();

            Assert.True(receiver.Messages[0].Equals("1"));
            Assert.True(receiver.Messages[1].Equals(1));
            Assert.True(receiver.Messages[2].Equals("2"));
            Assert.True(receiver.Messages[3].Equals(2));
            Assert.True(receiver.Messages[4].Equals("3"));
            Assert.True(receiver.Messages[5].Equals(3));
            Assert.True(receiver.Messages[6].Equals("4"));
            Assert.True(receiver.Messages[7].Equals(4));
            Assert.Equal(0, receiver.InputCount);
            Assert.Equal(9, receiver.ProcessedCount);
            Assert.Equal(8, receiver.Messages.Count);
        }

        [Fact]
        [SuppressMessage("ReSharper", "SA1116", Justification = "Easier to read")]
        internal void MessagingPerformanceTest()
        {
            // Arrange
            var receiver = new MockComponent(this.container);
            receiver.RegisterHandler<int>(receiver.OnMessage);

            PerformanceHarness.Test(() =>
            {
                for (var i = 0; i < 1000000; i++)
                {
                    receiver.Endpoint.SendAsync(i);
                    receiver.Endpoint.SendAsync(i);
                }

                while (receiver.ProcessedCount < 2000000)
                {
                    // Wait
                }
            },
                1,
                1,
                this.Output);

            this.Output.WriteLine(receiver.Messages.Count.ToString());

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
