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

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
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

            // Assert
            // Receives only the first 10 messages.
            for (var i = 0; i < 10; i++)
            {
                var x1 = this.testReceiver.Messages;
            }

            // Wait for the throttle duration interval.
            Task.Delay(100).Wait();

            // Receives the next 10 messages.
            for (var j = 0; j < 10; j++)
            {
                var x2 = this.testReceiver.Messages;
            }

            // Receives final message.
            Task.Delay(100).Wait();
            var x3 = this.testReceiver.Messages;

            LogDumper.Dump(this.mockLoggingAdapter, this.output);
        }

        [Fact]
        internal void Test_can_throttle_one_hundred_messages_per_one_hundred_milliseconds()
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

            // Assert
            // Receives only the first 10 messages.
            for (var i = 0; i < 10; i++)
            {
                var x1 = this.testReceiver.Messages;
            }

            // Wait for the throttle duration interval.
            Task.Delay(100).Wait();

            // Receives the next message.

            // this.ExpectMsg<string>();
            for (var i = 0; i < 22; i++)
            {
                throttler.Endpoint.Send($"Message2-{i + 1}");
            }

            // Wait for the throttle duration interval.
            Task.Delay(100).Wait();

            // Receives the next 100 messages.
            for (var j = 0; j < 10; j++)
            {
                var x = this.testReceiver.Messages;
            }

            LogDumper.Dump(this.mockLoggingAdapter, this.output);
        }
    }
}
