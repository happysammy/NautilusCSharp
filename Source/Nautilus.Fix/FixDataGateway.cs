﻿//--------------------------------------------------------------------------------------------------
// <copyright file="FixDataGateway.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix
{
    using System.Collections.Generic;
    using Nautilus.Common.Data;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Provides a gateway to the data providers FIX network.
    /// </summary>
    public sealed class FixDataGateway : DataBusConnected, IDataGateway
    {
        private readonly IFixClient fixClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixDataGateway"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="dataBusAdapter">The data bus adapter.</param>
        /// <param name="fixClient">The FIX client.</param>
        public FixDataGateway(
            IComponentryContainer container,
            IDataBusAdapter dataBusAdapter,
            IFixClient fixClient)
            : base(container, dataBusAdapter)
        {
            this.fixClient = fixClient;

            this.RegisterHandler<ConnectSession>(this.OnMessage);
            this.RegisterHandler<DisconnectSession>(this.OnMessage);
        }

        /// <inheritdoc />
        public Brokerage Brokerage => this.fixClient.Brokerage;

        /// <inheritdoc />
        public bool IsConnected => this.fixClient.IsConnected;

        /// <inheritdoc />
        public void MarketDataSubscribe(Symbol symbol)
        {
            this.fixClient.MarketDataRequestSubscribe(symbol);
        }

        /// <inheritdoc />
        public void MarketDataSubscribeAll()
        {
            this.fixClient.MarketDataRequestSubscribeAll();
        }

        /// <inheritdoc />
        public void UpdateInstrumentSubscribe(Symbol symbol)
        {
            this.fixClient.SecurityListRequestSubscribe(symbol);
        }

        /// <inheritdoc />
        public void UpdateInstrumentsSubscribeAll()
        {
            this.fixClient.SecurityListRequestSubscribeAll();
        }

        /// <inheritdoc />
        public void OnData(Tick tick)
        {
            this.SendToBus(tick);
        }

        /// <inheritdoc />
        public void OnData(IEnumerable<Instrument> instruments)
        {
            foreach (var instrument in instruments)
            {
                this.SendToBus(instrument);
            }
        }

        /// <inheritdoc />
        public void OnMessage(string message)
        {
            // No implemented yet (general business messages)
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
    }
}
