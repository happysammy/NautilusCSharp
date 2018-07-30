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
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Interfaces;
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
        public FixClient(
            IComponentryContainer container,
            ITickProcessor tickProcessor,
            IFixMessageHandler fixMessageHandler,
            IFixMessageRouter fixMessageRouter,
            FixCredentials credentials,
            Broker broker,
            IReadOnlyList<string> brokerSymbols,
            IReadOnlyList<Symbol> symbols)
        : base(
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

            this.Broker = broker;
            this.brokerSymbols = brokerSymbols;
            this.symbols = symbols;
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
        /// The initializes the execution gateway.
        /// </summary>
        /// <param name="gateway">The execution gateway.</param>
        public void InitializeGateway(IExecutionGateway gateway)
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
        /// Submit a command to execute the given order.
        /// </summary>
        /// <param name="order">The order to submit.</param>
        public void SubmitOrder(IOrder order)
        {
            Debug.NotNull(order, nameof(order));

            this.Execute(() =>
            {
                this.FixMessageRouter.SubmitOrder(order);
            });
        }

        /// <summary>
        /// Submit a command to execute the given trade.
        /// </summary>
        /// <param name="atomicOrder">The atomic order to submit.</param>
        public void SubmitOrder(IAtomicOrder atomicOrder)
        {
            Debug.NotNull(atomicOrder, nameof(atomicOrder));

            this.Execute(() =>
            {
                this.FixMessageRouter.SubmitOrder(atomicOrder);
            });
        }

        /// <summary>
        /// Submit a command to the execution system to modify the given order.
        /// </summary>
        /// <param name="order">The order to modify.</param>
        /// <param name="modifiedPrice">The modified order price.</param>
        public void ModifyOrder(IOrder order, Price modifiedPrice)
        {
            Debug.NotNull(order, nameof(order));

            this.FixMessageRouter.ModifyOrder(order, modifiedPrice);
        }

        /// <summary>
        /// Submit the command to the execution system to cancel the given order.
        /// </summary>
        /// <param name="command">The cancel order command.</param>
        public void CancelOrder(IOrder command)
        {
            Debug.NotNull(command, nameof(command));

            this.FixMessageRouter.CancelOrder(command);
        }

        /// <summary>
        /// Submit the command to the execution system to close the given position.
        /// </summary>
        /// <param name="command">The close position command.</param>
        public void ClosePosition(IPosition command)
        {
            Debug.NotNull(command, nameof(command));

            this.FixMessageRouter.ClosePosition(command);
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
