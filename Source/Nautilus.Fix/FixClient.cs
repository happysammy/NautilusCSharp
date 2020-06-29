//--------------------------------------------------------------------------------------------------
// <copyright file="FixClient.cs" company="Nautech Systems Pty Ltd">
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

namespace Nautilus.Fix
{
    using Microsoft.Extensions.Logging;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Fix.Interfaces;

    /// <summary>
    /// Provides a FIX client.
    /// </summary>
    public sealed class FixClient : FixComponent, IFixClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FixClient"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="messageBusAdapter">The messaging adapter.</param>
        /// <param name="config">The FIX configuration.</param>
        /// <param name="messageHandler">The FIX message handler.</param>
        /// <param name="messageRouter">The FIX message router.</param>
        public FixClient(
            IComponentryContainer container,
            IMessageBusAdapter messageBusAdapter,
            FixConfiguration config,
            IFixMessageHandler messageHandler,
            IFixMessageRouter messageRouter)
        : base(
            container,
            messageBusAdapter,
            config,
            messageHandler,
            messageRouter)
        {
        }

        /// <inheritdoc />
        public void Connect()
        {
            if (this.IsConnected)
            {
                this.Logger.LogWarning($"Already connected to session {this.SessionId}.");
                return;
            }

            this.Logger.LogDebug("Connecting...");
            this.ConnectFix();
        }

        /// <inheritdoc />
        public void Disconnect()
        {
            if (this.IsDisconnected && this.SocketStopped)
            {
                this.Logger.LogWarning($"Already disconnected from session {this.SessionId}.");
                return;
            }

            this.Logger.LogDebug($"Disconnecting from session {this.SessionId}...");
            this.DisconnectFix();
        }

        /// <inheritdoc />
        public void CollateralInquiry()
        {
            this.FixMessageRouter.CollateralInquiry();
        }

        /// <inheritdoc />
        public void RequestForAllPositionsSubscribe()
        {
            this.FixMessageRouter.RequestForOpenPositionsSubscribe();
            this.FixMessageRouter.RequestForClosedPositionsSubscribe();
        }

        /// <inheritdoc />
        public void NewOrderSingle(Order order, PositionIdBroker? positionIdBroker)
        {
            this.FixMessageRouter.NewOrderSingle(order, positionIdBroker);
        }

        /// <inheritdoc />
        public void NewOrderList(AtomicOrder atomicOrder)
        {
            this.FixMessageRouter.NewOrderList(atomicOrder);
        }

        /// <inheritdoc />
        public void OrderCancelReplaceRequest(Order order, Quantity modifiedQuantity, Price modifiedPrice)
        {
            this.FixMessageRouter.OrderCancelReplaceRequest(order, modifiedQuantity, modifiedPrice);
        }

        /// <inheritdoc />
        public void OrderCancelRequest(Order command)
        {
            this.FixMessageRouter.OrderCancelRequest(command);
        }

        /// <inheritdoc />
        public void TradingSessionStatusRequest()
        {
            this.FixMessageRouter.TradingSessionStatusRequest();
        }

        /// <inheritdoc />
        public void SecurityListRequestSubscribe(Symbol symbol)
        {
            this.FixMessageRouter.SecurityListRequestSubscribe(symbol);
        }

        /// <inheritdoc />
        public void SecurityListRequestSubscribeAll()
        {
            this.FixMessageRouter.SecurityListRequestSubscribeAll();
        }

        /// <inheritdoc />
        public void MarketDataRequestSubscribe(Symbol symbol)
        {
            this.FixMessageRouter.MarketDataRequestSubscribe(symbol);
        }

        /// <inheritdoc />
        public void MarketDataRequestSubscribeAll()
        {
            this.FixMessageRouter.MarketDataRequestSubscribeAll();
        }
    }
}
