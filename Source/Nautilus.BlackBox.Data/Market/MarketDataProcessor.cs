//--------------------------------------------------------------------------------------------------
// <copyright file="MarketDataProcessor.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Data.Market
{
    using System.Collections.Generic;
    using Akka.Actor;
    using NautechSystems.CSharp.Extensions;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core.Messages.SystemCommands;
    using Nautilus.BlackBox.Core.Setup;
    using Nautilus.BlackBox.Core;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The sealed <see cref="MarketDataProcessor"/> class.
    /// </summary>
    public sealed class MarketDataProcessor : ActorComponentBusConnectedBase
    {
        private readonly BlackBoxSetupContainer storedSetupContainer;
        private readonly Symbol symbol;
        private readonly IDictionary<TradeType, IActorRef> barAggregators = new Dictionary<TradeType, IActorRef>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketDataProcessor"/> class.
        /// </summary>
        /// <param name="setupContainer">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="symbol">The symbol.</param>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public MarketDataProcessor(
            BlackBoxSetupContainer setupContainer,
            IMessagingAdapter messagingAdapter,
            Symbol symbol)
            : base(
            BlackBoxService.Data,
            LabelFactory.Component(nameof(MarketDataProcessor), symbol),
            setupContainer,
            messagingAdapter)
        {
            Validate.NotNull(setupContainer, nameof(setupContainer));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(symbol, nameof(symbol));

            this.storedSetupContainer = setupContainer;
            this.symbol = symbol;

            this.SetupCommandMessageHandling();
            this.SetupEventMessageHandling();
        }

        /// <summary>
        /// Sets up all <see cref="CommandMessage"/> handling methods.
        /// </summary>
        private void SetupCommandMessageHandling()
        {
            this.Receive<SubscribeSymbolDataType>(msg => this.OnMessage(msg));
            this.Receive<UnsubscribeSymbolDataType>(msg => this.OnMessage(msg));
        }

        /// <summary>
        /// Sets up all <see cref="EventMessage"/> handling methods.
        /// </summary>
        private void SetupEventMessageHandling()
        {
            this.Receive<Tick>(msg => this.OnMessage(msg));
        }

        private void OnMessage(Tick quote)
        {
            Debug.NotNull(quote, nameof(quote));

            this.barAggregators.ForEach(b => b.Value.Tell(quote, this.Self));
        }

        private void OnMessage(SubscribeSymbolDataType message)
        {
            Debug.NotNull(message, nameof(message));
            Validate.EqualTo(message.Symbol, nameof(message.Symbol), this.symbol);
            Validate.DictionaryDoesNotContainKey(message.TradeType, nameof(message.TradeType), this.barAggregators);

            if (message.BarSpecification.Resolution == BarResolution.Tick)
            {
                var barAggregatorRef = Context.ActorOf(Props.Create(() => new TickBarAggregator(
                    this.storedSetupContainer,
                    this.MessagingAdapter,
                    message)));

                this.barAggregators.Add(message.TradeType, barAggregatorRef);
            }

            if (message.BarSpecification.Resolution != BarResolution.Tick)
            {
                var barAggregatorRef = Context.ActorOf(Props.Create(() => new TimeBarAggregator(
                    this.storedSetupContainer,
                    this.MessagingAdapter,
                    message)));

                this.barAggregators.Add(message.TradeType, barAggregatorRef);
            }

            this.Log(LogLevel.Debug, $"Setup for {message.BarSpecification} bars");

            Debug.DictionaryContainsKey(message.TradeType, nameof(message.TradeType), this.barAggregators);
        }

        private void OnMessage(UnsubscribeSymbolDataType message)
        {
            Debug.NotNull(message, nameof(message));
            Validate.EqualTo(message.Symbol, nameof(message.Symbol), this.symbol);
            Validate.DictionaryContainsKey(message.TradeType, nameof(message.TradeType), this.barAggregators);

            this.barAggregators[message.TradeType].Tell(PoisonPill.Instance);
            this.barAggregators.Remove(message.TradeType);

            this.Log(LogLevel.Information, $"Data for {this.symbol}({message.TradeType}) bars deregistered");

            Debug.DictionaryDoesNotContainKey(message.TradeType, nameof(message.TradeType), this.barAggregators);
        }
    }
}
