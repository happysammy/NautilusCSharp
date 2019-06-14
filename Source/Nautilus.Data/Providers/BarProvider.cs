//--------------------------------------------------------------------------------------------------
// <copyright file="BarProvider.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Providers
{
    using System;
    using System.Linq;
    using System.Text;
    using Nautilus.Common.Interfaces;
    using Nautilus.Data.Interfaces;
    using Nautilus.Data.Messages.Requests;
    using Nautilus.Data.Messages.Responses;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging;
    using Nautilus.Network;
    using Nautilus.Network.Messages;

    /// <summary>
    /// Provides a responder for <see cref="Instrument"/> data requests.
    /// </summary>
    public sealed class BarProvider : MessageServer<Request, Response>
    {
        private readonly IBarRepository repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="BarProvider"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="repository">The instrument repository.</param>
        /// <param name="inboundSerializer">The inbound message serializer.</param>
        /// <param name="outboundSerializer">The outbound message serializer.</param>
        /// <param name="host">The host address.</param>
        /// <param name="port">The port.</param>
        public BarProvider(
            IComponentryContainer container,
            IBarRepository repository,
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

            this.RegisterHandler<Envelope<BarDataRequest>>(this.OnMessage);
        }

        private void OnMessage(Envelope<BarDataRequest> envelope)
        {
            var request = envelope.Message;
            var barType = new BarType(request.Symbol, request.BarSpecification);
            var query = this.repository.Find(
                barType,
                request.FromDateTime,
                request.ToDateTime);

            if (query.IsFailure)
            {
                this.SendQueryFailure(query.Message, request.Id, envelope.Sender);
                this.Log.Warning($"{envelope.Message} query failed ({query.Message}).");
                return;
            }

            var bars = query
                .Value
                .Bars
                .Select(b => Encoding.UTF8.GetBytes(b.ToString()))
                .ToArray();

            var response = new BarDataResponse(
                request.Symbol,
                request.BarSpecification,
                bars,
                request.Id,
                this.NewGuid(),
                this.TimeNow());

            this.SendMessage(response, envelope.Sender);
        }
    }
}
