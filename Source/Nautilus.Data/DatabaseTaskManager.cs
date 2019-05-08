//--------------------------------------------------------------------------------------------------
// <copyright file="DatabaseTaskManager.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data
{
    using System;
    using System.Collections.Generic;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Documents;
    using Nautilus.Core.Extensions;
    using Nautilus.Data.Interfaces;
    using Nautilus.Data.Messages.Commands;
    using Nautilus.Data.Messages.Documents;
    using Nautilus.Data.Messages.Events;
    using Nautilus.Data.Types;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging;
    using NodaTime;

    /// <summary>
    /// The component manages the queue of job messages being sent to the database.
    /// </summary>
    public class DatabaseTaskManager : ComponentBase
    {
        private readonly IBarRepository barRepository;
        private readonly IInstrumentRepository instrumentRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseTaskManager"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="barRepository">The bar repository.</param>
        /// <param name="instrumentRepository">The instrument repository.</param>
        public DatabaseTaskManager(
            IComponentryContainer container,
            IBarRepository barRepository,
            IInstrumentRepository instrumentRepository)
            : base(
                NautilusService.Data,
                LabelFactory.Create(nameof(DatabaseTaskManager)),
                container)
        {
            this.barRepository = barRepository;
            this.instrumentRepository = instrumentRepository;

            this.RegisterHandler<DataDelivery<BarClosed>>(this.OnMessage);
            this.RegisterHandler<DataDelivery<BarDataFrame>>(this.OnMessage);
            this.RegisterHandler<TrimBarData>(this.OnMessage);
        }

        /// <summary>
        /// Actions to be performed prior to stopping the <see cref="DatabaseTaskManager"/>.
        /// </summary>
        public override void Stop()
        {
            this.barRepository.SnapshotDatabase();
        }

        private void OnMessage(TrimBarData message)
        {
            foreach (var resolution in message.Resolutions)
            {
                this.barRepository
                    .TrimToDays(resolution, message.RollingWindowSize)
                    .OnSuccess(result => this.Log.Information(result.Message))
                    .OnFailure(result => this.Log.Error(result.Message));
            }
        }

        private void OnMessage(DataStatusRequest<BarType> message, IEndpoint sender)
        {
            var lastBarTimestampQuery = this.barRepository.LastBarTimestamp(message.DataType);

            sender.Send(new DataStatusResponse<ZonedDateTime>(lastBarTimestampQuery, Guid.NewGuid(), this.TimeNow()));
        }

        private void OnMessage(QueryRequest<BarType> message, IEndpoint sender)
        {
            var barDataQuery = this.barRepository.Find(
                message.DataType,
                message.FromDateTime,
                message.ToDateTime);

            sender.Send(
                new QueryResponse<BarDataFrame>(
                    barDataQuery,
                    this.NewGuid(),
                    this.TimeNow()));
        }

        private void OnMessage(DataDelivery<BarClosed> message)
        {
            this.barRepository
                .Add(
                    message.Data.BarType,
                    message.Data.Bar)
                .OnSuccess(result => this.Log.Verbose(result.Message))
                .OnFailure(result => this.Log.Warning(result.Message));
        }

        private void OnMessage(DataDelivery<BarDataFrame> message)
        {
            this.barRepository
                .Add(message.Data)
                .OnSuccess(result => this.SendLastBarTimestamp(message.Data.BarType))
                .OnFailure(result => this.Log.Warning(result.Message));
        }

        private void SendLastBarTimestamp(BarType barType)
        {
            this.barRepository
                .LastBarTimestamp(barType)
                .OnSuccess(query =>
                {
                    if (query != default)
                    {
// this.Sender.Tell(new DataPersisted<BarType>(
//                            barType,
//                            query,
//                            this.NewGuid(),
//                            this.TimeNow()));
                    }
                });
        }

        private void OnMessage(DataDelivery<IReadOnlyCollection<Instrument>> message)
        {
            foreach (var instrument in message.Data)
            {
                this.instrumentRepository
                    .Add(instrument, this.TimeNow())
                    .OnSuccess(result => this.Log.Information(result.Message))
                    .OnFailure(result => this.Log.Error(result.Message));
            }
        }
    }
}
