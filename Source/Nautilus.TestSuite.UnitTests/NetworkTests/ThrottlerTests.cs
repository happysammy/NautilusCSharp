//--------------------------------------------------------------------------------------------------
// <copyright file="ThrottlerTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.NetworkTests
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Nautilus.Common.Interfaces;
    using Nautilus.Network;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NodaTime;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class ThrottlerTests
    {
        private readonly ITestOutputHelper output;
        private readonly IComponentryContainer container;
        private readonly MockLoggingAdapter loggingAdapter;
        private readonly MockMessagingAgent receiver;

        public ThrottlerTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var containerFactory = new StubComponentryContainerProvider();
            this.container = containerFactory.Create();
            this.loggingAdapter = containerFactory.LoggingAdapter;
            this.receiver = new MockMessagingAgent();
            this.receiver.RegisterHandler<string>(this.receiver.OnMessage);
        }

        [Fact]
        internal void CanThrottleMessagesPer100Milliseconds_Test1()
        {
            // Arrange
            var throttler = new Throttler(
                this.container,
                this.receiver.Endpoint,
                Duration.FromMilliseconds(100),
                10);

            // Act
            for (var i = 0; i < 21; i++)
            {
                throttler.Endpoint.Send($"Message-{i + 1}");
            }

            Task.Delay(50).Wait();

            // Should receives only the first 10 messages
            var count1 = this.receiver.Messages.Count;

            // Wait for the throttle duration interval
            Task.Delay(100).Wait();

            // Should receive the next 10 messages
            var count2 = this.receiver.Messages.Count;

            // Should receive the final message
            Task.Delay(100).Wait();
            var count3 = this.receiver.Messages.Count;

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Equal(10, count1);
            Assert.Equal(20, count2);
            Assert.Equal(21, count3);
            Assert.Equal(0, throttler.QueueCount);
            Assert.True(throttler.IsIdle);
        }

        [Fact]
        internal void CanThrottleMessagesPer100Milliseconds_Test2()
        {
            // Arrange
            var throttler = new Throttler(
                this.container,
                this.receiver.Endpoint,
                Duration.FromMilliseconds(100),
                10);

            // Act
            for (var i = 0; i < 11; i++)
            {
                throttler.Endpoint.Send($"Message-{i + 1}");
            }

            Task.Delay(50).Wait();

            // Should receive only the first 10 messages
            var count1 = this.receiver.Messages.Count;

            for (var i = 0; i < 20; i++)
            {
                throttler.Endpoint.Send($"Message2-{i + 1}");
            }

            // Wait for all messages to send
            Task.Delay(400).Wait();

            // Receives the next 100 messages
            var count2 = this.receiver.Messages.Count;

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Equal(10, count1);
            Assert.Equal(31, count2);
            Assert.Equal(0, throttler.QueueCount);
            Assert.True(throttler.IsIdle);
        }
    }
}
