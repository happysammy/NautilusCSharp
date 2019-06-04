//--------------------------------------------------------------------------------------------------
// <copyright file="InstrumentResponder.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Network
{
    using System;
    using System.Linq;
    using Nautilus.Common.Interfaces;
    using Nautilus.Data.Interfaces;
    using Nautilus.Data.Messages.Requests;
    using Nautilus.Data.Messages.Responses;
    using Nautilus.DomainModel.Entities;
    using Nautilus.Network;

    /// <summary>
    /// Provides a responder for <see cref="Instrument"/> data requests.
    /// </summary>
    public sealed class InstrumentResponder : Responder
    {
        private readonly IInstrumentRepository repository;
        private readonly IInstrumentSerializer instrumentSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstrumentResponder"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="repository">The instrument repository.</param>
        /// <param name="instrumentSerializer">The instrument serializer.</param>
        /// <param name="requestSerializer">The request serializer.</param>
        /// <param name="responseSerializer">The response serializer.</param>
        /// <param name="host">The host address.</param>
        /// <param name="port">The port.</param>
        public InstrumentResponder(
            IComponentryContainer container,
            IInstrumentRepository repository,
            IInstrumentSerializer instrumentSerializer,
            IRequestSerializer requestSerializer,
            IResponseSerializer responseSerializer,
            NetworkAddress host,
            NetworkPort port)
            : base(
                container,
                requestSerializer,
                responseSerializer,
                host,
                port,
                Guid.NewGuid())
        {
            this.repository = repository;
            this.instrumentSerializer = instrumentSerializer;

            this.RegisterHandler<InstrumentRequest>(this.OnMessage);
            this.RegisterHandler<InstrumentsRequest>(this.OnMessage);
        }

        private void OnMessage(InstrumentRequest request)
        {
            var query = this.repository.FindInCache(request.Symbol);

            if (query.IsFailure)
            {
                this.SendBadRequest(request, query.Message);
                this.Log.Error(query.Message);
            }

            var instrument = new[] { this.instrumentSerializer.Serialize(query.Value) };
            var response = new InstrumentDataResponse(
                instrument,
                request.Id,
                Guid.NewGuid(),
                this.TimeNow());

            this.SendResponse(request.RequesterId, response);
        }

        private void OnMessage(InstrumentsRequest request)
        {
            var query = this.repository.FindInCache(request.Venue);

            if (query.IsFailure)
            {
                this.SendBadRequest(request, query.Message);
                this.Log.Error(query.Message);
            }

            var instruments = query
                .Value
                .Select(i => this.instrumentSerializer.Serialize(i))
                .ToArray();

            var response = new InstrumentDataResponse(
                instruments,
                request.Id,
                Guid.NewGuid(),
                this.TimeNow());

            this.SendResponse(request.RequesterId, response);
        }
    }
}
