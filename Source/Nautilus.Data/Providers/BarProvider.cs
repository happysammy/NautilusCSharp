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
    using Microsoft.Extensions.Logging;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Logging;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Message;
    using Nautilus.Data.Interfaces;
    using Nautilus.Data.Keys;
    using Nautilus.Data.Messages.Requests;
    using Nautilus.Data.Messages.Responses;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Network.Messages;

    /// <summary>
    /// Provides <see cref="Bar"/> data for requests.
    /// </summary>
    public sealed class BarProvider : MessageBusConnected
    {
        private const string DataType = nameof(DataType);

        private readonly IBarRepositoryReadOnly repository;
        private readonly IDataSerializer<Bar> dataSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="BarProvider"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="repository">The instrument repository.</param>
        /// <param name="dataSerializer">The data serializer for the provider.</param>
        public BarProvider(
            IComponentryContainer container,
            IMessageBusAdapter messagingAdapter,
            IBarRepositoryReadOnly repository,
            IDataSerializer<Bar> dataSerializer)
            : base(container, messagingAdapter)
        {
            this.repository = repository;
            this.dataSerializer = dataSerializer;

            this.RegisterHandler<DataRequest>(this.OnMessage);
        }

        /// <summary>
        /// Perform a query operation for the give data request.
        /// </summary>
        /// <param name="request">The data request.</param>
        /// <returns>The response.</returns>
        internal Response FindData(DataRequest request)
        {
            try
            {
                var dataType = request.Query[DataType];
                if (dataType != typeof(Bar[]).Name)
                {
                    return this.QueryFailure($"Incorrect DataType requested, was {dataType}", request.Id);
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
                    return this.QueryFailure(dataQuery.Message, request.Id);
                }

                return new DataResponse(
                    this.dataSerializer.SerializeBlob(dataQuery.Value, request.Query),
                    dataType,
                    this.dataSerializer.BlobEncoding,
                    request.Id,
                    this.NewGuid(),
                    this.TimeNow());
            }
            catch (Exception ex)
            {
                this.Logger.LogError(LogId.Component, $"{ex}");
                return this.QueryFailure(ex.Message, request.Id);
            }
        }

        /// <inheritdoc />
        protected override void OnStart(Start start)
        {
        }

        /// <inheritdoc />
        protected override void OnStop(Stop stop)
        {
        }

        private Response QueryFailure(string message, Guid correlationId)
        {
            return new QueryFailure(
                message,
                correlationId,
                this.NewGuid(),
                this.TimeNow());
        }

        private void OnMessage(DataRequest request)
        {
            this.Logger.LogInformation(LogId.Component, $"<--[REQ] {request}.");

            var response = this.FindData(request);
            if (response is DataResponse)
            {
                this.Logger.LogInformation(LogId.Component, $"[RES]--> {response}.");
            }
            else
            {
                this.Logger.LogWarning(LogId.Component, $"[RES]--> {response}.");
            }

            this.Send(response, ServiceAddress.DataServer);
        }
    }
}
