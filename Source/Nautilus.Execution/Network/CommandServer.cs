// -------------------------------------------------------------------------------------------------
// <copyright file="CommandServer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Execution.Network
{
    using System;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core;
    using Nautilus.Messaging;
    using Nautilus.Messaging.Interfaces;
    using Nautilus.Network;
    using Nautilus.Network.Messages;

    /// <summary>
    /// Provides a command server for received and deserializing commands.
    /// </summary>
    public sealed class CommandServer : MessageServer<Command, Response>
    {
        private readonly IEndpoint receiver;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandServer"/> class.
        /// </summary>
        /// <param name="container">The component setup container.</param>
        /// <param name="inboundSerializer">The inbound message serializer.</param>
        /// <param name="outboundSerializer">The outbound message serializer.</param>
        /// <param name="receiver">The receiver endpoint.</param>
        /// <param name="host">The consumers host address.</param>
        /// <param name="port">The consumers port.</param>
        public CommandServer(
            IComponentryContainer container,
            IMessageSerializer<Command> inboundSerializer,
            IMessageSerializer<Response> outboundSerializer,
            IEndpoint receiver,
            NetworkAddress host,
            NetworkPort port)
            : base(
                container,
                inboundSerializer,
                outboundSerializer,
                host,
                port,
                Guid.NewGuid())
        {
            this.receiver = receiver;

            this.RegisterHandler<Envelope<Command>>(this.OnMessage);
        }

        private void OnMessage(Envelope<Command> envelope)
        {
            var command = envelope.Message;
            this.receiver.Send(command);

            this.SendReceived(command, envelope.Receiver);
        }
    }
}
