//--------------------------------------------------------------------------------------------------
// <copyright file="FxcmFixMessageRouter.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Brokerage.FXCM
{
    using System;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Fix;
    using Nautilus.Fix.Interfaces;
    using Nautilus.Fix.MessageFactories;
    using QuickFix;

    /// <summary>
    /// Provides an implementation for routing FXCM FIX messages.
    /// </summary>
    public class FxcmFixMessageRouter : ComponentBase, IFixMessageRouter
    {
        private readonly InstrumentDataProvider instrumentData;
        private readonly string accountNumber;

        private Session? fixSession;

        /// <summary>
        /// Initializes a new instance of the <see cref="FxcmFixMessageRouter"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="instrumentData">The instrument data provider.</param>
        /// <param name="accountNumber">The FIX account number.</param>
        /// <exception cref="ArgumentException">If the account number is empty or white space.</exception>
        public FxcmFixMessageRouter(
            IComponentryContainer container,
            InstrumentDataProvider instrumentData,
            string accountNumber)
        : base(
            NautilusService.FIX,
            LabelFactory.Create(nameof(FxcmFixMessageRouter)),
            container)
        {
            Precondition.NotEmptyOrWhiteSpace(accountNumber, nameof(accountNumber));

            this.instrumentData = instrumentData;
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
            var fxcmSymbol = this.instrumentData.GetBrokerSymbol(symbol.Code);
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
            var brokerSymbol = this.instrumentData.GetBrokerSymbol(symbol.Code).Value;

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
            foreach (var brokerSymbol in this.instrumentData.GetAllBrokerSymbols())
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
                this.instrumentData.GetBrokerSymbol(order.Symbol.Code).Value,
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
            var brokerSymbol = this.instrumentData.GetBrokerSymbol(atomicOrder.Symbol.Code).Value;

            if (atomicOrder.ProfitTarget.HasValue)
            {
                var message = NewOrderListEntryFactory.CreateWithStopLossAndProfitTarget(
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
                this.instrumentData.GetBrokerSymbol(order.Symbol.Code).Value,
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
                this.instrumentData.GetBrokerSymbol(order.Symbol.Code).Value,
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
            this.Log.Information($"{this.fixSession} => {message}");
        }
    }
}
