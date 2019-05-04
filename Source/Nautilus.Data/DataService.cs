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
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Common.Messages.Events;
    using Nautilus.Common.Messages.Jobs;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Annotations;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;
    using Quartz;

    /// <summary>
    /// The main macro object which contains the <see cref="DataService"/> and presents its API.
    /// </summary>
    [PerformanceOptimized]
    public sealed class DataService : ComponentBusConnectedBase
    {
        private readonly IFixGateway gateway;
        private readonly bool updateInstruments;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataService"/> class.
        /// </summary>
        /// <param name="setupContainer">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="gateway">The FIX gateway.</param>
        /// <param name="updateInstruments">The option flag to update instruments on connection.</param>
        public DataService(
            IComponentryContainer setupContainer,
            IMessagingAdapter messagingAdapter,
            IFixGateway gateway,
            bool updateInstruments)
            : base(
                NautilusService.Data,
                LabelFactory.Create(nameof(DataService)),
                setupContainer,
                messagingAdapter)
        {
            this.gateway = gateway;
            this.updateInstruments = updateInstruments;

            this.RegisterHandler<ConnectFixJob>(this.OnMessage);
            this.RegisterHandler<DisconnectFixJob>(this.OnMessage);
            this.RegisterHandler<FixSessionConnected>(this.OnMessage);
            this.RegisterHandler<FixSessionDisconnected>(this.OnMessage);
            this.RegisterHandler<Subscribe<BarType>>(this.OnMessage);
            this.RegisterHandler<Unsubscribe<BarType>>(this.OnMessage);
        }

        /// <summary>
        /// Start method called when the <see cref="StartSystem"/> message is received.
        /// </summary>
        public override void Start()
        {
            this.Log.Information($"Started at {this.StartTime}.");

            this.CreateConnectFixJob();
            this.CreateDisconnectFixJob();
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
                    this.Endpoint,
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
                    this.Endpoint,
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

        private void OnMessage(FixSessionConnected message)
        {
            this.Log.Information($"{message.SessionId} session is connected.");

            if (this.updateInstruments)
            {
                this.gateway.UpdateInstrumentsSubscribeAll();
            }

            this.gateway.MarketDataSubscribeAll();
        }

        private void OnMessage(FixSessionDisconnected message)
        {
            this.Log.Warning($"{message.SessionId} session has been disconnected.");
        }

        private void OnMessage(Subscribe<BarType> message)
        {
            this.Send(DataServiceAddress.DataCollectionManager, message);
        }

        private void OnMessage(Unsubscribe<BarType> message)
        {
            this.Send(DataServiceAddress.DataCollectionManager, message);
        }
    }
}
