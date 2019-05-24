//--------------------------------------------------------------------------------------------------
// <copyright file="DukascopyFixMessageRouter.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Brokerage.Dukascopy
{
    using System;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Fix;
    using Nautilus.Fix.Interfaces;
    using Nautilus.Fix.MessageFactories;
    using QuickFix;

    /// <summary>
    /// Provides an Dukascopy implementation for routing FIX messages.
    /// </summary>
    public class DukascopyFixMessageRouter : ComponentBase, IFixMessageRouter
    {
        private readonly SymbolConverter symbolConverter;
        private readonly string accountNumber;

        private Session? fixSession;

        /// <summary>
        /// Initializes a new instance of the <see cref="DukascopyFixMessageRouter"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="symbolConverter">The symbol provider.</param>
        /// <param name="accountNumber">The FIX account number.</param>
        /// <exception cref="ArgumentException">If the account number is empty or white space.</exception>
        public DukascopyFixMessageRouter(
            IComponentryContainer container,
            SymbolConverter symbolConverter,
            string accountNumber)
        : base(container)
        {
            Condition.NotEmptyOrWhiteSpace(accountNumber, nameof(accountNumber));

            this.symbolConverter = symbolConverter;
            this.accountNumber = accountNumber;
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
            var message = CollateralInquiryFactory.Create(this.TimeNow(), Brokerage.FXCM);

            this.SendFixMessage(message);
        }

        /// <summary>
        /// Send a new trading session status FIX message.
        /// </summary>
        public void TradingSessionStatus()
        {
            var message = TradingSessionStatusRequestFactory.Create(this.TimeNow());

            this.SendFixMessage(message);
        }

        /// <summary>
        /// The request all positions.
        /// </summary>
        public void RequestAllPositions()
        {
            var message = RequestForOpenPositionsFactory.Create(this.TimeNow());

            this.SendFixMessage(message);
        }

        /// <summary>
        /// Updates the instrument from the given symbol via a security status request FIX message.
        /// </summary>
        /// <param name="symbol">
        /// The symbol.
        /// </param>
        public void UpdateInstrumentSubscribe(Symbol symbol)
        {
            var fxcmSymbol = this.symbolConverter.GetBrokerSymbol(symbol.Code);
            var message = SecurityListRequestFactory.Create(
                fxcmSymbol.Value,
                this.TimeNow());

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
            var brokerSymbol = this.symbolConverter.GetBrokerSymbol(symbol.Code).Value;

            var message = MarketDataRequestFactory.Create(
                brokerSymbol,
                0,
                this.TimeNow());

            this.SendFixMessage(message);
        }

        /// <summary>
        /// Subscribes to market data for all symbol.
        /// </summary>
        public void MarketDataRequestSubscribeAll()
        {
            foreach (var brokerSymbol in this.symbolConverter.GetAllBrokerSymbols())
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
            var message = NewOrderSingleFactory.Create(
                this.symbolConverter.GetBrokerSymbol(order.Symbol.Code).Value,
                this.accountNumber,
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
            var brokerSymbol = this.symbolConverter.GetBrokerSymbol(atomicOrder.Symbol.Code).Value;

            if (atomicOrder.TakeProfit != null)
            {
                var message = NewOrderListEntryFactory.CreateWithStopLossAndTakeProfit(
                    brokerSymbol,
                    this.accountNumber,
                    atomicOrder,
                    this.TimeNow());

                this.SendFixMessage(message);
            }
            else
            {
                var message = NewOrderListEntryFactory.CreateWithStopLoss(
                    brokerSymbol,
                    this.accountNumber,
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
            var message = OrderCancelReplaceRequestFactory.Create(
                this.symbolConverter.GetBrokerSymbol(order.Symbol.Code).Value,
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
            var message = OrderCancelRequestFactory.Create(
                this.symbolConverter.GetBrokerSymbol(order.Symbol.Code).Value,
                order,
                this.TimeNow());

            this.SendFixMessage(message);
        }

        private void SendFixMessage(Message message)
        {
            if (this.fixSession is null)
            {
                this.Log.Error($"Cannot send {message} (the FIX session is null).");
                return;
            }

            this.fixSession.Send(message);
            this.Log.Information($"Sent => {message}");
        }
    }
}
