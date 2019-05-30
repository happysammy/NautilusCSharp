// -------------------------------------------------------------------------------------------------
// <copyright file="CommandServer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Execution
{
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Execution.Messages.Commands;
    using Nautilus.Execution.Network;
    using Nautilus.Network;
    using NodaTime;

    /// <summary>
    /// Provides a messaging server using the ZeroMQ protocol.
    /// </summary>
    [PerformanceOptimized]
    public class CommandServer : ComponentBusConnectedBase
    {
        private readonly Throttler<Command> commandThrottler;
        private readonly Throttler<SubmitOrder> newOrderThrottler;
        private readonly CommandRouter commandRouter;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandServer"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="commandSerializer">The command serializer.</param>
        /// <param name="config">The service configuration.</param>
        public CommandServer(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            ICommandSerializer commandSerializer,
            Configuration config)
            : base(container, messagingAdapter)
        {
            this.commandThrottler = new Throttler<Command>(
                container,
                this.Endpoint,
                Duration.FromSeconds(1),
                config.CommandsPerSecond);

            this.newOrderThrottler = new Throttler<SubmitOrder>(
                container,
                this.commandThrottler.Endpoint,
                Duration.FromSeconds(1),
                config.NewOrdersPerSecond);

            this.commandRouter = new CommandRouter(
                container,
                commandSerializer,
                this.commandThrottler.Endpoint,
                config.ServerAddress,
                config.CommandsPort);

            this.RegisterHandler<SubmitOrder>(this.OnMessage);
            this.RegisterHandler<SubmitAtomicOrder>(this.OnMessage);
            this.RegisterHandler<CancelOrder>(this.OnMessage);
            this.RegisterHandler<ModifyOrder>(this.OnMessage);
            this.RegisterHandler<CollateralInquiry>(this.OnMessage);
        }

        private void OnMessage(SubmitOrder message)
        {
            this.Send(ExecutionServiceAddress.OrderManager, message);
        }

        private void OnMessage(SubmitAtomicOrder message)
        {
            this.Send(ExecutionServiceAddress.OrderManager, message);
        }

        private void OnMessage(CancelOrder message)
        {
            this.Send(ExecutionServiceAddress.OrderManager, message);
        }

        private void OnMessage(ModifyOrder message)
        {
            this.Send(ExecutionServiceAddress.OrderManager, message);
        }

        private void OnMessage(CollateralInquiry message)
        {
            this.Send(ExecutionServiceAddress.OrderManager, message);
        }
    }
}
