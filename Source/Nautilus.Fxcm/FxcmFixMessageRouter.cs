//--------------------------------------------------------------------------------------------------
// <copyright file="FxcmFixMessageRouter.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
//--------------------------------------------------------------------------------------------------

using System;
using Microsoft.Extensions.Logging;
using Nautilus.Common.Componentry;
using Nautilus.Common.Interfaces;
using Nautilus.Common.Logging;
using Nautilus.DomainModel.Aggregates;
using Nautilus.DomainModel.Entities;
using Nautilus.DomainModel.Identifiers;
using Nautilus.DomainModel.ValueObjects;
using Nautilus.Fix.Interfaces;
using Nautilus.Fxcm.MessageFactories;
using QuickFix;

namespace Nautilus.Fxcm
{
    /// <summary>
    /// Provides a router for FXCM FIX messages.
    /// </summary>
    public sealed class FxcmFixMessageRouter : Component, IFixMessageRouter
    {
        private const string Sent = "-->";
        private const string Protocol = "[FIX]";

        private readonly AccountId accountId;

        private Session? session;

        /// <summary>
        /// Initializes a new instance of the <see cref="FxcmFixMessageRouter"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="accountId">The account identifier for the router.</param>
        public FxcmFixMessageRouter(
            IComponentryContainer container,
            AccountId accountId)
        : base(container)
        {
            this.accountId = accountId;
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
            var message = SecurityListRequestFactory.Create(symbol.Code, this.TimeNow());

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
            var message = MarketDataRequestFactory.Create(
                symbol.Code,
                0,
                this.TimeNow());

            this.SendFixMessage(message);
        }

        /// <inheritdoc />
        public void NewOrderSingle(Order order, PositionIdBroker? positionIdBroker)
        {
            var message = NewOrderSingleFactory.Create(
                this.accountId.AccountNumber,
                order,
                positionIdBroker,
                this.TimeNow());

            this.SendFixMessage(message);
        }

        /// <inheritdoc />
        public void NewOrderList(BracketOrder bracketOrder)
        {
            if (bracketOrder.TakeProfit != null)
            {
                var message = NewOrderListEntryFactory.CreateWithStopLossAndTakeProfit(
                    bracketOrder.Symbol.Code,
                    this.accountId.AccountNumber,
                    bracketOrder,
                    this.TimeNow());

                this.SendFixMessage(message);
            }
            else
            {
                var message = NewOrderListEntryFactory.CreateWithStopLoss(
                    bracketOrder.Symbol.Code,
                    this.accountId.AccountNumber,
                    bracketOrder,
                    this.TimeNow());

                this.SendFixMessage(message);
            }
        }

        /// <inheritdoc />
        public void OrderCancelReplaceRequest(Order order, Quantity modifiedQuantity, Price modifiedPrice)
        {
            var message = OrderCancelReplaceRequestFactory.Create(
                order,
                modifiedQuantity.Value,
                modifiedPrice.Value,
                this.TimeNow());

            this.SendFixMessage(message);
        }

        /// <inheritdoc />
        public void OrderCancelRequest(Order order)
        {
            var message = OrderCancelRequestFactory.Create(
                order,
                this.TimeNow());

            this.SendFixMessage(message);
        }

        private void SendFixMessage(Message message)
        {
            if (this.session is null)
            {
                this.Logger.LogError(LogId.Network, "Cannot send FIX message (the session is null).");
                return;
            }

            try
            {
                this.session.Send(message);
                this.LogMessageSent(message);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(LogId.Network, ex.Message, ex);
            }
        }

        [System.Diagnostics.Conditional("DEBUG")]
        private void LogMessageSent(Message message)
        {
            this.Logger.LogDebug($"{Protocol}{Sent} {message.GetType().Name}");
        }
    }
}
