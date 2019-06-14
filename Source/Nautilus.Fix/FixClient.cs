//--------------------------------------------------------------------------------------------------
// <copyright file="FixClient.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix
{
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Annotations;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Fix.Interfaces;

    /// <summary>
    /// Provides a FIX client.
    /// </summary>
    [PerformanceOptimized]
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
                this.Log.Warning($"Already connected to FIX session {this.Broker}-{this.Account}...");
                return;
            }

            this.Log.Information($"Connecting to FIX session {this.Broker}-{this.Account}...");
            this.ConnectFix();
        }

        /// <inheritdoc />
        public void Disconnect()
        {
            if (!this.IsConnected)
            {
                this.Log.Warning($"Already disconnected from FIX session {this.Broker}-{this.Account}...");
                return;
            }

            this.Log.Information($"Disconnecting from FIX session {this.Broker}-{this.Account}...");
            this.DisconnectFix();
        }

        /// <inheritdoc />
        public void SubmitOrder(Order order)
        {
            this.FixMessageRouter.SubmitOrder(order);
        }

        /// <inheritdoc />
        public void SubmitOrder(AtomicOrder atomicOrder)
        {
            this.FixMessageRouter.SubmitOrder(atomicOrder);
        }

        /// <inheritdoc />
        public void ModifyOrder(Order order, Price modifiedPrice)
        {
            this.FixMessageRouter.ModifyOrder(order, modifiedPrice);
        }

        /// <inheritdoc />
        public void CancelOrder(Order command)
        {
            this.FixMessageRouter.CancelOrder(command);
        }

        /// <inheritdoc />
        public void CollateralInquiry()
        {
            this.FixMessageRouter.CollateralInquiry();
        }

        /// <inheritdoc />
        public void TradingSessionStatus()
        {
            this.FixMessageRouter.TradingSessionStatus();
        }

        /// <inheritdoc />
        public void UpdateInstrumentSubscribe(Symbol symbol)
        {
            this.FixMessageRouter.UpdateInstrumentSubscribe(symbol);
        }

        /// <inheritdoc />
        public void UpdateInstrumentsSubscribeAll()
        {
            this.FixMessageRouter.UpdateInstrumentsSubscribeAll();
        }

        /// <inheritdoc />
        public void RequestMarketDataSubscribe(Symbol symbol)
        {
            this.FixMessageRouter.MarketDataRequestSubscribe(symbol);
        }

        /// <inheritdoc />
        public void RequestMarketDataSubscribeAll()
        {
            this.FixMessageRouter.MarketDataRequestSubscribeAll();
        }
    }
}
