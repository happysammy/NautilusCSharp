// -------------------------------------------------------------------------------------------------
// <copyright file="CommandServer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
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
    using Nautilus.Messaging.Interfaces;
    using Nautilus.Network;

    /// <summary>
    /// Provides a command server which deserializes command messages and forwards them to the
    /// specified receiver endpoint.
    /// </summary>
    public sealed class CommandServer : MessageServer<Command, Response>
    {
        private readonly IEndpoint commandsSink;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandServer"/> class.
        /// </summary>
        /// <param name="container">The component setup container.</param>
        /// <param name="inboundSerializer">The inbound message serializer.</param>
        /// <param name="outboundSerializer">The outbound message serializer.</param>
        /// <param name="commandsSink">The commands endpoint for deserialized commands.</param>
        /// <param name="port">The consumers port.</param>
        public CommandServer(
            IComponentryContainer container,
            IMessageSerializer<Command> inboundSerializer,
            IMessageSerializer<Response> outboundSerializer,
            IEndpoint commandsSink,
            NetworkPort port)
            : base(
                container,
                inboundSerializer,
                outboundSerializer,
                Nautilus.Network.NetworkAddress.LocalHost,
                port,
                Guid.NewGuid())
        {
            this.commandsSink = commandsSink;

            this.RegisterHandler<SubmitOrder>(this.OnMessage);
            this.RegisterHandler<SubmitAtomicOrder>(this.OnMessage);
            this.RegisterHandler<CancelOrder>(this.OnMessage);
            this.RegisterHandler<ModifyOrder>(this.OnMessage);
            this.RegisterHandler<AccountInquiry>(this.OnMessage);
        }

        private void OnMessage(SubmitOrder command)
        {
            this.commandsSink.Send(command);
            this.SendReceived(command);
        }

        private void OnMessage(SubmitAtomicOrder command)
        {
            this.commandsSink.Send(command);
            this.SendReceived(command);
        }

        private void OnMessage(CancelOrder command)
        {
            this.commandsSink.Send(command);
            this.SendReceived(command);
        }

        private void OnMessage(ModifyOrder command)
        {
            this.commandsSink.Send(command);
            this.SendReceived(command);
        }

        private void OnMessage(AccountInquiry command)
        {
            this.commandsSink.Send(command);
            this.SendReceived(command);
        }
    }
}
