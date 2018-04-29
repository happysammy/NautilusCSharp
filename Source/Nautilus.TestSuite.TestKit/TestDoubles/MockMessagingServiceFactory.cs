//--------------------------------------------------------------
// <copyright file="MockMessagingServiceFactory.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System;
    using System.Collections.Generic;
    using Akka.Actor;
    using Akka.Event;
    using Nautilus.BlackBox.Core;
    using Nautilus.Common.Componentry;
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
            ComponentryContainer container)
        {
            var messageWarhouse = new InMemoryMessageStore();

            var messageStoreRef = actorSystem.ActorOf(Props.Create(() => new MessageStorer(messageWarhouse))); // TODO: make disposable so that test don't break

            var commandBusRef = actorSystem.ActorOf(Props.Create(() => new MessageBus<CommandMessage>(
                ServiceContext.Messaging,
                new Label(BlackBoxService.CommandBus.ToString()),
                container,
                messageStoreRef)));

            var eventBusRef = actorSystem.ActorOf(Props.Create(() => new MessageBus<EventMessage>(
                ServiceContext.Messaging,
                new Label(BlackBoxService.EventBus.ToString()),
                container,
                messageStoreRef)));

            var serviceBusRef = actorSystem.ActorOf(Props.Create(() => new MessageBus<DocumentMessage>(
                ServiceContext.Messaging,
                new Label(BlackBoxService.DocumentBus.ToString()),
                container,
                messageStoreRef)));

            var messagingAdapter = new MessagingAdapter(commandBusRef, eventBusRef, serviceBusRef);

            var addresses = new Dictionary<Enum, IActorRef>
            {
                { BlackBoxService.AlphaModel, new StandardOutLogger() },
                { BlackBoxService.Brokerage, new StandardOutLogger() },
                { BlackBoxService.Data, new StandardOutLogger() },
                { BlackBoxService.Portfolio, new StandardOutLogger() },
                { BlackBoxService.Risk, new StandardOutLogger() },
                { BlackBoxService.Execution, new StandardOutLogger() }
            };

            var switchboard = new Switchboard(addresses);

            var initializeSwitchboard = new InitializeMessageSwitchboard(
                switchboard,
                Guid.NewGuid(),
                container.Clock.TimeNow());

            messagingAdapter.Send(initializeSwitchboard);

            this.MessagingAdapter = messagingAdapter;
            this.InMemoryMessageStore = messageWarhouse;
        }
    }
}
