//--------------------------------------------------------------------------------------------------
// <copyright file="FixClient.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix
{
    using System.Collections.Generic;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Annotations;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Fix.Interfaces;
    using Nautilus.Messaging.Interfaces;

    /// <summary>
    /// Provides a generic FIX client.
    /// </summary>
    [PerformanceOptimized]
    public class FixClient : FixComponent, IFixClient
    {
        private readonly SymbolConverter symbolConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixClient"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="config">The FIX configuration.</param>
        /// <param name="messageHandler">The FIX message handler.</param>
        /// <param name="messageRouter">The FIX message router.</param>
        /// <param name="symbolConverter">The symbol provider.</param>
        public FixClient(
            IComponentryContainer container,
            FixConfiguration config,
            IFixMessageHandler messageHandler,
            IFixMessageRouter messageRouter,
            SymbolConverter symbolConverter)
        : base(
            container,
            config,
            messageHandler,
            messageRouter)
        {
            this.symbolConverter = symbolConverter;
        }

        /// <summary>
        /// Gets a value indicating whether the FIX session is connected.
        /// </summary>
        /// <returns>A <see cref="bool"/>.</returns>
        public bool IsConnected => this.IsFixConnected;

        /// <summary>
        /// Connects to the FIX session.
        /// </summary>
        public void Connect()
        {
            this.Log.Information($"Connecting to FIX session {this.Broker}-{this.Account}...");
            this.ConnectFix();
        }

        /// <summary>
        /// Disconnects from the FIX session.
        /// </summary>
        public void Disconnect()
        {
            this.Log.Information($"Disconnecting from FIX session {this.Broker}-{this.Account}...");
            this.DisconnectFix();
        }

        /// <summary>
        /// Returns a read-only list of all <see cref="Symbol"/>(s) provided by the FIX client.
        /// </summary>
        /// <returns>The list of symbols.</returns>
        public IEnumerable<Symbol> GetAllSymbols() =>
            this.symbolConverter.GetAllSymbols();

        /// <summary>
        /// Submit a command to execute the given order.
        /// </summary>
        /// <param name="order">The order to submit.</param>
        public void SubmitOrder(Order order)
        {
            this.Execute(() =>
            {
                this.FixMessageRouter.SubmitOrder(order);
            });
        }

        /// <summary>
        /// Submit a command to execute the given trade.
        /// </summary>
        /// <param name="atomicOrder">The atomic order to submit.</param>
        public void SubmitOrder(AtomicOrder atomicOrder)
        {
            this.Execute(() =>
            {
                this.FixMessageRouter.SubmitOrder(atomicOrder);
            });
        }

        /// <summary>
        /// Submit a command to the execution system to modify the given order.
        /// </summary>
        /// <param name="order">The order to modify.</param>
        /// <param name="modifiedPrice">The modified order price.</param>
        public void ModifyOrder(Order order, Price modifiedPrice)
        {
            this.FixMessageRouter.ModifyOrder(order, modifiedPrice);
        }

        /// <summary>
        /// Submit the command to the execution system to cancel the given order.
        /// </summary>
        /// <param name="command">The cancel order command.</param>
        public void CancelOrder(Order command)
        {
            this.FixMessageRouter.CancelOrder(command);
        }

        /// <summary>
        /// The collateral inquiry.
        /// </summary>
        public void CollateralInquiry()
        {
            this.FixMessageRouter.CollateralInquiry();
        }

        /// <summary>
        /// The trading session status.
        /// </summary>
        public void TradingSessionStatus()
        {
            this.FixMessageRouter.TradingSessionStatus();
        }

        /// <summary>
        /// Sends request all positions.
        /// </summary>
        public void RequestAllPositions()
        {
            this.FixMessageRouter.RequestAllPositions();
        }

        /// <summary>
        /// The update instrument subscribe.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        public void UpdateInstrumentSubscribe(Symbol symbol)
        {
            this.FixMessageRouter.UpdateInstrumentSubscribe(symbol);
        }

        /// <summary>
        /// The update instruments subscribe all.
        /// </summary>
        public void UpdateInstrumentsSubscribeAll()
        {
            this.FixMessageRouter.UpdateInstrumentsSubscribeAll();
        }

        /// <summary>
        /// The request market data subscribe.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        public void RequestMarketDataSubscribe(Symbol symbol)
        {
            this.FixMessageRouter.MarketDataRequestSubscribe(symbol);
        }

        /// <summary>
        /// The request market data subscribe all.
        /// </summary>
        public void RequestMarketDataSubscribeAll()
        {
            this.FixMessageRouter.MarketDataRequestSubscribeAll();
        }
    }
}
