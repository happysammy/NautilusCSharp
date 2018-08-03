//--------------------------------------------------------------------------------------------------
// <copyright file="DataService.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data
{
    using System;
    using System.Collections.Generic;
    using Nautilus.Common.Commands;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The main macro object which contains the <see cref="DataService"/> and presents its API.
    /// </summary>
    [PerformanceOptimized]
    public sealed class DataService : ActorComponentBusConnectedBase
    {
        private readonly IFixClient fixClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataService"/> class.
        /// </summary>
        /// <param name="setupContainer">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="fixClient">The FIX client.</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public DataService(
            IComponentryContainer setupContainer,
            IMessagingAdapter messagingAdapter,
            IFixClient fixClient)
            : base(
                NautilusService.Data,
                LabelFactory.Component(nameof(DataService)),
                setupContainer,
                messagingAdapter)
        {
            Validate.NotNull(setupContainer, nameof(setupContainer));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(fixClient, nameof(fixClient));

            this.fixClient = fixClient;

            // Command message handling.
            this.Receive<SystemStart>(msg => this.OnMessage(msg));
            this.Receive<SystemShutdown>(msg => this.OnMessage(msg));
        }

        private void OnMessage(SystemStart message)
        {
            this.fixClient.Connect();

            while (!this.fixClient.IsConnected)
            {
                // Wait for connection.
            }

            this.fixClient.UpdateInstrumentsSubscribeAll();
            this.fixClient.RequestMarketDataSubscribeAll();

            var startSystem = new SystemStart(Guid.NewGuid(), this.TimeNow());
            this.Send(NautilusService.DataCollectionManager, startSystem);

            var barSpecs = new List<BarSpecification>
            {
                new BarSpecification(QuoteType.Bid, Resolution.Second, 1),
                new BarSpecification(QuoteType.Ask, Resolution.Second, 1),
                new BarSpecification(QuoteType.Mid, Resolution.Second, 1),
                new BarSpecification(QuoteType.Bid, Resolution.Minute, 1),
                new BarSpecification(QuoteType.Ask, Resolution.Minute, 1),
                new BarSpecification(QuoteType.Mid, Resolution.Minute, 1),
                new BarSpecification(QuoteType.Bid, Resolution.Hour, 1),
                new BarSpecification(QuoteType.Ask, Resolution.Hour, 1),
                new BarSpecification(QuoteType.Mid, Resolution.Hour, 1),
            };

            foreach (var symbol in this.fixClient.GetAllSymbols())
            {
                foreach (var barSpec in barSpecs)
                {
                    var barType = new BarType(symbol, barSpec);
                    var subscribe = new Subscribe<BarType>(
                        barType,
                        this.NewGuid(),
                        this.TimeNow());

                    this.Send(NautilusService.DataCollectionManager, subscribe);
                }
            }
        }

        private void OnMessage(SystemShutdown message)
        {
            this.fixClient.Disconnect();
            Context.Stop(Context.Self);
        }
    }
}
