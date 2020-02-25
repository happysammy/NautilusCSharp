// -------------------------------------------------------------------------------------------------
// <copyright file="MessageServer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Network
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Logging;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Core.Message;
    using Nautilus.Core.Types;
    using Nautilus.Messaging;
    using Nautilus.Messaging.Interfaces;
    using Nautilus.Network.Encryption;
    using Nautilus.Network.Identifiers;
    using Nautilus.Network.Messages;
    using NetMQ;
    using NetMQ.Sockets;

    /// <summary>
    /// The base class for all messaging servers.
    /// </summary>
    public abstract class MessageServer : MessagingComponent, IDisposable
    {
        private const int ExpectedFrameCount = 4; // Version 1.0

        private readonly ServerId serverId;
        private readonly IMessageSerializer<Request> requestSerializer;
        private readonly IMessageSerializer<Response> responseSerializer;
        private readonly ICompressor compressor;
        private readonly RouterSocket socket;
        private readonly ConcurrentDictionary<ClientId, SessionId> peers;
        private readonly ConcurrentDictionary<Guid, Address> correlationIndex;
        private readonly CancellationTokenSource cts;

        // TODO: Temporary hack
        private IMessageSerializer<Command>? commandSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageServer"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="requestSerializer">The request serializer.</param>
        /// <param name="responseSerializer">The response serializer.</param>
        /// <param name="compressor">The message compressor.</param>
        /// <param name="encryption">The encryption configuration.</param>
        /// <param name="networkAddress">The zmq network address.</param>
        protected MessageServer(
            IComponentryContainer container,
            IMessageSerializer<Request> requestSerializer,
            IMessageSerializer<Response> responseSerializer,
            ICompressor compressor,
            EncryptionSettings encryption,
            ZmqNetworkAddress networkAddress)
            : base(container)
        {
            this.serverId = new ServerId(this.Name.Value);
            this.requestSerializer = requestSerializer;
            this.responseSerializer = responseSerializer;
            this.compressor = compressor;

            this.cts = new CancellationTokenSource();
            this.socket = new RouterSocket()
            {
                Options =
                {
                    Identity = Encoding.UTF8.GetBytes($"{nameof(Nautilus)}-{this.Name.Value}"),
                    Linger = TimeSpan.FromSeconds(1),
                    RouterMandatory = true,
                },
            };

            this.peers = new ConcurrentDictionary<ClientId, SessionId>();
            this.correlationIndex = new ConcurrentDictionary<Guid, Address>();

            this.NetworkAddress = networkAddress;

            if (encryption.UseEncryption)
            {
                EncryptionProvider.SetupSocket(encryption, this.socket);
                this.Logger.LogInformation(
                    LogId.Networking,
                    $"{encryption.Algorithm} encryption setup for {this.NetworkAddress}");
            }
            else
            {
                this.Logger.LogWarning(
                    LogId.Networking,
                    $"No encryption setup for {this.NetworkAddress}");
            }

            this.CountReceived = 0;
            this.CountSent = 0;

            this.RegisterHandler<IEnvelope>(this.OnEnvelope);
        }

        /// <summary>
        /// Gets the network address for the router.
        /// </summary>
        public ZmqNetworkAddress NetworkAddress { get; }

        /// <summary>
        /// Gets the server received message count.
        /// </summary>
        public int CountReceived { get; private set; }

        /// <summary>
        /// Gets the server processed message count.
        /// </summary>
        public int CountSent { get; private set; }

        /// <summary>
        /// Dispose of the socket.
        /// </summary>
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
                this.Logger.LogCritical(LogId.Networking, ex.Message, ex);
            }

            this.Logger.LogInformation(LogId.Operation, "Disposed.");
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
                this.socket.Bind(this.NetworkAddress.Value);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(LogId.Networking, ex.Message, ex);
            }

            var logMsg = $"Bound {this.socket.GetType().Name} to {this.NetworkAddress}";
            this.Logger.LogInformation(LogId.Networking, logMsg);

            Task.Run(this.StartWork, this.cts.Token);
        }

        /// <inheritdoc />
        protected override void OnStop(Stop stop)
        {
            this.cts.Cancel();

            foreach (var session in this.peers.Values)
            {
                this.Logger.LogError(LogId.Networking, $"Server was still connected to session {session.Value}.");
            }

            try
            {
                this.socket.Unbind(this.NetworkAddress.Value);
                this.Logger.LogInformation(LogId.Networking, $"Unbound {this.socket.GetType().Name} from {this.NetworkAddress}");
            }
            catch (Exception ex)
            {
                this.Logger.LogError(LogId.Networking, ex.Message, ex);
            }
        }

        /// <summary>
        /// Sends a MessageReceived message to the given receiver address.
        /// </summary>
        /// <param name="receivedMessage">The received message.</param>
        protected void SendReceived(Message receivedMessage)
        {
            var received = new MessageReceived(
                receivedMessage.Type.Name,
                receivedMessage.Id,
                Guid.NewGuid(),
                this.TimeNow());

            this.SendMessage(received, receivedMessage.Id);
        }

        /// <summary>
        /// Sends a MessageRejected message to the given receiver address.
        /// </summary>
        /// <param name="failureMessage">The query failure message.</param>
        /// <param name="correlationId">The message correlation identifier.</param>
        protected void SendQueryFailure(string failureMessage, Guid correlationId)
        {
            var failure = new QueryFailure(
                failureMessage,
                correlationId,
                Guid.NewGuid(),
                this.TimeNow());

            this.SendMessage(failure, correlationId);
        }

        /// <summary>
        /// Sends a message with the given payload to the given receiver address.
        /// </summary>
        /// <param name="outbound">The outbound message to send.</param>
        /// <param name="correlationId">The correlation identifier for the receiver address.</param>
        protected void SendMessage(Response outbound, Guid correlationId)
        {
            if (this.correlationIndex.Remove(correlationId, out var receiver))
            {
            }
            else
            {
                this.Logger.LogError(
                    LogId.Networking,
                    $"Cannot send message {outbound} no receiver address found for {correlationId}).");
                return;
            }

            if (!receiver.HasBytesValue)
            {
                this.Logger.LogError(
                    LogId.Networking,
                    $"Cannot send message {outbound} (no receiver address found for {correlationId}).");
                return;
            }

            this.SendMessage(outbound, receiver);
        }

        private Task StartWork()
        {
            while (!this.cts.IsCancellationRequested)
            {
                this.ReceiveMessage();
            }

            this.Logger.LogDebug(LogId.Networking, "Stopped receiving messages.");
            return Task.CompletedTask;
        }

        private void ReceiveMessage()
        {
            List<byte[]> frames;
            try
            {
                frames = this.socket.ReceiveMultipartBytes();
            }
            catch (Exception ex)
            {
                // Interaction with NetMQ
                this.Logger.LogError(LogId.Networking, ex.Message, ex);
                return;
            }
            finally
            {
                this.CountReceived++;
            }

            this.LogFrames(frames.Count);

            // Check for expected frames
            if (frames.Count != ExpectedFrameCount)
            {
                var errorMsg = $"Message was malformed (expected {ExpectedFrameCount} frames, received {frames.Count}).";
                this.Reject(frames, errorMsg);
                return;
            }

            // Deconstruct message
            // msg[0] reply address
            // msg[1] header: payload message type
            // msg[2] header: payload uncompressed size
            // msg[3] payload
            var headerSender = frames[0];
            var headerType = frames[1];
            var headerSize = frames[2];
            var payload = frames[3];

            // Get sender address
            Address sender;
            try
            {
                sender = new Address(headerSender, Encoding.UTF8.GetString);
            }
            catch (ArgumentException)
            {
                var errorMsg = "Message sender address was malformed.";
                this.Reject(frames, errorMsg);
                return;
            }

            // Get message type
            string type;
            try
            {
                type = Encoding.UTF8.GetString(headerType);
            }
            catch (ArgumentException)
            {
                var errorMsg = "Message type header was malformed.";
                this.SendRejected(errorMsg, sender);
                return;
            }

            // Get expected uncompressed size
            long size;
            try
            {
                size = BitConverter.ToInt64(headerSize);
            }
            catch (ArgumentOutOfRangeException)
            {
                var errorMsg = "Message size header was malformed.";
                this.SendRejected(errorMsg, sender);
                return;
            }

            this.LogSizes(size, payload.Length);

            // Check there is a message payload
            if (payload.Length == 0)
            {
                var errorMsg = "Message payload was empty.";
                this.SendRejected(errorMsg, sender);
                return;
            }

            // Decompress message
            var decompressed = this.compressor.Decompress(payload);
            if (decompressed.Length != size)
            {
                var errorMsg = $"Message decompressed size {decompressed.Length} != header size {size}.";
                this.SendRejected(errorMsg, sender);
            }

            this.HandleMessage(type, decompressed, sender);
        }

        private void Reject(List<byte[]> frames, string errorMsg)
        {
            if (frames.Count == 0)
            {
                this.Logger.LogError(
                    LogId.Networking,
                    errorMsg.Remove(errorMsg.Length - 1, 1) + "with no reply address.");
                return;
            }

            var receiver = new Address(frames[0], Encoding.UTF8.GetString);
            this.SendRejected(errorMsg, receiver);
        }

        private void HandleMessage(string type, byte[] payload, Address sender)
        {
            switch (type)
            {
                case nameof(Request):
                    this.HandleRequest(payload, sender);
                    break;
                case nameof(Command) when this.commandSerializer != null:
                    this.HandleCommand(payload, sender, this.commandSerializer);
                    break;
                default:
                {
                    var errorMessage = $"Message type '{type}' is not valid at this address {this.NetworkAddress}.";
                    this.SendRejected(errorMessage, sender);
                    break;
                }
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
                sessionId = new SessionId(connect.ClientId, this.TimeNow());
                this.peers.TryAdd(connect.ClientId, sessionId);
                message = $"{sender.StringValue} connected to session {sessionId.Value} at {this.NetworkAddress}";
                this.Logger.LogInformation(LogId.Networking, message);
            }
            else
            {
                // Peer already connected to a session
                message = $"{sender.StringValue} already connected to session {sessionId.Value} at {this.NetworkAddress}";
                this.Logger.LogWarning(LogId.Networking, message);
            }

            var connected = new Connected(
                message,
                this.serverId,
                sessionId,
                connect.Id,
                this.NewGuid(),
                this.TimeNow());

            this.SendMessage(connected, sender);
        }

        private void HandleDisconnection(Disconnect disconnect, Address sender)
        {
            var sessionId = this.peers.GetValueOrDefault(disconnect.ClientId);
            string message;
            if (sessionId is null)
            {
                // Peer not previously connected to a session
                message = $"{sender.StringValue} has no session to disconnect at {this.NetworkAddress}.";
                sessionId = SessionId.None();
                this.Logger.LogWarning(LogId.Networking, message);
            }
            else
            {
                // Peer connected to a session
                this.peers.TryRemove(disconnect.ClientId, out var _); // Pop from dictionary
                message = $"{sender.StringValue} disconnected from session {sessionId.Value} at {this.NetworkAddress}";
                this.Logger.LogInformation(LogId.Networking, message);
            }

            var disconnected = new Disconnected(
                message,
                this.serverId,
                sessionId,
                disconnect.Id,
                this.NewGuid(),
                this.TimeNow());

            this.SendMessage(disconnected, sender);
        }

        private void SendRejected(string rejectedMessage, Address receiver)
        {
            var rejected = new MessageRejected(
                rejectedMessage,
                Guid.Empty,
                Guid.NewGuid(),
                this.TimeNow());

            this.SendMessage(rejected, receiver);
            this.Logger.LogWarning(LogId.Networking, rejectedMessage);
        }

        private void SendMessage(Response outbound, Address receiver)
        {
            try
            {
                var serialized = this.responseSerializer.Serialize(outbound);
                var type = Encoding.UTF8.GetBytes(outbound.Type.Name);
                var size = BitConverter.GetBytes((long)serialized.Length);
                var payload = this.compressor.Compress(serialized);

                this.socket.SendMultipartBytes(
                    receiver.BytesValue,
                    type,
                    size,
                    payload);

                this.CountSent++;
                this.LogSent(outbound.ToString(), receiver.StringValue);
            }
            catch (Exception ex)
            {
                // Interaction with NetMQ
                this.Logger.LogError(LogId.Networking, ex.Message, ex);
            }
        }

        [Conditional("DEBUG")]
        private void LogFrames(int framesCount)
        {
            this.Logger.LogTrace(LogId.Networking, $"<--[{this.CountReceived}] Received {framesCount} byte[] frames.");
        }

        [Conditional("DEBUG")]
        private void LogReceived(string message)
        {
            this.Logger.LogTrace(LogId.Networking, $"<--[{this.CountReceived}] Received {message}");
        }

        [Conditional("DEBUG")]
        private void LogSizes(long headerLength, long compressedLength)
        {
            this.Logger.LogTrace(LogId.Networking, $"HeaderLength={headerLength:N0} bytes, Compressed={compressedLength:N0} bytes");
        }

        [Conditional("DEBUG")]
        private void LogSent(string outbound, string receiver)
        {
            this.Logger.LogTrace(LogId.Networking, $"[{this.CountSent}]--> {outbound} to {receiver}.");
        }
    }
}
