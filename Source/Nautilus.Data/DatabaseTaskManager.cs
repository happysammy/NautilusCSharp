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
    using Akka.Actor;
    using Nautilus.Common.Commands;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Validation;
    using Nautilus.Data.Interfaces;
    using Nautilus.Data.Messages.Commands;
    using Nautilus.Data.Messages.Documents;
    using Nautilus.Data.Messages.Events;
    using Nautilus.Data.Types;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The component manages the queue of job messages being sent to the database.
    /// </summary>
    public class DatabaseTaskManager : ActorComponentBase
    {
        private readonly IBarRepository barRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseTaskManager"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="barRepository">The market data repository.</param>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public DatabaseTaskManager(
            IComponentryContainer container,
            IBarRepository barRepository)
            : base(
                NautilusService.Data,
                LabelFactory.Component(nameof(DatabaseTaskManager)),
                container)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(barRepository, nameof(barRepository));

            this.barRepository = barRepository;

            // Setup message handling.
            this.Receive<QueryRequest<BarType>>(msg => this.OnMessage(msg, this.Sender));
            this.Receive<DataStatusRequest<BarType>>(msg => this.OnMessage(msg, this.Sender));
            this.Receive<TrimBarData>(this.OnMessage);
            this.Receive<DataDelivery<BarClosed>>(msg => this.OnMessage(msg, this.Sender));
            this.Receive<DataDelivery<BarDataFrame>>(msg => this.OnMessage(msg, this.Sender));
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

        private void OnMessage(DataStatusRequest<BarType> message, IActorRef sender)
        {
            Debug.NotNull(message, nameof(message));

            var lastBarTimestampQuery = this.barRepository.LastBarTimestamp(message.DataType);

            sender.Tell(new DataStatusResponse<ZonedDateTime>(lastBarTimestampQuery, Guid.NewGuid(), this.TimeNow()));
        }

        private void OnMessage(DataDelivery<BarClosed> message, IActorRef sender)
        {
            Debug.NotNull(message, nameof(message));
            Debug.NotNull(sender, nameof(sender));

            var result = this.barRepository.Add(
                message.Data.BarType,
                message.Data.Bar);

            this.Log.Result(result);
        }

        private void OnMessage(DataDelivery<BarDataFrame> message, IActorRef sender)
        {
            Debug.NotNull(message, nameof(message));
            Debug.NotNull(sender, nameof(sender));

            var barType = message.Data.BarType;
            var result = this.barRepository.Add(message.Data);
            this.Log.Result(result);

            var lastBarTimeQuery = this.barRepository.LastBarTimestamp(barType);

            if (result.IsSuccess
             && lastBarTimeQuery.IsSuccess
             && lastBarTimeQuery.Value != default)
            {
                this.Sender.Tell(new DataPersisted<BarType>(
                    barType,
                    lastBarTimeQuery.Value,
                    this.NewGuid(),
                    this.TimeNow()));
            }
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
                var result = this.barRepository.TrimToDays(resolution, message.RollingWindowSize);
                this.Log.Result(result);
            }
        }
    }
}
