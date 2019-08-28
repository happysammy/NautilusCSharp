// -------------------------------------------------------------------------------------------------
// <copyright file="MessageServer{TInbound,TOutbound}.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Network
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Message;
    using Nautilus.Core.Types;
    using Nautilus.Messaging;
    using Nautilus.Messaging.Interfaces;
    using Nautilus.Network.Messages;
    using NetMQ;
    using NetMQ.Sockets;

    /// <summary>
    /// The base class for all messaging servers.
    /// </summary>
    /// <typeparam name="TInbound">The inbound message type.</typeparam>
    /// <typeparam name="TOutbound">The outbound response type.</typeparam>
    [SuppressMessage("ReSharper", "SA1310", Justification = "Easier to read.")]
    public abstract class MessageServer<TInbound, TOutbound> : Component
        where TInbound : Message
        where TOutbound : Response
    {
        private const int EXPECTED_FRAMES_COUNT = 3;

        private readonly byte[] delimiter = { };
        private readonly CancellationTokenSource cts;
        private readonly RouterSocket socket;
        private readonly IMessageSerializer<TInbound> inboundSerializer;
        private readonly IMessageSerializer<TOutbound> outboundSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageServer{TInbound, TOutbound}"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="inboundSerializer">The inbound message serializer.</param>
        /// <param name="outboundSerializer">The outbound message serializer.</param>
        /// <param name="host">The consumer host address.</param>
        /// <param name="port">The consumer port.</param>
        /// <param name="id">The consumer identifier.</param>
        protected MessageServer(
            IComponentryContainer container,
            IMessageSerializer<TInbound> inboundSerializer,
            IMessageSerializer<TOutbound> outboundSerializer,
            NetworkAddress host,
            NetworkPort port,
            Guid id)
            : base(container)
        {
            Condition.NotDefault(id, nameof(id));

            this.cts = new CancellationTokenSource();
            this.socket = new RouterSocket()
            {
                Options =
                {
                    Linger = TimeSpan.Zero,
                    Identity = Encoding.Unicode.GetBytes(id.ToString()),
                },
            };

            this.inboundSerializer = inboundSerializer;
            this.outboundSerializer = outboundSerializer;

            this.ServerAddress = new ZmqServerAddress(host, port);
            this.ReceivedCount = 0;
            this.SentCount = 0;

            this.RegisterHandler<Envelope<Start>>(this.OnMessage);
            this.RegisterHandler<Envelope<Stop>>(this.OnMessage);
            this.RegisterUnhandled(this.UnhandledRequest);
        }

        /// <summary>
        /// Gets the server address for the router.
        /// </summary>
        public ZmqServerAddress ServerAddress { get; }

        /// <summary>
        /// Gets the server received message count.
        /// </summary>
        public int ReceivedCount { get; private set; }

        /// <summary>
        /// Gets the server processed message count.
        /// </summary>
        public int SentCount { get; private set; }

        /// <inheritdoc />
        protected override void OnStart(Start start)
        {
            this.socket.Bind(this.ServerAddress.Value);
            this.Log.Debug($"Bound {this.socket.GetType().Name} to {this.ServerAddress}");

            Task.Run(this.StartWork, this.cts.Token);
        }

        /// <inheritdoc />
        protected override void OnStop(Stop stop)
        {
            this.cts.Cancel();
            this.socket.Unbind(this.ServerAddress.Value);
            this.Log.Debug($"Unbound {this.socket.GetType().Name} from {this.ServerAddress}");

            this.socket.Dispose();
        }

        /// <summary>
        /// Sends a message with the given payload to the given receiver address.
        /// </summary>
        /// <param name="outbound">The outbound message to send.</param>
        /// <param name="receiver">The receiver address.</param>
        protected void SendMessage(TOutbound outbound, Address? receiver)
        {
            Debug.NotNull(receiver, nameof(receiver));
            Debug.True(receiver.HasValue, nameof(receiver));

            this.Execute(() =>
            {
                if (receiver is null || !receiver.Value.HasBytesValue)
                {
                    this.Log.Error($"Cannot send message {outbound} (no receiver address).");
                    return;
                }

                this.socket.SendMultipartBytes(
                    receiver.Value.BytesValue,
                    this.delimiter,
                    this.outboundSerializer.Serialize(outbound));

                this.SentCount++;
                this.Log.Debug($"[{this.SentCount}]--> {outbound} to Address({receiver}).");
            });
        }

        /// <summary>
        /// Sends a MessageReceived message to the given receiver address.
        /// </summary>
        /// <param name="receivedMessage">The received message.</param>
        /// <param name="receiver">The receiver address.</param>
        protected void SendReceived(Message receivedMessage, Address? receiver)
        {
            this.Execute(() =>
            {
                var received = new MessageReceived(
                    receivedMessage.Type.Name,
                    receivedMessage.Id,
                    Guid.NewGuid(),
                    this.TimeNow()) as TOutbound;

                if (received is null)
                {
                    // This should never happen due to generic type constraints
                    throw new InvalidOperationException(
                        $"Design time error (message was not of type {typeof(TOutbound)}).");
                }

                this.SendMessage(received, receiver);
            });
        }

        /// <summary>
        /// Sends a MessageRejected message to the given receiver address.
        /// </summary>
        /// <param name="failureMessage">The query failure message.</param>
        /// <param name="correlationId">The message correlation identifier.</param>
        /// <param name="receiver">The receiver address.</param>
        protected void SendQueryFailure(
            string failureMessage,
            Guid correlationId,
            Address? receiver)
        {
            this.Execute(() =>
            {
                var failure = new QueryFailure(
                    failureMessage,
                    correlationId,
                    Guid.NewGuid(),
                    this.TimeNow()) as TOutbound;

                if (failure is null)
                {
                    // This should never happen due to generic type constraints
                    throw new InvalidOperationException(
                        $"Design time error (message was not of type {typeof(TOutbound)}).");
                }

                this.SendMessage(failure, receiver);
            });
        }

        /// <summary>
        /// Sends a MessageRejected message to the given receiver address.
        /// </summary>
        /// <param name="rejectedMessage">The rejected message.</param>
        /// <param name="correlationId">The message correlation identifier.</param>
        /// <param name="receiver">The receiver address.</param>
        private void SendRejected(
            string rejectedMessage,
            Guid correlationId,
            Address? receiver)
        {
            this.Execute(() =>
            {
                var rejected = new MessageRejected(
                    rejectedMessage,
                    correlationId,
                    Guid.NewGuid(),
                    this.TimeNow()) as TOutbound;

                if (rejected is null)
                {
                    // This should never happen due to generic type constraints
                    throw new InvalidOperationException(
                        $"Design time error (message was not of type {typeof(TOutbound)}).");
                }

                this.SendMessage(rejected, receiver);
            });
        }

        private Task StartWork()
        {
            while (!this.cts.IsCancellationRequested)
            {
                this.ReceiveMessage();
            }

            this.Log.Debug("Stopped receiving messages.");
            return Task.CompletedTask;
        }

        private void ReceiveMessage()
        {
            this.Execute(() =>
            {
                var msg = this.socket.ReceiveMultipartBytes(EXPECTED_FRAMES_COUNT);  // msg[1] should be empty byte array delimiter

                if (msg.Count != EXPECTED_FRAMES_COUNT)
                {
                    var error = $"Message was malformed (expected {EXPECTED_FRAMES_COUNT} frames, received {msg.Count}).";
                    if (msg.Count >= 1)
                    {
                        this.SendRejected(error, Guid.Empty, new Address(msg[0]));
                    }

                    this.Log.Error(error);
                }
                else
                {
                    this.DeserializeMessage(new Address(msg[0]), msg[2]);
                }
            });
        }

        private void DeserializeMessage(Address sender, byte[] payload)
        {
            try
            {
                var received = this.inboundSerializer.Deserialize(payload);
                var envelope = EnvelopeFactory.Create(
                    received,
                    null,
                    sender,
                    this.TimeNow());

                this.SendToSelf(envelope);

                this.ReceivedCount++;
                this.Log.Debug($"[{this.ReceivedCount}]<-- {received} from Address({sender}).");
            }
            catch (SerializationException ex)
            {
                var message = "Unable to deserialize message.";
                this.Log.Error(message + Environment.NewLine + ex);
                this.SendRejected(message, Guid.Empty, sender);
            }
        }

        private void OnMessage(Envelope<Start> message)
        {
            this.Open(message);
        }

        private void OnMessage(Envelope<Stop> message)
        {
            this.Open(message);
        }

        /// <summary>
        /// Handle the given unhandled request message.
        /// </summary>
        /// <param name="request">The unhandled request object.</param>
        private void UnhandledRequest(object request)
        {
            if (request is IEnvelope envelope)
            {
                var errorMessage = $"Message type {envelope.MessageType.Name} not valid at this address {this.ServerAddress}.";
                this.SendRejected(errorMessage, envelope.MessageBase.Id, envelope.Sender);

                this.Log.Error(errorMessage);
            }

            this.AddToUnhandledMessages(request);
        }
    }
}
