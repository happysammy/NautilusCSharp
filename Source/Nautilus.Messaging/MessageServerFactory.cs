// -------------------------------------------------------------------------------------------------
// <copyright file="MessageServerFactory.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Messaging
{
    using Akka.Actor;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Validation;

    /// <summary>
    /// Provides MessageServer instances for the system.
    /// </summary>
    public static class MessageServerFactory
    {
        /// <summary>
        /// Create and return a new instance of the MessageServer.
        /// </summary>
        /// <param name="actorSystem">The actor system.</param>
        /// <param name="setupContainer">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="commandSerializer">The command serializer.</param>
        /// <param name="eventSerializer">The event serializer.</param>
        /// <param name="serverAddress">The servers address.</param>
        /// <param name="commandsPort">The servers commands port.</param>
        /// <param name="eventsPort">The servers events port.</param>
        /// <returns>The server endpoint.</returns>
        public static IEndpoint Create(
            ActorSystem actorSystem,
            IComponentryContainer setupContainer,
            IMessagingAdapter messagingAdapter,
            ICommandSerializer commandSerializer,
            IEventSerializer eventSerializer,
            string serverAddress,
            int commandsPort,
            int eventsPort)
        {
            Validate.NotNull(actorSystem, nameof(actorSystem));
            Validate.NotNull(setupContainer, nameof(setupContainer));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(commandSerializer, nameof(commandSerializer));
            Validate.NotNull(eventSerializer, nameof(eventSerializer));
            Validate.NotNull(serverAddress, nameof(serverAddress));
            Validate.NotEqualTo(commandsPort, nameof(commandsPort), 0);
            Validate.NotEqualTo(eventsPort, nameof(eventsPort), 0);
            Validate.NotEqualTo(commandsPort, nameof(commandsPort), eventsPort);

            return new ActorEndpoint(actorSystem.ActorOf(Props.Create(
                () => new MessageServer(
                        setupContainer,
                        messagingAdapter,
                        commandSerializer,
                        eventSerializer,
                        serverAddress,
                        commandsPort,
                        eventsPort))));
        }
    }
}
