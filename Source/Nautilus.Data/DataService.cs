﻿//--------------------------------------------------------------------------------------------------
// <copyright file="DataService.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data
{
    using System;
    using System.Collections.Generic;
    using Nautilus.Common;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Common.Messages.Events;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.Data.Messages.Commands;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging;
    using Nautilus.Messaging.Interfaces;
    using Nautilus.Scheduler;
    using NodaTime;

    /// <summary>
    /// Provides a data service.
    /// </summary>
    public sealed class DataService : ComponentBusConnected
    {
        private readonly IScheduler scheduler;
        private readonly IFixGateway fixGateway;
        private readonly IReadOnlyCollection<Symbol> subscribingSymbols;
        private readonly IReadOnlyCollection<BarSpecification> barSpecifications;
        private readonly (IsoDayOfWeek Day, LocalTime Time) fixConnectTime;
        private readonly (IsoDayOfWeek Day, LocalTime Time) fixDisconnectTime;
        private readonly (IsoDayOfWeek Day, LocalTime Time) barDataTrimTime;
        private readonly int barRollingWindowDays;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataService"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="fixGateway">The FIX gateway.</param>
        /// <param name="addresses">The data service address dictionary.</param>
        /// <param name="config">The service configuration.</param>
        /// <exception cref="ArgumentException">If the addresses is empty.</exception>
        public DataService(
            IComponentryContainer container,
            MessagingAdapter messagingAdapter,
            Dictionary<Address, IEndpoint> addresses,
            IScheduler scheduler,
            IFixGateway fixGateway,
            Configuration config)
            : base(container, messagingAdapter)
        {
            Condition.NotEmpty(addresses, nameof(addresses));

            VersionChecker.Run(this.Log, "NautilusData - Financial Market Data Service");

            this.scheduler = scheduler;
            this.fixGateway = fixGateway;
            this.subscribingSymbols = config.SubscribingSymbols;
            this.barSpecifications = config.BarSpecifications;

            this.fixConnectTime = config.FixConfiguration.ConnectTime;
            this.fixDisconnectTime = config.FixConfiguration.DisconnectTime;
            this.barDataTrimTime = config.BarDataTrimTime;
            this.barRollingWindowDays = config.BarDataTrimWindowDays;

            addresses.Add(DataServiceAddress.Core, this.Endpoint);
            messagingAdapter.Send(new InitializeSwitchboard(
                Switchboard.Create(addresses),
                this.NewGuid(),
                this.TimeNow()));

            this.RegisterHandler<ConnectFix>(this.OnMessage);
            this.RegisterHandler<DisconnectFix>(this.OnMessage);
            this.RegisterHandler<FixSessionConnected>(this.OnMessage);
            this.RegisterHandler<FixSessionDisconnected>(this.OnMessage);
            this.RegisterHandler<MarketOpened>(this.OnMessage);
            this.RegisterHandler<MarketClosed>(this.OnMessage);
            this.RegisterHandler<TrimBarData>(this.OnMessage);
        }

        /// <inheritdoc />
        protected override void OnStart(Start start)
        {
            if (TimingProvider.IsOutsideWeeklyInterval(
                this.fixDisconnectTime,
                this.fixConnectTime,
                this.InstantNow()))
            {
                this.Send(DataServiceAddress.FixGateway, start);
            }
            else
            {
                this.CreateConnectFixJob();
            }

            this.CreateMarketOpenedJob();
            this.CreateMarketClosedJob();
            this.CreateTrimBarDataJob();

            this.Send(DataServiceAddress.TickResponder, start);
            this.Send(DataServiceAddress.TickPublisher, start);
            this.Send(DataServiceAddress.BarResponder, start);
            this.Send(DataServiceAddress.BarPublisher, start);
            this.Send(DataServiceAddress.InstrumentResponder, start);
            this.Send(DataServiceAddress.InstrumentPublisher, start);
        }

        /// <inheritdoc />
        protected override void OnStop(Stop stop)
        {
            this.Send(DataServiceAddress.DatabaseTaskManager, stop);
            this.Send(DataServiceAddress.FixGateway, stop);
            this.Send(DataServiceAddress.TickResponder, stop);
            this.Send(DataServiceAddress.TickPublisher, stop);
            this.Send(DataServiceAddress.BarResponder, stop);
            this.Send(DataServiceAddress.BarPublisher, stop);
            this.Send(DataServiceAddress.InstrumentResponder, stop);
            this.Send(DataServiceAddress.InstrumentPublisher, stop);
        }

        private void OnMessage(ConnectFix message)
        {
            // Forward message.
            this.Send(DataServiceAddress.FixGateway, message);
        }

        private void OnMessage(DisconnectFix message)
        {
            // Forward message.
            this.Send(DataServiceAddress.FixGateway, message);
        }

        private void OnMessage(FixSessionConnected message)
        {
            this.Log.Information($"{message.SessionId} session is connected.");

            this.fixGateway.UpdateInstrumentsSubscribeAll();

            foreach (var symbol in this.subscribingSymbols)
            {
                this.fixGateway.MarketDataSubscribe(symbol);

                foreach (var barSpec in this.barSpecifications)
                {
                    var barType = new BarType(symbol, barSpec);
                    var subscribe = new Subscribe<BarType>(
                        barType,
                        this.NewGuid(),
                        this.TimeNow());
                    this.Send(DataServiceAddress.BarAggregationController, subscribe);
                }
            }

            this.CreateDisconnectFixJob();
            this.CreateConnectFixJob();
        }

        private void OnMessage(FixSessionDisconnected message)
        {
            this.Log.Warning($"{message.SessionId} session has been disconnected.");
        }

        private void OnMessage(MarketOpened message)
        {
            this.Log.Information($"Received {message}.");

            // Forward message.
            this.Send(DataServiceAddress.BarAggregationController, message);

            this.CreateMarketClosedJob();
        }

        private void OnMessage(MarketClosed message)
        {
            this.Log.Information($"Received {message}.");

            // Forward message.
            this.Send(DataServiceAddress.BarAggregationController, message);

            this.CreateMarketOpenedJob();
        }

        private void OnMessage(TrimBarData message)
        {
            this.Log.Information($"Received {message}.");

            // Forward message.
            this.Send(DataServiceAddress.DatabaseTaskManager, message);

            this.CreateTrimBarDataJob();
        }

        private void CreateConnectFixJob()
        {
            var now = this.InstantNow();
            var nextTime = TimingProvider.GetNextUtc(
                this.fixConnectTime.Day,
                this.fixConnectTime.Time,
                now);
            var durationToNext = TimingProvider.GetDurationToNextUtc(nextTime, now);

            var job = new ConnectFix(
                nextTime,
                this.NewGuid(),
                this.TimeNow());

            this.scheduler.ScheduleSendOnceCancelable(
                durationToNext,
                this.Endpoint,
                job,
                this.Endpoint);

            this.Log.Information($"Created scheduled job {job} for {nextTime.ToIsoString()}");
        }

        private void CreateDisconnectFixJob()
        {
            var now = this.InstantNow();
            var nextTime = TimingProvider.GetNextUtc(
                this.fixDisconnectTime.Day,
                this.fixDisconnectTime.Time,
                now);
            var durationToNext = TimingProvider.GetDurationToNextUtc(nextTime, now);

            var job = new DisconnectFix(
                nextTime,
                this.NewGuid(),
                this.TimeNow());

            this.scheduler.ScheduleSendOnceCancelable(
                durationToNext,
                this.Endpoint,
                job,
                this.Endpoint);

            this.Log.Information($"Created scheduled job {job} for {nextTime.ToIsoString()}");
        }

        private void CreateMarketOpenedJob()
        {
            var jobDay = IsoDayOfWeek.Sunday;
            var jobTime = new LocalTime(21, 00);
            var now = this.InstantNow();

            foreach (var symbol in this.subscribingSymbols)
            {
                var nextTime = TimingProvider.GetNextUtc(jobDay, jobTime, now);
                var durationToNext = TimingProvider.GetDurationToNextUtc(nextTime, this.InstantNow());

                var marketOpened = new MarketOpened(
                    symbol,
                    nextTime,
                    this.NewGuid(),
                    this.TimeNow());

                this.scheduler.ScheduleSendOnceCancelable(
                    durationToNext,
                    this.Endpoint,
                    marketOpened,
                    this.Endpoint);

                this.Log.Information($"Created scheduled event {marketOpened}Event[{symbol}] for {nextTime.ToIsoString()}");
            }
        }

        private void CreateMarketClosedJob()
        {
            var jobDay = IsoDayOfWeek.Saturday;
            var jobTime = new LocalTime(20, 00);
            var now = this.InstantNow();

            foreach (var symbol in this.subscribingSymbols)
            {
                var nextTime = TimingProvider.GetNextUtc(jobDay, jobTime, now);
                var durationToNext = TimingProvider.GetDurationToNextUtc(nextTime, this.InstantNow());

                var marketClosed = new MarketClosed(
                    symbol,
                    nextTime,
                    this.NewGuid(),
                    this.TimeNow());

                this.scheduler.ScheduleSendOnceCancelable(
                    durationToNext,
                    this.Endpoint,
                    marketClosed,
                    this.Endpoint);

                this.Log.Information($"Created scheduled event {marketClosed}Event[{symbol}] for {nextTime.ToIsoString()}");
            }
        }

        private void CreateTrimBarDataJob()
        {
            var now = this.InstantNow();
            var nextTime = TimingProvider.GetNextUtc(
                this.barDataTrimTime.Day,
                this.barDataTrimTime.Time,
                now);
            var durationToNext = TimingProvider.GetDurationToNextUtc(nextTime, now);

            var job = new TrimBarData(
                this.barSpecifications,
                this.barRollingWindowDays,
                nextTime,
                this.NewGuid(),
                this.TimeNow());

            this.scheduler.ScheduleSendOnceCancelable(
                durationToNext,
                this.Endpoint,
                job,
                this.Endpoint);

            this.Log.Information($"Created scheduled job {job} for {nextTime.ToIsoString()}");
        }
    }
}
