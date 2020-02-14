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
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Message;
    using Nautilus.Data.Interfaces;
    using Nautilus.Data.Keys;
    using Nautilus.Data.Messages.Requests;
    using Nautilus.Data.Messages.Responses;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Network;

    /// <summary>
    /// Provides <see cref="Bar"/> data to requests.
    /// </summary>
    public sealed class BarProvider : MessageServer<Request, Response>
    {
        private readonly IBarRepositoryReadOnly repository;
        private readonly IDataSerializer<Bar> dataSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="BarProvider"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="repository">The instrument repository.</param>
        /// <param name="dataSerializer">The data serializer for the provider.</param>
        /// <param name="inboundSerializer">The inbound message serializer.</param>
        /// <param name="outboundSerializer">The outbound message serializer.</param>
        /// <param name="encryption">The encryption configuration.</param>
        /// <param name="port">The port.</param>
        public BarProvider(
            IComponentryContainer container,
            IBarRepositoryReadOnly repository,
            IDataSerializer<Bar> dataSerializer,
            IMessageSerializer<Request> inboundSerializer,
            IMessageSerializer<Response> outboundSerializer,
            EncryptionConfig encryption,
            NetworkPort port)
            : base(
                container,
                inboundSerializer,
                outboundSerializer,
                encryption,
                Network.NetworkAddress.LocalHost,
                port,
                Guid.NewGuid())
        {
            this.repository = repository;
            this.dataSerializer = dataSerializer;

            this.RegisterHandler<DataRequest>(this.OnMessage);
        }

        private void OnMessage(DataRequest request)
        {
            try
            {
                this.Log.Information($"<--[REQ] {request}.");

                var dataType = request.Query["DataType"];
                if (dataType != typeof(Bar[]).Name)
                {
                    this.SendQueryFailure($"incorrect DataType requested, was {dataType}", request.Id);
                    return;
                }

                // Query objects
                var symbol = Symbol.FromString(request.Query["Symbol"]);
                var barSpec = BarSpecification.FromString(request.Query["Specification"]);
                var barType = new BarType(symbol, barSpec);
                var fromDate = DateKey.FromString(request.Query["FromDate"]);
                var toDate = DateKey.FromString(request.Query["ToDate"]);
                var limit = Convert.ToInt32(request.Query["Limit"]);

                var dataQuery = this.repository.GetBarData(
                    barType,
                    fromDate,
                    toDate,
                    limit);

                if (dataQuery.IsFailure)
                {
                    this.SendQueryFailure(dataQuery.Message, request.Id);
                    this.Log.Warning($"{request} QueryFailure({dataQuery.Message}).");
                    return;
                }

                var response = new DataResponse(
                    this.dataSerializer.SerializeBlob(dataQuery.Value, request.Query),
                    dataType,
                    this.dataSerializer.BlobEncoding,
                    request.Id,
                    this.NewGuid(),
                    this.TimeNow());

                this.Log.Information($"[RES]--> {response}.");
                this.SendMessage(response, request.Id);
            }
            catch (Exception ex)
            {
                this.Log.Error($"{ex}");
                this.SendQueryFailure(ex.Message, request.Id);
            }
        }
    }
}
