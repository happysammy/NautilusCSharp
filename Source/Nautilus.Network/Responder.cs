// -------------------------------------------------------------------------------------------------
// <copyright file="Responder.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.Common.Messages.Responses;
    using Nautilus.Core;
    using Nautilus.Core.Correctness;
    using NetMQ;
    using NetMQ.Sockets;

    /// <summary>
    /// Provides a messaging router.
    /// </summary>
    public abstract class Responder : Component
    {
        private static readonly byte[] Delimiter = { };

        private readonly CancellationTokenSource cts;
        private readonly ResponseSocket socket;
        private readonly IRequestSerializer requestSerializer;
        private readonly IResponseSerializer responseSerializer;

        private bool isResponding;
        private int cycles;

        /// <summary>
        /// Initializes a new instance of the <see cref="Responder"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="requestSerializer">The request serializer.</param>
        /// <param name="responseSerializer">The response serializer.</param>
        /// <param name="host">The consumer host address.</param>
        /// <param name="port">The consumer port.</param>
        /// <param name="id">The consumer identifier.</param>
        protected Responder(
            IComponentryContainer container,
            IRequestSerializer requestSerializer,
            IResponseSerializer responseSerializer,
            NetworkAddress host,
            NetworkPort port,
            Guid id)
            : base(container)
        {
            Condition.NotDefault(id, nameof(id));

            this.cts = new CancellationTokenSource();
            this.socket = new ResponseSocket()
            {
                Options =
                {
                    Linger = TimeSpan.Zero,
                    Identity = Encoding.Unicode.GetBytes(id.ToString()),
                },
            };
            this.requestSerializer = requestSerializer;
            this.responseSerializer = responseSerializer;

            this.ServerAddress = new ZmqServerAddress(host, port);

            this.RegisterUnhandled(this.UnhandledRequest);
        }

        /// <summary>
        /// Gets the server address for the router.
        /// </summary>
        public ZmqServerAddress ServerAddress { get; }

        /// <inheritdoc />
        protected override void OnStart(Start start)
        {
            this.socket.Bind(this.ServerAddress.Value);
            this.Log.Debug($"Bound router socket to {this.ServerAddress}");

            this.isResponding = true;
            Task.Run(this.StartResponding, this.cts.Token);
        }

        /// <inheritdoc />
        protected override void OnStop(Stop stop)
        {
            this.isResponding = false;
            this.cts.Cancel();
            this.socket.Unbind(this.ServerAddress.Value);
            this.Log.Debug($"Unbound router socket from {this.ServerAddress}");

            this.socket.Dispose();
        }

        /// <summary>
        /// Respond to the last request with the given response.
        /// </summary>
        /// <param name="requesterId">The requester identifier to respond to.</param>
        /// <param name="response">The response bytes.</param>
        protected void SendResponse(string requesterId, Response response)
        {
            Debug.NotEmptyOrWhiteSpace(requesterId, nameof(requesterId));

            var requesterIdBytes = Encoding.UTF8.GetBytes(requesterId);
            var responseBytes = this.responseSerializer.Serialize(response);
            this.socket.SendMultipartBytes(requesterIdBytes, Delimiter, responseBytes);

            this.cycles++;
            this.Log.Verbose($"Responded to message[{this.cycles}] on {this.ServerAddress.Value}.");
        }

        /// <summary>
        /// Respond to the last request with a <see cref="BadRequest"/> containing the given message.
        /// </summary>
        /// <param name="request">The bad request.</param>
        /// <param name="message">The bad request message.</param>
        protected void SendBadRequest(Request request, string message)
        {
            var badRequest = new BadRequest(
                message,
                request.Id,
                Guid.NewGuid(),
                this.TimeNow());

            this.SendResponse(request.RequesterId, badRequest);
        }

        /// <summary>
        /// Handle the given unhandled request message.
        /// </summary>
        /// <param name="message">The unhandled object.</param>
        private void UnhandledRequest(object message)
        {
            if (message is Request request)
            {
                var errorMessage = $"request type {request.Type} not valid on this port";
                this.SendBadRequest(request, errorMessage);
                this.Log.Error(errorMessage);
            }

            this.AddToUnhandledMessages(message);
        }

        private Task StartResponding()
        {
            while (this.isResponding)
            {
                this.ListenForRequests();
            }

            this.Log.Debug("Stopped responding to messages.");
            return Task.CompletedTask;
        }

        private void ListenForRequests()
        {
            try
            {
                var requestBytes = this.socket.ReceiveMultipartBytes(); // [1] is empty bytes delimiter
                var request = this.requestSerializer.Deserialize(requestBytes[2]);
                request.SetRequesterId(Encoding.UTF8.GetString(requestBytes[0]));

                this.SendToSelf(request);
            }
            catch (Exception ex)
            {
                this.Log.Error(ex.Message);
            }
        }
    }
}
