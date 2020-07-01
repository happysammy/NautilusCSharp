//--------------------------------------------------------------------------------------------------
// <copyright file="DataService.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
//--------------------------------------------------------------------------------------------------

using System.Threading;

namespace Nautilus.Data
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Data;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Common.Messages.Events;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Types;
    using Nautilus.Data.Messages.Commands;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging;
    using Nautilus.Scheduling;
    using Nautilus.Service;
    using NodaTime;

    /// <summary>
    /// Provides a data service.
    /// </summary>
    public sealed class DataService : NautilusServiceBase
    {
        private readonly DataBusAdapter dataBus;
        private readonly List<Address> managedComponents;
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
        /// <param name="dataBusAdapter">The data bus adapter.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="dataGateway">The data gateway.</param>
        /// <param name="config">The service configuration.</param>
        /// <exception cref="ArgumentException">If the addresses is empty.</exception>
        public DataService(
            IComponentryContainer container,
            MessageBusAdapter messageBusAdapter,
            DataBusAdapter dataBusAdapter,
            IScheduler scheduler,
            IDataGateway dataGateway,
            ServiceConfiguration config)
            : base(
                container,
                messageBusAdapter,
                scheduler,
                config.FixConfig)
        {
            this.dataBus = dataBusAdapter;
            this.dataGateway = dataGateway;
            this.managedComponents = new List<Address>
            {
                ServiceAddress.DataServer,
                ServiceAddress.DataPublisher,
                ServiceAddress.TickRepository,
                ServiceAddress.TickPublisher,
                ServiceAddress.TickProvider,
                ServiceAddress.BarRepository,
                ServiceAddress.BarProvider,
                ServiceAddress.InstrumentRepository,
                ServiceAddress.InstrumentProvider,
            };

            this.subscribingSymbols = config.DataConfig.SubscribingSymbols;
            this.barSpecifications = config.DataConfig.BarSpecifications;

            this.trimTimeTicks = config.DataConfig.TickDataTrimTime;
            this.trimTimeBars = config.DataConfig.BarDataTrimTime;
            this.trimWindowDaysTicks = config.DataConfig.TickDataTrimWindowDays;
            this.trimWindowDaysBars = config.DataConfig.BarDataTrimWindowDays;

            this.RegisterConnectionAddress(ServiceAddress.DataGateway);

            // Commands
            this.RegisterHandler<TrimTickData>(this.OnMessage);
            this.RegisterHandler<TrimBarData>(this.OnMessage);

            // Events
            this.RegisterHandler<MarketOpened>(this.OnMessage);
            this.RegisterHandler<MarketClosed>(this.OnMessage);
        }

        /// <inheritdoc />
        protected override void OnServiceStart(Start start)
        {
            this.CreateMarketOpenedJob();
            this.CreateMarketClosedJob();

            this.CreateTrimTickDataJob();
            this.CreateTrimBarDataJob();

            // Forward start message
            this.Send(start, this.managedComponents);
        }

        /// <inheritdoc />
        protected override void OnServiceStop(Stop stop)
        {
            // Forward stop message
            this.Send(stop, this.managedComponents);

            // Message bus already stopping in service base
            this.dataBus.Stop();
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
            this.Logger.LogInformation($"Received {message}.");

            // Forward message
            this.Send(message, ServiceAddress.BarAggregationController);

            this.CreateMarketClosedJob();
        }

        private void OnMessage(MarketClosed message)
        {
            this.Logger.LogInformation($"Received {message}.");

            // Forward message
            this.Send(message, ServiceAddress.BarAggregationController);

            this.CreateMarketOpenedJob();
        }

        private void OnMessage(TrimTickData message)
        {
            this.Logger.LogInformation($"Received {message}.");

            // Forward message
            this.Send(message, ServiceAddress.TickRepository);

            this.CreateTrimTickDataJob();
        }

        private void OnMessage(TrimBarData message)
        {
            this.Logger.LogInformation($"Received {message}.");

            // Forward message
            this.Send(message, ServiceAddress.BarRepository);

            this.CreateTrimBarDataJob();
        }

        private void CreateMarketOpenedJob()
        {
            var weeklyTime = new WeeklyTime(IsoDayOfWeek.Sunday, new LocalTime(21, 00));
            var now = this.InstantNow();

            foreach (var symbol in this.subscribingSymbols)
            {
                var nextTime = TimingProvider.GetNextUtc(weeklyTime, now);
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

                this.Logger.LogInformation($"Created scheduled event {marketOpened}-{symbol.Value} for {nextTime.ToIso8601String()}");
            }
        }

        private void CreateMarketClosedJob()
        {
            var weeklyTime = new WeeklyTime(IsoDayOfWeek.Saturday, new LocalTime(20, 00));
            var now = this.InstantNow();

            foreach (var symbol in this.subscribingSymbols)
            {
                var nextTime = TimingProvider.GetNextUtc(weeklyTime, now);
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

                this.Logger.LogInformation($"Created scheduled event {marketClosed}-{symbol.Value} for {nextTime.ToIso8601String()}");
            }
        }

        private void CreateTrimTickDataJob()
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

            this.Logger.LogInformation($"Created scheduled job {job} for {nextTime.ToIso8601String()}");
        }

        private void CreateTrimBarDataJob()
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

            this.Logger.LogInformation($"Created scheduled job {job} for {nextTime.ToIso8601String()}");
        }
    }
}
