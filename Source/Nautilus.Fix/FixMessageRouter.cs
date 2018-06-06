//--------------------------------------------------------------------------------------------------
// <copyright file="FxcmFixMessageRouter.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix
{
    using System;
    using System.Collections.Generic;
    using Nautilus.Core.Validation;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.Brokerage.FXCM;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Fix.MessageFactories;
    using QuickFix;

    /// <summary>
    /// The <see cref="FixMessageRouter"/>.
    /// </summary>
    public class FixMessageRouter
    {
        private IBrokerageGateway brokerageGateway;
        private Session fixSession;
        private Session fixSessionMd;

        /// <summary>
        /// The initialize brokerage gateway.
        /// </summary>
        /// <param name="gateway">The brokerage gateway.</param>
        public void InitializeBrokerageGateway(IBrokerageGateway gateway)
        {
            Validate.NotNull(gateway, nameof(gateway));

            this.brokerageGateway = gateway;
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
            var message = CollateralInquiryFactory.Create(this.brokerageGateway.GetTimeNow());

            this.fixSession.Send(message);

            Console.WriteLine($"CollateralInquiry + SubscribeCollateralReports...");
        }

        /// <summary>
        /// Send a new trading session status FIX message.
        /// </summary>
        public void TradingSessionStatus()
        {
            var message = TradingSessionStatusRequestFactory.Create(this.brokerageGateway.GetTimeNow());

            this.fixSession.Send(message);

            Console.WriteLine($"TradingSessionStatusRequest...");
        }

        /// <summary>
        /// The request all positions.
        /// </summary>
        public void RequestAllPositions()
        {
            var message = RequestForOpenPositionsFactory.Create(this.brokerageGateway.GetTimeNow());

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
            var fxcmSymbol = FxcmSymbolMapper.GetFxcmSymbol(symbol.Code);

            this.fixSession.Send(SecurityListRequestFactory.Create(this.brokerageGateway.GetTimeNow()));

            Console.WriteLine($"SecurityStatusRequest + SubscribeUpdates ({symbol})...");
        }

        /// <summary>
        /// Updates all instruments via a security status request FIX message.
        /// </summary>
        public void UpdateInstrumentsSubscribeAll()
        {
            this.fixSession.Send(SecurityListRequestFactory.Create(this.brokerageGateway.GetTimeNow()));

            Console.WriteLine($"SecurityStatusRequest + SubscribeUpdates (ALL)...");
        }

        /// <summary>
        /// Subscribes to market data for the given symbol.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        public void MarketDataRequestSubscribe(Symbol symbol)
        {
            var fxcmSymbol = FxcmSymbolMapper.GetFxcmSymbol(symbol.Code).Value;

            this.fixSessionMd.Send(MarketDataRequestSubscriptionFactory.Create(fxcmSymbol, this.brokerageGateway.GetTimeNow()));

            Console.WriteLine($"MarketDataRequest + SubscribeUpdates ({symbol})...");
        }

        /// <summary>
        /// Submits an entry limit stop order.
        /// </summary>
        /// <param name="elsOrder">The ELS order.</param>
        public void SubmitEntryLimitStopOrder(AtomicOrder elsOrder)
        {
            var message = NewOrderListEntryLimitStopFactory.Create(
                elsOrder,
                this.brokerageGateway.GetTimeNow());

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
                elsOrder,
                this.brokerageGateway.GetTimeNow());

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
                orderModification.Key,
                orderModification.Value,
                this.brokerageGateway.GetTimeNow());

            this.fixSession.Send(message);

            Console.WriteLine($"{orderModification.Key.Symbol} Submitting OrderReplaceRequest: (ClOrdId={orderModification.Key.OrderId}, OrderId={orderModification.Key.BrokerOrderId}) => {Broker.FXCM}");
        }

        /// <summary>
        /// Submits a cancel order.
        /// </summary>
        /// <param name="order">The order to cancel.</param>
        public void CancelOrder(Order order)
        {
            var message = OrderCancelRequestFactory.Create(order, this.brokerageGateway.GetTimeNow());

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
    }
}
