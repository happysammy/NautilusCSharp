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
        private readonly IEndpoint commandThrottler;
        private readonly IEndpoint newOrderThrottler;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly IEndpoint tradeCommandBus;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionService"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="scheduler">The system scheduler.</param>
        /// <param name="fixGateway">The execution gateway.</param>
        /// <param name="addresses">The execution service addresses.</param>
        /// <param name="commandsPerSecond">The commands per second throttling.</param>
        /// <param name="newOrdersPerSecond">The new orders per second throttling.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the commandsPerSecond is not positive (> 0).</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the newOrdersPerSecond is not positive (> 0).</exception>
        public ExecutionService(
            IComponentryContainer container,
            MessagingAdapter messagingAdapter,
            Dictionary<Address, IEndpoint> addresses,
            IScheduler scheduler,
            IFixGateway fixGateway,
            int commandsPerSecond,
            int newOrdersPerSecond)
            : base(
            NautilusService.Execution,
            container,
            messagingAdapter)
        {
            Condition.PositiveInt32(commandsPerSecond, nameof(commandsPerSecond));
            Condition.PositiveInt32(newOrdersPerSecond, nameof(newOrdersPerSecond));

            addresses.Add(ExecutionServiceAddress.Core, this.Endpoint);
            messagingAdapter.Send(new InitializeSwitchboard(
                Switchboard.Create(addresses),
                this.NewGuid(),
                this.TimeNow()));

            this.scheduler = scheduler;
            this.fixGateway = fixGateway;

            this.tradeCommandBus = new OrderCommandBus(
                container,
                messagingAdapter,
                fixGateway).Endpoint;

            this.commandThrottler = new Throttler<Command>(
                container,
                NautilusService.Execution,
                this.tradeCommandBus,
                Duration.FromSeconds(1),
                commandsPerSecond).Endpoint;

            this.newOrderThrottler = new Throttler<SubmitOrder>(
                container,
                NautilusService.Execution,
                this.commandThrottler,
                Duration.FromSeconds(1),
                newOrdersPerSecond).Endpoint;

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
            this.Log.Information($"Started from {message}...");

            this.Send(ExecutionServiceAddress.FixGateway, message);

            // this.CreateConnectFixJob();
            // this.CreateDisconnectFixJob();
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
            var jobDay = IsoDayOfWeek.Sunday;
            var jobTime = new LocalTime(20, 00);
            var timeNow = this.TimeNow().ToInstant();

            var nextTime = ZonedDateTimeExtensions.GetNextUtc(jobDay, jobTime, timeNow);
            var durationToNext = ZonedDateTimeExtensions.GetDurationToNextUtc(nextTime, this.TimeNow().ToInstant());

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
            var jobDay = IsoDayOfWeek.Saturday;
            var jobTime = new LocalTime(20, 00);
            var timeNow = this.TimeNow().ToInstant();

            var nextTime = ZonedDateTimeExtensions.GetNextUtc(jobDay, jobTime, timeNow);
            var durationToNext = ZonedDateTimeExtensions.GetDurationToNextUtc(nextTime, this.TimeNow().ToInstant());

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
