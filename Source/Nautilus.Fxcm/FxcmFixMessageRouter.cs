//--------------------------------------------------------------------------------------------------
// <copyright file="FxcmFixMessageRouter.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fxcm
{
    using System.Collections.Immutable;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Fix;
    using Nautilus.Fix.Interfaces;
    using Nautilus.Fxcm.MessageFactories;
    using QuickFix;

    /// <summary>
    /// Provides an implementation for routing FXCM FIX messages.
    /// </summary>
    public sealed class FxcmFixMessageRouter : Component, IFixMessageRouter
    {
        private const string Sent = "-->";
        private const string Protocol = "[FIX]";

        private readonly SymbolConverter symbolConverter;
        private readonly AccountId accountId;

        private Session? session;

        /// <summary>
        /// Initializes a new instance of the <see cref="FxcmFixMessageRouter"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="accountId">The account identifier for the router.</param>
        /// <param name="symbolMap">The symbol provider.</param>
        public FxcmFixMessageRouter(
            IComponentryContainer container,
            AccountId accountId,
            ImmutableDictionary<string, string> symbolMap)
        : base(container)
        {
            this.accountId = accountId;
            this.symbolConverter = new SymbolConverter(symbolMap);
        }

        /// <inheritdoc />
        public void InitializeSession(Session newSession)
        {
            this.session = newSession;
        }

        /// <inheritdoc />
        public void CollateralInquiry()
        {
            this.SendFixMessage(CollateralInquiryFactory.Create(this.TimeNow()));
        }

        /// <inheritdoc />
        public void TradingSessionStatusRequest()
        {
            this.SendFixMessage(TradingSessionStatusRequestFactory.Create(this.TimeNow()));
        }

        /// <inheritdoc />
        public void RequestForOpenPositionsSubscribe()
        {
            this.SendFixMessage(RequestForPositionsFactory.OpenAll(this.TimeNow(), this.accountId.AccountNumber));
        }

        /// <inheritdoc />
        public void RequestForClosedPositionsSubscribe()
        {
            this.SendFixMessage(RequestForPositionsFactory.ClosedAll(this.TimeNow(), this.accountId.AccountNumber));
        }

        /// <inheritdoc />
        public void SecurityListRequestSubscribe(Symbol symbol)
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

        /// <inheritdoc />
        public void SecurityListRequestSubscribeAll()
        {
            var message = SecurityListRequestFactory.Create(this.TimeNow());

            this.SendFixMessage(message);
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public void NewOrderSingle(Order order, PositionIdBroker? positionIdBroker)
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
                positionIdBroker,
                this.TimeNow());

            this.SendFixMessage(message);
        }

        /// <inheritdoc />
        public void NewOrderList(AtomicOrder atomicOrder)
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

        /// <inheritdoc />
        public void OrderCancelReplaceRequest(Order order, Quantity modifiedQuantity, Price modifiedPrice)
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
                modifiedQuantity.Value,
                modifiedPrice.Value,
                this.TimeNow());

            this.SendFixMessage(message);
        }

        /// <inheritdoc />
        public void OrderCancelRequest(Order order)
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
            if (this.session is null)
            {
                this.Log.Error($"Cannot send FIX message (the session is null).");
                return;
            }

            this.session.Send(message);

            this.LogMessageSent(message);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        private void LogMessageSent(Message message)
        {
            this.Log.Debug($"{Protocol}{Sent} {message.GetType().Name}");
        }
    }
}
