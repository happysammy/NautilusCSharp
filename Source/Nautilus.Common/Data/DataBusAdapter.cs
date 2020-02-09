// -------------------------------------------------------------------------------------------------
// <copyright file="DataBusAdapter.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
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
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Types;
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
        private readonly DataBus<Tick> tickBus;
        private readonly DataBus<BarData> barBus;
        private readonly DataBus<Instrument> instrumentBus;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataBusAdapter"/> class.
        /// </summary>
        /// <param name="tickBus">The tick bus endpoint.</param>
        /// <param name="barBus">The bar bus endpoint.</param>
        /// <param name="instrumentBus">The instrument bus endpoint.</param>
        public DataBusAdapter(
            DataBus<Tick> tickBus,
            DataBus<BarData> barBus,
            DataBus<Instrument> instrumentBus)
        {
            this.endpoints = new Dictionary<Type, IEndpoint>
            {
                { tickBus.BusType, tickBus.Endpoint },
                { barBus.BusType, barBus.Endpoint },
                { instrumentBus.BusType, instrumentBus.Endpoint },
            }.ToImmutableDictionary();

            this.tickBus = tickBus;
            this.barBus = barBus;
            this.instrumentBus = instrumentBus;
        }

        /// <inheritdoc />
        public void SendToBus(Tick data)
        {
            this.tickBus.Endpoint.Send(data);
        }

        /// <inheritdoc />
        public void SendToBus(BarData data)
        {
            this.barBus.Endpoint.Send(data);
        }

        /// <inheritdoc />
        public void SendToBus(Instrument data)
        {
            this.instrumentBus.Endpoint.Send(data);
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

        /// <summary>
        /// Stops the message bus.
        /// </summary>
        public void Stop()
        {
            this.tickBus.Stop();
            this.barBus.Stop();
            this.instrumentBus.Stop();
        }

        private void Send(Type subscription, Message message)
        {
            if (this.endpoints.TryGetValue(subscription, out var endpoint))
            {
                endpoint.Send(message);
            }
            else
            {
                throw new InvalidOperationException($"Invalid data type for subscription ({subscription}).");
            }
        }
    }
}
