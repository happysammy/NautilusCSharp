//--------------------------------------------------------------------------------------------------
// <copyright file="ExecutionServiceFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Execution
{
    using System.Collections.Generic;
    using Akka.Actor;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.Messaging.Network;

    /// <summary>
    /// Provides a factory for creating the <see cref="ExecutionService"/>.
    /// </summary>
    [Stateless]
    public static class ExecutionServiceFactory
    {
        /// <summary>
        /// Creates a new <see cref="ExecutionService"/> and returns its <see cref="IActorRef"/>
        /// actor address.
        /// </summary>
        /// <param name="actorSystem">The actor system.</param>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="gateway">The FIX gateway.</param>
        /// <param name="commandSerializer">The command serializer.</param>
        /// <param name="eventSerializer">The event serializer.</param>
        /// <param name="serviceAddress">The service address.</param>
        /// <param name="commandsPort">The commands port.</param>
        /// <param name="eventsPort">The events port.</param>
        /// <param name="commandsPerSecond">The commands per second throttling.</param>
        /// <param name="newOrdersPerSecond">The new orders per second throttling.</param>
        /// <returns>The services switchboard.</returns>
        public static Switchboard Create(
            ActorSystem actorSystem,
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            IFixGateway gateway,
            ICommandSerializer commandSerializer,
            IEventSerializer eventSerializer,
            NetworkAddress serviceAddress,
            Port commandsPort,
            Port eventsPort,
            int commandsPerSecond = 100,
            int newOrdersPerSecond = 15)
        {
            Validate.NotNull(actorSystem, nameof(actorSystem));
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.PositiveInt32(commandsPerSecond, nameof(commandsPerSecond));
            Validate.PositiveInt32(newOrdersPerSecond, nameof(newOrdersPerSecond));

            var messageServer = new ActorEndpoint(
                actorSystem.ActorOf(Props.Create(
                    () => new MessageServer(
                        container,
                        messagingAdapter,
                        commandSerializer,
                        eventSerializer,
                        serviceAddress,
                        commandsPort,
                        eventsPort))));

            var orderManager = new ActorEndpoint(
                actorSystem.ActorOf(Props.Create(
                    () => new OrderManager(
                        container,
                        messagingAdapter))));

            gateway.RegisterEventReceiver(orderManager);

            var executionService = new ActorEndpoint(
                actorSystem.ActorOf(Props.Create(
                    () => new ExecutionService(
                        container,
                        messagingAdapter,
                        gateway,
                        commandsPerSecond,
                        newOrdersPerSecond))));

            return new Switchboard(new Dictionary<NautilusService, IEndpoint>
            {
                { NautilusService.MessageServer, messageServer },
                { NautilusService.OrderManager, orderManager },
                { NautilusService.Execution, executionService },
            });
        }
    }
}
