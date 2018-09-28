//--------------------------------------------------------------------------------------------------
// <copyright file="MockMessagingServiceFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Akka.Actor;
    using Akka.Event;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Common.MessageStore;
    using Nautilus.Common.Messaging;
    using Nautilus.Core;
    using Address = Nautilus.Common.Messaging.Address;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class MockMessagingServiceFactory
    {
        public IMessagingAdapter MessagingAdapter { get; private set; }

        public InMemoryMessageStore InMemoryMessageStore { get; private set; }

        public void Create(
            ActorSystem actorSystem,
            IComponentryContainer container)
        {
            var messageWarehouse = new InMemoryMessageStore();
            var messageStore = new ActorEndpoint(
                actorSystem.ActorOf(Props.Create(() => new MessageStorer(messageWarehouse))));

            var commandBus = new ActorEndpoint(
                actorSystem.ActorOf(Props.Create(() => new MessageBus<Command>(
                container,
                messageStore))));

            var eventBus = new ActorEndpoint(
                actorSystem.ActorOf(Props.Create(() => new MessageBus<Event>(
                container,
                messageStore))));

            var serviceBus = new ActorEndpoint(
                actorSystem.ActorOf(Props.Create(() => new MessageBus<Document>(
                container,
                messageStore))));

            var messagingAdapter = new MessagingAdapter(commandBus, eventBus, serviceBus);

            var mockEndpoint = new ActorEndpoint(new StandardOutLogger());
            var addresses = new Dictionary<Address, IEndpoint>
            {
                { ServiceAddress.Alpha, mockEndpoint },
                { ServiceAddress.Data, mockEndpoint },
                { ServiceAddress.Portfolio, mockEndpoint },
                { ServiceAddress.Risk, mockEndpoint },
                { ServiceAddress.Execution, mockEndpoint },
            };

            var switchboard = new Switchboard(addresses);
            var initializeSwitchboard = new InitializeSwitchboard(
                switchboard,
                Guid.NewGuid(),
                container.Clock.TimeNow());

            messagingAdapter.Send(initializeSwitchboard);

            this.MessagingAdapter = messagingAdapter;
            this.InMemoryMessageStore = messageWarehouse;
        }
    }
}
