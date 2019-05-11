//--------------------------------------------------------------------------------------------------
// <copyright file="MessageBusTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.CommonTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Common.MessageStore;
    using Nautilus.Common.Messaging;
    using Nautilus.Core;
    using Nautilus.Messaging;
    using Nautilus.Messaging.Interfaces;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class MessageBusTests
    {
        private readonly ITestOutputHelper output;
        private readonly MockLoggingAdapter mockLoggingAdapter;
        private readonly IEndpoint messageBus;

        public MessageBusTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var containerFactory = new StubComponentryContainerFactory();
            var container = containerFactory.Create();
            var messageWarehouse = new InMemoryMessageStore();
            var messageStorer = new MessageStorer(container, messageWarehouse);
            this.mockLoggingAdapter = containerFactory.LoggingAdapter;
            this.messageBus = new MessageBus<Command>(container, messageStorer.Endpoint).Endpoint;

            var addresses = new Dictionary<Address, IEndpoint>
            {
                { ServiceAddress.Alpha, messageStorer.Endpoint },
                { ServiceAddress.Data, messageStorer.Endpoint },
                { ServiceAddress.Execution, messageStorer.Endpoint },
                { ServiceAddress.Portfolio, messageStorer.Endpoint },
                { ServiceAddress.Risk, messageStorer.Endpoint },
            };

            this.messageBus.Send(new InitializeSwitchboard(
                Switchboard.Create(addresses),
                Guid.NewGuid(),
                containerFactory.Clock.TimeNow()));
        }

        [Fact]
        internal void GivenEmptyStringMessage_Handles()
        {
            // Arrange

            // Act
            this.messageBus.Send(string.Empty);

            // Assert
            // Assert something...
        }
    }
}
