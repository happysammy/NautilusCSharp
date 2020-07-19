//--------------------------------------------------------------------------------------------------
// <copyright file="InstrumentProvider.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
//--------------------------------------------------------------------------------------------------

using System;
using Microsoft.Extensions.Logging;
using Nautilus.Common.Interfaces;
using Nautilus.Common.Logging;
using Nautilus.Common.Messages.Commands;
using Nautilus.Common.Messaging;
using Nautilus.Core.Message;
using Nautilus.Data.Interfaces;
using Nautilus.Data.Messages.Requests;
using Nautilus.Data.Messages.Responses;
using Nautilus.DomainModel.Entities;
using Nautilus.DomainModel.Identifiers;
using Nautilus.Network.Messages;

namespace Nautilus.Data.Providers
{
    /// <summary>
    /// Provides <see cref="Instrument"/> data for requests.
    /// </summary>
    public sealed class InstrumentProvider : MessageBusConnected
    {
        private const string DataType = nameof(DataType);

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
                if (dataType != typeof(Instrument[]).Name)
                {
                    return this.QueryFailure($"Incorrect DataType requested, was {dataType}", request.Id);
                }

                // Query for symbol instrument
                if (request.Query.ContainsKey("Symbol"))
                {
                    var symbol = Symbol.FromString(request.Query["Symbol"]);
                    var dataQuery = this.repository.GetInstrumentData(symbol);
                    if (dataQuery.IsFailure)
                    {
                        return this.QueryFailure(dataQuery.Message, request.Id);
                    }

                    if (dataQuery.Value.Length == 0)
                    {
                        var message = $"Cannot find instruments for the '{symbol}' symbol";
                        return this.QueryFailure(message, request.Id);
                    }

                    return new DataResponse(
                        this.dataSerializer.SerializeBlob(dataQuery.Value, request.Query),
                        dataType,
                        this.dataSerializer.BlobEncoding,
                        request.Id,
                        Guid.NewGuid(),
                        this.TimeNow());
                }

                // Query for all venue instruments
                if (request.Query.ContainsKey("Venue"))
                {
                    var venue = new Venue(request.Query["Venue"]);
                    var dataQuery = this.repository.GetInstrumentData(venue);
                    if (dataQuery.IsFailure)
                    {
                        return this.QueryFailure(dataQuery.Message, request.Id);
                    }

                    if (dataQuery.Value.Length == 0)
                    {
                        var message = $"Cannot find instruments for the '{venue}' venue";
                        return this.QueryFailure(message, request.Id);
                    }

                    return new DataResponse(
                        this.dataSerializer.SerializeBlob(dataQuery.Value, request.Query),
                        dataType,
                        this.dataSerializer.BlobEncoding,
                        request.Id,
                        Guid.NewGuid(),
                        this.TimeNow());
                }

                return this.QueryFailure("Invalid Instrument query, must contain 'Symbol' or 'Venue'", request.Id);
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

            this.Send(response, ComponentAddress.DataServer);
        }
    }
}
