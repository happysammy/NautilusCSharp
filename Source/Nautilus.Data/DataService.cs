//--------------------------------------------------------------------------------------------------
// <copyright file="DataService.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data
{
    using Nautilus.Common.Commands;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Events;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The main macro object which contains the <see cref="DataService"/> and presents its API.
    /// </summary>
    [PerformanceOptimized]
    public sealed class DataService : ActorComponentBusConnectedBase
    {
        private readonly IFixGateway gateway;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataService"/> class.
        /// </summary>
        /// <param name="setupContainer">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="gateway">The FIX gateway.</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public DataService(
            IComponentryContainer setupContainer,
            IMessagingAdapter messagingAdapter,
            IFixGateway gateway)
            : base(
                NautilusService.Data,
                LabelFactory.Component(nameof(DataService)),
                setupContainer,
                messagingAdapter)
        {
            Validate.NotNull(setupContainer, nameof(setupContainer));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(gateway, nameof(gateway));

            this.gateway = gateway;

            // Setup message handling.
            this.Receive<BrokerageConnected>(this.OnMessage);
            this.Receive<BrokerageDisconnected>(this.OnMessage);
            this.Receive<Subscribe<BarType>>(this.OnMessage);
            this.Receive<Unsubscribe<BarType>>(this.OnMessage);
        }

        private void OnMessage(BrokerageConnected message)
        {
            Debug.NotNull(message, nameof(message));

            this.Log.Information($"{message.Broker} brokerage {message.Session} session is connected.");

            if (message.Session.Contains("MD"))
            {
                this.gateway.UpdateInstrumentsSubscribeAll();
                this.gateway.RequestMarketDataSubscribeAll();
            }
        }

        private void OnMessage(BrokerageDisconnected message)
        {
            Debug.NotNull(message, nameof(message));

            this.Log.Warning($"{message.Broker} brokerage {message.Session} session has been disconnected.");
        }

        private void OnMessage(Subscribe<BarType> message)
        {
            Debug.NotNull(message, nameof(message));

            this.Send(DataServiceAddress.DataCollectionManager, message);
        }

        private void OnMessage(Unsubscribe<BarType> message)
        {
            Debug.NotNull(message, nameof(message));

            this.Send(DataServiceAddress.DataCollectionManager, message);
        }
    }
}
