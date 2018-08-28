// -------------------------------------------------------------------------------------------------
// <copyright file="EventPublisher.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Execution
{
    using System;
    using Akka.Actor;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.Core;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Factories;
    using Nautilus.Messaging;

    /// <summary>
    /// Provides an event publisher for the messaging server.
    /// </summary>
    public class EventPublisher : ActorComponentBase
    {
        private readonly IEventSerializer serializer;
        private readonly IEndpoint publisher;

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
            string host,
            int port)
            : base(
                NautilusService.Messaging,
                LabelFactory.Component(nameof(CommandConsumer)),
                container)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(container, nameof(container));

            this.serializer = serializer;

            this.publisher = new ActorEndpoint(
                Context.ActorOf(Props.Create(
                    () => new Publisher(
                        container,
                        LabelFactory.Component("EventPublisher"),
                        "nautilus_execution_events",
                        host,
                        port,
                        Guid.NewGuid()))));

            this.Receive<Event>(msg => this.OnMessage(msg));
        }

        private void OnMessage(Event message)
        {
            Debug.NotNull(message, nameof(message));

            this.publisher.Send(this.serializer.Serialize(message));
        }
    }
}
