//--------------------------------------------------------------------------------------------------
// <copyright file="MockMessagingServiceFactory.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System;
    using System.Collections.Generic;
    using Akka.Actor;
    using Akka.Event;
    using Nautilus.Common.Commands;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.MessageStore;
    using Nautilus.Common.Messaging;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The mock messaging service factory.
    /// </summary>
    public class MockMessagingServiceFactory
    {
        /// <summary>
        /// Gets the messaging adapter.
        /// </summary>
        public IMessagingAdapter MessagingAdapter { get; private set; }

        /// <summary>
        /// Gets the message warehouse.
        /// </summary>
        public InMemoryMessageStore InMemoryMessageStore { get; private set; }

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="actorSystem">
        /// The actor system.
        /// </param>
        /// <param name="container">
        /// The container.
        /// </param>
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
                { NautilusService.Execution, mockEndpoint }
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
