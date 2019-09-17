//--------------------------------------------------------------------------------------------------
// <copyright file="InstrumentProvider.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Providers
{
    using System;
    using System.Linq;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Message;
    using Nautilus.Data.Interfaces;
    using Nautilus.Data.Messages.Requests;
    using Nautilus.Data.Messages.Responses;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.Messaging;
    using Nautilus.Network;

    /// <summary>
    /// Provides <see cref="Instrument"/> data to requests.
    /// </summary>
    public sealed class InstrumentProvider : MessageServer<Request, Response>
    {
        private readonly IInstrumentRepository repository;
        private readonly IDataSerializer<Instrument[]> dataSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstrumentProvider"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="repository">The instrument repository.</param>
        /// <param name="dataSerializer">The data serializer.</param>
        /// <param name="inboundSerializer">The inbound message serializer.</param>
        /// <param name="outboundSerializer">The outbound message serializer.</param>
        /// <param name="port">The port.</param>
        public InstrumentProvider(
            IComponentryContainer container,
            IInstrumentRepository repository,
            IDataSerializer<Instrument[]> dataSerializer,
            IMessageSerializer<Request> inboundSerializer,
            IMessageSerializer<Response> outboundSerializer,
            NetworkPort port)
            : base(
                container,
                inboundSerializer,
                outboundSerializer,
                NetworkHost.LocalHost,
                port,
                Guid.NewGuid())
        {
            this.repository = repository;
            this.dataSerializer = dataSerializer;

            this.RegisterHandler<Envelope<DataRequest>>(this.OnMessage);
        }

        private void OnMessage(Envelope<DataRequest> envelope)
        {
            var request = envelope.Message;

            try
            {
                var dataType = request.Query["DataType"];
                switch (dataType)
                {
                    case "Instrument":
                    {
                        var symbol = Symbol.FromString(request.Query["Symbol"]);
                        var query = this.repository.FindInCache(symbol);

                        if (query.IsFailure)
                        {
                            this.SendQueryFailure(query.Message, request.Id, envelope.Sender);
                            this.Log.Warning($"{envelope.Message} QueryFailure({query.Message}).");
                            return;
                        }

                        var response = new DataResponse(
                            this.dataSerializer.Serialize(new[] { query.Value }),
                            this.dataSerializer.DataEncoding,
                            request.Id,
                            Guid.NewGuid(),
                            this.TimeNow());

                        this.SendMessage(response, envelope.Sender);
                        break;
                    }

                    case "Instrument[]":
                    {
                        var venue = new Venue(request.Query["Venue"]);
                        var query = this.repository.FindInCache(venue);

                        if (query.IsFailure)
                        {
                            this.SendQueryFailure(query.Message, request.Id, envelope.Sender);
                            this.Log.Warning($"{envelope.Message} QueryFailure({query.Message}).");
                            return;
                        }

                        var response = new DataResponse(
                            this.dataSerializer.Serialize(query.Value.ToArray()),
                            this.dataSerializer.DataEncoding,
                            request.Id,
                            Guid.NewGuid(),
                            this.TimeNow());

                        this.SendMessage(response, envelope.Sender);
                        break;
                    }

                    default:
                        this.SendQueryFailure($"Incorrect DataType requested, was {dataType}", request.Id, envelope.Sender);
                        return;
                }
            }
            catch (Exception ex)
            {
                this.SendQueryFailure(ex.Message, request.Id, envelope.Sender);
            }
        }
    }
}
