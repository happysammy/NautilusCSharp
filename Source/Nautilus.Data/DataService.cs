//--------------------------------------------------------------------------------------------------
// <copyright file="DataService.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
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
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging;
    using Nautilus.Messaging.Interfaces;
    using Nautilus.Scheduler;
    using Nautilus.Service;
    using NodaTime;

    /// <summary>
    /// Provides a data service.
    /// </summary>
    public sealed class DataService : NautilusServiceBase
    {
        private readonly IDataGateway dataGateway;
        private readonly IReadOnlyCollection<Symbol> subscribingSymbols;
        private readonly IReadOnlyCollection<BarSpecification> barSpecifications;
        private readonly LocalTime trimTimeTicks;
        private readonly LocalTime trimTimeBars;
        private readonly int trimWindowDaysTicks;
        private readonly int trimWindowDaysBars;

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
            : base(
                container,
                messageBusAdapter,
                scheduler,
                config.FixConfiguration)
        {
            Condition.NotEmpty(addresses, nameof(addresses));

            this.dataGateway = dataGateway;
            this.subscribingSymbols = config.SubscribingSymbols;
            this.barSpecifications = config.BarSpecifications;

            this.trimTimeTicks = config.TickDataTrimTime;
            this.trimWindowDaysTicks = config.TickDataTrimWindowDays;
            this.trimTimeBars = config.BarDataTrimTime;
            this.trimWindowDaysBars = config.BarDataTrimWindowDays;

            addresses.Add(ServiceAddress.DataService, this.Endpoint);
            messageBusAdapter.Send(new InitializeSwitchboard(
                Switchboard.Create(addresses),
                this.NewGuid(),
                this.TimeNow()));

            this.RegisterConnectionAddress(ServiceAddress.DataGateway);

            // Commands
            this.RegisterHandler<TrimTickData>(this.OnMessage);
            this.RegisterHandler<TrimBarData>(this.OnMessage);

            // Events
            this.RegisterHandler<MarketOpened>(this.OnMessage);
            this.RegisterHandler<MarketClosed>(this.OnMessage);
        }

        /// <inheritdoc />
        protected override void ServiceStart(Start start)
        {
            this.Execute(() =>
            {
                this.CreateMarketOpenedJob();
                this.CreateMarketClosedJob();

                this.CreateTrimTickDataJob();
                this.CreateTrimBarDataJob();

                // Forward start message
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

                this.Send(start, receivers);
            });
        }

        /// <inheritdoc />
        protected override void ServiceStop(Stop stop)
        {
            // Forward stop message
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

            this.Send(stop, receivers);
        }

        /// <inheritdoc />
        protected override void OnConnected()
        {
            this.dataGateway.UpdateInstrumentsSubscribeAll();
            foreach (var symbol in this.subscribingSymbols)
            {
                this.dataGateway.MarketDataSubscribe(symbol);
            }

            if (!this.hasSentBarSubscriptions)
            {
                foreach (var symbol in this.subscribingSymbols)
                {
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
            this.Send(message, ServiceAddress.TickRepository);

            this.CreateTrimTickDataJob();
        }

        private void OnMessage(TrimBarData message)
        {
            this.Log.Information($"Received {message}.");

            // Forward message
            this.Send(message, ServiceAddress.BarRepository);

            this.CreateTrimBarDataJob();
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

                    this.Scheduler.ScheduleSendOnceCancelable(
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

                    this.Scheduler.ScheduleSendOnceCancelable(
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
                var nextTime = TimingProvider.GetNextUtc(this.trimTimeTicks, now);
                var durationToNext = TimingProvider.GetDurationToNextUtc(nextTime, now);

                var job = new TrimTickData(
                    this.trimWindowDaysTicks,
                    nextTime,
                    this.NewGuid(),
                    this.TimeNow());

                this.Scheduler.ScheduleSendOnceCancelable(
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
                var nextTime = TimingProvider.GetNextUtc(this.trimTimeBars, now);
                var durationToNext = TimingProvider.GetDurationToNextUtc(nextTime, now);

                var job = new TrimBarData(
                    this.barSpecifications,
                    this.trimWindowDaysBars,
                    nextTime,
                    this.NewGuid(),
                    this.TimeNow());

                this.Scheduler.ScheduleSendOnceCancelable(
                    durationToNext,
                    this.Endpoint,
                    job,
                    this.Endpoint);

                this.Log.Information($"Created scheduled job {job} for {nextTime.ToIsoString()}");
            });
        }
    }
}
