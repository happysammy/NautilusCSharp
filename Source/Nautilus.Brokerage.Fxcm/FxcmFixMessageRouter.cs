//--------------------------------------------------------------------------------------------------
// <copyright file="FxcmFixMessageRouter.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Brokerage.FXCM
{
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Validation;
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
        public FxcmFixMessageRouter(
            IComponentryContainer container,
            InstrumentDataProvider instrumentData,
            string accountNumber)
        : base(
            NautilusService.FIX,
            LabelFactory.Create(nameof(FxcmFixMessageRouter)),
            container)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(instrumentData, nameof(instrumentData));
            Validate.NotEmptyOrWhiteSpace(accountNumber, nameof(accountNumber));

            this.instrumentData = instrumentData;
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
        /// Sends a new collateral inquiry FIX message.
        /// </summary>
        public void CollateralInquiry()
        {
            this.Execute(() =>
            {
                var message = CollateralInquiryFactory.Create(this.TimeNow(), Brokerage.FXCM);

                this.fixSession?.Send(message);

                this.Log.Debug($"CollateralInquiry + SubscribeCollateralReports...");
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

                this.fixSession?.Send(message);

                this.Log.Debug($"TradingSessionStatusRequest...");
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

                this.fixSession?.Send(message);

                this.Log.Debug($"RequestForOpenPositions + SubscribePositionReports...");
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
                var fxcmSymbol = this.instrumentData.GetBrokerSymbol(symbol.Code);

                this.fixSession?.Send(SecurityListRequestFactory.Create(
                    fxcmSymbol.Value,
                    this.TimeNow()));

                this.Log.Debug($"SecurityStatusRequest + SubscribeUpdates ({symbol})...");
            });
        }

        /// <summary>
        /// Updates all instruments via a security status request FIX message.
        /// </summary>
        public void UpdateInstrumentsSubscribeAll()
        {
            this.Execute(() =>
            {
                this.fixSession?.Send(SecurityListRequestFactory.Create(this.TimeNow()));

                this.Log.Debug($"SecurityStatusRequest + SubscribeUpdates (ALL)...");
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
                var brokerSymbol = this.instrumentData.GetBrokerSymbol(symbol.Code).Value;

                this.fixSession?.Send(MarketDataRequestFactory.Create(
                    brokerSymbol,
                    0,
                    this.TimeNow()));

                this.Log.Debug($"MarketDataRequest + SubscribeUpdates ({symbol})...");
            });
        }

        /// <summary>
        /// Subscribes to market data for all symbol.
        /// </summary>
        public void MarketDataRequestSubscribeAll()
        {
            this.Execute(() =>
            {
                foreach (var brokerSymbol in this.instrumentData.GetAllBrokerSymbols())
                {
                    this.fixSession?.Send(MarketDataRequestFactory.Create(
                        brokerSymbol,
                        0,
                        this.TimeNow()));

                    this.Log.Debug($"MarketDataRequest + SubscribeUpdates ({brokerSymbol})...");
                }
            });
        }

        /// <summary>
        /// Submits an order.
        /// </summary>
        /// <param name="order">The order to submit.</param>
        public void SubmitOrder(Order order)
        {
            Debug.NotNull(order, nameof(order));

            this.Execute(() =>
            {
                var message = NewOrderSingleFactory.Create(
                    this.instrumentData.GetBrokerSymbol(order.Symbol.Code).Value,
                    this.accountNumber,
                    order,
                    this.TimeNow());

                this.fixSession?.Send(message);

                this.Log.Information($"Submitting Order => {Brokerage.FXCM}");
            });
        }

        /// <summary>
        /// Submits a trade.
        /// </summary>
        /// <param name="atomicOrder">The atomic order to submit.</param>
        public void SubmitOrder(AtomicOrder atomicOrder)
        {
            Debug.NotNull(atomicOrder, nameof(atomicOrder));

            this.Execute(() =>
            {
                var brokerSymbol = this.instrumentData.GetBrokerSymbol(atomicOrder.Symbol.Code).Value;

                if (atomicOrder.ProfitTarget.HasValue)
                {
                    var message = NewOrderListEntryFactory.CreateWithStopLossAndProfitTarget(
                        brokerSymbol,
                        this.accountNumber,
                        atomicOrder,
                        this.TimeNow());
                    this.fixSession?.Send(message);
                }
                else
                {
                    var message = NewOrderListEntryFactory.CreateWithStopLoss(
                        brokerSymbol,
                        this.accountNumber,
                        atomicOrder,
                        this.TimeNow());
                    this.fixSession?.Send(message);
                }

                this.Log.Information($"Submitting ELS Order => {Brokerage.FXCM}");
            });
        }

        /// <summary>
        /// Submits a FIX order cancel replace request.
        /// </summary>
        /// <param name="order">The order to modify.</param>
        /// <param name="modifiedPrice">The modified order price.</param>
        public void ModifyOrder(Order order, Price modifiedPrice)
        {
            Debug.NotNull(order, nameof(order));
            Debug.NotNull(modifiedPrice, nameof(modifiedPrice));

            this.Execute(() =>
            {
                var message = OrderCancelReplaceRequestFactory.Create(
                    this.instrumentData.GetBrokerSymbol(order.Symbol.Code).Value,
                    order,
                    modifiedPrice.Value,
                    this.TimeNow());

                this.fixSession?.Send(message);

                this.Log.Information(
                    $"{order.Symbol} Submitting OrderReplaceRequest: " +
                    $"(ClOrdId={order.Id}, " +
                    $"OrderId={order.IdBroker}) => {Brokerage.FXCM}");
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
                    this.instrumentData.GetBrokerSymbol(order.Symbol.Code).Value,
                    order,
                    this.TimeNow());

                this.fixSession?.Send(message);

                this.Log.Information(
                    $"{order.Symbol} Submitting OrderCancelRequestFactory: " +
                    $"(ClOrdId={order.Id}, OrderId={order.IdBroker}) => {Brokerage.FXCM}");
            });
        }

        /// <summary>
        /// Submits a request to close a position.
        /// </summary>
        /// <param name="command">The close position command.</param>
        public void ClosePosition(Position command)
        {
            Debug.NotNull(command, nameof(command));

            this.Execute(() =>
            {
                // TODO
            });
        }
    }
}
