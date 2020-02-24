﻿//--------------------------------------------------------------------------------------------------
// <copyright file="FixTradingGateway.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix
{
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Common.Messaging;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Provides a gateway to the brokers FIX trading network.
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
        public void SubmitOrder(AtomicOrder atomicOrder)
        {
            this.fixClient.NewOrderList(atomicOrder);
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
