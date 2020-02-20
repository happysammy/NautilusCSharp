//--------------------------------------------------------------------------------------------------
// <copyright file="BarAggregationController.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Aggregation
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.Extensions.Logging;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Data;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Common.Messages.Events;
    using Nautilus.Core.Extensions;
    using Nautilus.Data.Messages.Commands;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging.Interfaces;
    using Nautilus.Scheduling;
    using NodaTime;

    /// <summary>
    /// Provides a bar aggregation controller to manage bar aggregators for many symbols.
    /// </summary>
    public sealed class BarAggregationController : DataBusConnected
    {
        private readonly IComponentryContainer storedContainer;
        private readonly IScheduler scheduler;
        private readonly Dictionary<Symbol, BarAggregator> barAggregators;
        private readonly Dictionary<BarType, ICancelable?> subscriptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="BarAggregationController"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="dataBusAdapter">The messaging adapter.</param>
        /// <param name="scheduler">The scheduler.</param>
        public BarAggregationController(
            IComponentryContainer container,
            IDataBusAdapter dataBusAdapter,
            IScheduler scheduler)
            : base(container, dataBusAdapter)
        {
            this.storedContainer = container;
            this.scheduler = scheduler;
            this.barAggregators = new Dictionary<Symbol, BarAggregator>();
            this.subscriptions = new Dictionary<BarType, ICancelable?>();

            this.RegisterHandler<Tick>(this.OnMessage);
            this.RegisterHandler<BarData>(this.OnMessage);
            this.RegisterHandler<Subscribe<BarType>>(this.OnMessage);
            this.RegisterHandler<Unsubscribe<BarType>>(this.OnMessage);
            this.RegisterHandler<MarketOpened>(this.OnMessage);
            this.RegisterHandler<MarketClosed>(this.OnMessage);

            this.Subscribe<Tick>();
        }

        /// <summary>
        /// Gets the dictionary of bar aggregators.
        /// </summary>
        public IReadOnlyDictionary<Symbol, BarAggregator> BarAggregators => this.barAggregators.ToImmutableDictionary();

        /// <summary>
        /// Gets the dictionary of bar subscriptions.
        /// </summary>
        public IReadOnlyDictionary<BarType, ICancelable?> Subscriptions => this.subscriptions.ToImmutableDictionary();

        /// <summary>
        /// Actions to be performed on component stop. Called when a Stop message is received.
        /// After this method is called the component state will become Stopped.
        /// Note: A log event will be created for the Stop message.
        /// </summary>
        /// <param name="stop">The stop message.</param>
        protected override void OnStop(Stop stop)
        {
        }

        private static bool IsMarketOpen(Instant now)
        {
            return TimingProvider.IsOutsideInterval(
                (IsoDayOfWeek.Saturday, new LocalTime(20, 00)),
                (IsoDayOfWeek.Sunday, new LocalTime(21, 00)),
                now);
        }

        private void OnMessage(Tick tick)
        {
            if (this.barAggregators.TryGetValue(tick.Symbol, out var aggregator))
            {
                aggregator.Endpoint.Send(tick);
            }
            else
            {
                this.Logger.LogWarning($"No bar aggregator for {tick.Symbol} ticks.");
            }
        }

        private void OnMessage(BarData data)
        {
            this.SendToBus(data);
        }

        private void OnMessage(Subscribe<BarType> message)
        {
            var symbol = message.Subscription.Symbol;
            var barSpec = message.Subscription.Specification;
            var barType = message.Subscription;

            if (!this.barAggregators.ContainsKey(symbol))
            {
                var barAggregator = new BarAggregator(
                        this.storedContainer,
                        this.Endpoint,
                        symbol);

                this.barAggregators.Add(message.Subscription.Symbol, barAggregator);

                this.Logger.LogDebug($"Created BarAggregator[{symbol}].");
            }

            if (this.subscriptions.ContainsKey(message.Subscription))
            {
                this.Logger.LogError($"Already subscribed to {barType}.");
                return;
            }

            this.barAggregators[symbol].Endpoint.Send(message);

            if (IsMarketOpen(this.InstantNow()))
            {
                // Create close bar job schedule
                var initialDelay = (this.TimeNow().Floor(barSpec.Duration) + barSpec.Duration) - this.TimeNow();
                var cancellable = this.scheduler.ScheduleRepeatedlyCancelable(
                    initialDelay,
                    barSpec.Duration,
                    () =>
                    {
                        this.CreateCloseBarDelegate(barSpec, this.barAggregators[symbol].Endpoint);
                    });

                this.subscriptions.Add(barType, cancellable);
            }
            else
            {
                this.subscriptions.Add(barType, null);
            }

            this.Logger.LogInformation($"Building {message.Subscription} bars.");
        }

        private void CreateCloseBarDelegate(BarSpecification barSpec, IEndpoint aggregator)
        {
            aggregator.Send(new CloseBar(
                barSpec,
                this.TimeNow().Floor(barSpec.Duration),
                this.NewGuid(),
                this.TimeNow()));
        }

        private void OnMessage(Unsubscribe<BarType> message)
        {
            var symbol = message.Subscription.Symbol;
            var barType = message.Subscription;

            if (!this.barAggregators.ContainsKey(symbol) || !this.subscriptions.ContainsKey(barType))
            {
                this.Logger.LogError($"Already unsubscribed from {barType}.");
                return;
            }

            this.barAggregators[symbol].Endpoint.Send(message);

            if (this.subscriptions[barType] != null)
            {
                // Cancel close bar job schedule
                this.subscriptions[barType]?.Cancel();
            }

            this.subscriptions.Remove(barType);

            if (this.subscriptions.All(s => s.Key.Symbol != symbol))
            {
                this.barAggregators.Remove(symbol);
                this.Logger.LogDebug($"Removed BarAggregator[{symbol}].");
            }

            this.Logger.LogInformation($"Stopped building {message.Subscription} bars.");
        }

        private void OnMessage(MarketOpened message)
        {
            foreach (var (barType, buildingJob) in this.SubscriptionsForSymbol(message.Symbol))
            {
                // Cancel old scheduled job if it still exists (this shouldn't need to happen)
                if (buildingJob != null)
                {
                    buildingJob.Cancel();
                    this.subscriptions[barType] = null;
                }

                // Create close bar job schedule
                var barDuration = barType.Specification.Duration;
                var initialDelay = TimingProvider.GetDelayToNextDuration(this.TimeNow(), barDuration);
                var scheduledCancelable = this.scheduler.ScheduleRepeatedlyCancelable(
                    initialDelay,
                    barDuration,
                    () =>
                    {
                        this.CreateCloseBarDelegate(barType.Specification, this.barAggregators[barType.Symbol].Endpoint);
                    });

                this.subscriptions[barType] = scheduledCancelable;

                this.Logger.LogDebug($"MarketOpened: Started CloseBar job for {barType}.");
            }
        }

        private void OnMessage(MarketClosed message)
        {
            foreach (var (barType, buildingJob) in this.SubscriptionsForSymbol(message.Symbol))
            {
                buildingJob?.Cancel();
                this.subscriptions[barType] = null;

                this.Logger.LogDebug($"MarketClosed: Cancelled CloseBar job for {barType}.");
            }
        }

        private IEnumerable<KeyValuePair<BarType, ICancelable?>> SubscriptionsForSymbol(Symbol symbol)
        {
            return this.subscriptions.Where(s => s.Key.Symbol == symbol);
        }
    }
}
