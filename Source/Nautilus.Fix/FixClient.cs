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
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core.Enums;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.Brokerage.FXCM;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Provides a generic QuickFix client.
    /// </summary>
    public class FixClient : FixComponentBase, IBrokerageClient
    {
        private readonly IList<Symbol> marketDataSubscriptions = new List<Symbol>();

        private IBrokerageGateway brokerageGateway;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixClient"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="broker">The account brokerage.</param>
        /// <param name="credentials">The FIX account credentials.</param>
        public FixClient(
            IComponentryContainer container,
            FixCredentials credentials,
            Broker broker)
        : base(
            ServiceContext.FIX,
            LabelFactory.Service(BlackBoxService.AlphaModel),
            container,
            credentials)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(credentials, nameof(credentials));

            this.Broker = broker;

            foreach (var symbol in ConversionRateCurrencySymbols.GetList())
            {
                if (!this.marketDataSubscriptions.Contains(symbol))
                {
                    this.marketDataSubscriptions.Add(symbol);
                }
            }
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
        /// The initialize brokerage gateway.
        /// </summary>
        /// <param name="gateway">The brokerage gateway.</param>
        public void InitializeBrokerageGateway(IBrokerageGateway gateway)
        {
            Validate.NotNull(gateway, nameof(gateway));

            this.brokerageGateway = gateway;
            this.FixMessageHandler.InitializeBrokerageGateway(gateway);
            this.FixMessageRouter.InitializeBrokerageGateway(gateway);
        }

        public void Connect()
        {
            this.ConnectFix();

            this.CollateralInquiry();
            this.TradingSessionStatus();
            this.RequestAllPositions();
            this.UpdateInstrumentsSubscribeAll();

            foreach (var symbol in this.marketDataSubscriptions)
            {
                this.RequestMarketDataSubscribe(symbol);
            }
        }

        public void Disconnect()
        {
            this.DisconnectFix();
        }

        /// <summary>
        /// The submit entry limit stop order.
        /// </summary>
        /// <param name="elsOrder">The ELS order.</param>
        public void SubmitEntryLimitStopOrder(AtomicOrder elsOrder)
        {
            this.Execute(() =>
            {
                this.FixMessageRouter.SubmitEntryLimitStopOrder(elsOrder);
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
                this.FixMessageRouter.SubmitEntryStopOrder(elsOrder);
            });
        }

        /// <summary>
        /// The modify stop-loss order.
        /// </summary>
        /// <param name="orderModification">The order modification.</param>
        public void ModifyStoplossOrder(KeyValuePair<Order, Price> orderModification)
        {
            this.FixMessageRouter.ModifyStoplossOrder(orderModification);
        }

        /// <summary>
        /// The cancel order.
        /// </summary>
        /// <param name="order">The order.</param>
        public void CancelOrder(Order order)
        {
            this.FixMessageRouter.CancelOrder(order);
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
            if (!this.marketDataSubscriptions.Contains(symbol))
            {
                this.marketDataSubscriptions.Add(symbol);
            }

            this.FixMessageRouter.MarketDataRequestSubscribe(symbol);
        }
    }
}
