//--------------------------------------------------------------------------------------------------
// <copyright file="FxcmFixMessageRouter.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Brokerage.Fxcm
{
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Fix;
    using Nautilus.Fix.Interfaces;
    using Nautilus.Fix.MessageFactories;
    using QuickFix;

    /// <summary>
    /// Provides an implementation for routing FXCM FIX messages.
    /// </summary>
    public sealed class FxcmFixMessageRouter : Component, IFixMessageRouter
    {
        private const string SENT = "-->";
        private const string FIX = "[FIX]";

        private readonly SymbolConverter symbolConverter;
        private readonly AccountId accountId;

        private Session? fixSession;

        /// <summary>
        /// Initializes a new instance of the <see cref="FxcmFixMessageRouter"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="accountId">The account identifier for the router.</param>
        /// <param name="symbolConverter">The symbol provider.</param>
        public FxcmFixMessageRouter(
            IComponentryContainer container,
            AccountId accountId,
            SymbolConverter symbolConverter)
        : base(container)
        {
            this.accountId = accountId;
            this.symbolConverter = symbolConverter;
        }

        /// <summary>
        /// Connects the FIX session.
        /// </summary>
        /// <param name="session">The FIX session.</param>
        public void ConnectSession(Session session)
        {
            this.fixSession = session;
        }

        /// <summary>
        /// Sends a new collateral inquiry FIX message.
        /// </summary>
        public void CollateralInquiry()
        {
            this.SendFixMessage(CollateralInquiryFactory.Create(this.TimeNow(), new Brokerage("FXCM")));
        }

        /// <summary>
        /// Send a new trading session status FIX message.
        /// </summary>
        public void TradingSessionStatus()
        {
            this.SendFixMessage(TradingSessionStatusRequestFactory.Create(this.TimeNow()));
        }

        /// <summary>
        /// The request all positions.
        /// </summary>
        public void RequestAllPositions()
        {
            this.SendFixMessage(RequestForOpenPositionsFactory.Create(this.TimeNow()));
        }

        /// <summary>
        /// Updates the instrument from the given symbol via a security status request FIX message.
        /// </summary>
        /// <param name="symbol">
        /// The symbol.
        /// </param>
        public void UpdateInstrumentSubscribe(Symbol symbol)
        {
            var brokerSymbolCode = this.symbolConverter.GetBrokerSymbolCode(symbol.Code);
            if (brokerSymbolCode is null)
            {
                this.Log.Error($"Cannot find broker symbol for {symbol.Code}.");
                return;
            }

            var message = SecurityListRequestFactory.Create(brokerSymbolCode, this.TimeNow());

            this.SendFixMessage(message);
        }

        /// <summary>
        /// Updates all instruments via a security status request FIX message.
        /// </summary>
        public void UpdateInstrumentsSubscribeAll()
        {
            var message = SecurityListRequestFactory.Create(this.TimeNow());

            this.SendFixMessage(message);
        }

        /// <summary>
        /// Subscribes to market data for the given symbol.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        public void MarketDataRequestSubscribe(Symbol symbol)
        {
            var brokerSymbolCode = this.symbolConverter.GetBrokerSymbolCode(symbol.Code);
            if (brokerSymbolCode is null)
            {
                this.Log.Error($"Cannot find broker symbol for {symbol.Code}.");
                return;
            }

            var message = MarketDataRequestFactory.Create(
                brokerSymbolCode,
                0,
                this.TimeNow());

            this.SendFixMessage(message);
        }

        /// <summary>
        /// Subscribes to market data for all symbol.
        /// </summary>
        public void MarketDataRequestSubscribeAll()
        {
            foreach (var brokerSymbol in this.symbolConverter.BrokerSymbolCodes)
            {
                var message = MarketDataRequestFactory.Create(
                    brokerSymbol,
                    0,
                    this.TimeNow());

                this.SendFixMessage(message);
            }
        }

        /// <summary>
        /// Submits an order.
        /// </summary>
        /// <param name="order">The order to submit.</param>
        public void SubmitOrder(Order order)
        {
            var brokerSymbolCode = this.symbolConverter.GetBrokerSymbolCode(order.Symbol.Code);
            if (brokerSymbolCode is null)
            {
                this.Log.Error($"Cannot find broker symbol for {order.Symbol.Code}.");
                return;
            }

            var message = NewOrderSingleFactory.Create(
                brokerSymbolCode,
                this.accountId.AccountNumber,
                order,
                this.TimeNow());

            this.SendFixMessage(message);
        }

        /// <summary>
        /// Submits a trade.
        /// </summary>
        /// <param name="atomicOrder">The atomic order to submit.</param>
        public void SubmitOrder(AtomicOrder atomicOrder)
        {
            var brokerSymbolCode = this.symbolConverter.GetBrokerSymbolCode(atomicOrder.Symbol.Code);
            if (brokerSymbolCode is null)
            {
                this.Log.Error($"Cannot find broker symbol for {atomicOrder.Symbol.Code}.");
                return;
            }

            if (atomicOrder.TakeProfit != null)
            {
                var message = NewOrderListEntryFactory.CreateWithStopLossAndTakeProfit(
                    brokerSymbolCode,
                    this.accountId.AccountNumber,
                    atomicOrder,
                    this.TimeNow());

                this.SendFixMessage(message);
            }
            else
            {
                var message = NewOrderListEntryFactory.CreateWithStopLoss(
                    brokerSymbolCode,
                    this.accountId.AccountNumber,
                    atomicOrder,
                    this.TimeNow());

                this.SendFixMessage(message);
            }
        }

        /// <summary>
        /// Submits a FIX order cancel replace request.
        /// </summary>
        /// <param name="order">The order to modify.</param>
        /// <param name="modifiedPrice">The modified order price.</param>
        public void ModifyOrder(Order order, Price modifiedPrice)
        {
            var brokerSymbolCode = this.symbolConverter.GetBrokerSymbolCode(order.Symbol.Code);
            if (brokerSymbolCode is null)
            {
                this.Log.Error($"Cannot find broker symbol for {order.Symbol.Code}.");
                return;
            }

            var message = OrderCancelReplaceRequestFactory.Create(
                brokerSymbolCode,
                order,
                modifiedPrice.Value,
                this.TimeNow());

            this.SendFixMessage(message);
        }

        /// <summary>
        /// Submits a cancel order.
        /// </summary>
        /// <param name="order">The order to cancel.</param>
        public void CancelOrder(Order order)
        {
            var brokerSymbolCode = this.symbolConverter.GetBrokerSymbolCode(order.Symbol.Code);
            if (brokerSymbolCode is null)
            {
                this.Log.Error($"Cannot find broker symbol for {order.Symbol.Code}.");
                return;
            }

            var message = OrderCancelRequestFactory.Create(
                brokerSymbolCode,
                order,
                this.TimeNow());

            this.SendFixMessage(message);
        }

        private void SendFixMessage(Message message)
        {
            if (this.fixSession is null)
            {
                this.Log.Error($"Cannot send FIX message (the session is null).");
                return;
            }

            this.fixSession.Send(message);

            this.LogMessageSent(message);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        private void LogMessageSent(Message message)
        {
            this.Log.Debug($"{FIX}{SENT} {message.GetType().Name}");
        }
    }
}
