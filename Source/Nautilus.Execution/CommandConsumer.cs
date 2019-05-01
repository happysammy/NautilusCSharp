// -------------------------------------------------------------------------------------------------
// <copyright file="CommandConsumer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
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
    using Nautilus.DomainModel.Factories;
    using Nautilus.Messaging;
    using Nautilus.Messaging.Network;

    /// <summary>
    /// Provides a command consumer for the messaging server.
    /// </summary>
    public class CommandConsumer : ActorComponentBase
    {
        private readonly ICommandSerializer serializer;
        private readonly IEndpoint consumer;
        private readonly IEndpoint receiver;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandConsumer"/> class.
        /// </summary>
        /// <param name="container">The component setup container.</param>
        /// <param name="serializer">The command serializer.</param>
        /// <param name="receiver">The receiver endpoint.</param>
        /// <param name="host">The consumers host address.</param>
        /// <param name="port">The consumers port.</param>
        public CommandConsumer(
            IComponentryContainer container,
            ICommandSerializer serializer,
            IEndpoint receiver,
            NetworkAddress host,
            Port port)
            : base(
                NautilusService.Messaging,
                LabelFactory.Create(nameof(CommandConsumer)),
                container)
        {
            Precondition.NotNull(container, nameof(container));
            Precondition.NotNull(serializer, nameof(serializer));
            Precondition.NotNull(receiver, nameof(receiver));
            Precondition.NotNull(host, nameof(host));
            Precondition.NotNull(port, nameof(host));

            this.serializer = serializer;
            this.receiver = receiver;

            this.consumer = new ActorEndpoint(
                Context.ActorOf(Props.Create(
                    () => new Consumer(
                        container,
                        new ActorEndpoint(Context.Self),
                        LabelFactory.Create("RouterSocket"),
                        host,
                        port,
                        Guid.NewGuid()))));

            this.Receive<byte[]>(this.OnMessage);
        }

        private void OnMessage(byte[] message)
        {
            Debug.NotNull(message, nameof(message));

            this.Execute(() =>
            {
                var command = this.serializer.Deserialize(message);
                this.Log.Debug($"Received {command}.");

                this.receiver.Send(command);
            });
        }
    }
}
