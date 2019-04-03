//--------------------------------------------------------------------------------------------------
// <copyright file="NautilusDatabase.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusDB
{
    using System.Collections.Generic;
    using Nautilus.Common;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Contains the Nautilus Database system.
    /// </summary>
    public sealed class NautilusDatabase : ComponentBusConnectedBase
    {
        private readonly SystemController systemController;
        private readonly IFixClient fixClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="NautilusDatabase"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="systemController">The system controller.</param>
        /// <param name="fixClient">The FIX client.</param>
        public NautilusDatabase(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            SystemController systemController,
            IFixClient fixClient)
            : base(
                NautilusService.Core,
                LabelFactory.Create(nameof(NautilusDatabase)),
                container,
                messagingAdapter)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(systemController, nameof(systemController));

            this.systemController = systemController;
            this.fixClient = fixClient;
        }

        /// <summary>
        /// Starts the system.
        /// </summary>
        public void Start()
        {
            this.fixClient.Connect();

            while (!this.fixClient.IsConnected)
            {
                // TODO: Create timeout.
                // Wait for connection.
            }

            this.systemController.Start();

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

                    this.Send(ServiceAddress.Data, subscribe);
                }
            }
        }

        /// <summary>
        /// Shuts down the system.
        /// </summary>
        public void Shutdown()
        {
            this.fixClient.Disconnect();
            this.systemController.Shutdown();
        }
    }
}
