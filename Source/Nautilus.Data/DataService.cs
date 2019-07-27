//--------------------------------------------------------------------------------------------------
// <copyright file="DataService.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data
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
    using Nautilus.Data.Messages.Commands;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging;
    using Nautilus.Messaging.Interfaces;
    using Nautilus.Scheduler;
    using NodaTime;

    /// <summary>
    /// Provides a data service.
    /// </summary>
    public sealed class DataService : MessageBusConnected
    {
        private readonly IScheduler scheduler;
        private readonly IDataGateway dataGateway;
        private readonly IReadOnlyCollection<Symbol> subscribingSymbols;
        private readonly IReadOnlyCollection<BarSpecification> barSpecifications;
        private readonly (IsoDayOfWeek Day, LocalTime Time) fixConnectTime;
        private readonly (IsoDayOfWeek Day, LocalTime Time) fixDisconnectTime;
        private readonly (IsoDayOfWeek Day, LocalTime Time) tickDataTrimTime;
        private readonly (IsoDayOfWeek Day, LocalTime Time) barDataTrimTime;
        private readonly int tickRollingWindowDays;
        private readonly int barRollingWindowDays;

        private bool hasSentBarSubscriptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataService"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="messageBusAdapter">The messaging adapter.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="dataGateway">The data gateway.</param>
        /// <param name="addresses">The data service address dictionary.</param>
        /// <param name="config">The service configuration.</param>
        /// <exception cref="ArgumentException">If the addresses is empty.</exception>
        public DataService(
            IComponentryContainer container,
            MessageBusAdapter messageBusAdapter,
            Dictionary<Address, IEndpoint> addresses,
            IScheduler scheduler,
            IDataGateway dataGateway,
            Configuration config)
            : base(container, messageBusAdapter)
        {
            Condition.NotEmpty(addresses, nameof(addresses));

            this.scheduler = scheduler;
            this.dataGateway = dataGateway;
            this.subscribingSymbols = config.SubscribingSymbols;
            this.barSpecifications = config.BarSpecifications;

            this.fixConnectTime = config.FixConfiguration.ConnectTime;
            this.fixDisconnectTime = config.FixConfiguration.DisconnectTime;
            this.tickDataTrimTime = config.TickDataTrimTime;
            this.tickRollingWindowDays = config.TickDataTrimWindowDays;
            this.barDataTrimTime = config.BarDataTrimTime;
            this.barRollingWindowDays = config.BarDataTrimWindowDays;

            addresses.Add(ServiceAddress.DataService, this.Endpoint);
            messageBusAdapter.Send(new InitializeSwitchboard(
                Switchboard.Create(addresses),
                this.NewGuid(),
                this.TimeNow()));

            this.RegisterHandler<ConnectFix>(this.OnMessage);
            this.RegisterHandler<DisconnectFix>(this.OnMessage);
            this.RegisterHandler<FixSessionConnected>(this.OnMessage);
            this.RegisterHandler<FixSessionDisconnected>(this.OnMessage);
            this.RegisterHandler<MarketOpened>(this.OnMessage);
            this.RegisterHandler<MarketClosed>(this.OnMessage);
            this.RegisterHandler<TrimTickData>(this.OnMessage);
            this.RegisterHandler<TrimBarData>(this.OnMessage);

            this.Subscribe<FixSessionConnected>();
            this.Subscribe<FixSessionDisconnected>();
        }

        /// <inheritdoc />
        protected override void OnStart(Start start)
        {
            this.Execute(() =>
            {
                if (TimingProvider.IsOutsideWeeklyInterval(
                    this.fixDisconnectTime,
                    this.fixConnectTime,
                    this.InstantNow()))
                {
                    this.Send(start, ServiceAddress.DataGateway);
                }
                else
                {
                    this.CreateConnectFixJob();
                }

                this.CreateMarketOpenedJob();
                this.CreateMarketClosedJob();

                // TODO: Below is crashing the service
                // this.CreateTrimTickDataJob();
                // this.CreateTrimBarDataJob();
                var receivers = new List<Address>
                {
                    ServiceAddress.TickProvider,
                    ServiceAddress.TickPublisher,
                    ServiceAddress.BarProvider,
                    ServiceAddress.BarPublisher,
                    ServiceAddress.InstrumentProvider,
                    ServiceAddress.InstrumentPublisher,
                };

                this.SendAll(start, receivers);
            });
        }

        /// <inheritdoc />
        protected override void OnStop(Stop stop)
        {
            var receivers = new List<Address>
            {
                ServiceAddress.DataGateway,
                ServiceAddress.TickProvider,
                ServiceAddress.TickPublisher,
                ServiceAddress.BarProvider,
                ServiceAddress.BarPublisher,
                ServiceAddress.InstrumentProvider,
                ServiceAddress.InstrumentPublisher,
            };

            this.SendAll(stop, receivers);
        }

        private void OnMessage(ConnectFix message)
        {
            // Forward message
            this.Send(message, ServiceAddress.DataGateway);
        }

        private void OnMessage(DisconnectFix message)
        {
            // Forward message
            this.Send(message, ServiceAddress.DataGateway);
        }

        private void OnMessage(FixSessionConnected message)
        {
            this.Log.Information($"{message.SessionId} session is connected.");

            this.dataGateway.UpdateInstrumentsSubscribeAll();
            if (!this.hasSentBarSubscriptions)
            {
                foreach (var symbol in this.subscribingSymbols)
                {
                    this.dataGateway.MarketDataSubscribe(symbol);

                    foreach (var barSpec in this.barSpecifications)
                    {
                        var barType = new BarType(symbol, barSpec);
                        var subscribe = new Subscribe<BarType>(
                            barType,
                            this.Mailbox,
                            this.NewGuid(),
                            this.TimeNow());
                        this.Send(subscribe, ServiceAddress.BarAggregationController);
                    }
                }

                this.hasSentBarSubscriptions = true;
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

            // Forward message
            this.Send(message, ServiceAddress.BarAggregationController);

            this.CreateMarketClosedJob();
        }

        private void OnMessage(MarketClosed message)
        {
            this.Log.Information($"Received {message}.");

            // Forward message
            this.Send(message, ServiceAddress.BarAggregationController);

            this.CreateMarketOpenedJob();
        }

        private void OnMessage(TrimTickData message)
        {
            this.Log.Information($"Received {message}.");

            // Forward message
            this.Send(message, ServiceAddress.TickStore);

            this.CreateTrimTickDataJob();
        }

        private void OnMessage(TrimBarData message)
        {
            this.Log.Information($"Received {message}.");

            // Forward message
            this.Send(message, ServiceAddress.DatabaseTaskManager);

            this.CreateTrimBarDataJob();
        }

        private void CreateConnectFixJob()
        {
            this.Execute(() =>
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
            });
        }

        private void CreateDisconnectFixJob()
        {
            this.Execute(() =>
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
            });
        }

        private void CreateMarketOpenedJob()
        {
            this.Execute(() =>
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

                    this.Log.Information($"Created scheduled event {marketOpened}-{symbol} for {nextTime.ToIsoString()}");
                }
            });
        }

        private void CreateMarketClosedJob()
        {
            this.Execute(() =>
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

                    this.Log.Information($"Created scheduled event {marketClosed}-{symbol} for {nextTime.ToIsoString()}");
                }
            });
        }

        private void CreateTrimTickDataJob()
        {
            this.Execute(() =>
            {
                var now = this.InstantNow();
                var nextTime = TimingProvider.GetNextUtc(
                    this.tickDataTrimTime.Day,
                    this.tickDataTrimTime.Time,
                    now);
                var durationToNext = TimingProvider.GetDurationToNextUtc(nextTime, now);

                var job = new TrimTickData(
                    nextTime - Duration.FromDays(this.tickRollingWindowDays),
                    nextTime,
                    this.NewGuid(),
                    this.TimeNow());

                this.scheduler.ScheduleSendOnceCancelable(
                    durationToNext,
                    this.Endpoint,
                    job,
                    this.Endpoint);

                this.Log.Information($"Created scheduled job {job} for {nextTime.ToIsoString()}");
            });
        }

        private void CreateTrimBarDataJob()
        {
            this.Execute(() =>
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
            });
        }
    }
}
