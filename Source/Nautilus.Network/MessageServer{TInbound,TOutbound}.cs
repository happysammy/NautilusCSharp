// -------------------------------------------------------------------------------------------------
// <copyright file="MessageServer{TInbound,TOutbound}.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Network
{
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Core;
    using Nautilus.Core.Correctness;
    using Nautilus.Messaging;
    using Nautilus.Network.Messages;
    using NetMQ;
    using NetMQ.Sockets;

    /// <summary>
    /// The base class for all messaging servers.
    /// </summary>
    /// <typeparam name="TInbound">The inbound message type.</typeparam>
    /// <typeparam name="TOutbound">The outbound response type.</typeparam>
    public abstract class MessageServer<TInbound, TOutbound> : Component
        where TInbound : Message
        where TOutbound : Response
    {
        private readonly byte[] delimiter = { };
        private readonly IMessageSerializer<TInbound> inboundSerializer;
        private readonly IMessageSerializer<TOutbound> outboundSerializer;
        private readonly CancellationTokenSource cts;
        private readonly RouterSocket socket;

        private bool isReceiving;

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

            this.inboundSerializer = inboundSerializer;
            this.outboundSerializer = outboundSerializer;
            this.cts = new CancellationTokenSource();
            this.socket = new RouterSocket()
            {
                Options =
                {
                    Linger = TimeSpan.Zero,
                    Identity = Encoding.Unicode.GetBytes(id.ToString()),
                },
            };

            this.ServerAddress = new ZmqServerAddress(host, port);
            this.ReceivedCount = 0;
            this.SentCount = 0;

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

            this.isReceiving = true;
            Task.Run(this.StartWork, this.cts.Token);
        }

        /// <inheritdoc />
        protected override void OnStop(Stop stop)
        {
            this.isReceiving = false;
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
            try
            {
                Debug.NotNull(receiver, nameof(receiver));
                Debug.True(receiver.HasValue, nameof(receiver));

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
                this.Log.Debug($"Sent message[{this.SentCount}] {outbound} to Address({receiver}).");
            }
            catch (Exception ex)
            {
                this.Log.Error(ex.Message);
            }
        }

        /// <summary>
        /// Sends a MessageReceived message to the given receiver address.
        /// </summary>
        /// <param name="receivedMessage">The received message.</param>
        /// <param name="receiver">The receiver address.</param>
        protected void SendReceived(Message receivedMessage, Address? receiver)
        {
            var received = new MessageReceived(
                receivedMessage.Type.Name,
                receivedMessage.Id,
                Guid.NewGuid(),
                this.TimeNow()) as TOutbound;

            if (received is null)
            {
                return; // This can never happen due to generic type constraints.
            }

            this.SendMessage(received, receiver);
        }

        /// <summary>
        /// Sends a MessageRejected message to the given receiver address.
        /// </summary>
        /// <param name="rejectedReason">The rejected reason.</param>
        /// <param name="rejectedMessage">The rejected message.</param>
        /// <param name="receiver">The receiver address.</param>
        protected void SendRejected(
            string rejectedReason,
            Message rejectedMessage,
            Address? receiver)
        {
            var rejected = new MessageRejected(
                rejectedReason,
                rejectedMessage.Id,
                Guid.NewGuid(),
                this.TimeNow()) as TOutbound;

            if (rejected is null)
            {
                return; // This can never happen due to generic type constraints.
            }

            this.SendMessage(rejected, receiver);
        }

        private Task StartWork()
        {
            while (this.isReceiving)
            {
                this.ReceiveMessage();
            }

            this.Log.Debug("Stopped receiving messages.");
            return Task.CompletedTask;
        }

        private void ReceiveMessage()
        {
            var zmqMessage = this.socket.ReceiveMultipartBytes(3);
            var sender = new Address(zmqMessage[0]);
            var received = this.inboundSerializer.Deserialize(zmqMessage[2]);

            var envelope = new Envelope<TInbound>(
                received,
                null,
                sender,
                this.TimeNow());

            this.SendToSelf(envelope);

            this.ReceivedCount++;
            this.Log.Debug($"Received message[{this.ReceivedCount}] {received} from Address({sender}).");
        }

        /// <summary>
        /// Handle the given unhandled request message.
        /// </summary>
        /// <param name="message">The unhandled object.</param>
        private void UnhandledRequest(object message)
        {
            if (message is Envelope<TInbound> request)
            {
                var errorMessage = $"Message type {request.Message.Type.Name} not valid at this address {this.ServerAddress}.";
                this.SendRejected(errorMessage, request.Message, request.Sender);

                this.Log.Error(errorMessage);
            }

            this.AddToUnhandledMessages(message);
        }

        private void Open(Envelope<Command> envelope)
        {
            this.SendToSelf(envelope.Message);
        }

        private void Open(Envelope<Event> envelope)
        {
            this.SendToSelf(envelope.Message);
        }

        private void Open(Envelope<Document> envelope)
        {
            this.SendToSelf(envelope.Message);
        }
    }
}
