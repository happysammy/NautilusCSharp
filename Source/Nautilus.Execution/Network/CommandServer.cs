// -------------------------------------------------------------------------------------------------
// <copyright file="CommandServer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Execution.Network
{
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Message;
    using Nautilus.DomainModel.Commands;
    using Nautilus.Messaging.Interfaces;
    using Nautilus.Network;
    using Nautilus.Network.Encryption;

    /// <summary>
    /// Provides a command server which deserializes command messages and forwards them to the
    /// specified receiver endpoint.
    /// </summary>
    public sealed class CommandServer : MessageServer<Command, Response>
    {
        private readonly IEndpoint commandRouter;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandServer"/> class.
        /// </summary>
        /// <param name="container">The component setup container.</param>
        /// <param name="inboundSerializer">The inbound message serializer.</param>
        /// <param name="outboundSerializer">The outbound message serializer.</param>
        /// <param name="compressor">The message compressor.</param>
        /// <param name="commandRouter">The command router endpoint.</param>
        /// <param name="encryption">The encryption configuration.</param>
        /// <param name="port">The consumers port.</param>
        public CommandServer(
            IComponentryContainer container,
            IMessageSerializer<Command> inboundSerializer,
            IMessageSerializer<Response> outboundSerializer,
            ICompressor compressor,
            IEndpoint commandRouter,
            EncryptionSettings encryption,
            Port port)
            : base(
                container,
                inboundSerializer,
                outboundSerializer,
                compressor,
                encryption,
                Nautilus.Network.NetworkAddress.LocalHost,
                port)
        {
            this.commandRouter = commandRouter;

            this.RegisterHandler<SubmitOrder>(this.OnMessage);
            this.RegisterHandler<SubmitAtomicOrder>(this.OnMessage);
            this.RegisterHandler<CancelOrder>(this.OnMessage);
            this.RegisterHandler<ModifyOrder>(this.OnMessage);
            this.RegisterHandler<AccountInquiry>(this.OnMessage);
        }

        private void OnMessage(SubmitOrder command)
        {
            this.commandRouter.Send(command);
            this.SendReceived(command);
        }

        private void OnMessage(SubmitAtomicOrder command)
        {
            this.commandRouter.Send(command);
            this.SendReceived(command);
        }

        private void OnMessage(CancelOrder command)
        {
            this.commandRouter.Send(command);
            this.SendReceived(command);
        }

        private void OnMessage(ModifyOrder command)
        {
            this.commandRouter.Send(command);
            this.SendReceived(command);
        }

        private void OnMessage(AccountInquiry command)
        {
            this.commandRouter.Send(command);
            this.SendReceived(command);
        }
    }
}
