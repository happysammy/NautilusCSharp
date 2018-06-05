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
        private Session session;
        private Session sessionMd;

        /// <summary>
        /// The initialize brokerage gateway.
        /// </summary>
        /// <param name="gateway">
        /// The gateway.
        /// </param>
        public void InitializeBrokerageGateway(IBrokerageGateway gateway)
        {
            Validate.NotNull(gateway, nameof(gateway));

            this.brokerageGateway = gateway;
        }

        /// <summary>
        /// The connect session.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        public void ConnectSession(Session session)
        {
            // TODO?
            this.session = session;
        }

        /// <summary>
        /// The connect session md.
        /// </summary>
        /// <param name="sessionMd">
        /// The session md.
        /// </param>
        public void ConnectSessionMd(Session sessionMd)
        {
            // TODO?
            this.sessionMd = sessionMd;
        }

        /// <summary>
        /// The collateral inquiry.
        /// </summary>
        public void CollateralInquiry()
        {
            var message = CollateralInquiryFactory.Create(this.brokerageGateway.GetTimeNow());

            this.SendMessage(message);

            Console.WriteLine($"CollateralInquiry + SubscribeCollateralReports...");
        }

        /// <summary>
        /// The trading session status.
        /// </summary>
        public void TradingSessionStatus()
        {
            var message = TradingSessionStatusRequestFactory.Create(this.brokerageGateway.GetTimeNow());

            this.SendMessage(message);

            Console.WriteLine($"TradingSessionStatusRequest...");
        }

        /// <summary>
        /// The request all positions.
        /// </summary>
        public void RequestAllPositions()
        {
            var message = RequestForOpenPositionsFactory.Create(this.brokerageGateway.GetTimeNow());

            this.SendMessage(message);

            Console.WriteLine($"RequestForOpenPositions + SubscribePositionReports...");
        }

        /// <summary>
        /// The update instrument subscribe.
        /// </summary>
        /// <param name="symbol">
        /// The symbol.
        /// </param>
        public void UpdateInstrumentSubscribe(Symbol symbol)
        {
            var fxcmSymbol = FxcmSymbolMapper.GetFxcmSymbol(symbol.Code);

            this.SendMessage(SecurityListRequestFactory.Create(this.brokerageGateway.GetTimeNow()));

            Console.WriteLine($"SecurityStatusRequest + SubscribeUpdates ({symbol})...");
        }

        /// <summary>
        /// The update instruments subscribe all.
        /// </summary>
        public void UpdateInstrumentsSubscribeAll()
        {
            this.SendMessage(SecurityListRequestFactory.Create(this.brokerageGateway.GetTimeNow()));

            Console.WriteLine($"SecurityStatusRequest + SubscribeUpdates (ALL)...");
        }

        /// <summary>
        /// The market data request subscribe.
        /// </summary>
        /// <param name="symbol">
        /// The symbol.
        /// </param>
        public void MarketDataRequestSubscribe(Symbol symbol)
        {
            var fxcmSymbol = FxcmSymbolMapper.GetFxcmSymbol(symbol.Code).Value;

            this.SendMessageMd(MarketDataRequestSubscriptionFactory.Create(fxcmSymbol, this.brokerageGateway.GetTimeNow()));

            Console.WriteLine($"MarketDataRequest + SubscribeUpdates ({symbol})...");
        }

        /// <summary>
        /// The submit entry limit stop order.
        /// </summary>
        /// <param name="elsOrder">
        /// The ELS order.
        /// </param>
        public void SubmitEntryLimitStopOrder(AtomicOrder elsOrder)
        {
            var message = NewOrderListEntryLimitStopFactory.Create(
                elsOrder,
                this.brokerageGateway.GetTimeNow());

            this.SendMessage(message);

            Console.WriteLine($"Submitting ELS Order => {Broker.FXCM}");
        }

        /// <summary>
        /// The submit entry stop order.
        /// </summary>
        /// <param name="elsOrder">
        /// The ELS order.
        /// </param>
        public void SubmitEntryStopOrder(AtomicOrder elsOrder)
        {
            var message = NewOrderListEntryStopFactory.Create(
                elsOrder,
                this.brokerageGateway.GetTimeNow());

            this.SendMessage(message);

            Console.WriteLine($"Submitting ELS Order => {Broker.FXCM}");
        }

        /// <summary>
        /// The modify stop-loss order.
        /// </summary>
        /// <param name="orderModification">
        /// The order modification.
        /// </param>
        public void ModifyStoplossOrder(KeyValuePair<Order, Price> orderModification)
        {
            var message = OrderCancelReplaceRequestFactory.Create(
                orderModification.Key,
                orderModification.Value,
                this.brokerageGateway.GetTimeNow());

            this.SendMessage(message);

            Console.WriteLine($"{orderModification.Key.Symbol} Submitting OrderReplaceRequest: (ClOrdId={orderModification.Key.OrderId}, OrderId={orderModification.Key.BrokerOrderId}) => {Broker.FXCM}");
        }

        /// <summary>
        /// The cancel order.
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        public void CancelOrder(Order order)
        {
            var message = OrderCancelRequestFactory.Create(order, this.brokerageGateway.GetTimeNow());

            this.SendMessage(message);

            Console.WriteLine($"{order.Symbol} Submitting OrderCancelRequestFactory: (ClOrdId={order.OrderId}, OrderId={order.BrokerOrderId}) => {Broker.FXCM}");
        }

        /// <summary>
        /// The close position.
        /// </summary>
        /// <param name="position">
        /// The position.
        /// </param>
        public void ClosePosition(Position position)
        {

        }

        /// <summary>
        /// The send message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        private void SendMessage(Message message)
        {
            if (this.session != null)
            {
                this.session.Send(message);
            }
            else
            {
                // This probably won't ever happen.
                Console.WriteLine($"Error: Cannot send message (session not created)");
            }
        }

        /// <summary>
        /// The send message md.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        private void SendMessageMd(Message message)
        {
            if (this.sessionMd != null)
            {
                this.sessionMd.Send(message);
            }
            else
            {
                // This probably won't ever happen.
                Console.WriteLine($"Cannot send message (session not created)");
            }
        }
    }
}
