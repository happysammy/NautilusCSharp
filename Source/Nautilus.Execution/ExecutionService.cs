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
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Common.Messages.Events;
    using Nautilus.Common.Messages.Jobs;
    using Nautilus.Common.Messaging;
    using Nautilus.Core;
    using Nautilus.Core.Correctness;
    using Nautilus.Messaging.Interfaces;
    using Nautilus.Network;
    using NodaTime;
    using Quartz;

    /// <summary>
    /// The service context which handles all execution related operations.
    /// </summary>
    public sealed class ExecutionService : ComponentBusConnectedBase
    {
        private readonly IFixGateway gateway;
        private readonly IEndpoint commandThrottler;
        private readonly IEndpoint newOrderThrottler;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly IEndpoint tradeCommandBus;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionService"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="gateway">The execution gateway.</param>
        /// <param name="commandsPerSecond">The commands per second throttling.</param>
        /// <param name="newOrdersPerSecond">The new orders per second throttling.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the commandsPerSecond is not positive (> 0).</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the newOrdersPerSecond is not positive (> 0).</exception>
        public ExecutionService(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            IFixGateway gateway,
            int commandsPerSecond,
            int newOrdersPerSecond)
            : base(
            NautilusService.Execution,
            container,
            messagingAdapter)
        {
            Precondition.PositiveInt32(commandsPerSecond, nameof(commandsPerSecond));
            Precondition.PositiveInt32(newOrdersPerSecond, nameof(newOrdersPerSecond));

            this.gateway = gateway;

            this.tradeCommandBus = new OrderCommandBus(
                container,
                messagingAdapter,
                gateway).Endpoint;

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
            this.RegisterHandler<ConnectFixJob>(this.OnMessage);
            this.RegisterHandler<DisconnectFixJob>(this.OnMessage);
        }

        /// <summary>
        /// Start method called when the <see cref="SystemStart"/> message is received.
        /// </summary>
        public override void Start()
        {
            this.Log.Information($"Started at {this.StartTime}.");

            this.CreateConnectFixJob();
            this.CreateDisconnectFixJob();
        }

        /// <summary>
        /// Actions to be performed when the component is stopped.
        /// </summary>
        public override void Stop()
        {
// this.tradeCommandBus.Send(PoisonPill.Instance);
//                this.newOrderThrottler.Send(PoisonPill.Instance);
//                this.commandThrottler.Send(PoisonPill.Instance);
        }

        private void CreateConnectFixJob()
        {
            var schedule = CronScheduleBuilder
                .WeeklyOnDayAndHourAndMinute(DayOfWeek.Sunday, 20, 00)
                .InTimeZone(TimeZoneInfo.Utc)
                .WithMisfireHandlingInstructionFireAndProceed();

            var jobKey = new JobKey("connect_fix", "fix44");
            var trigger = TriggerBuilder
                .Create()
                .WithIdentity(jobKey.Name, jobKey.Group)
                .WithSchedule(schedule)
                .Build();

            var createJob = new CreateJob(
                this.Endpoint,
                new ConnectFixJob(),
                jobKey,
                trigger,
                this.NewGuid(),
                this.TimeNow());

            this.Send(ServiceAddress.Scheduler, createJob);
            this.Log.Information("Created ConnectFixJob for Sundays 20:00 (UTC).");
        }

        private void CreateDisconnectFixJob()
        {
            var schedule = CronScheduleBuilder
                .WeeklyOnDayAndHourAndMinute(DayOfWeek.Saturday, 20, 00)
                .InTimeZone(TimeZoneInfo.Utc)
                .WithMisfireHandlingInstructionFireAndProceed();

            var jobKey = new JobKey("disconnect_fix", "fix44");
            var trigger = TriggerBuilder
                .Create()
                .WithIdentity(jobKey.Name, jobKey.Group)
                .WithSchedule(schedule)
                .Build();

            var createJob = new CreateJob(
                this.Endpoint,
                new DisconnectFixJob(),
                jobKey,
                trigger,
                this.NewGuid(),
                this.TimeNow());

            this.Send(ServiceAddress.Scheduler, createJob);
            this.Log.Information("Created DisconnectFixJob for Saturdays 20:00 (UTC).");
        }

        private void OnMessage(ConnectFixJob message)
        {
            this.gateway.Connect();
        }

        private void OnMessage(DisconnectFixJob message)
        {
            this.gateway.Disconnect();
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

            this.gateway.UpdateInstrumentsSubscribeAll();
            this.gateway.CollateralInquiry();
            this.gateway.TradingSessionStatus();
        }

        private void OnMessage(FixSessionDisconnected message)
        {
            this.Log.Warning($"{message.SessionId} session has been disconnected.");
        }
    }
}
