// -------------------------------------------------------------------------------------------------
// <copyright file="EventServer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Execution
{
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Execution.Network;
    using Nautilus.Messaging.Interfaces;

    /// <summary>
    /// Provides a messaging server using the ZeroMQ protocol.
    /// </summary>
    [PerformanceOptimized]
    public class EventServer : ComponentBusConnectedBase
    {
        private readonly IEndpoint eventPublisher;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventServer"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="eventSerializer">The event serializer.</param>
        /// <param name="config">The service configuration.</param>
        public EventServer(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            IEventSerializer eventSerializer,
            Configuration config)
            : base(container, messagingAdapter)
        {
            this.eventPublisher = new EventPublisher(
                container,
                eventSerializer,
                config.ServerAddress,
                config.EventsPort).Endpoint;

            this.RegisterHandler<Event>(this.OnMessage);
        }

        private void OnMessage(Event @event)
        {
            this.eventPublisher.Send(@event);
            this.Log.Debug($"Published event {@event}.");
        }
    }
}
