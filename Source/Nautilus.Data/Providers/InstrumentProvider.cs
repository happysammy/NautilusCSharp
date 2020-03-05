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
    using Nautilus.Common.Messaging;
    using Nautilus.Data.Interfaces;
    using Nautilus.Data.Messages.Requests;
    using Nautilus.Data.Messages.Responses;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.Network.Messages;

    /// <summary>
    /// Provides <see cref="Instrument"/> data to requests.
    /// </summary>
    public sealed class InstrumentProvider : MessageBusConnected
    {
        private readonly IInstrumentRepositoryReadOnly repository;
        private readonly IDataSerializer<Instrument> dataSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstrumentProvider"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="repository">The instrument repository.</param>
        /// <param name="dataSerializer">The data serializer.</param>
        public InstrumentProvider(
            IComponentryContainer container,
            IMessageBusAdapter messagingAdapter,
            IInstrumentRepositoryReadOnly repository,
            IDataSerializer<Instrument> dataSerializer)
            : base(container, messagingAdapter)
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

                    this.Send(response, ServiceAddress.DataServer);
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
                    this.Send(response, ServiceAddress.DataServer);
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

        private void SendQueryFailure(string message, Guid correlationId)
        {
            var response = new QueryFailure(
                message,
                correlationId,
                this.NewGuid(),
                this.TimeNow());

            this.Send(response, ServiceAddress.DataServer);
            this.Logger.LogWarning(response.ToString());
        }
    }
}
