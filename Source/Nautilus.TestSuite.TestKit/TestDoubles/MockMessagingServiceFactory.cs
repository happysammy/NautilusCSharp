// -------------------------------------------------------------------------------------------------
// <copyright file="MockMessagingServiceFactory.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System;
    using System.Collections.Generic;
    using Akka.Actor;
    using Akka.Event;
    using Nautilus.BlackBox.Core.Enums;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.BlackBox.Core.Messages.SystemCommands;
    using Nautilus.BlackBox.Messaging;
    using Nautilus.BlackBox.Messaging.MessageStore;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging.Base;

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
        public MessageWarehouse MessageWarehouse { get; private set; }

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="actorSystem">
        /// The actor system.
        /// </param>
        /// <param name="environment">
        /// The environment.
        /// </param>
        /// <param name="clock">
        /// The clock.
        /// </param>
        /// <param name="loggerFactory">
        /// The logger factory.
        /// </param>
        public void Create(
            ActorSystem actorSystem,
            BlackBoxEnvironment environment,
            IZonedClock clock,
            ILoggerFactory loggerFactory)
        {
            var messageWarhouse = new MessageWarehouse();

            var messageStoreRef = actorSystem.ActorOf(Props.Create(() => new MessageStorer(messageWarhouse))); // TODO: make disposable so that test don't break

            var commandBusRef = actorSystem.ActorOf(Props.Create(() => new MessageBus<CommandMessage>(
                new Label(BlackBoxService.CommandBus.ToString()),
                environment,
                clock,
                loggerFactory,
                messageStoreRef)));

            var eventBusRef = actorSystem.ActorOf(Props.Create(() => new MessageBus<EventMessage>(
                new Label(BlackBoxService.EventBus.ToString()),
                environment,
                clock,
                loggerFactory,
                messageStoreRef)));

            var serviceBusRef = actorSystem.ActorOf(Props.Create(() => new MessageBus<DocumentMessage>(
                new Label(BlackBoxService.ServiceBus.ToString()),
                environment,
                clock,
                loggerFactory,
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

            var initializeSwitchboard = new InitializeMessageSwitchboard(switchboard, Guid.NewGuid(), clock.TimeNow());

            messagingAdapter.Send(initializeSwitchboard);

            this.MessagingAdapter = messagingAdapter;
            this.MessageWarehouse = messageWarhouse;
        }
    }
}
