// -------------------------------------------------------------------------------------------------
// <copyright file="CommandRouter.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Execution
{
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Message;
    using Nautilus.DomainModel.Commands;
    using Nautilus.Messaging.Interfaces;
    using NodaTime;

    /// <summary>
    /// Provides a <see cref="Command"/> message server using the ZeroMQ protocol.
    /// </summary>
    public class CommandRouter : MessageBusConnected
    {
        private readonly Throttler commandThrottler;
        private readonly Throttler newOrderThrottler;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandRouter"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="messageBusAdapter">The messaging adapter.</param>
        /// <param name="executionEngine">The execution engine endpoint.</param>
        /// <param name="config">The service configuration.</param>
        public CommandRouter(
            IComponentryContainer container,
            IMessageBusAdapter messageBusAdapter,
            IEndpoint executionEngine,
            Configuration config)
            : base(container, messageBusAdapter)
        {
            this.commandThrottler = new Throttler(
                container,
                executionEngine,
                Duration.FromSeconds(1),
                config.CommandsPerSecond);

            this.newOrderThrottler = new Throttler(
                container,
                this.commandThrottler.Endpoint,
                Duration.FromSeconds(1),
                config.NewOrdersPerSecond);

            this.RegisterHandler<SubmitOrder>(this.OnMessage);
            this.RegisterHandler<SubmitAtomicOrder>(this.OnMessage);
            this.RegisterHandler<CancelOrder>(this.OnMessage);
            this.RegisterHandler<ModifyOrder>(this.OnMessage);
            this.RegisterHandler<AccountInquiry>(this.OnMessage);
        }

        /// <inheritdoc />
        protected override void OnStart(Start message)
        {
            this.commandThrottler.Start();
            this.newOrderThrottler.Start();
        }

        /// <inheritdoc />
        protected override void OnStop(Stop message)
        {
            // Forward stop message
            this.commandThrottler.Stop();
            this.newOrderThrottler.Stop();
        }

        private void OnMessage(SubmitOrder message)
        {
            this.newOrderThrottler.Endpoint.Send(message);
        }

        private void OnMessage(SubmitAtomicOrder message)
        {
            this.newOrderThrottler.Endpoint.Send(message);
        }

        private void OnMessage(CancelOrder message)
        {
            this.commandThrottler.Endpoint.Send(message);
        }

        private void OnMessage(ModifyOrder message)
        {
            this.commandThrottler.Endpoint.Send(message);
        }

        private void OnMessage(AccountInquiry message)
        {
            this.commandThrottler.Endpoint.Send(message);
        }
    }
}
