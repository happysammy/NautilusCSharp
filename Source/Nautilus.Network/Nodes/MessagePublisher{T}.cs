// -------------------------------------------------------------------------------------------------
// <copyright file="MessagePublisher{T}.cs" company="Nautech Systems Pty Ltd">
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

using System;
using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Logging;
using Nautilus.Common.Componentry;
using Nautilus.Common.Enums;
using Nautilus.Common.Interfaces;
using Nautilus.Common.Logging;
using Nautilus.Common.Messages.Commands;
using Nautilus.Core.Types;
using Nautilus.Messaging.Interfaces;
using Nautilus.Network.Encryption;
using NetMQ;
using NetMQ.Sockets;

namespace Nautilus.Network.Nodes
{
    /// <summary>
    /// The base class for all message publishers.
    /// </summary>
    /// <typeparam name="T">The publishing message type.</typeparam>
    public abstract class MessagePublisher<T> : MessagingComponent, IDisposable
    {
        private readonly ISerializer<T> serializer;
        private readonly ICompressor compressor;
        private readonly PublisherSocket socket;
        private readonly ZmqNetworkAddress networkAddress;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagePublisher{T}"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="serializer">The message serializer.</param>
        /// <param name="compressor">The message compressor.</param>
        /// <param name="encryption">The encryption configuration.</param>
        /// <param name="serviceName">The service name.</param>
        /// <param name="networkAddress">The publishers network address.</param>
        protected MessagePublisher(
            IComponentryContainer container,
            ISerializer<T> serializer,
            ICompressor compressor,
            EncryptionSettings encryption,
            Label serviceName,
            ZmqNetworkAddress networkAddress)
            : base(container)
        {
            this.serializer = serializer;
            this.compressor = compressor;

            this.socket = new PublisherSocket
            {
                Options =
                {
                    Identity = Encoding.UTF8.GetBytes($"{serviceName.Value}-{this.Name.Value}"),
                    Linger = TimeSpan.FromSeconds(1),
                },
            };

            this.networkAddress = networkAddress;

            if (encryption.UseEncryption)
            {
                EncryptionProvider.SetupSocket(encryption, this.socket);
                this.Logger.LogInformation(LogId.Network, $"{encryption.Algorithm} encryption setup for {this.networkAddress}");
            }
            else
            {
                this.Logger.LogWarning(LogId.Network, $"No encryption setup for {this.networkAddress}");
            }

            this.SentCount = 0;

            this.RegisterHandler<IEnvelope>(this.OnEnvelope);
        }

        /// <summary>
        /// Gets the server published message count.
        /// </summary>
        public int SentCount { get; private set; }

        /// <inheritdoc />
        public void Dispose()
        {
            if (this.ComponentState == ComponentState.Running)
            {
                throw new InvalidOperationException("Cannot dispose a running component.");
            }

            try
            {
                if (!this.socket.IsDisposed)
                {
                    this.socket.Dispose();
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogCritical(LogId.Network, ex.Message, ex);
            }

            this.Logger.LogInformation(LogId.Component, "Disposed.");
        }

        /// <inheritdoc />
        protected override void OnStart(Start start)
        {
            try
            {
                this.socket.Bind(this.networkAddress.Value);
                this.Logger.LogInformation(LogId.Network, $"Bound {this.socket.GetType().Name} to {this.networkAddress}");
            }
            catch (Exception ex)
            {
                this.Logger.LogError(LogId.Network, ex.Message, ex);
            }
        }

        /// <inheritdoc />
        protected override void OnStop(Stop stop)
        {
            try
            {
                this.socket.Unbind(this.networkAddress.Value);
                this.Logger.LogInformation(LogId.Network, $"Unbound {this.socket.GetType().Name} from {this.networkAddress}");
            }
            catch (Exception ex)
            {
                this.Logger.LogError(LogId.Network, ex.Message, ex);
            }
        }

        /// <summary>
        /// Sends the given message onto the socket.
        /// </summary>
        /// <param name="topic">The messages topic.</param>
        /// <param name="message">The message to publish.</param>
        protected void Publish(string topic, T message)
        {
            var body = this.compressor.Compress(this.serializer.Serialize(message));

            this.socket.SendMultipartBytes(Encoding.UTF8.GetBytes(topic), body);

            this.SentCount++;
            this.LogPublished(topic, body.Length);
        }

        [Conditional("DEBUG")]
        private void LogPublished(string topic, int compressedLength)
        {
            var logMessage = $"[{this.SentCount}]--> Topic={topic}, Body={compressedLength} bytes";

            this.Logger.LogTrace(LogId.Network, logMessage);
        }
    }
}
