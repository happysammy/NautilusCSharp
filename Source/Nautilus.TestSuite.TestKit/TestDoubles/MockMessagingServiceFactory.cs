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
    using Nautilus.Common.Commands;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.MessageStore;
    using Nautilus.Common.Messaging;
    using Nautilus.DomainModel.ValueObjects;

    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Reviewed. Suppression is OK within the Test Suite.")]
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
                actorSystem.ActorOf(Props.Create(() => new MessageBus<CommandMessage>(
                new Label(MessagingComponent.CommandBus.ToString()),
                container,
                messageStore))));

            var eventBus = new ActorEndpoint(
                actorSystem.ActorOf(Props.Create(() => new MessageBus<EventMessage>(
                new Label(MessagingComponent.EventBus.ToString()),
                container,
                messageStore))));

            var serviceBus = new ActorEndpoint(
                actorSystem.ActorOf(Props.Create(() => new MessageBus<DocumentMessage>(
                new Label(MessagingComponent.DocumentBus.ToString()),
                container,
                messageStore))));

            var messagingAdapter = new MessagingAdapter(commandBus, eventBus, serviceBus);

            var mockEndpoint = new ActorEndpoint(new StandardOutLogger());
            var addresses = new Dictionary<NautilusService, IEndpoint>
            {
                { NautilusService.AlphaModel, mockEndpoint },
                { NautilusService.Brokerage, mockEndpoint },
                { NautilusService.Data, mockEndpoint },
                { NautilusService.Portfolio, mockEndpoint },
                { NautilusService.Risk, mockEndpoint },
                { NautilusService.Execution, mockEndpoint },
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
