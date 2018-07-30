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
            var messageStoreRef = actorSystem.ActorOf(Props.Create(() => new MessageStorer(messageWarehouse))); // TODO: make disposable so that test don't break

            var commandBusRef = actorSystem.ActorOf(Props.Create(() => new MessageBus<CommandMessage>(
                new Label(MessagingComponent.CommandBus.ToString()),
                container,
                messageStoreRef)));

            var eventBusRef = actorSystem.ActorOf(Props.Create(() => new MessageBus<EventMessage>(
                new Label(MessagingComponent.EventBus.ToString()),
                container,
                messageStoreRef)));

            var serviceBusRef = actorSystem.ActorOf(Props.Create(() => new MessageBus<DocumentMessage>(
                new Label(MessagingComponent.DocumentBus.ToString()),
                container,
                messageStoreRef)));

            var messagingAdapter = new MessagingAdapter(commandBusRef, eventBusRef, serviceBusRef);

            var addresses = new Dictionary<Enum, IActorRef>
            {
                { NautilusService.AlphaModel, new StandardOutLogger() },
                { NautilusService.Brokerage, new StandardOutLogger() },
                { NautilusService.Data, new StandardOutLogger() },
                { NautilusService.Portfolio, new StandardOutLogger() },
                { NautilusService.Risk, new StandardOutLogger() },
                { NautilusService.Execution, new StandardOutLogger() }
            };

            var switchboard = new Switchboard(addresses);
            var initializeSwitchboard = new InitializeMessageSwitchboard(
                switchboard,
                Guid.NewGuid(),
                container.Clock.TimeNow());

            messagingAdapter.Send(initializeSwitchboard);

            this.MessagingAdapter = messagingAdapter;
            this.InMemoryMessageStore = messageWarehouse;
        }
    }
}
