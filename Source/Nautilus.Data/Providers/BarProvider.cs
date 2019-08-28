//--------------------------------------------------------------------------------------------------
// <copyright file="BarProvider.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Message;
    using Nautilus.Data.Interfaces;
    using Nautilus.Data.Messages.Requests;
    using Nautilus.Data.Messages.Responses;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.Frames;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging;
    using Nautilus.Network;

    /// <summary>
    /// Provides <see cref="Bar"/> data to requests.
    /// </summary>
    public sealed class BarProvider : MessageServer<Request, Response>
    {
        private readonly IBarRepository repository;
        private readonly IDataSerializer<BarDataFrame> dataSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="BarProvider"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="repository">The instrument repository.</param>
        /// <param name="inboundSerializer">The inbound message serializer.</param>
        /// <param name="outboundSerializer">The outbound message serializer.</param>
        /// <param name="dataSerializer">The data serializer.</param>
        /// <param name="host">The host address.</param>
        /// <param name="port">The port.</param>
        public BarProvider(
            IComponentryContainer container,
            IBarRepository repository,
            IDataSerializer<BarDataFrame> dataSerializer,
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
            this.dataSerializer = dataSerializer;

            this.RegisterHandler<Envelope<DataRequest>>(this.OnMessage);
        }

        private void OnMessage(Envelope<DataRequest> envelope)
        {
            var request = envelope.Message;

            try
            {
                var dataType = request.Query["DataType"];
                if (dataType != "Bar[]")
                {
                    this.SendQueryFailure($"Incorrect DataType requested (was {dataType})", request.Id, envelope.Sender);
                    return;
                }

                var symbol = Symbol.FromString(request.Query["Symbol"]);
                var barSpec = DomainObjectParser.ParseBarSpecification(request.Query["Specification"]);
                var barType = new BarType(symbol, barSpec);
                var fromDateTime = request.Query["FromDateTime"].ToZonedDateTimeFromIso();
                var toDateTime = request.Query["ToDateTime"].ToZonedDateTimeFromIso();

                var query = this.repository.Find(
                    barType,
                    fromDateTime,
                    toDateTime);

                if (query.IsFailure)
                {
                    this.SendQueryFailure(query.Message, request.Id, envelope.Sender);
                    this.Log.Warning($"{envelope.Message} query failed ({query.Message}).");
                    return;
                }

                var bars = query
                    .Value
                    .Bars
                    .ToArray();

                var data = this.dataSerializer.Serialize(new BarDataFrame(barType, bars));

                var response = new DataResponse(
                    data,
                    this.dataSerializer.DataEncoding,
                    request.Id,
                    this.NewGuid(),
                    this.TimeNow());

                this.SendMessage(response, envelope.Sender);
            }
            catch (Exception ex)
            {
                this.SendQueryFailure(ex.Message, request.Id, envelope.Sender);
            }
        }
    }
}
