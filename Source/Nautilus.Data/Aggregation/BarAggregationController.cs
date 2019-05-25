//--------------------------------------------------------------------------------------------------
// <copyright file="BarAggregationController.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Aggregation
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Documents;
    using Nautilus.Common.Messages.Events;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Extensions;
    using Nautilus.Data.Messages.Commands;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging.Interfaces;
    using Nautilus.Scheduler;
    using NodaTime;

    /// <summary>
    /// Provides a bar aggregation controller to manage bar aggregators for many symbols.
    /// </summary>
    [PerformanceOptimized]
    public sealed class BarAggregationController : ComponentBusConnectedBase
    {
        private readonly IComponentryContainer storedContainer;
        private readonly IScheduler scheduler;
        private readonly IEndpoint barPublisher;
        private readonly Dictionary<Symbol, BarAggregator> barAggregators;
        private readonly Dictionary<BarType, ICancelable?> subscriptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="BarAggregationController"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="barPublisher">The bar publisher endpoint.</param>
        public BarAggregationController(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            IScheduler scheduler,
            IEndpoint barPublisher)
            : base(container, messagingAdapter)
        {
            this.storedContainer = container;
            this.scheduler = scheduler;
            this.barPublisher = barPublisher;
            this.barAggregators = new Dictionary<Symbol, BarAggregator>();
            this.subscriptions = new Dictionary<BarType, ICancelable?>();

            this.RegisterHandler<Tick>(this.OnMessage);
            this.RegisterHandler<(BarType, Bar)>(this.OnMessage);
            this.RegisterHandler<Subscribe<BarType>>(this.OnMessage);
            this.RegisterHandler<Unsubscribe<BarType>>(this.OnMessage);
            this.RegisterHandler<MarketOpened>(this.OnMessage);
            this.RegisterHandler<MarketClosed>(this.OnMessage);
        }

        /// <summary>
        /// Gets the dictionary of bar aggregators.
        /// </summary>
        public IReadOnlyDictionary<Symbol, BarAggregator> BarAggregators => this.barAggregators.ToImmutableDictionary();

        /// <summary>
        /// Gets the dictionary of bar subscriptions.
        /// </summary>
        public IReadOnlyDictionary<BarType, ICancelable?> Subscriptions => this.subscriptions.ToImmutableDictionary();

        private static bool IsMarketOpen(Instant now)
        {
            return TimingProvider.IsOutsideWeeklyInterval(
                (IsoDayOfWeek.Saturday, new LocalTime(20, 00)),
                (IsoDayOfWeek.Sunday, new LocalTime(21, 00)),
                now);
        }

        private void OnMessage(Tick tick)
        {
            if (this.barAggregators.ContainsKey(tick.Symbol))
            {
                this.barAggregators[tick.Symbol].Endpoint.Send(tick);

                return;
            }

            // Log for debugging purposes.
            this.Log.Warning($"No bar aggregator for {tick.Symbol} ticks.");
        }

        private void OnMessage((BarType, Bar) data)
        {
            // Forward data to bar publisher.
            this.barPublisher.Send(data);

            var dataDelivery = new DataDelivery<(BarType, Bar)>(
                data,
                this.NewGuid(),
                this.TimeNow());

            this.Send(DataServiceAddress.DatabaseTaskManager, dataDelivery);
        }

        private void OnMessage(Subscribe<BarType> message)
        {
            var symbol = message.DataType.Symbol;
            var barType = message.DataType;
            var barSpec = message.DataType.Specification;

            if (!this.barAggregators.ContainsKey(symbol))
            {
                var barAggregator = new BarAggregator(
                        this.storedContainer,
                        this.Endpoint,
                        symbol);

                this.barAggregators.Add(message.DataType.Symbol, barAggregator);

                this.Log.Debug($"Created BarAggregator[{symbol}].");
            }

            if (this.subscriptions.ContainsKey(message.DataType))
            {
                this.Log.Error($"Already subscribed to {barType}.");
                return;
            }

            this.barAggregators[symbol].Endpoint.Send(message);

            if (IsMarketOpen(this.InstantNow()))
            {
                // Create close bar job schedule.
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

            this.Log.Information($"Subscribed to {message.DataType} bars.");
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
            var symbol = message.DataType.Symbol;
            var barType = message.DataType;

            if (!this.barAggregators.ContainsKey(symbol) || !this.subscriptions.ContainsKey(barType))
            {
                this.Log.Error($"Already unsubscribed from {barType}.");
                return;
            }

            this.barAggregators[symbol].Endpoint.Send(message);

            if (this.subscriptions[barType] != null)
            {
                // Cancel close bar job schedule.
                this.subscriptions[barType]?.Cancel();
            }

            this.subscriptions.Remove(barType);

            if (this.subscriptions.All(s => s.Key.Symbol != symbol))
            {
                this.barAggregators.Remove(symbol);
                this.Log.Debug($"Removed BarAggregator[{symbol}].");
            }

            this.Log.Information($"Unsubscribed from {message.DataType} bars.");
        }

        private void OnMessage(MarketOpened message)
        {
            foreach (var subscription in this.SubscriptionsForSymbol(message.Symbol))
            {
                // Cancel old scheduled job if it still exists (this shouldn't need to happen).
                if (subscription.Value != null)
                {
                    this.subscriptions[subscription.Key]?.Cancel();
                    this.subscriptions[subscription.Key] = null;
                }

                // Create close bar job schedule.
                var barDuration = subscription.Key.Specification.Duration;
                var initialDelay = TimingProvider.GetDelayToNextDuration(this.TimeNow(), barDuration);
                var scheduledCancelable = this.scheduler.ScheduleRepeatedlyCancelable(
                    initialDelay,
                    barDuration,
                    () =>
                    {
                        this.CreateCloseBarDelegate(subscription.Key.Specification, this.barAggregators[subscription.Key.Symbol].Endpoint);
                    });

                this.subscriptions[subscription.Key] = scheduledCancelable;

                this.Log.Debug($"MarketOpened: Started CloseBar job for {subscription.Key}.");
            }
        }

        private void OnMessage(MarketClosed message)
        {
            foreach (var subscription in this.SubscriptionsForSymbol(message.Symbol))
            {
                subscription.Value?.Cancel();
                this.subscriptions[subscription.Key] = null;

                this.Log.Debug($"MarketClosed: Cancelled CloseBar job for {subscription.Key}.");
            }
        }

        private IEnumerable<KeyValuePair<BarType, ICancelable?>> SubscriptionsForSymbol(Symbol symbol)
        {
            return this.subscriptions.Where(s => s.Key.Symbol == symbol);
        }
    }
}
