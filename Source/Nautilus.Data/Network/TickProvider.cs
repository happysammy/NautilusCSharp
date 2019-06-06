//--------------------------------------------------------------------------------------------------
// <copyright file="TickProvider.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Network
{
    using System;
    using System.Linq;
    using System.Text;
    using Nautilus.Common.Interfaces;
    using Nautilus.Data.Interfaces;
    using Nautilus.Data.Messages.Requests;
    using Nautilus.Data.Messages.Responses;
    using Nautilus.DomainModel.Entities;
    using Nautilus.Network;
    using Nautilus.Network.Messages;

    /// <summary>
    /// Provides a responder for <see cref="Instrument"/> data requests.
    /// </summary>
    public sealed class TickProvider : MessageServer<Request, Response>
    {
        private readonly ITickRepository repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="TickProvider"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="repository">The tick repository.</param>
        /// <param name="inboundSerializer">The inbound message serializer.</param>
        /// <param name="outboundSerializer">The outbound message serializer.</param>
        /// <param name="host">The host address.</param>
        /// <param name="port">The port.</param>
        public TickProvider(
            IComponentryContainer container,
            ITickRepository repository,
            IMessageSerializer<Request> inboundSerializer,
            IMessageSerializer<Response> outboundSerializer,
            NetworkAddress host,
            NetworkPort port)
            : base(
                container,
                inboundSerializer,
                outboundSerializer,
                host,
                port,
                Guid.NewGuid())
        {
            this.repository = repository;

            this.RegisterHandler<ReceivedMessage<TickDataRequest>>(this.OnMessage);
        }

        private void OnMessage(ReceivedMessage<TickDataRequest> message)
        {
            var request = message.Payload;

            var query = this.repository.Find(
                request.Symbol,
                request.FromDateTime,
                request.ToDateTime);

            if (query.IsFailure)
            {
                this.SendRejected(message.SenderId, request.Id, query.Message);
                this.Log.Error(query.Message);
            }

            var ticks = query
                .Value
                .Select(t => Encoding.UTF8.GetBytes(t.ToString()))
                .ToArray();

            var response = new TickDataResponse(
                request.Symbol,
                ticks,
                request.Id,
                this.NewGuid(),
                this.TimeNow());

            this.SendMessage(message.SenderId, response);
        }
    }
}
