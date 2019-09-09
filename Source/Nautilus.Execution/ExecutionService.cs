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
        private readonly (IsoDayOfWeek Day, LocalTime Time) fixConnectTime;
        private readonly (IsoDayOfWeek Day, LocalTime Time) fixDisconnectTime;

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
            this.fixConnectTime = config.FixConfiguration.ConnectTime;
            this.fixDisconnectTime = config.FixConfiguration.DisconnectTime;

            this.RegisterHandler<FixSessionConnected>(this.OnMessage);
            this.RegisterHandler<FixSessionDisconnected>(this.OnMessage);
            this.RegisterHandler<Connect>(this.OnMessage);
            this.RegisterHandler<Disconnect>(this.OnMessage);

            // Subscribe to connection events
            this.Subscribe<FixSessionConnected>();
            this.Subscribe<FixSessionDisconnected>();
        }

        /// <inheritdoc />
        protected override void OnStart(Start start)
        {
            if (TimingProvider.IsOutsideWeeklyInterval(
                this.fixDisconnectTime,
                this.fixConnectTime,
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
            this.Send(message, ServiceAddress.TradingGateway);
        }

        private void OnMessage(FixSessionConnected message)
        {
            this.Log.Information($"{message.SessionId} session is connected.");

            this.tradingGateway.AccountInquiry();

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
            var nextTime = TimingProvider.GetNextUtc(
                this.fixConnectTime.Day,
                this.fixConnectTime.Time,
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

            this.Log.Information($"Created scheduled job {job} for {nextTime.ToIsoString()}.");
        }

        private void CreateDisconnectFixJob()
        {
            var now = this.InstantNow();
            var nextTime = TimingProvider.GetNextUtc(
                this.fixDisconnectTime.Day,
                this.fixDisconnectTime.Time,
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

            this.Log.Information($"Created scheduled job {job} for {nextTime.ToIsoString()}.");
        }
    }
}
