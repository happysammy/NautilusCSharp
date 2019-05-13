//--------------------------------------------------------------------------------------------------
// <copyright file="DataService.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data
{
    using System;
    using System.Collections.Immutable;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Common.Messages.Events;
    using Nautilus.Common.Messages.Jobs;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Annotations;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging;
    using Nautilus.Messaging.Interfaces;
    using Quartz;

    /// <summary>
    /// Provides a data service.
    /// </summary>
    [PerformanceOptimized]
    public sealed class DataService : ComponentBusConnectedBase
    {
        private readonly ImmutableDictionary<Address, Endpoint> addresses;
        private readonly IFixGateway fixGateway;
        private readonly bool updateInstruments;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataService"/> class.
        /// </summary>
        /// <param name="setupContainer">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="fixGateway">The FIX gateway.</param>
        /// <param name="addresses">The data service address dictionary.</param>
        /// <param name="updateInstruments">The option flag to update instruments on connection.</param>
        public DataService(
            IComponentryContainer setupContainer,
            MessagingAdapter messagingAdapter,
            ImmutableDictionary<Address, IEndpoint> addresses,
            IFixGateway fixGateway,
            bool updateInstruments)
            : base(
                NautilusService.Data,
                setupContainer,
                messagingAdapter)
        {
            this.addresses = this.addresses.ToImmutableDictionary();
            this.fixGateway = fixGateway;
            this.updateInstruments = updateInstruments;

            messagingAdapter.Send(new InitializeSwitchboard(
                Switchboard.Create(addresses),
                this.NewGuid(),
                this.TimeNow()));

            this.RegisterHandler<ConnectFixJob>(this.OnMessage);
            this.RegisterHandler<DisconnectFixJob>(this.OnMessage);
            this.RegisterHandler<FixSessionConnected>(this.OnMessage);
            this.RegisterHandler<FixSessionDisconnected>(this.OnMessage);
            this.RegisterHandler<Subscribe<BarType>>(this.OnMessage);
            this.RegisterHandler<Unsubscribe<BarType>>(this.OnMessage);

            // Wire up system
            this.fixGateway.RegisterConnectionEventReceiver(this.Endpoint);
            this.fixGateway.RegisterTickReceiver(this.addresses[DataServiceAddress.TickPublisher]);
            this.fixGateway.RegisterTickReceiver(this.addresses[DataServiceAddress.BarAggregationController]);
            this.fixGateway.RegisterInstrumentReceiver(DataServiceAddress.DatabaseTaskManager);
        }

        /// <summary>
        /// Actions to be performed when the component is started.
        /// </summary>
        public override void Start()
        {
            this.Log.Information($"Started at {this.StartTime}.");

            this.fixGateway.Connect();
            this.CreateConnectFixJob();
            this.CreateDisconnectFixJob();
        }

        /// <summary>
        /// Actions to be performed when the component is stopped.
        /// </summary>
        public override void Stop()
        {
            this.Log.Information($"Stopping...");

            this.fixGateway.Disconnect();
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
            this.fixGateway.Connect();
        }

        private void OnMessage(DisconnectFixJob message)
        {
            this.fixGateway.Disconnect();
        }

        private void OnMessage(FixSessionConnected message)
        {
            this.Log.Information($"{message.SessionId} session is connected.");

            if (this.updateInstruments)
            {
                this.fixGateway.UpdateInstrumentsSubscribeAll();
            }

            this.fixGateway.MarketDataSubscribeAll();
        }

        private void OnMessage(FixSessionDisconnected message)
        {
            this.Log.Warning($"{message.SessionId} session has been disconnected.");
        }

        private void OnMessage(Subscribe<BarType> message)
        {
            this.Log.Debug($"Received {message}");

            this.Send(DataServiceAddress.DataCollectionManager, message);
        }

        private void OnMessage(Unsubscribe<BarType> message)
        {
            this.Send(DataServiceAddress.DataCollectionManager, message);
        }
    }
}
