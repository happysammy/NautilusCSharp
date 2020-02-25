//--------------------------------------------------------------------------------------------------
// <copyright file="InstrumentProvider.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Providers
{
    using System;
    using Microsoft.Extensions.Logging;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Message;
    using Nautilus.Data.Interfaces;
    using Nautilus.Data.Messages.Requests;
    using Nautilus.Data.Messages.Responses;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.Network;
    using Nautilus.Network.Encryption;

    /// <summary>
    /// Provides <see cref="Instrument"/> data to requests.
    /// </summary>
    public sealed class InstrumentProvider : MessageServer
    {
        private readonly IInstrumentRepositoryReadOnly repository;
        private readonly IDataSerializer<Instrument> dataSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstrumentProvider"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="repository">The instrument repository.</param>
        /// <param name="dataSerializer">The data serializer.</param>
        /// <param name="requestSerializer">The inbound message serializer.</param>
        /// <param name="responseSerializer">The outbound message serializer.</param>
        /// <param name="compressor">The data compressor.</param>
        /// <param name="encryption">The encryption configuration.</param>
        /// <param name="port">The port.</param>
        public InstrumentProvider(
            IComponentryContainer container,
            IInstrumentRepositoryReadOnly repository,
            IDataSerializer<Instrument> dataSerializer,
            IMessageSerializer<Request> requestSerializer,
            IMessageSerializer<Response> responseSerializer,
            ICompressor compressor,
            EncryptionSettings encryption,
            Port port)
            : base(
                container,
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
                if (dataType != typeof(Instrument[]).Name)
                {
                    this.SendQueryFailure($"Incorrect DataType requested, was {dataType}", request.Id);
                    return;
                }

                // Query for symbol instrument
                if (request.Query.ContainsKey("Symbol"))
                {
                    var symbol = Symbol.FromString(request.Query["Symbol"]);
                    var dataQuery = this.repository.GetInstrumentData(symbol);
                    if (dataQuery.IsFailure)
                    {
                        this.SendQueryFailure(dataQuery.Message, request.Id);
                        this.Logger.LogWarning($"{request} QueryFailure({dataQuery.Message}).");
                        return;
                    }

                    if (dataQuery.Value.Length == 0)
                    {
                        var message = $"Cannot find instruments for the '{symbol}' symbol";
                        this.SendQueryFailure(dataQuery.Message, request.Id);
                        this.Logger.LogWarning($"{request} QueryFailure({message}).");
                        return;
                    }

                    var response = new DataResponse(
                        this.dataSerializer.SerializeBlob(dataQuery.Value, request.Query),
                        dataType,
                        this.dataSerializer.BlobEncoding,
                        request.Id,
                        Guid.NewGuid(),
                        this.TimeNow());

                    this.SendMessage(response, request.Id);
                }

                // Query for all venue instruments
                else if (request.Query.ContainsKey("Venue"))
                {
                    var venue = new Venue(request.Query["Venue"]);
                    var dataQuery = this.repository.GetInstrumentData(venue);
                    if (dataQuery.IsFailure)
                    {
                        this.SendQueryFailure(dataQuery.Message, request.Id);
                        this.Logger.LogWarning($"{request} QueryFailure({dataQuery.Message}).");
                        return;
                    }

                    if (dataQuery.Value.Length == 0)
                    {
                        var message = $"Cannot find instruments for the '{venue}' venue";
                        this.SendQueryFailure(message, request.Id);
                        this.Logger.LogWarning($"{request} QueryFailure({message}).");
                        return;
                    }

                    var response = new DataResponse(
                        this.dataSerializer.SerializeBlob(dataQuery.Value, request.Query),
                        dataType,
                        this.dataSerializer.BlobEncoding,
                        request.Id,
                        Guid.NewGuid(),
                        this.TimeNow());

                    this.Logger.LogInformation($"[RES]--> {response}.");
                    this.SendMessage(response, request.Id);
                }
                else
                {
                    this.SendQueryFailure("Invalid Instrument query, must contain 'Symbol' or 'Venue'", request.Id);
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"{ex}");
                this.SendQueryFailure(ex.Message, request.Id);
            }
        }
    }
}
