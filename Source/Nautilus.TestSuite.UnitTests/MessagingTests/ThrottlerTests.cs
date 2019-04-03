//--------------------------------------------------------------------------------------------------
// <copyright file="ThrottlerTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.MessagingTests
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Akka.Actor;
    using Akka.TestKit.Xunit2;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.Messaging;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NodaTime;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class ThrottlerTests : TestKit
    {
        private readonly ITestOutputHelper output;
        private readonly IComponentryContainer setupContainer;
        private readonly MockLoggingAdapter mockLoggingAdapter;
        private readonly IEndpoint testEndpoint;

        public ThrottlerTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var setupFactory = new StubComponentryContainerFactory();
            this.setupContainer = setupFactory.Create();
            this.mockLoggingAdapter = setupFactory.LoggingAdapter;

            this.testEndpoint = new ActorEndpoint(this.TestActor);
        }

        [Fact]
        internal void Test_can_throttle_ten_messages_per_one_hundred_milliseconds()
        {
            // Arrange
            var throttler = this.Sys.ActorOf(Props.Create(() => new Throttler<string>(
                this.setupContainer,
                NautilusService.Execution,
                this.testEndpoint,
                Duration.FromMilliseconds(100),
                10)));

            // Act
            for (var i = 0; i < 21; i++)
            {
                throttler.Tell($"Message-{i + 1}");
            }

            // Assert
            // Receives only the first 10 messages.
            for (var i = 0; i < 10; i++)
            {
                this.ExpectMsg<string>();
            }

            // Wait for the throttle duration interval.
            Task.Delay(100).Wait();

            // Receives the next 10 messages.
            for (var j = 0; j < 10; j++)
            {
                this.ExpectMsg<string>();
            }

            // Receives final message.
            Task.Delay(100).Wait();
            this.ExpectMsg<string>();

            LogDumper.Dump(this.mockLoggingAdapter, this.output);
        }

        [Fact]
        internal void Test_can_throttle_one_hundred_messages_per_one_hundred_milliseconds()
        {
            // Arrange
            var throttler = this.Sys.ActorOf(Props.Create(() => new Throttler<string>(
                this.setupContainer,
                NautilusService.Execution,
                this.testEndpoint,
                Duration.FromMilliseconds(100),
                10)));

            // Act
            for (var i = 0; i < 11; i++)
            {
                throttler.Tell($"Message-{i + 1}");
            }

            // Assert
            // Receives only the first 10 messages.
            for (var i = 0; i < 10; i++)
            {
                this.ExpectMsg<string>();
            }

            // Wait for the throttle duration interval.
            Task.Delay(100).Wait();

            // Receives the next message.
            this.ExpectMsg<string>();

            for (var i = 0; i < 22; i++)
            {
                throttler.Tell($"Message2-{i + 1}");
            }

            // Wait for the throttle duration interval.
            Task.Delay(100).Wait();

            // Receives the next 100 messages.
            for (var j = 0; j < 10; j++)
            {
                this.ExpectMsg<string>();
            }

            LogDumper.Dump(this.mockLoggingAdapter, this.output);
        }
    }
}
