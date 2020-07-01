// -------------------------------------------------------------------------------------------------
// <copyright file="MessageServer.cs" company="Nautech Systems Pty Ltd">
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

namespace Nautilus.Network.Nodes
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using Microsoft.Extensions.Logging;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Logging;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Enums;
    using Nautilus.Core.Message;
    using Nautilus.Core.Types;
    using Nautilus.Messaging;
    using Nautilus.Network.Encryption;
    using Nautilus.Network.Identifiers;
    using Nautilus.Network.Messages;
    using NetMQ.Sockets;

    /// <summary>
    /// The base class for all messaging servers.
    /// </summary>
    public abstract class MessageServer : MessageBusConnected, IDisposable
    {
        private const int ExpectedFrameCount = 3; // Version 1.0

        private readonly ServerId serverId;
        private readonly ISerializer<Dictionary<string, string>> headerSerializer;
        private readonly IMessageSerializer<Request> requestSerializer;
        private readonly IMessageSerializer<Response> responseSerializer;
        private readonly ICompressor compressor;
        private readonly RouterSocket socketInbound;
        private readonly RouterSocket socketOutbound;
        private readonly ZmqNetworkAddress requestAddress;
        private readonly ZmqNetworkAddress responseAddress;
        private readonly Dictionary<ClientId, SessionId> peers;
        private readonly ConcurrentDictionary<Guid, Address> correlationIndex;
        private readonly MessageQueue queue;

        // TODO: Temporary hack
        private IMessageSerializer<Command>? commandSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageServer"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="messagingAdapter">The message bus adapter.</param>
        /// <param name="headerSerializer">The header serializer.</param>
        /// <param name="requestSerializer">The request serializer.</param>
        /// <param name="responseSerializer">The response serializer.</param>
        /// <param name="compressor">The message compressor.</param>
        /// <param name="encryption">The encryption configuration.</param>
        /// <param name="requestAddress">The inbound zmq network address.</param>
        /// <param name="responseAddress">The outbound zmq network address.</param>
        protected MessageServer(
            IComponentryContainer container,
            IMessageBusAdapter messagingAdapter,
            ISerializer<Dictionary<string, string>> headerSerializer,
            IMessageSerializer<Request> requestSerializer,
            IMessageSerializer<Response> responseSerializer,
            ICompressor compressor,
            EncryptionSettings encryption,
            ZmqNetworkAddress requestAddress,
            ZmqNetworkAddress responseAddress)
            : base(container, messagingAdapter)
        {
            this.serverId = new ServerId(this.Name.Value);
            this.headerSerializer = headerSerializer;
            this.requestSerializer = requestSerializer;
            this.responseSerializer = responseSerializer;
            this.compressor = compressor;

            this.socketInbound = new RouterSocket()
            {
                Options =
                {
                    Identity = Encoding.UTF8.GetBytes($"{nameof(Nautilus)}-{this.Name.Value}"),
                    Linger = TimeSpan.FromSeconds(1),
                    TcpKeepalive = true,
                    TcpKeepaliveInterval = TimeSpan.FromSeconds(2),
                    RouterHandover = true,
                },
            };

            this.socketOutbound = new RouterSocket()
            {
                Options =
                {
                    Identity = Encoding.UTF8.GetBytes($"{nameof(Nautilus)}-{this.Name.Value}"),
                    Linger = TimeSpan.FromSeconds(1),
                    TcpKeepalive = true,
                    TcpKeepaliveInterval = TimeSpan.FromSeconds(2),
                    RouterMandatory = true,
                    RouterHandover = true,
                },
            };

            this.requestAddress = requestAddress;
            this.responseAddress = responseAddress;
            this.peers = new Dictionary<ClientId, SessionId>();
            this.correlationIndex = new ConcurrentDictionary<Guid, Address>();

            this.queue = new MessageQueue(
                container,
                this.socketInbound,
                this.socketOutbound,
                this.HandlePayload);

            if (encryption.UseEncryption)
            {
                EncryptionProvider.SetupSocket(encryption, this.socketInbound);
                this.Logger.LogInformation(
                    LogId.Networking,
                    $"{encryption.Algorithm} encryption setup for {this.requestAddress}");

                EncryptionProvider.SetupSocket(encryption, this.socketOutbound);
                this.Logger.LogInformation(
                    LogId.Networking,
                    $"{encryption.Algorithm} encryption setup for {this.responseAddress}");
            }
            else
            {
                this.Logger.LogWarning(
                    LogId.Networking,
                    $"No encryption setup for {this.requestAddress}");

                this.Logger.LogWarning(
                    LogId.Networking,
                    $"No encryption setup for {this.responseAddress}");
            }

            this.ReceivedCount = 0;
            this.SentCount = 0;
        }

        /// <summary>
        /// Gets the count of received messages by the server.
        /// </summary>
        public int ReceivedCount { get; private set; }

        /// <summary>
        /// Gets the count of sent messages by the server.
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
                if (!this.socketInbound.IsDisposed)
                {
                    this.socketInbound.Dispose();
                }

                if (!this.socketOutbound.IsDisposed)
                {
                    this.socketOutbound.Dispose();
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogCritical(LogId.Networking, ex.Message, ex);
            }

            this.Logger.LogInformation(LogId.Component, "Disposed.");
        }

        /// <summary>
        /// Register the given serializer to deserialize inbound messages of type T.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        /// <typeparam name="T">The type of message.</typeparam>
        protected void RegisterSerializer<T>(IMessageSerializer<T> serializer)
            where T : Message
        {
            if (typeof(T).Name == nameof(Command))
            {
                this.commandSerializer = serializer as IMessageSerializer<Command>;
            }
            else
            {
                throw new InvalidOperationException("Only Command serializers supported.");
            }
        }

        /// <inheritdoc />
        protected override void OnStart(Start start)
        {
            try
            {
                this.socketInbound.Bind(this.requestAddress.Value);
                this.socketOutbound.Bind(this.responseAddress.Value);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(LogId.Networking, ex.Message, ex);
            }

            var logMsg1 = $"Bound {this.socketInbound.GetType().Name} to {this.requestAddress}";
            var logMsg2 = $"Bound {this.socketOutbound.GetType().Name} to {this.responseAddress}";
            this.Logger.LogInformation(LogId.Networking, logMsg1);
            this.Logger.LogInformation(LogId.Networking, logMsg2);
        }

        /// <inheritdoc />
        protected override void OnStop(Stop stop)
        {
            foreach (var (peer, session) in this.peers)
            {
                this.Logger.LogError(LogId.Networking, $"Server was still connected to peer {peer.Value} with session {session.Value}.");
            }

            try
            {
                this.queue.GracefulStop().Wait();
                this.socketInbound.Unbind(this.requestAddress.Value);
                this.socketOutbound.Unbind(this.responseAddress.Value);
                this.Logger.LogInformation(LogId.Networking, $"Unbound {this.socketInbound.GetType().Name} from {this.requestAddress}");
                this.Logger.LogInformation(LogId.Networking, $"Unbound {this.socketOutbound.GetType().Name} from {this.responseAddress}");
            }
            catch (Exception ex)
            {
                this.Logger.LogError(LogId.Networking, ex.Message, ex);
            }
        }

        /// <summary>
        /// Sends a MessageRejected message to the given receiver address.
        /// </summary>
        /// <param name="rejectedMessage">The rejected message.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        protected void SendRejected(string rejectedMessage, Guid correlationId)
        {
            var response = new MessageRejected(
                rejectedMessage,
                correlationId,
                Guid.NewGuid(),
                this.TimeNow());

            this.SendMessage(response);
        }

        /// <summary>
        /// Sends a MessageReceived message to the given receiver address.
        /// </summary>
        /// <param name="receivedMessage">The received message.</param>
        protected void SendReceived(Message receivedMessage)
        {
            var response = new MessageReceived(
                receivedMessage.Type.Name,
                receivedMessage.Id,
                Guid.NewGuid(),
                this.TimeNow());

            this.SendMessage(response);
        }

        /// <summary>
        /// Sends a MessageRejected message to the given receiver address.
        /// </summary>
        /// <param name="failureMessage">The query failure message.</param>
        /// <param name="correlationId">The message correlation identifier.</param>
        protected void SendQueryFailure(string failureMessage, Guid correlationId)
        {
            var response = new QueryFailure(
                failureMessage,
                correlationId,
                Guid.NewGuid(),
                this.TimeNow());

            this.SendMessage(response);
        }

        /// <summary>
        /// Sends a message with the given payload to the given receiver address.
        /// </summary>
        /// <param name="response">The outbound message to send.</param>
        protected void SendMessage(Response response)
        {
            if (this.correlationIndex.Remove(response.CorrelationId, out var receiver))
            {
                this.SendMessage(response, receiver);
            }
            else
            {
                this.Logger.LogError(
                    LogId.Networking,
                    $"Cannot send message {response}, no receiver address found for correlation id {response.CorrelationId}).");
            }
        }

        private static Address DecodeHeaderSender(byte[] headerSender)
        {
            try
            {
                return new Address(headerSender);
            }
            catch (ArgumentException)
            {
                return Address.None();
            }
        }

        private void HandlePayload(byte[][] frames)
        {
            this.ReceivedCount++;

            // Check for expected frames
            if (frames.Length != ExpectedFrameCount)
            {
                var errorMsg = $"Message was malformed (expected {ExpectedFrameCount} frames, received {frames.Length}).";
                this.Logger.LogError(LogId.Networking, errorMsg);
                this.Reject(frames, errorMsg);
                return;
            }

            // Deconstruct message
            // -------------------
            // frames[0] sender
            // frames[1] header
            // frames[2] body
            // -------------------
            var frameSender = frames[0];
            var frameHeader = this.compressor.Decompress(frames[1]);
            var frameBody = this.compressor.Decompress(frames[2]);

            // Get sender address
            var sender = DecodeHeaderSender(frameSender);
            if (sender.IsNone)
            {
                var errorMsg = "Message sender header address was malformed.";
                this.Logger.LogError(LogId.Networking, errorMsg);
                this.Reject(frames, errorMsg);
                return;
            }

            var header = this.headerSerializer.Deserialize(frameHeader);

            // Check there is a message body
            if (frameBody.Length == 0)
            {
                var errorMsg = "Message body was empty.";
                this.Logger.LogError(LogId.Networking, errorMsg);
                this.SendRejected(errorMsg, sender);
                return;
            }

            this.HandleMessage(header, frameBody, sender);
        }

        private void Reject(byte[][] frames, string errorMsg)
        {
            if (frames.Length == 0)
            {
                this.Logger.LogError(
                    LogId.Networking,
                    errorMsg.Remove(errorMsg.Length - 1, 1) + "with no reply address.");
                return;
            }

            var receiver = new Address(frames[0]);
            this.SendRejected(errorMsg, receiver);
        }

        private void HandleMessage(Dictionary<string, string> header, byte[] body, Address sender)
        {
            if (!header.TryGetValue(nameof(MessageType), out var messageType))
            {
                var errorMessage = $"Message header did not contain the 'MessageType' key.";
                this.Logger.LogWarning(LogId.Networking, errorMessage);
                this.SendRejected(errorMessage, sender);
                return;
            }

            switch (messageType)
            {
                case nameof(String):
                    this.HandleString(body, sender);
                    break;
                case nameof(Request):
                    this.HandleRequest(body, sender);
                    break;
                case nameof(Command) when this.commandSerializer != null:
                    this.HandleCommand(body, sender, this.commandSerializer);
                    break;
                default:
                {
                    var errorMessage = $"Message type '{messageType}' is not valid at this address {this.requestAddress}.";
                    this.Logger.LogWarning(LogId.Networking, errorMessage);
                    this.SendRejected(errorMessage, sender);
                    break;
                }
            }
        }

        private void HandleString(byte[] payload, Address sender)
        {
            try
            {
                this.SendToSelf(Encoding.UTF8.GetString(payload));
            }
            catch (Exception ex)
            {
                var errorMessage = $"Unable to deserialize message due {ex.GetType().Name}, {ex.Message}";
                this.Logger.LogError(LogId.Networking, errorMessage, ex);
                this.SendRejected(errorMessage, sender);
            }
        }

        private void HandleRequest(byte[] payload, Address sender)
        {
            try
            {
                var message = this.requestSerializer.Deserialize(payload);
                this.correlationIndex[message.Id] = sender;
                switch (message)
                {
                    case Connect connect:
                        this.HandleConnection(connect, sender);
                        return;
                    case Disconnect disconnect:
                        this.HandleDisconnection(disconnect, sender);
                        return;
                    default:
                        this.SendToSelf(message);
                        break;
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"Unable to deserialize message due {ex.GetType().Name}, {ex.Message}";
                this.Logger.LogError(LogId.Networking, errorMessage, ex);
                this.SendRejected(errorMessage, sender);
            }
        }

        private void HandleCommand(byte[] payload, Address sender, IMessageSerializer<Command> serializer)
        {
            try
            {
                var message = serializer.Deserialize(payload);

                this.correlationIndex[message.Id] = sender;
                this.LogReceived(message.ToString());

                this.SendToSelf(message);
            }
            catch (Exception ex)
            {
                var errorMessage = $"Unable to deserialize message due {ex.GetType().Name}, {ex.Message}";
                this.Logger.LogError(LogId.Networking, errorMessage, ex);
                this.SendRejected(errorMessage, sender);
            }
        }

        private void HandleConnection(Connect connect, Address sender)
        {
            var sessionId = this.peers.GetValueOrDefault(connect.ClientId);
            string message;
            if (sessionId is null)
            {
                // Peer not previously connected to a session
                sessionId = new SessionId(connect.Authentication);
                this.peers.TryAdd(connect.ClientId, sessionId);
                message = $"{sender.Value} connected to session {sessionId.Value} at {this.requestAddress}";
                this.Logger.LogInformation(LogId.Networking, message);
            }
            else
            {
                // Peer already connected to a session
                message = $"{sender.Value} already connected to session {sessionId.Value} at {this.requestAddress}";
                this.Logger.LogWarning(LogId.Networking, message);
            }

            var response = new Connected(
                message,
                this.serverId,
                sessionId,
                connect.Id,
                this.NewGuid(),
                this.TimeNow());

            this.SendMessage(response, sender);
        }

        private void HandleDisconnection(Disconnect disconnect, Address sender)
        {
            var sessionId = this.peers.GetValueOrDefault(disconnect.ClientId);
            string message;
            if (sessionId is null)
            {
                // Peer not previously connected to a session
                sessionId = SessionId.None();
                message = $"{sender.Value} had no session to disconnect at {this.requestAddress}";
                this.Logger.LogWarning(LogId.Networking, message);
            }
            else
            {
                // Peer connected to a session
                this.peers.Remove(disconnect.ClientId); // Pop from dictionary
                message = $"{sender.Value} disconnected from session {sessionId.Value} at {this.requestAddress}";
                this.Logger.LogInformation(LogId.Networking, message);
            }

            var response = new Disconnected(
                message,
                this.serverId,
                sessionId,
                disconnect.Id,
                this.NewGuid(),
                this.TimeNow());

            this.SendMessage(response, sender);
        }

        private void SendRejected(string rejectedMessage, Address receiver)
        {
            var response = new MessageRejected(
                rejectedMessage,
                Guid.Empty,
                Guid.NewGuid(),
                this.TimeNow());

            this.SendMessage(response, receiver);
        }

        private void SendMessage(Response response, Address receiver)
        {
            var header = new Dictionary<string, string>
            {
                { nameof(MessageType), response.MessageType.ToString() },
                { nameof(Type), response.Type.Name },
            };

            var frameHeader = this.compressor.Compress(this.headerSerializer.Serialize(header));
            var frameBody = this.compressor.Compress(this.responseSerializer.Serialize(response));

            this.queue.Send(receiver.Utf8Bytes, frameHeader, frameBody);

            this.SentCount++;
            this.LogSent(response.ToString(), receiver.Value);
        }

        [Conditional("DEBUG")]
        private void LogReceived(string message)
        {
            this.Logger.LogTrace(LogId.Networking, $"<--[{this.ReceivedCount.ToString()}] Received {message}");
        }

        [Conditional("DEBUG")]
        private void LogSent(string outbound, string receiver)
        {
            this.Logger.LogTrace(LogId.Networking, $"[{this.SentCount.ToString()}]--> {outbound} to {receiver}.");
        }
    }
}
