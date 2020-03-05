// -------------------------------------------------------------------------------------------------
// <copyright file="CommandServer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Execution.Network
{
    using System.Collections.Generic;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Message;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Commands;
    using Nautilus.Execution.Configuration;
    using Nautilus.Execution.Engine;
    using Nautilus.Network;
    using Nautilus.Network.Encryption;
    using Nautilus.Network.Nodes;
    using NodaTime;

    /// <summary>
    /// Provides a command server which receives command messages from the wire, throttles them and
    /// then forwards them to the <see cref="ExecutionEngine"/> as appropriate.
    /// </summary>
    public sealed class CommandServer : MessageServer
    {
        private readonly Throttler<Command> commandThrottler;
        private readonly Throttler<Command> orderThrottler;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandServer"/> class.
        /// </summary>
        /// <param name="container">The component setup container.</param>
        /// <param name="messagingAdapter">The message bus adapter.</param>
        /// <param name="headerSerializer">The header serializer.</param>
        /// <param name="requestSerializer">The request serializer.</param>
        /// <param name="responseSerializer">The response serializer.</param>
        /// <param name="commandSerializer">The command serializer.</param>
        /// <param name="compressor">The message compressor.</param>
        /// <param name="encryption">The encryption configuration.</param>
        /// <param name="config">The network configuration.</param>
        public CommandServer(
            IComponentryContainer container,
            IMessageBusAdapter messagingAdapter,
            ISerializer<Dictionary<string, string>> headerSerializer,
            IMessageSerializer<Request> requestSerializer,
            IMessageSerializer<Response> responseSerializer,
            IMessageSerializer<Command> commandSerializer,
            ICompressor compressor,
            EncryptionSettings encryption,
            NetworkConfiguration config)
            : base(
                container,
                messagingAdapter,
                headerSerializer,
                requestSerializer,
                responseSerializer,
                compressor,
                encryption,
                ZmqNetworkAddress.LocalHost(config.CommandReqPort),
                ZmqNetworkAddress.LocalHost(config.CommandResPort))
        {
            this.commandThrottler = new Throttler<Command>(
                container,
                this.SendToExecutionEngine,
                Duration.FromSeconds(1),
                config.CommandsPerSecond,
                nameof(Command));

            this.orderThrottler = new Throttler<Command>(
                container,
                this.commandThrottler.Endpoint.Send,
                Duration.FromSeconds(1),
                config.NewOrdersPerSecond,
                nameof(Order));

            this.RegisterSerializer(commandSerializer);
            this.RegisterHandler<SubmitOrder>(this.OnMessage);
            this.RegisterHandler<SubmitAtomicOrder>(this.OnMessage);
            this.RegisterHandler<CancelOrder>(this.OnMessage);
            this.RegisterHandler<ModifyOrder>(this.OnMessage);
            this.RegisterHandler<AccountInquiry>(this.OnMessage);

            this.commandThrottler.Start();
            this.orderThrottler.Start();
        }

        private void OnMessage(SubmitOrder command)
        {
            this.orderThrottler.Endpoint.Send(command);
            this.SendReceived(command);
        }

        private void OnMessage(SubmitAtomicOrder command)
        {
            this.orderThrottler.Endpoint.Send(command);
            this.SendReceived(command);
        }

        private void OnMessage(CancelOrder command)
        {
            this.commandThrottler.Endpoint.Send(command);
            this.SendReceived(command);
        }

        private void OnMessage(ModifyOrder command)
        {
            this.commandThrottler.Endpoint.Send(command);
            this.SendReceived(command);
        }

        private void OnMessage(AccountInquiry command)
        {
            this.commandThrottler.Endpoint.Send(command);
            this.SendReceived(command);
        }

        private void SendToExecutionEngine(Command command)
        {
            this.Send(command, ServiceAddress.ExecutionEngine);
        }
    }
}
