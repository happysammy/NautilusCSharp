//--------------------------------------------------------------------------------------------------
// <copyright file="FxcmFixClient.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix
{
    using System.Collections.Generic;
    using Nautilus.Core.Validation;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Factories;
    using Nautilus.Fix.Interfaces;
    using Price = Nautilus.DomainModel.ValueObjects.Price;
    using Symbol = Nautilus.DomainModel.ValueObjects.Symbol;

    /// <summary>
    /// Provides a generic QuickFix client.
    /// </summary>
    public class FixClient : FixMessageCracker, IFixClient
    {
        private readonly IReadOnlyList<string> brokerSymbols;
        private readonly IReadOnlyList<Symbol> symbols;
        private readonly IReadOnlyDictionary<string, int> tickSizeIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixClient"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="broker">The account brokerage.</param>
        /// <param name="tickProcessor">The tick data processor.</param>
        /// <param name="fixMessageHandler">The FIX message handler.</param>
        /// <param name="fixMessageRouter">The FIX message router.</param>
        /// <param name="credentials">The FIX account credentials.</param>
        /// <param name="brokerSymbols">The list of broker symbols.</param>
        /// <param name="symbols">The list of symbols.</param>
        /// <param name="tickSizeIndex">The list of tick sizes.</param>
        public FixClient(
            IComponentryContainer container,
            ITickProcessor tickProcessor,
            IFixMessageHandler fixMessageHandler,
            IFixMessageRouter fixMessageRouter,
            FixCredentials credentials,
            Broker broker,
            IReadOnlyList<string> brokerSymbols,
            IReadOnlyList<Symbol> symbols,
            IReadOnlyDictionary<string, int> tickSizeIndex)
        : base(
            ServiceContext.FIX,
            LabelFactory.Service(ServiceContext.FIX),
            container,
            tickProcessor,
            fixMessageHandler,
            fixMessageRouter,
            credentials)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(tickProcessor, nameof(tickProcessor));
            Validate.NotNull(fixMessageHandler, nameof(fixMessageHandler));
            Validate.NotNull(fixMessageRouter, nameof(fixMessageRouter));
            Validate.NotNull(credentials, nameof(credentials));
            Validate.CollectionNotNullOrEmpty(brokerSymbols, nameof(brokerSymbols));
            Validate.CollectionNotNullOrEmpty(symbols, nameof(symbols));
            Validate.CollectionNotNullOrEmpty(tickSizeIndex, nameof(tickSizeIndex));

            this.Broker = broker;
            this.brokerSymbols = brokerSymbols;
            this.symbols = symbols;
            this.tickSizeIndex = tickSizeIndex;
        }

        /// <summary>
        /// Gets the name of the brokerage.
        /// </summary>
        public Broker Broker { get; }

        /// <summary>
        /// Returns a value indicating whether the FIX session is connected.
        /// </summary>
        /// <returns>A <see cref="bool"/>.</returns>
        public bool IsConnected => this.IsFixConnected;

        /// <summary>
        /// The initializes the brokerage gateway.
        /// </summary>
        /// <param name="gateway">The brokerage gateway.</param>
        public void InitializeGateway(IFixGateway gateway)
        {
            Validate.NotNull(gateway, nameof(gateway));

            this.FixMessageHandler.InitializeGateway(gateway);
        }

        /// <summary>
        /// Connects to the FIX session.
        /// </summary>
        public void Connect()
        {
            this.ConnectFix();
        }

        /// <summary>
        /// Disconnects from the FIX session.
        /// </summary>
        public void Disconnect()
        {
            this.DisconnectFix();
        }

        /// <summary>
        /// Returns a read-only list of all symbol <see cref="string"/>(s) provided by the FIX client.
        /// </summary>
        /// <returns>The list of symbols.</returns>
        public IReadOnlyCollection<string> GetAllBrokerSymbols() => this.brokerSymbols;

        /// <summary>
        /// Returns a read-only list of all <see cref="Symbol"/>(s) provided by the FIX client.
        /// </summary>
        /// <returns>The list of symbols.</returns>
        public IReadOnlyCollection<Symbol> GetAllSymbols() => this.symbols;

        /// <summary>
        /// The submit entry limit stop order.
        /// </summary>
        /// <param name="elsOrder">The ELS order.</param>
        public void SubmitEntryLimitStopOrder(AtomicOrder elsOrder)
        {
            Debug.NotNull(elsOrder, nameof(elsOrder));

            this.Execute(() =>
            {
                this.FixMessageRouter.SubmitEntryLimitStopOrder(elsOrder);
            });
        }

        /// <summary>
        /// The submit entry stop order.
        /// </summary>
        /// <param name="elsOrder">The ELS order.</param>
        public void SubmitEntryStopOrder(AtomicOrder elsOrder)
        {
            Debug.NotNull(elsOrder, nameof(elsOrder));

            this.Execute(() =>
            {
                this.FixMessageRouter.SubmitEntryStopOrder(elsOrder);
            });
        }

        /// <summary>
        /// The modify stop-loss order.
        /// </summary>
        /// <param name="orderModification">The order modification.</param>
        public void ModifyStoplossOrder(KeyValuePair<Order, Price> orderModification)
        {
            Debug.NotNull(orderModification, nameof(orderModification));

            this.FixMessageRouter.ModifyStoplossOrder(orderModification);
        }

        /// <summary>
        /// The cancel order.
        /// </summary>
        /// <param name="order">The order.</param>
        public void CancelOrder(Order order)
        {
            Debug.NotNull(order, nameof(order));

            this.FixMessageRouter.CancelOrder(order);
        }

        /// <summary>
        /// The close position.
        /// </summary>
        /// <param name="position">The position.</param>
        public void ClosePosition(Position position)
        {
            Debug.NotNull(position, nameof(position));

            this.FixMessageRouter.ClosePosition(position);
        }

        /// <summary>
        /// The collateral inquiry.
        /// </summary>
        public void CollateralInquiry()
        {
            this.FixMessageRouter.CollateralInquiry();
        }

        /// <summary>
        /// The trading session status.
        /// </summary>
        public void TradingSessionStatus()
        {
            this.FixMessageRouter.TradingSessionStatus();
        }

        /// <summary>
        /// Sends request all positions.
        /// </summary>
        public void RequestAllPositions()
        {
            this.FixMessageRouter.RequestAllPositions();
        }

        /// <summary>
        /// The update instrument subscribe.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        public void UpdateInstrumentSubscribe(Symbol symbol)
        {
            Debug.NotNull(symbol, nameof(symbol));

            this.FixMessageRouter.UpdateInstrumentSubscribe(symbol);
        }

        /// <summary>
        /// The update instruments subscribe all.
        /// </summary>
        public void UpdateInstrumentsSubscribeAll()
        {
            this.FixMessageRouter.UpdateInstrumentsSubscribeAll();
        }

        /// <summary>
        /// The request market data subscribe.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        public void RequestMarketDataSubscribe(Symbol symbol)
        {
            Debug.NotNull(symbol, nameof(symbol));

            this.FixMessageRouter.MarketDataRequestSubscribe(symbol);
        }

        /// <summary>
        /// The request market data subscribe all.
        /// </summary>
        public void RequestMarketDataSubscribeAll()
        {
            this.FixMessageRouter.MarketDataRequestSubscribeAll();
        }
    }
}
