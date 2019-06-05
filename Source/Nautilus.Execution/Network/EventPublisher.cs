// -------------------------------------------------------------------------------------------------
// <copyright file="EventPublisher.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Execution.Network
{
    using System;
    using System.Text;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Events.Base;
    using Nautilus.Network;

    /// <summary>
    /// Provides an event publisher for the messaging server.
    /// </summary>
    public sealed class EventPublisher : MessagePublisher
    {
        private const string NAUTILUS = "NAUTILUS";
        private const string ACCOUNT = "ACCOUNT";
        private const string EXECUTION = "EXECUTION";
        private readonly ISerializer<Event> serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventPublisher"/> class.
        /// </summary>
        /// <param name="container">The component setup container.</param>
        /// <param name="serializer">The event serializer.</param>
        /// <param name="host">The publishers host address.</param>
        /// <param name="port">The publishers port.</param>
        public EventPublisher(
            IComponentryContainer container,
            ISerializer<Event> serializer,
            NetworkAddress host,
            NetworkPort port)
            : base(
                container,
                host,
                port,
                Guid.NewGuid())
        {
            this.serializer = serializer;

            this.RegisterHandler<Event>(this.OnMessage);
        }

        private void OnMessage(Event message)
        {
            switch (message)
            {
                case OrderEvent @event:
                    this.Publish($"{NAUTILUS}:{EXECUTION}:{@event.OrderId}", message);
                    break;
                case AccountEvent @event:
                    this.Publish($"{NAUTILUS}:{ACCOUNT}:{@event.AccountId}", message);
                    break;
                default:
                    this.Publish(NAUTILUS, message);
                    break;
            }
        }

        private void Publish(string topic, Event message)
        {
            this.Log.Debug($"Publishing[{topic}] {message}");
            this.Publish(Encoding.UTF8.GetBytes(topic), this.serializer.Serialize(message));
        }
    }
}
