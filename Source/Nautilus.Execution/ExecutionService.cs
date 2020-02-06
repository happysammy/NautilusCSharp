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
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Common.Messages.Events;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.Messaging;
    using Nautilus.Messaging.Interfaces;
    using Nautilus.Scheduler;
    using NodaTime;

    /// <summary>
    /// Provides an execution service.
    /// </summary>
    public sealed class ExecutionService : MessageBusConnected
    {
        private readonly IScheduler scheduler;
        private readonly ITradingGateway tradingGateway;
        private readonly (IsoDayOfWeek Day, LocalTime Time) scheduledConnect;
        private readonly (IsoDayOfWeek Day, LocalTime Time) scheduledDisconnect;

        private ZonedDateTime nextConnectTime;
        private ZonedDateTime nextDisconnectTime;
        private bool autoReconnect;

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
            Dictionary<Address, IEndpoint> addresses,
            IScheduler scheduler,
            ITradingGateway tradingGateway,
            Configuration config)
            : base(container, messageBusAdapter)
        {
            Condition.NotEmpty(addresses, nameof(addresses));

            addresses.Add(ServiceAddress.ExecutionService, this.Endpoint);
            messageBusAdapter.Send(new InitializeSwitchboard(
                Switchboard.Create(addresses),
                this.NewGuid(),
                this.TimeNow()));

            this.scheduler = scheduler;
            this.tradingGateway = tradingGateway;
            this.scheduledConnect = config.FixConfiguration.ConnectTime;
            this.scheduledDisconnect = config.FixConfiguration.DisconnectTime;
            this.nextConnectTime = TimingProvider.GetNextUtc(
                this.scheduledConnect.Day,
                this.scheduledConnect.Time,
                this.InstantNow());
            this.nextDisconnectTime = TimingProvider.GetNextUtc(
                this.scheduledDisconnect.Day,
                this.scheduledDisconnect.Time,
                this.InstantNow());
            this.autoReconnect = true;

            // Commands
            this.RegisterHandler<Connect>(this.OnMessage);
            this.RegisterHandler<Disconnect>(this.OnMessage);

            // Events
            this.RegisterHandler<SessionConnected>(this.OnMessage);
            this.RegisterHandler<SessionDisconnected>(this.OnMessage);

            // Event Subscriptions
            this.Subscribe<SessionConnected>();
            this.Subscribe<SessionDisconnected>();
        }

        /// <inheritdoc />
        protected override void OnStart(Start start)
        {
            if (TimingProvider.IsOutsideWeeklyInterval(
                this.scheduledDisconnect,
                this.scheduledConnect,
                this.InstantNow()))
            {
                var receivers = new List<Address>
                {
                    ServiceAddress.TradingGateway,
                    ServiceAddress.CommandServer,
                    ServiceAddress.EventPublisher,
                };

                this.Send(start, receivers);

                // Inside connection schedule weekly interval
                this.CreateDisconnectFixJob();
                this.CreateConnectFixJob();
            }
            else
            {
                // Outside connection schedule weekly interval
                this.CreateConnectFixJob();
                this.CreateDisconnectFixJob();
            }
        }

        /// <inheritdoc />
        protected override void OnStop(Stop stop)
        {
            this.autoReconnect = false;  // Avoid immediate reconnection

            // Forward stop message
            var receivers = new List<Address>
            {
                ServiceAddress.TradingGateway,
                ServiceAddress.CommandServer,
                ServiceAddress.EventPublisher,
            };

            this.Send(stop, receivers);
        }

        private void OnMessage(Connect message)
        {
            // Forward message
            this.Send(message, ServiceAddress.TradingGateway);
        }

        private void OnMessage(Disconnect message)
        {
            // Forward message
            this.autoReconnect = false;  // Avoid immediate reconnection
            this.Send(message, ServiceAddress.TradingGateway);
        }

        private void OnMessage(SessionConnected message)
        {
            this.Log.Information($"Connected to session {message.SessionId}.");

            if (this.nextDisconnectTime.IsLessThanOrEqualTo(this.TimeNow()))
            {
                this.CreateDisconnectFixJob();
            }

            this.tradingGateway.AccountInquiry();
            this.tradingGateway.SubscribeToPositionEvents();
        }

        private void OnMessage(SessionDisconnected message)
        {
            if (this.autoReconnect)
            {
                this.Log.Warning($"Disconnected from session {message.SessionId}. Initiating auto reconnect...");

                var connect = new Connect(
                    this.TimeNow(),
                    this.NewGuid(),
                    this.TimeNow());

                this.Send(connect, ServiceAddress.TradingGateway);
            }
            else
            {
                this.Log.Information($"Disconnected from session {message.SessionId}.");
            }

            if (this.nextConnectTime.IsLessThanOrEqualTo(this.TimeNow()))
            {
                this.CreateConnectFixJob();
            }
        }

        private void CreateConnectFixJob()
        {
            var now = this.InstantNow();
            var nextTime = TimingProvider.GetNextUtc(
                this.scheduledConnect.Day,
                this.scheduledConnect.Time,
                now);
            var durationToNext = TimingProvider.GetDurationToNextUtc(nextTime, now);

            var job = new Connect(
                nextTime,
                this.NewGuid(),
                this.TimeNow());

            this.scheduler.ScheduleSendOnceCancelable(
                durationToNext,
                this.Endpoint,
                job,
                this.Endpoint);

            this.nextConnectTime = nextTime;

            this.Log.Information($"Created scheduled job {job} for {nextTime.ToIsoString()}.");
        }

        private void CreateDisconnectFixJob()
        {
            var now = this.InstantNow();
            var nextTime = TimingProvider.GetNextUtc(
                this.scheduledDisconnect.Day,
                this.scheduledDisconnect.Time,
                now);
            var durationToNext = TimingProvider.GetDurationToNextUtc(nextTime, now);

            var job = new Disconnect(
                nextTime,
                this.NewGuid(),
                this.TimeNow());

            this.scheduler.ScheduleSendOnceCancelable(
                durationToNext,
                this.Endpoint,
                job,
                this.Endpoint);

            this.nextDisconnectTime = nextTime;

            this.Log.Information($"Created scheduled job {job} for {nextTime.ToIsoString()}.");
        }
    }
}
