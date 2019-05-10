// -------------------------------------------------------------------------------------------------
// <copyright file="EventPublisher.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Execution
{
    using System;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core;
    using Nautilus.Network;

    /// <summary>
    /// Provides an event publisher for the messaging server.
    /// </summary>
    public class EventPublisher : Publisher
    {
        private readonly IEventSerializer serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventPublisher"/> class.
        /// </summary>
        /// <param name="container">The component setup container.</param>
        /// <param name="serializer">The event serializer.</param>
        /// <param name="host">The publishers host address.</param>
        /// <param name="port">The publishers port.</param>
        public EventPublisher(
            IComponentryContainer container,
            IEventSerializer serializer,
            NetworkAddress host,
            Port port)
            : base(
                container,
                "nautilus_execution_events",
                host,
                port,
                Guid.NewGuid())
        {
            this.serializer = serializer;

            this.RegisterHandler<Event>(this.OnMessage);
        }

        private void OnMessage(Event message)
        {
            this.Publish(this.serializer.Serialize(message));
        }
    }
}
