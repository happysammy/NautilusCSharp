//--------------------------------------------------------------------------------------------------
// <copyright file="FxcmFixMessageRouter.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Brokerage.FXCM
{
    using System;
    using System.Collections.Generic;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Fix.Interfaces;
    using Nautilus.Fix.MessageFactories;
    using QuickFix;

    /// <summary>
    /// The <see cref="FxcmFixMessageRouter"/>.
    /// </summary>
    public class FxcmFixMessageRouter : ComponentBase, IFixMessageRouter
    {
        private Session fixSession;
        private Session fixSessionMd;

        /// <summary>
        /// Initializes a new instance of the <see cref="FxcmFixMessageRouter"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        public FxcmFixMessageRouter(IComponentryContainer container)
        : base(
            ServiceContext.FIX,
            LabelFactory.Component(nameof(FxcmFixMessageRouter)),
            container)
        {
            Validate.NotNull(container, nameof(container));
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
        /// Connects the FIX session md.
        /// </summary>
        /// <param name="sessionMd">The FIX session md.
        /// </param>
        public void ConnectSessionMd(Session sessionMd)
        {
            this.fixSessionMd = sessionMd;
        }

        /// <summary>
        /// Sends a new collateral inquiry FIX message.
        /// </summary>
        public void CollateralInquiry()
        {
            var message = CollateralInquiryFactory.Create(this.TimeNow());

            this.fixSession.Send(message);

            Console.WriteLine($"CollateralInquiry + SubscribeCollateralReports...");
        }

        /// <summary>
        /// Send a new trading session status FIX message.
        /// </summary>
        public void TradingSessionStatus()
        {
            var message = TradingSessionStatusRequestFactory.Create(this.TimeNow());

            this.fixSession.Send(message);

            Console.WriteLine($"TradingSessionStatusRequest...");
        }

        /// <summary>
        /// The request all positions.
        /// </summary>
        public void RequestAllPositions()
        {
            var message = RequestForOpenPositionsFactory.Create(this.TimeNow());

            this.fixSession.Send(message);

            Console.WriteLine($"RequestForOpenPositions + SubscribePositionReports...");
        }

        /// <summary>
        /// Updates the instrument from the given symbol via a security status request FIX message.
        /// </summary>
        /// <param name="symbol">
        /// The symbol.
        /// </param>
        public void UpdateInstrumentSubscribe(Symbol symbol)
        {
            var fxcmSymbol = FxcmSymbolProvider.GetBrokerSymbol(symbol.Code);

            this.fixSession.Send(SecurityListRequestFactory.Create(
                fxcmSymbol.Value,
                this.TimeNow()));

            Console.WriteLine($"SecurityStatusRequest + SubscribeUpdates ({symbol})...");
        }

        /// <summary>
        /// Updates all instruments via a security status request FIX message.
        /// </summary>
        public void UpdateInstrumentsSubscribeAll()
        {
            this.fixSession.Send(SecurityListRequestFactory.Create(this.TimeNow()));

            Console.WriteLine($"SecurityStatusRequest + SubscribeUpdates (ALL)...");
        }

        /// <summary>
        /// Subscribes to market data for the given symbol.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        public void MarketDataRequestSubscribe(Symbol symbol)
        {
            var fxcmSymbol = FxcmSymbolProvider.GetBrokerSymbol(symbol.Code).Value;

            this.fixSessionMd.Send(MarketDataRequestSubscriptionFactory.Create(
                fxcmSymbol,
                this.TimeNow()));

            Console.WriteLine($"MarketDataRequest + SubscribeUpdates ({symbol})...");
        }

        /// <summary>
        /// Subscribes to market data for all symbol.
        /// </summary>
        public void MarketDataRequestSubscribeAll()
        {
            foreach (var fxcmSymbol in FxcmSymbolProvider.GetAllBrokerSymbols())
            {
                this.fixSessionMd.Send(MarketDataRequestSubscriptionFactory.Create(
                    fxcmSymbol,
                    this.TimeNow()));

                Console.WriteLine($"MarketDataRequest + SubscribeUpdates ({fxcmSymbol})...");
            }
        }

        /// <summary>
        /// Submits an entry limit stop order.
        /// </summary>
        /// <param name="elsOrder">The ELS order.</param>
        public void SubmitEntryLimitStopOrder(AtomicOrder elsOrder)
        {
            var message = NewOrderListEntryLimitStopFactory.Create(
                FxcmSymbolProvider.GetBrokerSymbol(elsOrder.Symbol.Code).Value,
                elsOrder,
                this.TimeNow());

            this.fixSession.Send(message);

            Console.WriteLine($"Submitting ELS Order => {Broker.FXCM}");
        }

        /// <summary>
        /// Submits an entry stop order.
        /// </summary>
        /// <param name="elsOrder">The ELS order.</param>
        public void SubmitEntryStopOrder(AtomicOrder elsOrder)
        {
            var message = NewOrderListEntryStopFactory.Create(
                FxcmSymbolProvider.GetBrokerSymbol(elsOrder.Symbol.Code).Value,
                elsOrder,
                this.TimeNow());

            this.fixSession.Send(message);

            Console.WriteLine($"Submitting ELS Order => {Broker.FXCM}");
        }

        /// <summary>
        /// Submits a modify stop-loss order.
        /// </summary>
        /// <param name="orderModification">The order modification.</param>
        public void ModifyStoplossOrder(KeyValuePair<Order, Price> orderModification)
        {
            var message = OrderCancelReplaceRequestFactory.Create(
                FxcmSymbolProvider.GetBrokerSymbol(orderModification.Key.Symbol.Code).Value,
                orderModification.Key,
                orderModification.Value.Value,
                this.TimeNow());

            this.fixSession.Send(message);

            Console.WriteLine($"{orderModification.Key.Symbol} Submitting OrderReplaceRequest: (ClOrdId={orderModification.Key.OrderId}, OrderId={orderModification.Key.BrokerOrderId}) => {Broker.FXCM}");
        }

        /// <summary>
        /// Submits a cancel order.
        /// </summary>
        /// <param name="order">The order to cancel.</param>
        public void CancelOrder(Order order)
        {
            var message = OrderCancelRequestFactory.Create(
                FxcmSymbolProvider.GetBrokerSymbol(order.Symbol.Code).Value,
                order,
                this.TimeNow());

            this.fixSession.Send(message);

            Console.WriteLine($"{order.Symbol} Submitting OrderCancelRequestFactory: (ClOrdId={order.OrderId}, OrderId={order.BrokerOrderId}) => {Broker.FXCM}");
        }

        /// <summary>
        /// Submits a request to close a position.
        /// </summary>
        /// <param name="position">The position to close.
        /// </param>
        public void ClosePosition(Position position)
        {

        }

        /// <summary>
        /// Returns a read-only list of all symbol <see cref="string"/>(s) provided by the FIX client.
        /// </summary>
        /// <returns>The list of symbols.</returns>
        public IReadOnlyList<string> GetAllBrokerSymbols() => FxcmSymbolProvider.GetAllBrokerSymbols();

        /// <summary>
        /// Returns a read-only list of all <see cref="Symbol"/>(s) provided by the FIX client.
        /// </summary>
        /// <returns>The list of symbols.</returns>
        public IReadOnlyList<Symbol> GetAllSymbols() => FxcmSymbolProvider.GetAllSymbols();

//        /// <summary>
//        /// Returns a read-only list of all tick values provided by the FIX client.
//        /// </summary>
//        /// <returns>The list of symbols</returns>
//        public IReadOnlyDictionary<string, int> GetTickValueIndex() => FxcmTickSizeProvider.GetIndex();
    }
}
