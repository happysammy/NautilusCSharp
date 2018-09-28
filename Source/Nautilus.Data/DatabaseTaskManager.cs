//--------------------------------------------------------------------------------------------------
// <copyright file="DatabaseTaskManager.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data
{
    using System;
    using System.Collections.Generic;
    using Akka.Actor;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Common.Messages.Documents;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Validation;
    using Nautilus.Data.Interfaces;
    using Nautilus.Data.Messages.Commands;
    using Nautilus.Data.Messages.Documents;
    using Nautilus.Data.Messages.Events;
    using Nautilus.Data.Types;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The component manages the queue of job messages being sent to the database.
    /// </summary>
    public class DatabaseTaskManager : ActorComponentBase
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
                LabelFactory.Component(nameof(DatabaseTaskManager)),
                container)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(barRepository, nameof(barRepository));
            Validate.NotNull(instrumentRepository, nameof(instrumentRepository));

            this.barRepository = barRepository;
            this.instrumentRepository = instrumentRepository;

            // Setup message handling.
            this.Receive<DataDelivery<BarClosed>>(this.OnMessage);
            this.Receive<DataDelivery<BarDataFrame>>(this.OnMessage);
            this.Receive<DataDelivery<IReadOnlyCollection<Instrument>>>(this.OnMessage);
            this.Receive<DataStatusRequest<BarType>>(msg => this.OnMessage(msg, this.Sender));
            this.Receive<QueryRequest<BarType>>(msg => this.OnMessage(msg, this.Sender));
            this.Receive<TrimBarData>(this.OnMessage);
        }

        /// <summary>
        /// Actions to be performed prior to stopping the <see cref="DatabaseTaskManager"/>.
        /// </summary>
        protected override void PostStop()
        {
            this.barRepository.SnapshotDatabase();
            base.PostStop();
        }

        private void OnMessage(SystemShutdown message)
        {
            this.PostStop();
        }

        private void OnMessage(DataDelivery<BarClosed> message)
        {
            Debug.NotNull(message, nameof(message));

            this.barRepository
                .Add(
                    message.Data.BarType,
                    message.Data.Bar)
                .OnSuccess(result => this.Log.Debug(result.Message))
                .OnFailure(result => this.Log.Warning(result.Message));
        }

        private void OnMessage(DataDelivery<BarDataFrame> message)
        {
            Debug.NotNull(message, nameof(message));

            this.barRepository
                .Add(message.Data)
                .OnSuccess(result => this.SendLastBarTimestamp(message.Data.BarType))
                .OnFailure(result => this.Log.Warning(result.Message));
        }

        private void SendLastBarTimestamp(BarType barType)
        {
            Debug.NotNull(barType, nameof(barType));

            this.barRepository
                .LastBarTimestamp(barType)
                .OnSuccess(query =>
                {
                    if (query != default)
                    {
                        this.Sender.Tell(new DataPersisted<BarType>(
                            barType,
                            query,
                            this.NewGuid(),
                            this.TimeNow()));
                    }
                });
        }

        private void OnMessage(DataDelivery<IReadOnlyCollection<Instrument>> message)
        {
            Debug.NotNull(message, nameof(message));

            foreach (var instrument in message.Data)
            {
                this.instrumentRepository
                    .Add(instrument, this.TimeNow())
                    .OnSuccess(result => this.Log.Information(result.Message))
                    .OnFailure(result => this.Log.Error(result.Message));
            }
        }

        private void OnMessage(DataStatusRequest<BarType> message, IActorRef sender)
        {
            Debug.NotNull(message, nameof(message));

            var lastBarTimestampQuery = this.barRepository.LastBarTimestamp(message.DataType);

            sender.Tell(new DataStatusResponse<ZonedDateTime>(lastBarTimestampQuery, Guid.NewGuid(), this.TimeNow()));
        }

        private void OnMessage(QueryRequest<BarType> message, IActorRef sender)
        {
            Debug.NotNull(message, nameof(message));
            Debug.NotNull(sender, nameof(sender));

            var barDataQuery = this.barRepository.Find(
                message.DataType,
                message.FromDateTime,
                message.ToDateTime);

            sender.Tell(
                new QueryResponse<BarDataFrame>(
                    barDataQuery,
                    this.NewGuid(),
                    this.TimeNow()));
        }

        private void OnMessage(TrimBarData message)
        {
            Debug.NotNull(message, nameof(message));

            foreach (var resolution in message.Resolutions)
            {
                this.barRepository
                    .TrimToDays(resolution, message.RollingWindowSize)
                    .OnSuccess(result => this.Log.Information(result.Message))
                    .OnFailure(result => this.Log.Error(result.Message));
            }
        }
    }
}
