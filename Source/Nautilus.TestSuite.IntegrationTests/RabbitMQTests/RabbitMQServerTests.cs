// -------------------------------------------------------------------------------------------------
// <copyright file="RabbitMQServerTests.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.IntegrationTests.RabbitMQTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Akka.TestKit.Xunit2;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.MsgPack;
    using Nautilus.RabbitMQ;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsShouldBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class RabbitMQServerTests : TestKit
    {
        private readonly ITestOutputHelper output;
        private readonly MockLoggingAdapter logger;
        private readonly IEndpoint serverRef;

        public RabbitMQServerTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var setupFactory = new StubSetupContainerFactory();
            var setupContainer = setupFactory.Create();
            this.logger = setupFactory.LoggingAdapter;

            var messagingFactory = new MockMessagingServiceFactory();
            messagingFactory.Create(this.Sys, setupContainer);

            var messagingAdapter = messagingFactory.MessagingAdapter;

            this.serverRef = RabbitMQServerFactory.Create(
                this.Sys,
                setupContainer,
                messagingAdapter,
                new MsgPackCommandSerializer(),
                new MsgPackEventSerializer());
        }

        [Fact]
        internal void Test_can_publish_message_on_order_event_exchange()
        {
            // Arrange
            var orderSubmitted = new OrderSubmitted(
                new Symbol("AUDUSD", Venue.FXCM),
                new OrderId("O123456"),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            Task.Delay(300).Wait();
            this.serverRef.Send(orderSubmitted);

            // Assert
            LogDumper.Dump(this.logger, this.output);
        }
    }
}
