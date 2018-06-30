//--------------------------------------------------------------------------------------------------
// <copyright file="FxcmFixMessageRouter.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Brokerage.FXCM
{
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
        private readonly string accountNumber;

        private Session fixSession;
        private Session fixSessionMd;

        /// <summary>
        /// Initializes a new instance of the <see cref="FxcmFixMessageRouter"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="accountNumber">The FIX account number.</param>
        public FxcmFixMessageRouter(IComponentryContainer container, string accountNumber)
        : base(
            ServiceContext.FIX,
            LabelFactory.Component(nameof(FxcmFixMessageRouter)),
            container)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(accountNumber, nameof(accountNumber));

            this.accountNumber = accountNumber;
        }

        /// <summary>
        /// Connects the FIX session.
        /// </summary>
        /// <param name="session">The FIX session.</param>
        public void ConnectSession(Session session)
        {
            Validate.NotNull(session, nameof(session));

            this.fixSession = session;
        }

        /// <summary>
        /// Connects the FIX session md.
        /// </summary>
        /// <param name="sessionMd">The FIX session md.
        /// </param>
        public void ConnectSessionMd(Session sessionMd)
        {
            Validate.NotNull(sessionMd, nameof(sessionMd));

            this.fixSessionMd = sessionMd;
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

        /// <summary>
        /// Sends a new collateral inquiry FIX message.
        /// </summary>
        public void CollateralInquiry()
        {
            this.Execute(() =>
            {
                var message = CollateralInquiryFactory.Create(this.TimeNow(), Broker.FXCM);

                this.fixSession.Send(message);

                this.Log.Information($"CollateralInquiry + SubscribeCollateralReports...");
            });
        }

        /// <summary>
        /// Send a new trading session status FIX message.
        /// </summary>
        public void TradingSessionStatus()
        {
            this.Execute(() =>
            {
                var message = TradingSessionStatusRequestFactory.Create(this.TimeNow());

                this.fixSession.Send(message);

                this.Log.Information($"TradingSessionStatusRequest...");
            });
        }

        /// <summary>
        /// The request all positions.
        /// </summary>
        public void RequestAllPositions()
        {
            this.Execute(() =>
            {
                var message = RequestForOpenPositionsFactory.Create(this.TimeNow());

                this.fixSession.Send(message);

                this.Log.Information($"RequestForOpenPositions + SubscribePositionReports...");
            });
        }

        /// <summary>
        /// Updates the instrument from the given symbol via a security status request FIX message.
        /// </summary>
        /// <param name="symbol">
        /// The symbol.
        /// </param>
        public void UpdateInstrumentSubscribe(Symbol symbol)
        {
            Debug.NotNull(symbol, nameof(symbol));

            this.Execute(() =>
            {
                var fxcmSymbol = FxcmSymbolProvider.GetBrokerSymbol(symbol.Code);

                this.fixSession.Send(SecurityListRequestFactory.Create(
                    fxcmSymbol.Value,
                    this.TimeNow()));

                this.Log.Information($"SecurityStatusRequest + SubscribeUpdates ({symbol})...");
            });
        }

        /// <summary>
        /// Updates all instruments via a security status request FIX message.
        /// </summary>
        public void UpdateInstrumentsSubscribeAll()
        {
            this.Execute(() =>
            {
                this.fixSession.Send(SecurityListRequestFactory.Create(this.TimeNow()));

                this.Log.Information($"SecurityStatusRequest + SubscribeUpdates (ALL)...");
            });
        }

        /// <summary>
        /// Subscribes to market data for the given symbol.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        public void MarketDataRequestSubscribe(Symbol symbol)
        {
            Debug.NotNull(symbol, nameof(symbol));

            this.Execute(() =>
            {
                var fxcmSymbol = FxcmSymbolProvider.GetBrokerSymbol(symbol.Code).Value;

                this.fixSessionMd.Send(MarketDataRequestFactory.Create(
                    fxcmSymbol,
                    this.TimeNow()));

                this.Log.Information($"MarketDataRequest + SubscribeUpdates ({symbol})...");
            });
        }

        /// <summary>
        /// Subscribes to market data for all symbol.
        /// </summary>
        public void MarketDataRequestSubscribeAll()
        {
            this.Execute(() =>
            {
                foreach (var fxcmSymbol in FxcmSymbolProvider.GetAllBrokerSymbols())
                {
                    this.fixSessionMd.Send(MarketDataRequestFactory.Create(
                        fxcmSymbol,
                        this.TimeNow()));

                    this.Log.Information($"MarketDataRequest + SubscribeUpdates ({fxcmSymbol})...");
                }
            });
        }

        /// <summary>
        /// Submits an ELS order.
        /// </summary>
        /// <param name="elsOrder">The atomic order.</param>
        public void SubmitEntryLimitStopOrder(AtomicOrder elsOrder)
        {
            Debug.NotNull(elsOrder, nameof(elsOrder));

            this.Execute(() =>
            {
                var message = NewOrderListEntryFactory.CreateWithLimit(
                    FxcmSymbolProvider.GetBrokerSymbol(elsOrder.Symbol.Code).Value,
                    this.accountNumber,
                    elsOrder,
                    this.TimeNow());

                this.fixSession.Send(message);

                this.Log.Information($"Submitting ELS Order => {Broker.FXCM}");
            });
        }

        /// <summary>
        /// Submits an ELS order.
        /// </summary>
        /// <param name="elsOrder">The atomic order.</param>
        public void SubmitEntryStopOrder(AtomicOrder elsOrder)
        {
            Debug.NotNull(elsOrder, nameof(elsOrder));

            this.Execute(() =>
            {
                var message = NewOrderListEntryFactory.CreateWithStop(
                    FxcmSymbolProvider.GetBrokerSymbol(elsOrder.Symbol.Code).Value,
                    this.accountNumber,
                    elsOrder,
                    this.TimeNow());

                this.fixSession.Send(message);

                this.Log.Information($"Submitting ELS Order => {Broker.FXCM}");
            });
        }

        /// <summary>
        /// Submits a modify stop-loss order.
        /// </summary>
        /// <param name="orderModification">The order modification.</param>
        public void ModifyStoplossOrder(KeyValuePair<Order, Price> orderModification)
        {
            Debug.NotNull(orderModification, nameof(orderModification));

            this.Execute(() =>
            {
                var message = OrderCancelReplaceRequestFactory.Create(
                    FxcmSymbolProvider.GetBrokerSymbol(orderModification.Key.Symbol.Code).Value,
                    orderModification.Key,
                    orderModification.Value.Value,
                    this.TimeNow());

                this.fixSession.Send(message);

                this.Log.Information(
                    $"{orderModification.Key.Symbol} Submitting OrderReplaceRequest: " +
                    $"(ClOrdId={orderModification.Key.OrderId}, " +
                    $"OrderId={orderModification.Key.BrokerOrderId}) => {Broker.FXCM}");
            });
        }

        /// <summary>
        /// Submits a cancel order.
        /// </summary>
        /// <param name="order">The order to cancel.</param>
        public void CancelOrder(Order order)
        {
            Debug.NotNull(order, nameof(order));

            this.Execute(() =>
            {
                var message = OrderCancelRequestFactory.Create(
                    FxcmSymbolProvider.GetBrokerSymbol(order.Symbol.Code).Value,
                    order,
                    this.TimeNow());

                this.fixSession.Send(message);

                this.Log.Information(
                    $"{order.Symbol} Submitting OrderCancelRequestFactory: " +
                    $"(ClOrdId={order.OrderId}, OrderId={order.BrokerOrderId}) => {Broker.FXCM}");
            });
        }

        /// <summary>
        /// Submits a request to close a position.
        /// </summary>
        /// <param name="position">The position to close.
        /// </param>
        public void ClosePosition(Position position)
        {
            Debug.NotNull(position, nameof(position));

            this.Execute(() =>
            {
                // TODO
            });
        }
    }
}
