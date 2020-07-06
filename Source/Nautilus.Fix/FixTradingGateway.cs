//--------------------------------------------------------------------------------------------------
// <copyright file="FixTradingGateway.cs" company="Nautech Systems Pty Ltd">
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

using Nautilus.Common.Interfaces;
using Nautilus.Common.Messages.Commands;
using Nautilus.Common.Messaging;
using Nautilus.DomainModel.Aggregates;
using Nautilus.DomainModel.Entities;
using Nautilus.DomainModel.Events;
using Nautilus.DomainModel.Identifiers;
using Nautilus.DomainModel.ValueObjects;

namespace Nautilus.Fix
{
    /// <summary>
    /// Provides a gateway to the brokers FIX trading servers.
    /// </summary>
    public sealed class FixTradingGateway : MessageBusConnected, ITradingGateway
    {
        private readonly IFixClient fixClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixTradingGateway"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="messageBusAdapter">The messaging adapter.</param>
        /// <param name="fixClient">The FIX client.</param>
        public FixTradingGateway(
            IComponentryContainer container,
            IMessageBusAdapter messageBusAdapter,
            IFixClient fixClient)
            : base(container, messageBusAdapter)
        {
            this.fixClient = fixClient;

            // Commands
            this.RegisterHandler<ConnectSession>(this.OnMessage);
            this.RegisterHandler<DisconnectSession>(this.OnMessage);

            // Events
            this.RegisterHandler<OrderSubmitted>(this.OnMessage);
            this.RegisterHandler<OrderAccepted>(this.OnMessage);
            this.RegisterHandler<OrderRejected>(this.OnMessage);
            this.RegisterHandler<OrderWorking>(this.OnMessage);
            this.RegisterHandler<OrderModified>(this.OnMessage);
            this.RegisterHandler<OrderCancelReject>(this.OnMessage);
            this.RegisterHandler<OrderExpired>(this.OnMessage);
            this.RegisterHandler<OrderCancelled>(this.OnMessage);
            this.RegisterHandler<OrderPartiallyFilled>(this.OnMessage);
            this.RegisterHandler<OrderFilled>(this.OnMessage);
            this.RegisterHandler<AccountStateEvent>(this.OnMessage);
        }

        /// <inheritdoc />
        public void AccountInquiry()
        {
            this.fixClient.CollateralInquiry();
        }

        /// <inheritdoc />
        public void SubscribeToPositionEvents()
        {
            this.fixClient.RequestForAllPositionsSubscribe();
        }

        /// <inheritdoc />
        public void SubmitOrder(Order order, PositionIdBroker? positionIdBroker)
        {
            this.fixClient.NewOrderSingle(order, positionIdBroker);
        }

        /// <inheritdoc />
        public void SubmitOrder(BracketOrder bracketOrder)
        {
            this.fixClient.NewOrderList(bracketOrder);
        }

        /// <inheritdoc />
        public void ModifyOrder(Order order, Quantity modifiedQuantity, Price modifiedPrice)
        {
            this.fixClient.OrderCancelReplaceRequest(order, modifiedQuantity, modifiedPrice);
        }

        /// <inheritdoc />
        public void CancelOrder(Order order)
        {
            this.fixClient.OrderCancelRequest(order);
        }

        /// <inheritdoc />
        protected override void OnStart(Start start)
        {
            this.fixClient.Connect();
        }

        /// <inheritdoc />
        protected override void OnStop(Stop stop)
        {
            this.fixClient.Disconnect();
        }

        private void OnMessage(ConnectSession message)
        {
            this.fixClient.Connect();
        }

        private void OnMessage(DisconnectSession message)
        {
            this.fixClient.Disconnect();
        }

        private void OnMessage(OrderSubmitted @event)
        {
            this.SendToBus(@event);
        }

        private void OnMessage(OrderAccepted @event)
        {
            this.SendToBus(@event);
        }

        private void OnMessage(OrderRejected @event)
        {
            this.SendToBus(@event);
        }

        private void OnMessage(OrderWorking @event)
        {
            this.SendToBus(@event);
        }

        private void OnMessage(OrderModified @event)
        {
            this.SendToBus(@event);
        }

        private void OnMessage(OrderCancelReject @event)
        {
            this.SendToBus(@event);
        }

        private void OnMessage(OrderExpired @event)
        {
            this.SendToBus(@event);
        }

        private void OnMessage(OrderCancelled @event)
        {
            this.SendToBus(@event);
        }

        private void OnMessage(OrderPartiallyFilled @event)
        {
            this.SendToBus(@event);
        }

        private void OnMessage(OrderFilled @event)
        {
            this.SendToBus(@event);
        }

        private void OnMessage(AccountStateEvent @event)
        {
            this.SendToBus(@event);
        }
    }
}
