// -------------------------------------------------------------------------------------------------
// <copyright file="CommandServer.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using Nautilus.Common.Interfaces;
using Nautilus.Common.Messaging;
using Nautilus.Core.Message;
using Nautilus.Core.Types;
using Nautilus.DomainModel.Aggregates;
using Nautilus.DomainModel.Commands;
using Nautilus.Execution.Configuration;
using Nautilus.Execution.Engine;
using Nautilus.Network;
using Nautilus.Network.Encryption;
using Nautilus.Network.Nodes;
using NodaTime;

namespace Nautilus.Execution.Network
{
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
        /// <param name="serviceName">The service name.</param>
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
            Label serviceName,
            NetworkConfiguration config)
            : base(
                container,
                messagingAdapter,
                headerSerializer,
                requestSerializer,
                responseSerializer,
                compressor,
                encryption,
                serviceName,
                ZmqNetworkAddress.AllInterfaces(config.CommandReqPort),
                ZmqNetworkAddress.AllInterfaces(config.CommandResPort))
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
            this.RegisterHandler<SubmitBracketOrder>(this.OnMessage);
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

        private void OnMessage(SubmitBracketOrder command)
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
            this.Send(command, ComponentAddress.ExecutionEngine);
        }
    }
}
