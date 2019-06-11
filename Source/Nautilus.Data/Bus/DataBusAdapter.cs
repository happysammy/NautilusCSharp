// -------------------------------------------------------------------------------------------------
// <copyright file="DataBusAdapter.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Bus
{
    using System;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
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
        private readonly IEndpoint tickBus;
        private readonly IEndpoint dataBus;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataBusAdapter"/> class.
        /// </summary>
        /// <param name="tickBus">The tick bus endpoint.</param>
        /// <param name="dataBus">The data bus endpoint.</param>
        public DataBusAdapter(IEndpoint tickBus, IEndpoint dataBus)
        {
            this.tickBus = tickBus;
            this.dataBus = dataBus;
        }

        /// <inheritdoc />
        public void SendTick(Tick tick)
        {
            this.tickBus.Send(tick);
        }

        /// <inheritdoc />
        public void SendData(object data)
        {
            this.tickBus.Send(data);
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
            switch (subscription.Name)
            {
                case nameof(Tick):
                    this.tickBus.Send(message);
                    break;
                case nameof(Bar):
                case nameof(Instrument):
                    this.dataBus.Send(message);
                    break;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(subscription, nameof(subscription));
            }
        }
    }
}
