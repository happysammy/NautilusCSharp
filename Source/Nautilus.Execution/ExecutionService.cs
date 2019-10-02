//--------------------------------------------------------------------------------------------------
// <copyright file="ExecutionService.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
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
        private readonly (IsoDayOfWeek Day, LocalTime Time) connectTime;
        private readonly (IsoDayOfWeek Day, LocalTime Time) disconnectTime;

        private ZonedDateTime? nextConnectTime;
        private ZonedDateTime? nextDisconnectTime;
        private bool reconnect;

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
            this.connectTime = config.FixConfiguration.ConnectTime;
            this.disconnectTime = config.FixConfiguration.DisconnectTime;
            this.reconnect = true;

            // Commands
            this.RegisterHandler<Connect>(this.OnMessage);
            this.RegisterHandler<Disconnect>(this.OnMessage);

            // Events
            this.RegisterHandler<FixSessionConnected>(this.OnMessage);
            this.RegisterHandler<FixSessionDisconnected>(this.OnMessage);

            // Event Subscriptions
            this.Subscribe<FixSessionConnected>();
            this.Subscribe<FixSessionDisconnected>();
        }

        /// <inheritdoc />
        protected override void OnStart(Start start)
        {
            if (TimingProvider.IsOutsideWeeklyInterval(
                this.disconnectTime,
                this.connectTime,
                this.InstantNow()))
            {
                var receivers = new List<Address>
                {
                    ServiceAddress.TradingGateway,
                    ServiceAddress.CommandServer,
                    ServiceAddress.EventPublisher,
                };

                this.SendAll(start, receivers);
            }
            else
            {
                this.CreateConnectFixJob();
            }
        }

        /// <inheritdoc />
        protected override void OnStop(Stop stop)
        {
            // Forward stop message
            var receivers = new List<Address>
            {
                ServiceAddress.TradingGateway,
                ServiceAddress.CommandServer,
                ServiceAddress.EventPublisher,
            };

            this.SendAll(stop, receivers);
        }

        private void OnMessage(Connect message)
        {
            // Forward message
            this.Send(message, ServiceAddress.TradingGateway);
        }

        private void OnMessage(Disconnect message)
        {
            // Forward message
            this.reconnect = false;  // Avoid immediate reconnection
            this.Send(message, ServiceAddress.TradingGateway);
        }

        private void OnMessage(FixSessionConnected message)
        {
            this.Log.Information($"Connected to FIX session {message.SessionId}.");

            if (this.nextDisconnectTime is null || this.nextDisconnectTime.Value.IsLessThanOrEqualTo(this.TimeNow()))
            {
                this.CreateDisconnectFixJob();
            }
        }

        private void OnMessage(FixSessionDisconnected message)
        {
            if (this.reconnect && (this.nextConnectTime is null || this.nextConnectTime.Value.IsLessThanOrEqualTo(this.TimeNow())))
            {
                this.CreateConnectFixJob();
            }

            this.Log.Warning($"Disconnected from FIX session {message.SessionId}.");
            this.reconnect = true; // Reset flag to default true
        }

        private void CreateConnectFixJob()
        {
            var now = this.InstantNow();
            var nextTime = TimingProvider.GetNextUtc(
                this.connectTime.Day,
                this.connectTime.Time,
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
                this.disconnectTime.Day,
                this.disconnectTime.Time,
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
