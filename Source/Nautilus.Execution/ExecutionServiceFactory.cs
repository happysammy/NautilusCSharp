//--------------------------------------------------------------------------------------------------
// <copyright file="ExecutionServiceFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Execution
{
    using System;
    using System.Collections.Generic;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Correctness;
    using Nautilus.Messaging;
    using Nautilus.Network;

    /// <summary>
    /// Provides a factory for creating the <see cref="ExecutionService"/>.
    /// </summary>
    public static class ExecutionServiceFactory
    {
        /// <summary>
        /// Creates a new <see cref="ExecutionService"/> and returns an address book of endpoints.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="client">The FIX client.</param>
        /// <param name="gateway">The FIX gateway.</param>
        /// <param name="commandSerializer">The command serializer.</param>
        /// <param name="eventSerializer">The event serializer.</param>
        /// <param name="serviceAddress">The service address.</param>
        /// <param name="commandsPort">The commands port.</param>
        /// <param name="eventsPort">The events port.</param>
        /// <param name="commandsPerSecond">The commands per second throttling.</param>
        /// <param name="newOrdersPerSecond">The new orders per second throttling.</param>
        /// <returns>The services switchboard.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the commandsPerSecond is not positive (> 0).</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the newOrdersPerSecond is not positive (> 0).</exception>
        public static Dictionary<Address, IEndpoint> Create(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            IFixClient client,
            IFixGateway gateway,
            ICommandSerializer commandSerializer,
            IEventSerializer eventSerializer,
            NetworkAddress serviceAddress,
            Port commandsPort,
            Port eventsPort,
            int commandsPerSecond,
            int newOrdersPerSecond)
        {
            Precondition.PositiveInt32(commandsPerSecond, nameof(commandsPerSecond));
            Precondition.PositiveInt32(newOrdersPerSecond, nameof(newOrdersPerSecond));

            var messageServer = new MessageServer(
                container,
                messagingAdapter,
                commandSerializer,
                eventSerializer,
                serviceAddress,
                commandsPort,
                eventsPort);

            var orderManager = new OrderManager(container, messagingAdapter);

            var executionService = new ExecutionService(
                container,
                messagingAdapter,
                gateway,
                commandsPerSecond,
                newOrdersPerSecond);

            gateway.RegisterEventReceiver(orderManager.Endpoint);
            client.RegisterConnectionEventReceiver(executionService.Endpoint);

            return new Dictionary<Address, IEndpoint>
            {
                { ServiceAddress.Execution, executionService.Endpoint },
                { ExecutionServiceAddress.MessageServer, messageServer.Endpoint },
                { ExecutionServiceAddress.OrderManager, orderManager.Endpoint },
            };
        }
    }
}
