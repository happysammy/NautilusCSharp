// -------------------------------------------------------------------------------------------------
// <copyright file="RabbitMQServerFactory.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.RabbitMQ
{
    using Akka.Actor;
    using Nautilus.Common.Interfaces;
    using global::RabbitMQ.Client;

    /// <summary>
    /// Provides RabbitMQServer instances for the system.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class RabbitMQServerFactory
    {
        /// <summary>
        /// Create and return a new instance of the RabbitMQServer.
        /// </summary>
        /// <returns></returns>
        public static IActorRef Create(
            ActorSystem actorSystem,
            IComponentryContainer setupContainer,
            IMessagingAdapter messagingAdapter,
            ICommandSerializer commandSerializer,
            IEventSerializer eventSerializer)
        {
            var factory = new ConnectionFactory() {HostName = RabbitConstants.LocalHost};
            var connection = factory.CreateConnection();

            return actorSystem.ActorOf(Props.Create(
                () => new RabbitMQServer(
                        setupContainer,
                    messagingAdapter,
                    commandSerializer,
                    eventSerializer,
                    connection)));
        }
    }
}
