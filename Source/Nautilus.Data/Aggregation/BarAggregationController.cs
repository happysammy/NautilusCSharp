//--------------------------------------------------------------------------------------------------
// <copyright file="BarAggregationController.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Aggregation
{
    using System;
    using System.Collections.Generic;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Documents;
    using Nautilus.Common.Messages.Events;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Extensions;
    using Nautilus.Data.Messages.Commands;
    using Nautilus.Data.Messages.Jobs;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging.Interfaces;
    using Nautilus.Scheduler;

    /// <summary>
    /// Provides a bar aggregation controller to manage bar aggregators for many symbols.
    /// </summary>
    [PerformanceOptimized]
    public sealed class BarAggregationController : ComponentBusConnectedBase
    {
        private readonly IComponentryContainer storedContainer;
        private readonly IScheduler scheduler;
        private readonly IEndpoint barPublisher;
        private readonly Dictionary<Symbol, IEndpoint> barAggregators;
        private readonly Dictionary<BarType, ICancelable> subscriptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="BarAggregationController"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="barPublisher">The bar publisher endpoint.</param>
        public BarAggregationController(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            IScheduler scheduler,
            IEndpoint barPublisher)
            : base(
            NautilusService.Data,
            container,
            messagingAdapter)
        {
            this.storedContainer = container;
            this.scheduler = scheduler;
            this.barPublisher = barPublisher;
            this.barAggregators = new Dictionary<Symbol, IEndpoint>();
            this.subscriptions = new Dictionary<BarType, ICancelable>();

            this.RegisterHandler<Tick>(this.OnMessage);
            this.RegisterHandler<(BarType, Bar)>(this.OnMessage);
            this.RegisterHandler<Subscribe<BarType>>(this.OnMessage);
            this.RegisterHandler<Unsubscribe<BarType>>(this.OnMessage);
            this.RegisterHandler<MarketStatusJob>(this.OnMessage);
        }

        private void OnMessage(Tick tick)
        {
            if (this.barAggregators.ContainsKey(tick.Symbol))
            {
                this.barAggregators[tick.Symbol].Send(tick);

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

            this.Log.Debug($"Received {data.Item1}({data.Item2}).");
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
                        symbol).Endpoint;

                this.barAggregators.Add(message.DataType.Symbol, barAggregator);
            }

            this.barAggregators[symbol].Send(message);

            if (this.subscriptions.ContainsKey(message.DataType))
            {
                this.Log.Error($"Already subscribed to {barType}.");
                return;
            }

            // Create close bar job schedule.
            var closeBar = new CloseBar(
                barSpec,
                Guid.NewGuid(),
                this.TimeNow());

            var initialDelay = (this.TimeNow().Floor(barSpec.Duration) + barSpec.Duration) - this.TimeNow();
            var cancellable = this.scheduler.ScheduleSendRepeatedlyCancelable(
                initialDelay,
                barSpec.Duration,
                this.barAggregators[symbol],
                closeBar,
                this.Endpoint);

            this.subscriptions.Add(barType, cancellable);
        }

        private void OnMessage(Unsubscribe<BarType> message)
        {
            var symbol = message.DataType.Symbol;
            var barType = message.DataType;

            if (!this.barAggregators.ContainsKey(symbol))
            {
                // Nothing to unsubscribe from.
                return;
            }

            this.barAggregators[symbol].Send(message);

            if (!this.subscriptions.ContainsKey(barType))
            {
                this.Log.Error($"Already unsubscribed from {barType}.");
                return;
            }

            // Cancel close bar job schedule.
            this.subscriptions[barType].Cancel();
            this.subscriptions.Remove(barType);
        }

        private void OnMessage(MarketStatusJob job) // TODO: Change to MarketOpen and MarketClosed events.
        {
            if (job.IsMarketOpen)
            {
                foreach (var barType in this.subscriptions.Keys)
                {
                    var barSpec = barType.Specification;
                    var closeBar = new CloseBar(
                        barSpec,
                        Guid.NewGuid(),
                        this.TimeNow());

                    var initialDelay = this.TimeNow() - this.TimeNow().Floor(barSpec.Duration) + barSpec.Duration;
                    var cancellable = this.scheduler.ScheduleSendRepeatedlyCancelable(
                        initialDelay,
                        barSpec.Duration,
                        this.barAggregators[barType.Symbol],
                        closeBar,
                        this.Endpoint);

                    this.subscriptions[barType] = cancellable;
                }
            }
            else
            {
                foreach (var (subscription, cancellable) in this.subscriptions)
                {
                    cancellable.Cancel();
                    this.Log.Debug($"Market is closed: Cancelled bar close job for {subscription}.");
                }

                // Tell all aggregators the market is now closed.
                var marketClosed = new MarketClosed(this.NewGuid(), this.TimeNow());
                foreach (var aggregator in this.barAggregators.Values)
                {
                    aggregator.Send(marketClosed);
                }
            }
        }
    }
}
