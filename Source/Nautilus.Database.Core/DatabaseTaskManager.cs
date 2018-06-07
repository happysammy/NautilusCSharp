//--------------------------------------------------------------------------------------------------
// <copyright file="DatabaseTaskManager.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Core
{
    using System;
    using Akka.Actor;
    using Nautilus.Core;
    using Nautilus.Core.Validation;
    using NodaTime;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Database.Core.Interfaces;
    using Nautilus.Database.Core.Messages;
    using Nautilus.Database.Core.Messages.Events;
    using Nautilus.Database.Core.Messages.Queries;
    using Nautilus.Database.Core.Types;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Factories;

    /// <summary>
    /// The component which manages the queue of job messages being sent to the database.
    /// </summary>
    public class DatabaseTaskManager : ActorComponentBase
    {
        private readonly IMarketDataRepository marketDataRepository;
        private readonly IEconomicEventRepository<EconomicEvent> economicEventRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseTaskManager"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="marketDataRepository">The market data repository.</param>
        /// <param name="economicEventRepository">The news event repository.</param>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public DatabaseTaskManager(
            IComponentryContainer container,
            IMarketDataRepository marketDataRepository,
            IEconomicEventRepository<EconomicEvent> economicEventRepository)
            : base(
                ServiceContext.Database,
                LabelFactory.Component(nameof(DatabaseTaskManager)),
                container)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(marketDataRepository, nameof(marketDataRepository));
            Validate.NotNull(economicEventRepository, nameof(economicEventRepository));

            this.marketDataRepository = marketDataRepository;
            this.economicEventRepository = economicEventRepository;

            this.Receive<DataStatusRequest>(msg => this.OnMessage(msg, this.Sender));
            this.Receive<MarketDataDelivery>(msg => this.OnMessage(msg, this.Sender));
            this.Receive<MarketDataQueryRequest>(msg => this.OnMessage(msg, this.Sender));
        }

        private void OnMessage(DataStatusRequest message, IActorRef sender)
        {
            Debug.NotNull(message, nameof(message));

            var lastBarTimestampQuery = this.marketDataRepository.LastBarTimestamp(message.SymbolBarSpec);

            sender.Tell(new DataStatusResponse(lastBarTimestampQuery, Guid.NewGuid(), this.TimeNow()));
        }

        private void OnMessage(MarketDataDelivery message, IActorRef sender)
        {
            Debug.NotNull(message, nameof(message));
            Debug.NotNull(sender, nameof(sender));

            var symbolBarData = message.MarketData.SymbolBarSpec;
            var result = this.marketDataRepository.Add(message.MarketData);
            this.Log.Result(result);

            var lastBarTimeQuery = this.marketDataRepository.LastBarTimestamp(symbolBarData);

            if (result.IsSuccess && lastBarTimeQuery.IsSuccess && lastBarTimeQuery.Value != default(ZonedDateTime))
            {
                this.Sender.Tell(new MarketDataPersisted(
                    symbolBarData,
                    lastBarTimeQuery.Value,
                    this.NewGuid(),
                    this.TimeNow()));
            }
        }

        private void OnMessage(MarketDataQueryRequest message, IActorRef sender)
        {
            Debug.NotNull(message, nameof(message));
            Debug.NotNull(sender, nameof(sender));

            var marketDataQuery = this.marketDataRepository.Find(
                new SymbolBarSpec(message.Symbol, message.BarSpecification),
                message.FromDateTime,
                message.ToDateTime);

            var marketData = marketDataQuery.IsSuccess
                           ? marketDataQuery.Value
                           : Option<MarketDataFrame>.None();

                sender.Tell(new MarketDataQueryResponse(
                    marketData,
                    marketDataQuery.IsSuccess,
                    marketDataQuery.Message,
                    this.NewGuid(),
                    this.TimeNow()));
        }
    }
}
