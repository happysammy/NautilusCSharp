// -------------------------------------------------------------------------------------------------
// <copyright file="MessageServer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Network.Nodes
{
    using System;
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
        private readonly ZmqNetworkAddress networkAddress;
        private readonly Dictionary<ClientId, SessionId> peers;
        private readonly Dictionary<Guid, Address> correlationIndex;
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

            this.networkAddress = networkAddress;
            this.peers = new Dictionary<ClientId, SessionId>();
            this.correlationIndex = new Dictionary<Guid, Address>();

            if (encryption.UseEncryption)
            {
                EncryptionProvider.SetupSocket(encryption, this.socket);
                this.Logger.LogInformation(
                    LogId.Networking,
                    $"{encryption.Algorithm} encryption setup for {this.networkAddress}");
            }
            else
            {
                this.Logger.LogWarning(
                    LogId.Networking,
                    $"No encryption setup for {this.networkAddress}");
            }

            this.CountReceived = 0;
            this.CountSent = 0;

            this.RegisterHandler<IEnvelope>(this.OnEnvelope);
        }

        /// <summary>
        /// Gets the count of received messages by the server.
        /// </summary>
        public int CountReceived { get; private set; }

        /// <summary>
        /// Gets the count of sent messages by the server.
        /// </summary>
        public int CountSent { get; private set; }

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
                this.socket.Bind(this.networkAddress.Value);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(LogId.Networking, ex.Message, ex);
            }

            var logMsg = $"Bound {this.socket.GetType().Name} to {this.networkAddress}";
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
                this.socket.Unbind(this.networkAddress.Value);
                this.Logger.LogInformation(LogId.Networking, $"Unbound {this.socket.GetType().Name} from {this.networkAddress}");
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

            this.SendMessage(outbound, receiver);
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

        private static string DecodeHeaderType(byte[] headerType)
        {
            try
            {
                return Encoding.UTF8.GetString(headerType);
            }
            catch (ArgumentException)
            {
                return string.Empty;
            }
        }

        private static long DecodeHeaderSize(byte[] headerSize)
        {
            try
            {
                return BitConverter.ToInt64(headerSize);
            }
            catch (ArgumentOutOfRangeException)
            {
                return -1;
            }
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
                frames = this.socket.ReceiveMultipartBytes(); // Blocking
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
            // -------------------
            // msg[0] reply address
            // msg[1] header: payload message type
            // msg[2] header: payload uncompressed size
            // msg[3] payload
            // -------------------
            var headerSender = frames[0];
            var headerType = frames[1];
            var headerSize = frames[2];
            var payload = frames[3];

            // Get sender address
            var sender = DecodeHeaderSender(headerSender);
            if (sender.IsNone)
            {
                var errorMsg = "Message sender header address was malformed.";
                this.Reject(frames, errorMsg);
                return;
            }

            // Get message type
            var type = DecodeHeaderType(headerType);
            if (type == string.Empty)
            {
                var errorMsg = "Message type header was malformed.";
                this.SendRejected(errorMsg, sender);
                return;
            }

            // Get expected uncompressed size
            var size = DecodeHeaderSize(headerSize);
            if (size == -1)
            {
                var errorMsg = "Message size header was malformed.";
                this.SendRejected(errorMsg, sender);
                return;
            }

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

            this.LogSizes(size, payload.Length);

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

            var receiver = new Address(frames[0]);
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
                    var errorMessage = $"Message type '{type}' is not valid at this address {this.networkAddress}.";
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
                message = $"{sender.Value} connected to session {sessionId.Value} at {this.networkAddress}";
                this.Logger.LogInformation(LogId.Networking, message);
            }
            else
            {
                // Peer already connected to a session
                message = $"{sender.Value} already connected to session {sessionId.Value} at {this.networkAddress}";
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
                message = $"{sender.Value} had no session to disconnect at {this.networkAddress}";
                this.Logger.LogWarning(LogId.Networking, message);
            }
            else
            {
                // Peer connected to a session
                this.peers.Remove(disconnect.ClientId, out var _); // Pop from dictionary
                message = $"{sender.Value} disconnected from session {sessionId.Value} at {this.networkAddress}";
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
                    receiver.Utf8Bytes,
                    type,
                    size,
                    payload);

                this.CountSent++;
                this.LogSent(outbound.ToString(), receiver.Value);
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
