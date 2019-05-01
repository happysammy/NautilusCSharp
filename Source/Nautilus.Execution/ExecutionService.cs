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
    using Akka.Actor;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Common.Messages.Events;
    using Nautilus.Common.Messages.Jobs;
    using Nautilus.Common.Messaging;
    using Nautilus.Core;
    using Nautilus.DomainModel.Factories;
    using Nautilus.Messaging;
    using NodaTime;
    using Quartz;

    /// <summary>
    /// The service context which handles all execution related operations.
    /// </summary>
    public sealed class ExecutionService : ActorComponentBusConnectedBase
    {
        private readonly IFixGateway gateway;
        private readonly IEndpoint commandThrottler;
        private readonly IEndpoint newOrderThrottler;
        private readonly IEndpoint tradeCommandBus;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionService"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="gateway">The execution gateway.</param>
        /// <param name="commandsPerSecond">The commands per second throttling.</param>
        /// <param name="newOrdersPerSecond">The new orders per second throttling.</param>
        public ExecutionService(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            IFixGateway gateway,
            int commandsPerSecond,
            int newOrdersPerSecond)
            : base(
            NautilusService.Execution,
            LabelFactory.Create(nameof(ExecutionService)),
            container,
            messagingAdapter)
        {
            Precondition.NotNull(container, nameof(container));
            Precondition.NotNull(messagingAdapter, nameof(messagingAdapter));
            Precondition.NotNull(gateway, nameof(gateway));
            Precondition.PositiveInt32(commandsPerSecond, nameof(commandsPerSecond));
            Precondition.PositiveInt32(newOrdersPerSecond, nameof(newOrdersPerSecond));

            this.gateway = gateway;

            this.tradeCommandBus = new ActorEndpoint(
                Context.ActorOf(Props.Create(
                    () => new OrderCommandBus(
                        container,
                        messagingAdapter,
                        gateway))));

            this.commandThrottler = new ActorEndpoint(
                Context.ActorOf(Props.Create(
                    () => new Throttler<Command>(
                        container,
                        NautilusService.Execution,
                        this.tradeCommandBus,
                        Duration.FromSeconds(1),
                        commandsPerSecond))));

            this.newOrderThrottler = new ActorEndpoint(
                Context.ActorOf(Props.Create(
                    () => new Throttler<SubmitOrder>(
                        container,
                        NautilusService.Execution,
                        this.commandThrottler,
                        Duration.FromSeconds(1),
                        newOrdersPerSecond))));

            // Command messages.
            this.Receive<ConnectFixJob>(this.OnMessage);
            this.Receive<DisconnectFixJob>(this.OnMessage);
            this.Receive<CollateralInquiry>(this.OnMessage);
            this.Receive<SubmitOrder>(this.OnMessage);
            this.Receive<ModifyOrder>(this.OnMessage);
            this.Receive<CancelOrder>(this.OnMessage);

            // Event messages.
            this.Receive<FixSessionConnected>(this.OnMessage);
            this.Receive<FixSessionDisconnected>(this.OnMessage);
        }

        /// <summary>
        /// Start method called when the <see cref="StartSystem"/> message is received.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void Start(StartSystem message)
        {
            this.Log.Information($"Started at {this.StartTime}.");

            this.CreateConnectFixJob();
            this.CreateDisconnectFixJob();
        }

        /// <summary>
        /// Actions to be performed after the actor base is stopped.
        /// </summary>
        protected override void PostStop()
        {
            this.Execute(() =>
            {
                this.tradeCommandBus.Send(PoisonPill.Instance);
                this.newOrderThrottler.Send(PoisonPill.Instance);
                this.commandThrottler.Send(PoisonPill.Instance);
                base.PostStop();
            });
        }

        private void CreateConnectFixJob()
        {
            this.Execute(() =>
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
                    new ActorEndpoint(this.Self),
                    new ConnectFixJob(),
                    jobKey,
                    trigger,
                    this.NewGuid(),
                    this.TimeNow());

                this.Send(ServiceAddress.Scheduler, createJob);
                this.Log.Information("Created ConnectFixJob for Sundays 20:00 (UTC).");
            });
        }

        private void CreateDisconnectFixJob()
        {
            this.Execute(() =>
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
                    new ActorEndpoint(this.Self),
                    new DisconnectFixJob(),
                    jobKey,
                    trigger,
                    this.NewGuid(),
                    this.TimeNow());

                this.Send(ServiceAddress.Scheduler, createJob);
                this.Log.Information("Created DisconnectFixJob for Saturdays 20:00 (UTC).");
            });
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
            Debug.NotNull(message, nameof(message));

            this.Execute(() =>
            {
                this.commandThrottler.Send(message);
            });
        }

        private void OnMessage(SubmitOrder message)
        {
            Debug.NotNull(message, nameof(message));

            this.Execute(() =>
            {
                this.newOrderThrottler.Send(message);
            });
        }

        private void OnMessage(ModifyOrder message)
        {
            Debug.NotNull(message, nameof(message));

            this.Execute(() =>
            {
                this.commandThrottler.Send(message);
            });
        }

        private void OnMessage(CancelOrder message)
        {
            Debug.NotNull(message, nameof(message));

            this.Execute(() =>
            {
                this.commandThrottler.Send(message);
            });
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
