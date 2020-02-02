//--------------------------------------------------------------------------------------------------
// <copyright file="BarProvider.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Providers
{
    using System;
    using System.Runtime.CompilerServices;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Message;
    using Nautilus.Data.Interfaces;
    using Nautilus.Data.Keys;
    using Nautilus.Data.Messages.Requests;
    using Nautilus.Data.Messages.Responses;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging;
    using Nautilus.Network;

    /// <summary>
    /// Provides <see cref="Bar"/> data to requests.
    /// </summary>
    public sealed class BarProvider : MessageServer<Request, Response>
    {
        private const string BARARRAY = "Bar[]";

        private readonly IBarRepositoryReadOnly repository;
        private readonly IByteArrayArraySerializer dataSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="BarProvider"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="repository">The instrument repository.</param>
        /// <param name="dataSerializer">The data serializer for the provider.</param>
        /// <param name="inboundSerializer">The inbound message serializer.</param>
        /// <param name="outboundSerializer">The outbound message serializer.</param>
        /// <param name="port">The port.</param>
        public BarProvider(
            IComponentryContainer container,
            IBarRepositoryReadOnly repository,
            IByteArrayArraySerializer dataSerializer,
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
                if (dataType != BARARRAY)
                {
                    this.SendQueryFailure($"incorrect DataType requested, was {dataType}", request.Id, envelope.Sender);
                    return;
                }

                // Query objects
                var symbol = Symbol.FromString(request.Query["Symbol"]);
                var barSpec = BarSpecification.FromString(request.Query["Specification"]);
                var barType = new BarType(symbol, barSpec);
                var fromDate = DateKey.FromString(request.Query["FromDate"]);
                var toDate = DateKey.FromString(request.Query["ToDate"]);
                var limit = Convert.ToInt32(request.Query["Limit"]);

                var query = this.repository.GetBarData(
                    barType,
                    fromDate,
                    toDate,
                    limit);

                if (query.IsFailure)
                {
                    this.SendQueryFailure(query.Message, request.Id, envelope.Sender);
                    this.Log.Warning($"{envelope.Message} QueryFailure({query.Message}).");
                    return;
                }

                var response = new DataResponse(
                    this.dataSerializer.Serialize(query.Value),
                    typeof(Bar[]).Name,
                    this.dataSerializer.Encoding,
                    request.Query,
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
