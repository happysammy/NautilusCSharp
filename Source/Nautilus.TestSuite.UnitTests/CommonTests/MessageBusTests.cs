//--------------------------------------------------------------------------------------------------
// <copyright file="MessageBusTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.CommonTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Akka.Actor;
    using Akka.Event;
    using Nautilus.BlackBox.Core;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Messaging;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.Extensions;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class MessageBusTests
    {
        private readonly ITestOutputHelper output;
        private readonly MockLoggingAdatper mockLoggingAdatper;
        private readonly IActorRef messageBusRef;

        public MessageBusTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var setupFactory = new StubSetupContainerFactory();
            var setupContainer = setupFactory.Create();
            this.mockLoggingAdatper = setupFactory.LoggingAdatper;

            var testActorSystem = ActorSystem.Create(nameof(MessageBusTests));

            this.messageBusRef = testActorSystem.ActorOf(Props.Create(() => new MessageBus<CommandMessage>(
                ServiceContext.Messaging,
                new Label(ServiceContext.CommandBus.ToString()),
                setupContainer,
                new StandardOutLogger())));

            var mockAlphaModelServiceRef = testActorSystem.ActorOf(Props.Create(() => new TestActor()));
            var mockDataServiceRef = testActorSystem.ActorOf(Props.Create(() => new TestActor()));
            var mockExecutionServiceRef = testActorSystem.ActorOf(Props.Create(() => new TestActor()));
            var mockPortfolioServiceRef = testActorSystem.ActorOf(Props.Create(() => new TestActor()));
            var mockRiskServiceRef = testActorSystem.ActorOf(Props.Create(() => new TestActor()));

            var addresses = new Dictionary<Enum, IActorRef>
            {
                { BlackBoxService.AlphaModel, mockAlphaModelServiceRef },
                { BlackBoxService.Data, mockDataServiceRef },
                { BlackBoxService.Execution, mockExecutionServiceRef },
                { BlackBoxService.Portfolio, mockPortfolioServiceRef },
                { BlackBoxService.Risk, mockRiskServiceRef }
            };

            this.messageBusRef.Tell(new InitializeMessageSwitchboard(
                new Switchboard(addresses),
                Guid.NewGuid(),
                setupContainer.Clock.TimeNow()));
        }

        [Fact]
        internal void GivenNullObjectMessage_Handles()
        {
            // Arrange

            // Act
            this.messageBusRef.Tell(string.Empty);

            // Assert
            LogDumper.Dump(this.mockLoggingAdatper, this.output);

            CustomAssert.EventuallyContains(
                "CommandBus: Unhandled message",
                this.mockLoggingAdatper,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);
        }
    }
}
