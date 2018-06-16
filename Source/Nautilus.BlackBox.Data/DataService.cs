//--------------------------------------------------------------------------------------------------
// <copyright file="DataService.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Akka.Actor;
    using Nautilus.Core.Validation;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.BlackBox.Core.Messages.SystemCommands;
    using Nautilus.BlackBox.Core.Messages.TradeCommands;
    using Nautilus.BlackBox.Data.Market;
    using Nautilus.BlackBox.Core;
    using Nautilus.BlackBox.Core.Build;
    using Nautilus.BlackBox.Core.Enums;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.Database.Aggregators;
    using Nautilus.Database.Messages.Commands;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Scheduler;

    /// <summary>
    /// The sealed <see cref="DataService"/> class. The <see cref="BlackBox"/> service context
    /// which handles all data related operations.
    /// </summary>
    public sealed class DataService : ActorComponentBusConnectedBase
    {
        private readonly BlackBoxContainer storedContainer;
        private readonly IScheduler scheduler;
        private readonly IActorRef marketDataPortRef;
        private readonly IDictionary<Symbol, IActorRef> marketDataProcessorIndex = new Dictionary<Symbol, IActorRef>();

        private IBrokerageGateway brokerageGateway;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataService"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public DataService(
            BlackBoxContainer container,
            IMessagingAdapter messagingAdapter,
            IScheduler scheduler)
            : base(
            BlackBoxService.Data,
            LabelFactory.Service(BlackBoxService.Data),
            container,
            messagingAdapter)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(scheduler, nameof(scheduler));

            this.storedContainer = container;
            this.scheduler = scheduler;
            this.marketDataPortRef = Context.ActorOf(
                Props.Create(() => new MarketDataPort(container, messagingAdapter)));

            this.SetupCommandMessageHandling();
        }

        /// <summary>
        /// Sets up all <see cref="CommandMessage"/> handling methods.
        /// </summary>
        private void SetupCommandMessageHandling()
        {
            this.Receive<InitializeBrokerageGateway>(msg => this.OnMessage(msg));
            this.Receive<SubscribeBarData>(msg => this.OnMessage(msg));
            this.Receive<UnsubscribeBarData>(msg => this.OnMessage(msg));
            this.Receive<ShutdownSystem>(msg => this.OnMessage(msg));
        }

        /// <summary>
        /// Creates a new <see cref="BarAggregationController"/> for the symbol and exchange. Registers
        /// the data type with the processor, updates the index of processors with the
        /// <see cref="MarketDataPort"/>. Then subscribes to the market data for the symbol and exchange.
        /// </summary>
        /// <param name="message">The message.</param>
        private void OnMessage(SubscribeBarData message)
        {
            Debug.NotNull(message, nameof(message));
            Debug.NotNull(this.brokerageGateway, nameof(this.brokerageGateway));

            this.Execute(() =>
            {
                Validate.DictionaryDoesNotContainKey(message.Symbol, nameof(message.Symbol), this.marketDataProcessorIndex.ToImmutableDictionary());

                var barReceivers = new List<Enum>{BlackBoxService.AlphaModel}.ToImmutableList();
                var schedulerRef = Context.ActorOf(Props.Create(() => new Scheduler()));
                var marketDataProcessorRef = Context.ActorOf(Props.Create(() => new BarAggregationController(
                    this.storedContainer,
                    this.GetMessagingAdapter(),
                    barReceivers,
                    schedulerRef,
                    BlackBoxService.Data)));

                this.marketDataProcessorIndex.Add(message.Symbol, marketDataProcessorRef);
                this.marketDataProcessorIndex[message.Symbol].Tell(message, this.Self);
                this.marketDataPortRef.Tell(new MarketDataProcessorIndexUpdate(this.marketDataProcessorIndex, this.NewGuid(), this.TimeNow()));
                this.brokerageGateway.RequestMarketDataSubscribe(message.Symbol);
            });
        }

        private void OnMessage(UnsubscribeBarData message)
        {
            Debug.NotNull(message, nameof(message));

            this.Execute(() =>
            {
                Validate.DictionaryContainsKey(message.Symbol, nameof(this.marketDataProcessorIndex), this.marketDataProcessorIndex.ToImmutableDictionary());

                this.marketDataProcessorIndex[message.Symbol].Tell(message, this.Self);
            });
        }

        // Brokerage Gateway should be null before receiving this message.
        private void OnMessage(InitializeBrokerageGateway message)
        {
            Debug.NotNull(message, nameof(message));

            this.Execute(() =>
                {
                    this.brokerageGateway = message.BrokerageGateway;
                    this.brokerageGateway.RegisterMarketDataPort(this.marketDataPortRef);
                });
        }

        private void OnMessage(ShutdownSystem message)
        {
            Debug.NotNull(message, nameof(message));

            this.Execute(() =>
            {
                // TODO
            });
        }
    }
}
