//--------------------------------------------------------------------------------------------------
// <copyright file="ThrottlerTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.NetworkTests
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Nautilus.Common.Enums;
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
        private readonly IComponentryContainer setupContainer;
        private readonly MockLoggingAdapter mockLoggingAdapter;
        private readonly MockMessagingAgent testReceiver;

        public ThrottlerTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var setupFactory = new StubComponentryContainerFactory();
            this.setupContainer = setupFactory.Create();
            this.mockLoggingAdapter = setupFactory.LoggingAdapter;
            this.testReceiver = new MockMessagingAgent();
            this.testReceiver.RegisterHandler<string>(this.testReceiver.OnMessage);
        }

        [Fact]
        internal void Test_can_throttle_ten_messages_per_one_hundred_milliseconds()
        {
            // Arrange
            var throttler = new Throttler<string>(
                this.setupContainer,
                NautilusService.Execution,
                this.testReceiver.Endpoint,
                Duration.FromMilliseconds(100),
                10);

            // Act
            for (var i = 0; i < 21; i++)
            {
                throttler.Endpoint.Send($"Message-{i + 1}");
            }

            Task.Delay(50).Wait();

            // Should receives only the first 10 messages.
            var count1 = this.testReceiver.Messages.Count;

            // Wait for the throttle duration interval.
            Task.Delay(100).Wait();

            // Should receive the next 10 messages.
            var count2 = this.testReceiver.Messages.Count;

            // Should receive the final message.
            Task.Delay(100).Wait();
            var count3 = this.testReceiver.Messages.Count;

            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
            Assert.Equal(10, count1);
            Assert.Equal(20, count2);
            Assert.Equal(21, count3);
            Assert.Equal(0, throttler.QueueCount);
            Assert.True(throttler.IsIdle);
        }

        [Fact]
        internal void Test_can_throttle_messages_per_one_hundred_milliseconds()
        {
            // Arrange
            var throttler = new Throttler<string>(
                this.setupContainer,
                NautilusService.Execution,
                this.testReceiver.Endpoint,
                Duration.FromMilliseconds(100),
                10);

            // Act
            for (var i = 0; i < 11; i++)
            {
                throttler.Endpoint.Send($"Message-{i + 1}");
            }

            Task.Delay(50).Wait();

            // Should receive only the first 10 messages.
            var count1 = this.testReceiver.Messages.Count;

            // Wait for the throttle duration interval.
            Task.Delay(100).Wait();

            // Should receive the next message
            var count2 = this.testReceiver.Messages.Count;

            for (var i = 0; i < 100; i++)
            {
                throttler.Endpoint.Send($"Message2-{i + 1}");
            }

            // Wait for the throttle duration interval.
            Task.Delay(100).Wait();

            // Receives the next 100 messages.
            var count3 = this.testReceiver.Messages.Count;

            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
            Assert.Equal(10, count1);
            Assert.Equal(11, count2);
            Assert.Equal(30, count3);
            Assert.Equal(51, throttler.QueueCount);
            Assert.False(throttler.IsIdle);
        }
    }
}
