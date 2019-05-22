//--------------------------------------------------------------------------------------------------
// <copyright file="ExecutionService.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Execution
{
    using System;
    using System.Collections.Generic;
    using Nautilus.Common;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Common.Messages.Events;
    using Nautilus.Common.Messaging;
    using Nautilus.Core;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.Execution.Messages.Commands;
    using Nautilus.Messaging;
    using Nautilus.Messaging.Interfaces;
    using Nautilus.Network;
    using Nautilus.Scheduler;
    using NodaTime;

    /// <summary>
    /// Provides an execution service.
    /// </summary>
    public sealed class ExecutionService : ComponentBusConnectedBase
    {
        private readonly IScheduler scheduler;
        private readonly IFixGateway fixGateway;
        private readonly IEndpoint orderCommandBus;
        private readonly IEndpoint commandThrottler;
        private readonly IEndpoint newOrderThrottler;
        private readonly (IsoDayOfWeek Day, LocalTime Time) fixConnectTime;
        private readonly (IsoDayOfWeek Day, LocalTime Time) fixDisconnectTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionService"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="scheduler">The system scheduler.</param>
        /// <param name="fixGateway">The execution gateway.</param>
        /// <param name="addresses">The execution service addresses.</param>
        /// <param name="config">The execution service configuration.</param>
        /// <exception cref="ArgumentException">If the addresses is empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the commandsPerSecond is not positive (> 0).</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the newOrdersPerSecond is not positive (> 0).</exception>
        public ExecutionService(
            IComponentryContainer container,
            MessagingAdapter messagingAdapter,
            Dictionary<Address, IEndpoint> addresses,
            IScheduler scheduler,
            IFixGateway fixGateway,
            Configuration config)
            : base(
            NautilusService.Execution,
            container,
            messagingAdapter)
        {
            Condition.NotEmpty(addresses, nameof(addresses));

            VersionChecker.Run(this.Log, "NautilusData - Financial Market Execution Service");

            addresses.Add(ExecutionServiceAddress.Core, this.Endpoint);
            messagingAdapter.Send(new InitializeSwitchboard(
                Switchboard.Create(addresses),
                this.NewGuid(),
                this.TimeNow()));

            this.scheduler = scheduler;
            this.fixGateway = fixGateway;

            this.orderCommandBus = new OrderCommandBus(
                container,
                messagingAdapter,
                fixGateway).Endpoint;

            this.commandThrottler = new Throttler<Command>(
                container,
                NautilusService.Execution,
                this.orderCommandBus,
                Duration.FromSeconds(1),
                config.CommandsPerSecond).Endpoint;

            this.newOrderThrottler = new Throttler<SubmitOrder>(
                container,
                NautilusService.Execution,
                this.commandThrottler,
                Duration.FromSeconds(1),
                config.NewOrdersPerSecond).Endpoint;

            this.fixConnectTime = config.FixConfiguration.ConnectTime;
            this.fixDisconnectTime = config.FixConfiguration.DisconnectTime;

            this.RegisterHandler<SubmitOrder>(this.OnMessage);
            this.RegisterHandler<ModifyOrder>(this.OnMessage);
            this.RegisterHandler<CancelOrder>(this.OnMessage);
            this.RegisterHandler<CollateralInquiry>(this.OnMessage);
            this.RegisterHandler<FixSessionConnected>(this.OnMessage);
            this.RegisterHandler<FixSessionDisconnected>(this.OnMessage);
            this.RegisterHandler<ConnectFix>(this.OnMessage);
            this.RegisterHandler<DisconnectFix>(this.OnMessage);
        }

        /// <inheritdoc />
        protected override void OnStart(Start message)
        {
            this.Log.Information($"Starting from {message}...");

            if (TimeProvider.IsOutsideWeeklyInterval(
                this.fixDisconnectTime,
                this.fixConnectTime,
                this.InstantNow()))
            {
                this.Send(ExecutionServiceAddress.FixGateway, message);
            }
            else
            {
                this.CreateConnectFixJob();
            }
        }

        /// <inheritdoc />
        protected override void OnStop(Stop message)
        {
            this.Log.Information($"Stopping from {message}...");

            // Forward message.
            this.Send(ExecutionServiceAddress.FixGateway, message);
        }

        private void OnMessage(ConnectFix message)
        {
            // Forward message.
            this.Send(ExecutionServiceAddress.FixGateway, message);
        }

        private void OnMessage(DisconnectFix message)
        {
            // Forward message.
            this.Send(ExecutionServiceAddress.FixGateway, message);
        }

        private void OnMessage(CollateralInquiry message)
        {
            this.commandThrottler.Send(message);
        }

        private void OnMessage(SubmitOrder message)
        {
            this.newOrderThrottler.Send(message);
        }

        private void OnMessage(ModifyOrder message)
        {
            this.commandThrottler.Send(message);
        }

        private void OnMessage(CancelOrder message)
        {
            this.commandThrottler.Send(message);
        }

        private void OnMessage(FixSessionConnected message)
        {
            this.Log.Information($"{message.SessionId} session is connected.");

            this.fixGateway.UpdateInstrumentsSubscribeAll();
            this.fixGateway.CollateralInquiry();
            this.fixGateway.TradingSessionStatus();

            this.CreateDisconnectFixJob();
            this.CreateConnectFixJob();
        }

        private void OnMessage(FixSessionDisconnected message)
        {
            this.Log.Warning($"{message.SessionId} session has been disconnected.");
        }

        private void CreateConnectFixJob()
        {
            var now = this.InstantNow();
            var nextTime = TimeProvider.GetNextUtc(
                this.fixConnectTime.Day,
                this.fixConnectTime.Time,
                now);
            var durationToNext = TimeProvider.GetDurationToNextUtc(nextTime, now);

            var job = new ConnectFix(
                nextTime,
                this.NewGuid(),
                this.TimeNow());

            this.scheduler.ScheduleSendOnceCancelable(
                durationToNext,
                this.Endpoint,
                job,
                this.Endpoint);

            this.Log.Information($"Created {job}Job for {nextTime.ToIsoString()}.");
        }

        private void CreateDisconnectFixJob()
        {
            var now = this.InstantNow();
            var nextTime = TimeProvider.GetNextUtc(
                this.fixDisconnectTime.Day,
                this.fixDisconnectTime.Time,
                now);
            var durationToNext = TimeProvider.GetDurationToNextUtc(nextTime, now);

            var job = new DisconnectFix(
                nextTime,
                this.NewGuid(),
                this.TimeNow());

            this.scheduler.ScheduleSendOnceCancelable(
                durationToNext,
                this.Endpoint,
                job,
                this.Endpoint);

            this.Log.Information($"Created {job}Job for {nextTime.ToIsoString()}.");
        }
    }
}
