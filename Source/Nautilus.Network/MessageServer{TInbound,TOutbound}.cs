// -------------------------------------------------------------------------------------------------
// <copyright file="MessageServer{TInbound,TOutbound}.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Network
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
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.Messaging;
    using Nautilus.Messaging.Interfaces;
    using Nautilus.Network.Encryption;
    using Nautilus.Network.Messages;
    using NetMQ;
    using NetMQ.Sockets;

    /// <summary>
    /// The base class for all messaging servers.
    /// </summary>
    /// <typeparam name="TInbound">The inbound message type.</typeparam>
    /// <typeparam name="TOutbound">The outbound response type.</typeparam>
    public abstract class MessageServer<TInbound, TOutbound> : MessagingComponent, IDisposable
        where TInbound : Message
        where TOutbound : Response
    {
        private const int ExpectedFrameCount = 4;

        private readonly IMessageSerializer<TInbound> inboundSerializer;
        private readonly IMessageSerializer<TOutbound> outboundSerializer;
        private readonly ICompressor compressor;
        private readonly CancellationTokenSource cts;
        private readonly RouterSocket socket;
        private readonly Dictionary<TraderId, SessionId> peers;
        private readonly Dictionary<Guid, Address> correlationIndex;
        private readonly byte[] delimiter = { };

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageServer{TInbound, TOutbound}"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="inboundSerializer">The inbound message serializer.</param>
        /// <param name="outboundSerializer">The outbound message serializer.</param>
        /// <param name="compressor">The message compressor.</param>
        /// <param name="encryption">The encryption configuration.</param>
        /// <param name="host">The consumer host address.</param>
        /// <param name="port">The consumer port.</param>
        protected MessageServer(
            IComponentryContainer container,
            IMessageSerializer<TInbound> inboundSerializer,
            IMessageSerializer<TOutbound> outboundSerializer,
            ICompressor compressor,
            EncryptionSettings encryption,
            NetworkAddress host,
            Port port)
            : base(container)
        {
            this.inboundSerializer = inboundSerializer;
            this.outboundSerializer = outboundSerializer;
            this.compressor = compressor;

            this.cts = new CancellationTokenSource();
            this.socket = new RouterSocket()
            {
                Options =
                {
                    Identity = Encoding.Unicode.GetBytes($"{nameof(Nautilus)}-{this.Name.Value}"),
                    Linger = TimeSpan.FromSeconds(1),
                },
            };

            this.peers = new Dictionary<TraderId, SessionId>();
            this.correlationIndex = new Dictionary<Guid, Address>();

            this.NetworkAddress = new ZmqNetworkAddress(host, port);

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

            if (!this.socket.IsDisposed)
            {
                this.socket.Dispose();
            }

            this.Logger.LogDebug("Disposed.");
        }

        /// <inheritdoc />
        protected override void OnStart(Start start)
        {
            this.socket.Bind(this.NetworkAddress.Value);
            this.Logger.LogInformation(
                LogId.Networking,
                $"Bound {this.socket.GetType().Name} to {this.NetworkAddress}");

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

            this.socket.Unbind(this.NetworkAddress.Value);
            this.Logger.LogInformation(
                LogId.Networking,
                $"Unbound {this.socket.GetType().Name} from {this.NetworkAddress}");
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
                this.TimeNow()) as TOutbound;

            // Exists to avoid warning CS8604 (this should never happen anyway due to generic type constraints)
            if (received is null)
            {
                throw new InvalidOperationException($"The message was not of type {typeof(TOutbound)}.");
            }

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
                this.TimeNow()) as TOutbound;

            // Exists to avoid warning CS8604 (this should never happen anyway due to generic type constraints)
            if (failure is null)
            {
                throw new InvalidOperationException($"The message was not of type {typeof(TOutbound)}.");
            }

            this.SendMessage(failure, correlationId);
        }

        /// <summary>
        /// Sends a message with the given payload to the given receiver address.
        /// </summary>
        /// <param name="outbound">The outbound message to send.</param>
        /// <param name="correlationId">The correlation identifier for the receiver address.</param>
        protected void SendMessage(TOutbound outbound, Guid correlationId)
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
            var frames = this.socket.ReceiveMultipartBytes();

            this.CountReceived++;
            this.LogFrames(frames);

            // Check for expected frames
            if (frames.Count != ExpectedFrameCount)
            {
                var errorMsg = $"Message was malformed (expected {ExpectedFrameCount} frames, received {frames.Count}).";
                this.Reject(frames, errorMsg);
                return;
            }

            // Deconstruct message
            // msg[0] reply address
            // msg[1] should be empty byte array delimiter
            // msg[2] length header
            // msg[3] payload
            var headerSender = frames[0];
            var headerLength = frames[2];
            var payload = frames[3];

            // Check there is a message payload
            if (payload.Length == 0)
            {
                var errorMsg = "Message payload was empty.";
                this.Reject(frames, errorMsg);
                return;
            }

            // Get expected decompressed length
            uint length;
            try
            {
                length = BitConverter.ToUInt32(headerLength);
            }
            catch (ArgumentOutOfRangeException)
            {
                var errorMsg = "Message length checksum was malformed.";
                this.Reject(frames, errorMsg);
                return;
            }

            this.LogSizes(length, (uint)payload.Length);

            // Decompress message
            var decompressed = this.compressor.Decompress(payload);
            if (decompressed.Length != length)
            {
                var errorMsg = $"Message decompressed length checksum was not equal to {length}, " +
                               $"was {decompressed.Length}.";
                this.Reject(frames, errorMsg);
            }

            // Deserialize message
            var sender = new Address(headerSender, Encoding.ASCII.GetString);
            this.DeserializeMessage(decompressed, length, sender);
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

            this.SendRejected(errorMsg, new Address(frames[0], Encoding.ASCII.GetString));
        }

        private void DeserializeMessage(byte[] payload, uint length, Address sender)
        {
            try
            {
                // length reserved for LZ4 implementation
                var received = this.inboundSerializer.Deserialize(payload);
                this.correlationIndex[received.Id] = sender;

                if (!received.Type.IsSubclassOf(typeof(TInbound)) && received.Type != typeof(TInbound))
                {
                    var errorMessage = $"Message type {received.Type} not valid at this address {this.NetworkAddress}.";
                    this.SendRejected(errorMessage, sender);
                    return;
                }

                this.LogReceived(received);

                switch (received)
                {
                    case Connect connect:
                        this.HandleConnection(connect, sender);
                        return;
                    case Disconnect disconnect:
                        this.HandleDisconnection(disconnect, sender);
                        return;
                }

                this.SendToSelf(received);
            }
            catch (Exception ex)
            {
                var errorMessage = $"Unable to deserialize message due {ex.GetType().Name}, {ex.Message}";
                this.SendRejected(errorMessage, sender);
            }
        }

        private void HandleConnection(Connect connect, Address sender)
        {
            var sessionId = this.peers.GetValueOrDefault(connect.TraderId);
            string message;
            if (sessionId is null)
            {
                // Peer not previously connected to a session
                sessionId = new SessionId(connect.TraderId, this.TimeNow());
                this.peers[connect.TraderId] = sessionId;
                message = $"{connect.TraderId.Value} connected to session {sessionId.Value}.";
                this.Logger.LogInformation(LogId.Networking, message);
            }
            else
            {
                // Peer already connected to a session
                message = $"{connect.TraderId.Value} was already connected to session {sessionId.Value}.";
                this.Logger.LogWarning(LogId.Networking, message);
            }

            var connected = new Connected(
                this.Name.Value,
                message,
                sessionId,
                connect.Id,
                this.NewGuid(),
                this.TimeNow()) as TOutbound;

            // Exists to avoid warning CS8604 (this should never happen anyway due to generic type constraints)
            if (connected is null)
            {
                throw new InvalidOperationException($"The message was not of type {typeof(TOutbound)}.");
            }

            this.SendMessage(connected, sender);
        }

        private void HandleDisconnection(Disconnect disconnect, Address sender)
        {
            var sessionId = this.peers.GetValueOrDefault(disconnect.TraderId);
            string message;
            if (sessionId is null)
            {
                // Peer not previously connected to a session
                message = $"{disconnect.TraderId.Value} had no session.";
                sessionId = SessionId.None();
                this.Logger.LogWarning(LogId.Networking, message);
            }
            else
            {
                // Peer connected to a session
                this.peers.Remove(disconnect.TraderId);
                message = $"{disconnect.TraderId.Value} disconnected from session {sessionId.Value}.";
                this.Logger.LogInformation(LogId.Networking, message);
            }

            var disconnected = new Disconnected(
                this.Name.Value,
                message,
                sessionId,
                disconnect.Id,
                this.NewGuid(),
                this.TimeNow()) as TOutbound;

            // Exists to avoid warning CS8604 (this should never happen anyway due to generic type constraints)
            if (disconnected is null)
            {
                throw new InvalidOperationException($"The message was not of type {typeof(TOutbound)}.");
            }

            this.SendMessage(disconnected, sender);
        }

        private void SendRejected(string rejectedMessage, Address receiver)
        {
            var rejected = new MessageRejected(
                rejectedMessage,
                Guid.Empty,
                Guid.NewGuid(),
                this.TimeNow()) as TOutbound;

            // Exists to avoid warning CS8604 (this should never happen anyway due to generic type constraints)
            if (rejected is null)
            {
                throw new InvalidOperationException($"The message was not of type {typeof(TOutbound)}.");
            }

            this.SendMessage(rejected, receiver);
            this.Logger.LogWarning(LogId.Networking, rejectedMessage);
        }

        private void SendMessage(TOutbound outbound, Address receiver)
        {
            var serialized = this.outboundSerializer.Serialize(outbound);
            var length = BitConverter.GetBytes((uint)serialized.Length);
            var payload = this.compressor.Compress(serialized);

            this.socket.SendMultipartBytes(
                receiver.BytesValue,
                this.delimiter,
                length,
                payload);

            this.CountSent++;
            this.LogSent(outbound, receiver);
        }

        [Conditional("DEBUG")]
        private void LogFrames(List<byte[]> frames)
        {
            this.Logger.LogTrace(LogId.Networking, $"<--[{this.CountReceived}] Received {frames.Count} byte[] frames.");
        }

        [Conditional("DEBUG")]
        private void LogReceived(TInbound message)
        {
            this.Logger.LogTrace(LogId.Networking, $"<--[{this.CountReceived}] Received {message}");
        }

        [Conditional("DEBUG")]
        private void LogSizes(uint headerLength, uint compressedLength)
        {
            this.Logger.LogTrace(LogId.Networking, $"HeaderLength={headerLength:N0} bytes, Compressed={compressedLength:N0} bytes");
        }

        [Conditional("DEBUG")]
        private void LogSent(TOutbound outbound, Address receiver)
        {
            this.Logger.LogTrace(LogId.Networking, $"[{this.CountSent}]--> {outbound} to Address({receiver}).");
        }
    }
}
