//--------------------------------------------------------------------------------------------------
// <copyright file="TickProvider.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Providers
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Message;
    using Nautilus.Data.Interfaces;
    using Nautilus.Data.Keys;
    using Nautilus.Data.Messages.Requests;
    using Nautilus.Data.Messages.Responses;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Network;
    using Nautilus.Network.Encryption;
    using Nautilus.Network.Nodes;

    /// <summary>
    /// Provides <see cref="Tick"/> data to requests.
    /// </summary>
    public sealed class TickProvider : MessageServer
    {
        private readonly ITickRepositoryReadOnly repository;
        private readonly IDataSerializer<Tick> dataSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="TickProvider"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="repository">The tick repository.</param>
        /// <param name="dataSerializer">The data serializer.</param>
        /// <param name="headerSerializer">The header serializer.</param>
        /// <param name="requestSerializer">The inbound message serializer.</param>
        /// <param name="responseSerializer">The outbound message serializer.</param>
        /// <param name="compressor">The data compressor.</param>
        /// <param name="encryption">The encryption configuration.</param>
        /// <param name="port">The port.</param>
        public TickProvider(
            IComponentryContainer container,
            ITickRepositoryReadOnly repository,
            IDataSerializer<Tick> dataSerializer,
            ISerializer<Dictionary<string, string>> headerSerializer,
            IMessageSerializer<Request> requestSerializer,
            IMessageSerializer<Response> responseSerializer,
            ICompressor compressor,
            EncryptionSettings encryption,
            Port port)
            : base(
                container,
                headerSerializer,
                requestSerializer,
                responseSerializer,
                compressor,
                encryption,
                ZmqNetworkAddress.LocalHost(port))
        {
            this.repository = repository;
            this.dataSerializer = dataSerializer;

            this.RegisterHandler<DataRequest>(this.OnMessage);
        }

        private void OnMessage(DataRequest request)
        {
            try
            {
                this.Logger.LogInformation($"<--[REQ] {request}.");

                var dataType = request.Query["DataType"];
                if (dataType != typeof(Tick[]).Name)
                {
                    this.SendQueryFailure($"incorrect DataType requested, was {dataType}", request.Id);
                    return;
                }

                // Query objects
                var symbol = Symbol.FromString(request.Query["Symbol"]);
                var fromDate = DateKey.FromString(request.Query["FromDate"]);
                var toDate = DateKey.FromString(request.Query["ToDate"]);
                var limit = Convert.ToInt32(request.Query["Limit"]);

                var dataQuery = this.repository.GetTickData(
                    symbol,
                    fromDate,
                    toDate,
                    limit);

                if (dataQuery.IsFailure)
                {
                    this.SendQueryFailure(dataQuery.Message, request.Id);
                    this.Logger.LogWarning($"{request} query failed ({dataQuery.Message}).");
                    return;
                }

                var response = new DataResponse(
                    this.dataSerializer.SerializeBlob(dataQuery.Value, request.Query),
                    dataType,
                    this.dataSerializer.BlobEncoding,
                    request.Id,
                    this.NewGuid(),
                    this.TimeNow());

                this.Logger.LogInformation($"[RES]--> {response}.");
                this.SendMessage(response, request.Id);
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"{ex}");
                this.SendQueryFailure(ex.Message, request.Id);
            }
        }
    }
}
