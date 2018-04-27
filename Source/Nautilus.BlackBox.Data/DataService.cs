// -------------------------------------------------------------------------------------------------
// <copyright file="DataService.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Data
{
    using System.Collections.Generic;
    using Akka.Actor;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core;
    using Nautilus.BlackBox.Core.Enums;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.BlackBox.Core.Messages.SystemCommands;
    using Nautilus.BlackBox.Core.Messages.TradeCommands;
    using Nautilus.BlackBox.Core.Setup;
    using Nautilus.BlackBox.Data.Market;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The sealed <see cref="DataService"/> class. The <see cref="BlackBox"/> service context
    /// which handles all data related operations.
    /// </summary>
    public sealed class DataService : ActorComponentBase
    {
        private readonly BlackBoxSetupContainer storedSetupContainer;
        private readonly IActorRef marketDataPortRef;
        private readonly IDictionary<Symbol, IActorRef> marketDataProcessorIndex = new Dictionary<Symbol, IActorRef>();

        private IBrokerageGateway brokerageGateway;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataService"/> class.
        /// </summary>
        /// <param name="setupContainer">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public DataService(
            BlackBoxSetupContainer setupContainer,
            IMessagingAdapter messagingAdapter)
            : base(
            BlackBoxService.Data,
            LabelFactory.Service(BlackBoxService.Data),
            setupContainer,
            messagingAdapter)
        {
            Validate.NotNull(setupContainer, nameof(setupContainer));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));

            this.storedSetupContainer = setupContainer;
            this.marketDataPortRef = Context.ActorOf(
                Props.Create(() => new MarketDataPort(setupContainer, messagingAdapter)));

            this.SetupServiceMessageHandling();
        }

        /// <summary>
        /// Sets up all <see cref="Message"/> handling methods.
        /// </summary>
        private void SetupServiceMessageHandling()
        {
            this.Receive<InitializeBrokerageGateway>(msg => this.OnMessage(msg));
            this.Receive<SubscribeSymbolDataType>(msg => this.OnMessage(msg));
            this.Receive<UnsubscribeSymbolDataType>(msg => this.OnMessage(msg));
            this.Receive<ShutdownSystem>(msg => this.OnMessage(msg));
        }

        /// <summary>
        /// Creates a new <see cref="MarketDataProcessor"/> for the symbol and exchange. Registers
        /// the data type with the processor, updates the index of processors with the
        /// <see cref="MarketDataPort"/>. Then subscribes to the market data for the symbol and exchange.
        /// </summary>
        /// <param name="message">The message.</param>
        private void OnMessage(SubscribeSymbolDataType message)
        {
            Debug.NotNull(message, nameof(message));
            Debug.NotNull(this.brokerageGateway, nameof(this.brokerageGateway));

            this.CommandHandler.Execute(() =>
            {
                Validate.DictionaryDoesNotContainKey(message.Symbol, nameof(message.Symbol), this.marketDataProcessorIndex);

                var marketDataProcessorRef = Context.ActorOf(Props.Create(() => new MarketDataProcessor(
                    this.storedSetupContainer,
                    this.MessagingAdapter,
                    message.Symbol)));

                this.marketDataProcessorIndex.Add(message.Symbol, marketDataProcessorRef);
                this.marketDataProcessorIndex[message.Symbol].Tell(message, this.Self);
                this.marketDataPortRef.Tell(new MarketDataProcessorIndexUpdate(this.marketDataProcessorIndex, this.NewGuid(), this.TimeNow()));
                this.brokerageGateway.RequestMarketDataSubscribe(message.Symbol);
            });
        }

        private void OnMessage(UnsubscribeSymbolDataType message)
        {
            Debug.NotNull(message, nameof(message));

            this.CommandHandler.Execute(() =>
            {
                Validate.DictionaryContainsKey(message.Symbol, nameof(this.marketDataProcessorIndex), this.marketDataProcessorIndex);

                this.marketDataProcessorIndex[message.Symbol].Tell(message, this.Self);
            });
        }

        // Brokerage Gateway should be null before receiving this message.
        private void OnMessage(InitializeBrokerageGateway message)
        {
            Debug.NotNull(message, nameof(message));

            this.CommandHandler.Execute(() =>
                {
                    this.brokerageGateway = message.BrokerageGateway;
                    this.brokerageGateway.RegisterMarketDataPort(this.marketDataPortRef);
                });
        }

        private void OnMessage(ShutdownSystem message)
        {
            Debug.NotNull(message, nameof(message));

            this.CommandHandler.Execute(() =>
            {
                // TODO
            });
        }
    }
}
