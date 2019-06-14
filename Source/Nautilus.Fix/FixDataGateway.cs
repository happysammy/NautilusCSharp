//--------------------------------------------------------------------------------------------------
// <copyright file="FixDataGateway.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix
{
    using System.Collections.Generic;
    using Nautilus.Common.Data;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Provides a gateway to, and anti-corruption layer from, the FIX module of the service.
    /// </summary>
    [PerformanceOptimized]
    public sealed class FixDataGateway : DataBusConnected, IDataGateway
    {
        private readonly IFixClient fixClient;
        private readonly Dictionary<string, Symbol> symbolCache;

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
            this.symbolCache = new Dictionary<string, Symbol>();

            this.RegisterHandler<ConnectFix>(this.OnMessage);
            this.RegisterHandler<DisconnectFix>(this.OnMessage);
        }

        /// <inheritdoc />
        public Brokerage Broker => this.fixClient.Broker;

        /// <inheritdoc />
        public bool IsConnected => this.fixClient.IsConnected;

        /// <inheritdoc />
        public void MarketDataSubscribe(Symbol symbol)
        {
            this.fixClient.RequestMarketDataSubscribe(symbol);
        }

        /// <inheritdoc />
        public void MarketDataSubscribeAll()
        {
            this.fixClient.RequestMarketDataSubscribeAll();
        }

        /// <inheritdoc />
        public void UpdateInstrumentSubscribe(Symbol symbol)
        {
            this.fixClient.UpdateInstrumentSubscribe(symbol);
        }

        /// <inheritdoc />
        public void UpdateInstrumentsSubscribeAll()
        {
            this.fixClient.UpdateInstrumentsSubscribeAll();
        }

        /// <inheritdoc />
        [SystemBoundary]
        [PerformanceOptimized]
        public void OnTick(
            Symbol symbol,
            decimal bid,
            decimal ask,
            ZonedDateTime timestamp)
        {
            this.Execute(() =>
            {
                var tick = new Tick(
                    symbol,
                    Price.Create(bid),
                    Price.Create(ask),
                    timestamp);

                this.SendToBus(tick);
            });
        }

        /// <inheritdoc />
        [SystemBoundary]
        public void OnBusinessMessage(string message)
        {
            this.Execute(() =>
            {
                Condition.NotEmptyOrWhiteSpace(message, nameof(message));

                this.Log.Debug($"BusinessMessageReject: {message}");
            });
        }

        /// <inheritdoc />
        [SystemBoundary]
        public void OnInstrumentsUpdate(
            IEnumerable<Instrument> instruments,
            string responseId,
            string result)
        {
            this.Execute(() =>
            {
                Condition.NotEmptyOrWhiteSpace(responseId, nameof(responseId));
                Condition.NotEmptyOrWhiteSpace(result, nameof(result));

                this.Log.Debug(
                    $"SecurityListReceived: " +
                    $"(SecurityResponseId={responseId}) result={result}");

                foreach (var instrument in instruments)
                {
                    this.SendToBus(instrument);
                }
            });
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

        private void OnMessage(ConnectFix message)
        {
            this.fixClient.Connect();
        }

        private void OnMessage(DisconnectFix message)
        {
            this.fixClient.Disconnect();
        }
    }
}
