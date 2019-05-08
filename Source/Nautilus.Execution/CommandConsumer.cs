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
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Factories;
    using Nautilus.Messaging;
    using Nautilus.Network;

    /// <summary>
    /// Provides a command consumer for the messaging server.
    /// </summary>
    public class CommandConsumer : ComponentBase
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
            this.serializer = serializer;
            this.receiver = receiver;

            this.consumer = new Consumer(
                        container,
                        this.Endpoint,
                        LabelFactory.Create("RouterSocket"),
                        host,
                        port,
                        Guid.NewGuid()).Endpoint;

            this.RegisterHandler<byte[]>(this.OnMessage);
        }

        private void OnMessage(byte[] message)
        {
            this.Execute(() =>
            {
                var command = this.serializer.Deserialize(message);
                this.Log.Debug($"Received {command}.");

                this.receiver.Send(command);
            });
        }
    }
}
