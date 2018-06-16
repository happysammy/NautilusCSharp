//--------------------------------------------------------------------------------------------------
// <copyright file="DatabaseTaskManager.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Database
{
    using System;
    using Akka.Actor;
    using Nautilus.Core;
    using Nautilus.Core.Validation;
    using NodaTime;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Database.Interfaces;
    using Nautilus.Database.Messages;
    using Nautilus.Database.Messages.Events;
    using Nautilus.Database.Messages.Queries;
    using Nautilus.Database.Types;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The component which manages the queue of job messages being sent to the database.
    /// </summary>
    public class DatabaseTaskManager : ActorComponentBase
    {
        private readonly IBarRepository barRepository;
        private readonly IEconomicEventRepository<EconomicEvent> economicEventRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseTaskManager"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="barRepository">The market data repository.</param>
        /// <param name="economicEventRepository">The news event repository.</param>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public DatabaseTaskManager(
            IComponentryContainer container,
            IBarRepository barRepository,
            IEconomicEventRepository<EconomicEvent> economicEventRepository)
            : base(
                ServiceContext.Database,
                LabelFactory.Component(nameof(DatabaseTaskManager)),
                container)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(barRepository, nameof(barRepository));
            Validate.NotNull(economicEventRepository, nameof(economicEventRepository));

            this.barRepository = barRepository;
            this.economicEventRepository = economicEventRepository;

            this.Receive<DataStatusRequest<SymbolBarSpec>>(msg => this.OnMessage(msg, this.Sender));
            this.Receive<DataDelivery<BarDataFrame>>(msg => this.OnMessage(msg, this.Sender));
            this.Receive<QueryRequest<SymbolBarSpec>>(msg => this.OnMessage(msg, this.Sender));
        }

        private void OnMessage(DataStatusRequest<SymbolBarSpec> message, IActorRef sender)
        {
            Debug.NotNull(message, nameof(message));

            var lastBarTimestampQuery = this.barRepository.LastBarTimestamp(message.DataType);

            sender.Tell(new DataStatusResponse<ZonedDateTime>(lastBarTimestampQuery, Guid.NewGuid(), this.TimeNow()));
        }

        private void OnMessage(DataDelivery<BarDataFrame> message, IActorRef sender)
        {
            Debug.NotNull(message, nameof(message));
            Debug.NotNull(sender, nameof(sender));

            var symbolBarData = message.Data.SymbolBarSpec;
            var result = this.barRepository.Add(message.Data);
            this.Log.Result(result);

            var lastBarTimeQuery = this.barRepository.LastBarTimestamp(symbolBarData);

            if (result.IsSuccess && lastBarTimeQuery.IsSuccess && lastBarTimeQuery.Value != default(ZonedDateTime))
            {
                this.Sender.Tell(new DataPersisted<SymbolBarSpec>(
                    symbolBarData,
                    lastBarTimeQuery.Value,
                    this.NewGuid(),
                    this.TimeNow()));
            }
        }

        private void OnMessage(QueryRequest<SymbolBarSpec> message, IActorRef sender)
        {
            Debug.NotNull(message, nameof(message));
            Debug.NotNull(sender, nameof(sender));

            var barDataQuery = this.barRepository.Find(
                message.DataType,
                message.FromDateTime,
                message.ToDateTime);

                sender.Tell(new QueryResponse<BarDataFrame>(
                    barDataQuery,
                    this.NewGuid(),
                    this.TimeNow()));
        }
    }
}
