// -------------------------------------------------------------------------------------------------
// <copyright file="CommandServer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Execution.Network
{
    using System;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Message;
    using Nautilus.DomainModel.Commands;
    using Nautilus.Messaging;
    using Nautilus.Messaging.Interfaces;
    using Nautilus.Network;

    /// <summary>
    /// Provides a command server which deserializes command messages and forwards them to the
    /// specified receiver endpoint.
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
        /// <param name="receiver">The receiver endpoint for deserialized commands.</param>
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

            this.RegisterHandler<Envelope<SubmitOrder>>(this.OnMessage);
            this.RegisterHandler<Envelope<SubmitAtomicOrder>>(this.OnMessage);
            this.RegisterHandler<Envelope<CancelOrder>>(this.OnMessage);
            this.RegisterHandler<Envelope<ModifyOrder>>(this.OnMessage);
            this.RegisterHandler<Envelope<AccountInquiry>>(this.OnMessage);
        }

        private void OnMessage(Envelope<SubmitOrder> envelope)
        {
            this.receiver.Send(envelope.Message);
            this.SendReceived(envelope.Message, envelope.Sender);
        }

        private void OnMessage(Envelope<SubmitAtomicOrder> envelope)
        {
            this.receiver.Send(envelope.Message);
            this.SendReceived(envelope.Message, envelope.Sender);
        }

        private void OnMessage(Envelope<CancelOrder> envelope)
        {
            this.receiver.Send(envelope.Message);
            this.SendReceived(envelope.Message, envelope.Sender);
        }

        private void OnMessage(Envelope<ModifyOrder> envelope)
        {
            this.receiver.Send(envelope.Message);
            this.SendReceived(envelope.Message, envelope.Sender);
        }

        private void OnMessage(Envelope<AccountInquiry> envelope)
        {
            this.receiver.Send(envelope.Message);
            this.SendReceived(envelope.Message, envelope.Sender);
        }
    }
}
