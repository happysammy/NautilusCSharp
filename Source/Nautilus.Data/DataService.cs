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
    using System.Collections.Generic;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Common.Messages.Events;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Annotations;
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
    [PerformanceOptimized]
    public sealed class DataService : ComponentBusConnectedBase
    {
        private readonly IScheduler scheduler;
        private readonly IFixGateway fixGateway;
        private readonly IReadOnlyCollection<Symbol> subscribingSymbols;
        private readonly IReadOnlyCollection<BarSpecification> barSpecifications;
        private readonly int barRollingWindowDays;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataService"/> class.
        /// </summary>
        /// <param name="setupContainer">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="scheduler">The system scheduler.</param>
        /// <param name="fixGateway">The FIX gateway.</param>
        /// <param name="addresses">The data service address dictionary.</param>
        /// <param name="subscribingSymbols">The symbols to subscribe to.</param>
        /// <param name="barSpecifications">The bar specifications to create.</param>
        /// <param name="barRollingWindowDays">The number of days to trim bar data to.</param>
        /// <exception cref="ArgumentException">If the addresses is empty.</exception>
        /// <exception cref="ArgumentException">If the subscribing symbols is empty.</exception>
        /// <exception cref="ArgumentException">If the bar specifications is empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the barRollingWindowDays is not positive (> 0).</exception>
        public DataService(
            IComponentryContainer setupContainer,
            MessagingAdapter messagingAdapter,
            Dictionary<Address, IEndpoint> addresses,
            IScheduler scheduler,
            IFixGateway fixGateway,
            IReadOnlyCollection<Symbol> subscribingSymbols,
            IReadOnlyCollection<BarSpecification> barSpecifications,
            int barRollingWindowDays)
            : base(
                NautilusService.Data,
                setupContainer,
                messagingAdapter)
        {
            Condition.NotEmpty(addresses, nameof(addresses));
            Condition.NotEmpty(subscribingSymbols, nameof(subscribingSymbols));
            Condition.NotEmpty(barSpecifications, nameof(barSpecifications));
            Condition.PositiveInt32(barRollingWindowDays, nameof(this.barRollingWindowDays));

            this.scheduler = scheduler;
            this.fixGateway = fixGateway;
            this.subscribingSymbols = subscribingSymbols;
            this.barSpecifications = barSpecifications;
            this.barRollingWindowDays = barRollingWindowDays;

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
            this.RegisterHandler<TrimBarData>(this.OnMessage);
        }

        /// <inheritdoc />
        protected override void OnStart(Start message)
        {
            this.Log.Information($"Starting from {message}...");

            this.Send(DataServiceAddress.FixGateway, message);

            this.CreateMarketOpenedJob();
            this.CreateMarketClosedJob();
            this.CreateTrimBarDataJob();
        }

        /// <inheritdoc />
        protected override void OnStop(Stop message)
        {
            this.Log.Information($"Stopping from {message}...");

            this.Send(DataServiceAddress.DatabaseTaskManager, message);
            this.Send(DataServiceAddress.FixGateway, message);
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

            this.Log.Information($"Created {job}Job for {nextTime.ToIsoString()}");
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

            this.Log.Information($"Created {job}Job for {nextTime.ToIsoString()}");
        }

        private void CreateMarketOpenedJob()
        {
            var jobDay = IsoDayOfWeek.Sunday;
            var jobTime = new LocalTime(21, 00);
            var timeNow = this.TimeNow().ToInstant();

            foreach (var symbol in this.subscribingSymbols)
            {
                var nextTime = ZonedDateTimeExtensions.GetNextUtc(jobDay, jobTime, timeNow);
                var durationToNext = ZonedDateTimeExtensions.GetDurationToNextUtc(nextTime, this.TimeNow().ToInstant());

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

                this.Log.Information($"Created {marketOpened}Job for {nextTime.ToIsoString()}");
            }
        }

        private void CreateMarketClosedJob()
        {
            var jobDay = IsoDayOfWeek.Saturday;
            var jobTime = new LocalTime(20, 00);
            var timeNow = this.TimeNow().ToInstant();

            foreach (var symbol in this.subscribingSymbols)
            {
                var nextTime = ZonedDateTimeExtensions.GetNextUtc(jobDay, jobTime, timeNow);
                var durationToNext = ZonedDateTimeExtensions.GetDurationToNextUtc(nextTime, this.TimeNow().ToInstant());

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

                this.Log.Information($"Created {marketClosed}Job for {nextTime.ToIsoString()}");
            }
        }

        private void CreateTrimBarDataJob()
        {
            var jobDay = IsoDayOfWeek.Sunday;
            var jobTime = new LocalTime(00, 01);
            var timeNow = this.TimeNow().ToInstant();

            var nextTime = ZonedDateTimeExtensions.GetNextUtc(jobDay, jobTime, timeNow);
            var durationToNext = ZonedDateTimeExtensions.GetDurationToNextUtc(nextTime, this.TimeNow().ToInstant());

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

            this.Log.Information($"Created {job}Job for {nextTime.ToIsoString()}");
        }
    }
}
