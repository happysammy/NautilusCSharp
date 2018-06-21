//--------------------------------------------------------------------------------------------------
// <copyright file="FxcmFixClient.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix
{
    using System.Collections.Generic;
    using Nautilus.Core.Validation;
    using Nautilus.BlackBox.Core.Enums;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Factories;
    using Nautilus.Fix.Interfaces;
    using Price = Nautilus.DomainModel.ValueObjects.Price;
    using Symbol = Nautilus.DomainModel.ValueObjects.Symbol;

    /// <summary>
    /// Provides a generic QuickFix client.
    /// </summary>
    public class FixClient : FixComponentBase, IDataFeedClient, IBrokerageClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FixClient"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="broker">The account brokerage.</param>
        /// <param name="tickDataProcessor">The tick data processor.</param>
        /// <param name="fixMessageHandler">The FIX message handler.</param>
        /// <param name="fixMessageRouter">The FIX message router.</param>
        /// <param name="credentials">The FIX account credentials.</param>
        public FixClient(
            IComponentryContainer container,
            ITickDataProcessor tickDataProcessor,
            IFixMessageHandler fixMessageHandler,
            IFixMessageRouter fixMessageRouter,
            FixCredentials credentials,
            Broker broker)
        : base(
            ServiceContext.FIX,
            LabelFactory.Service(BlackBoxService.Brokerage),
            container,
            tickDataProcessor,
            fixMessageHandler,
            fixMessageRouter,
            credentials)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(credentials, nameof(credentials));

            this.Broker = broker;
        }

        /// <summary>
        /// Gets the name of the brokerage.
        /// </summary>
        public Broker Broker { get; }

        /// <summary>
        /// Returns a value indicating whether the FIX session is connected.
        /// </summary>
        /// <returns>A <see cref="bool"/>.</returns>
        public bool IsConnected => this.IsFixConnected;

        /// <summary>
        /// The initializes the brokerage gateway.
        /// </summary>
        /// <param name="gateway">The brokerage gateway.</param>
        public void InitializeBrokerageGateway(IBrokerageGateway gateway)
        {
            Validate.NotNull(gateway, nameof(gateway));

            this.FxcmFixMessageHandler.InitializeBrokerageGateway(gateway);
            this.FxcmFixMessageRouter.InitializeBrokerageGateway(gateway);
        }

        /// <summary>
        /// Connects to the FIX session.
        /// </summary>
        public void Connect()
        {
            this.ConnectFix();
        }

        /// <summary>
        /// Disconnects from the FIX session.
        /// </summary>
        public void Disconnect()
        {
            this.DisconnectFix();
        }

        /// <summary>
        /// Initializes the FIX session. Performs actions on logon.
        /// </summary>
        public void InitializeSession()
        {
            this.CollateralInquiry();
            this.TradingSessionStatus();
            this.RequestAllPositions();
            this.UpdateInstrumentsSubscribeAll();
            // TODO: Subscribe to all market data.
        }

        /// <summary>
        /// The submit entry limit stop order.
        /// </summary>
        /// <param name="elsOrder">The ELS order.</param>
        public void SubmitEntryLimitStopOrder(AtomicOrder elsOrder)
        {
            this.Execute(() =>
            {
                this.FxcmFixMessageRouter.SubmitEntryLimitStopOrder(elsOrder);
            });
        }

        /// <summary>
        /// The submit entry stop order.
        /// </summary>
        /// <param name="elsOrder">The ELS order.</param>
        public void SubmitEntryStopOrder(AtomicOrder elsOrder)
        {
            this.Execute(() =>
            {
                this.FxcmFixMessageRouter.SubmitEntryStopOrder(elsOrder);
            });
        }

        /// <summary>
        /// The modify stop-loss order.
        /// </summary>
        /// <param name="orderModification">The order modification.</param>
        public void ModifyStoplossOrder(KeyValuePair<Order, Price> orderModification)
        {
            this.FxcmFixMessageRouter.ModifyStoplossOrder(orderModification);
        }

        /// <summary>
        /// The cancel order.
        /// </summary>
        /// <param name="order">The order.</param>
        public void CancelOrder(Order order)
        {
            this.FxcmFixMessageRouter.CancelOrder(order);
        }

        /// <summary>
        /// The close position.
        /// </summary>
        /// <param name="position">The position.</param>
        public void ClosePosition(Position position)
        {
        }

        /// <summary>
        /// The collateral inquiry.
        /// </summary>
        public void CollateralInquiry()
        {
            this.FxcmFixMessageRouter.CollateralInquiry();
        }

        /// <summary>
        /// The trading session status.
        /// </summary>
        public void TradingSessionStatus()
        {
            this.FxcmFixMessageRouter.TradingSessionStatus();
        }

        /// <summary>
        /// Sends request all positions.
        /// </summary>
        public void RequestAllPositions()
        {
            this.FxcmFixMessageRouter.RequestAllPositions();
        }

        /// <summary>
        /// The update instrument subscribe.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        public void UpdateInstrumentSubscribe(Symbol symbol)
        {
            this.FxcmFixMessageRouter.UpdateInstrumentSubscribe(symbol);
        }

        /// <summary>
        /// The update instruments subscribe all.
        /// </summary>
        public void UpdateInstrumentsSubscribeAll()
        {
            this.FxcmFixMessageRouter.UpdateInstrumentsSubscribeAll();
        }

        /// <summary>
        /// The request market data subscribe.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        public void RequestMarketDataSubscribe(Symbol symbol)
        {
            this.FxcmFixMessageRouter.MarketDataRequestSubscribe(symbol);
        }
    }
}
