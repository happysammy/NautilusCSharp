//--------------------------------------------------------------------------------------------------
// <copyright file="FixClient.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix
{
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
                this.Log.Warning($"Already connected to FIX session {this.FixSession}.");
                return;
            }

            this.Log.Information($"Connecting to FIX session for {this.AccountId.Value}...");
            this.ConnectFix();
        }

        /// <inheritdoc />
        public void Disconnect()
        {
            if (!this.IsConnected)
            {
                this.Log.Warning($"Already disconnected from FIX session {this.FixSession}.");
                return;
            }

            this.Log.Information($"Disconnecting from FIX session for {this.AccountId.Value}...");
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
