// -------------------------------------------------------------------------------------------------
// <copyright file="RabbitMQServerFactory.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.RabbitMQ
{
    using System;
    using Akka.Actor;
    using global::RabbitMQ.Client;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;

    /// <summary>
    /// Provides RabbitMQServer instances for the system.
    /// </summary>
    // ReSharper disable once InconsistentNaming (RabbitMQ is the correct name).
    public static class RabbitMQServerFactory
    {
        /// <summary>
        /// Create and return a new instance of the RabbitMQServer.
        /// </summary>
        /// <param name="actorSystem">The actor system.</param>
        /// <param name="setupContainer">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="commandSerializer">The command serializer.</param>
        /// <param name="eventSerializer">The event serializer.</param>
        /// <returns>The server endpoint.</returns>
        public static IEndpoint Create(
            ActorSystem actorSystem,
            IComponentryContainer setupContainer,
            IMessagingAdapter messagingAdapter,
            ICommandSerializer commandSerializer,
            IEventSerializer eventSerializer)
        {
            var factory = new ConnectionFactory
            {
                HostName = RabbitConstants.LocalHost,
                UserName = RabbitConstants.Username,
                Password = RabbitConstants.Password,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
            };

            return new ActorEndpoint(actorSystem.ActorOf(Props.Create(
                () => new RabbitMQServer(
                        setupContainer,
                        messagingAdapter,
                        commandSerializer,
                        eventSerializer,
                        factory.CreateConnection(),
                        factory.CreateConnection()))));
        }
    }
}
