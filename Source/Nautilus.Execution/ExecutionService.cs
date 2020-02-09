//--------------------------------------------------------------------------------------------------
// <copyright file="ExecutionService.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Execution
{
    using System;
    using System.Collections.Generic;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Correctness;
    using Nautilus.Messaging;
    using Nautilus.Scheduler;
    using Nautilus.Service;

    /// <summary>
    /// Provides an execution service.
    /// </summary>
    public sealed class ExecutionService : NautilusServiceBase
    {
        private readonly MessageBusAdapter messageBusAdapter;
        private readonly List<Address> addresses;
        private readonly ITradingGateway tradingGateway;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionService"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="messageBusAdapter">The messaging adapter.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="tradingGateway">The execution gateway.</param>
        /// <param name="addresses">The execution service addresses.</param>
        /// <param name="config">The execution service configuration.</param>
        /// <exception cref="ArgumentException">If the addresses is empty.</exception>
        public ExecutionService(
            IComponentryContainer container,
            MessageBusAdapter messageBusAdapter,
            List<Address> addresses,
            IScheduler scheduler,
            ITradingGateway tradingGateway,
            Configuration config)
            : base(
                container,
                messageBusAdapter,
                scheduler,
                config.FixConfiguration)
        {
            Condition.NotEmpty(addresses, nameof(addresses));

            this.messageBusAdapter = messageBusAdapter;
            this.addresses = addresses;
            this.tradingGateway = tradingGateway;

            this.RegisterConnectionAddress(ServiceAddress.TradingGateway);
        }

        /// <inheritdoc />
        protected override void OnServiceStart(Start start)
        {
            // Forward start message
            var receivers = new List<Address>
            {
                ServiceAddress.TradingGateway,
                ServiceAddress.CommandServer,
                ServiceAddress.EventPublisher,
            };

            this.Send(start, this.addresses);
        }

        /// <inheritdoc />
        protected override void OnServiceStop(Stop stop)
        {
            // Forward stop message
            var receivers = new List<Address>
            {
                ServiceAddress.TradingGateway,
                ServiceAddress.CommandServer,
                ServiceAddress.EventPublisher,
            };

            this.Send(stop, this.addresses);
            this.messageBusAdapter.Stop();
        }

        /// <inheritdoc />
        protected override void OnConnected()
        {
            this.tradingGateway.AccountInquiry();
            this.tradingGateway.SubscribeToPositionEvents();
        }
    }
}
