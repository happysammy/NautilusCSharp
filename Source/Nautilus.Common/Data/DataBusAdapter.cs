// -------------------------------------------------------------------------------------------------
// <copyright file="DataBusAdapter.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging;
    using Nautilus.Messaging.Interfaces;
    using NodaTime;

    /// <summary>
    /// Provides a means for components to subscribe to and unsubscribe from various data types.
    /// </summary>
    [Immutable]
    public class DataBusAdapter : IDataBusAdapter
    {
        private readonly ImmutableDictionary<Type, IEndpoint> endpoints;
        private readonly IEndpoint tickBus;
        private readonly IEndpoint barBus;
        private readonly IEndpoint instrumentBus;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataBusAdapter"/> class.
        /// </summary>
        /// <param name="endpoints">The data bus endpoints.</param>
        /// <param name="tickBus">The tick bus endpoint.</param>
        /// <param name="barBus">The bar bus endpoint.</param>
        /// <param name="instrumentBus">The instrument bus endpoint.</param>
        public DataBusAdapter(
            Dictionary<Type, IEndpoint> endpoints,
            IEndpoint tickBus,
            IEndpoint barBus,
            IEndpoint instrumentBus)
        {
            this.endpoints = endpoints.ToImmutableDictionary();

            // TODO: Make more generic
            this.tickBus = tickBus;
            this.barBus = barBus;
            this.instrumentBus = instrumentBus;
        }

        /// <inheritdoc />
        public void SendToBus(Tick data)
        {
            this.tickBus.Send(data);
        }

        /// <inheritdoc />
        public void SendToBus(BarData data)
        {
            this.barBus.Send(data);
        }

        /// <inheritdoc />
        public void SendToBus(Instrument data)
        {
            this.instrumentBus.Send(data);
        }

        /// <inheritdoc />
        public void Subscribe<T>(Mailbox subscriber, Guid id, ZonedDateTime timestamp)
        {
            var subscription = typeof(T);
            var message = new Subscribe<Type>(
                subscription,
                subscriber,
                id,
                timestamp);

            this.Send(subscription, message);
        }

        /// <inheritdoc />
        public void Unsubscribe<T>(Mailbox subscriber, Guid id, ZonedDateTime timestamp)
        {
            var subscription = typeof(T);
            var message = new Unsubscribe<Type>(
                subscription,
                subscriber,
                id,
                timestamp);

            this.Send(subscription, message);
        }

        private void Send(Type subscription, Message message)
        {
            if (this.endpoints.TryGetValue(subscription, out var endpoint))
            {
                endpoint.Send(message);
            }
            else
            {
                // Design time error
                throw new InvalidOperationException($"Invalid data type for subscription ({subscription}).");
            }
        }
    }
}
