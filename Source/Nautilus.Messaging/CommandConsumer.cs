// -------------------------------------------------------------------------------------------------
// <copyright file="CommandConsumer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Messaging
{
    using System;
    using Akka.Actor;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Factories;

    /// <summary>
    /// Provides a command consumer for the messaging server.
    /// </summary>
    public class CommandConsumer : ActorComponentBase
    {
        private readonly ICommandSerializer serializer;
        private readonly IEndpoint consumer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandConsumer"/> class.
        /// </summary>
        /// <param name="container">The component setup container.</param>
        /// <param name="serializer">The command serializer.</param>
        /// <param name="host">The consumers host address.</param>
        /// <param name="port">The consumers port.</param>
        public CommandConsumer(
            IComponentryContainer container,
            ICommandSerializer serializer,
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

            this.consumer = new ActorEndpoint(
                Context.ActorOf(Props.Create(
                    () => new Consumer(
                        container,
                        new ActorEndpoint(Context.Self),
                        LabelFactory.Component("CommandConsumer"),
                        host,
                        port,
                        Guid.NewGuid()))));

            this.Receive<byte[]>(msg => this.OnMessage(msg));
        }

        private void OnMessage(byte[] message)
        {
            Debug.NotNull(message, nameof(message));

            Context.Parent.Tell(this.serializer.Deserialize(message));
        }
    }
}
