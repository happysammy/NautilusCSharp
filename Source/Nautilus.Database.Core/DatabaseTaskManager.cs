//--------------------------------------------------------------
// <copyright file="DatabaseTaskManager.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

using System;
using Akka.Actor;
using NautechSystems.CSharp;
using NautechSystems.CSharp.Validation;
using NautilusDB.Core.Types;
using NodaTime;

namespace Nautilus.Database.Core
{
    using Nautilus.Common.Componentry;
    using Nautilus.Database.Core.Interfaces;
    using Nautilus.DomainModel.Entities;

    /// <summary>
    /// The component which manages the queue of job messages being sent to the database.
    /// </summary>
    public class DatabaseTaskManager : ActorComponentBase
    {
        private readonly IMarketDataRepository marketDataRepository;
        private readonly IEconomicNewsEventRepository<EconomicNewsEvent> economicNewsEventRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseTaskManager"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="marketDataRepository">The market data repository.</param>
        /// <param name="economicNewsEventRepository">The news event repository.</param>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public DatabaseTaskManager(
            ComponentryContainer container,
            IMarketDataRepository marketDataRepository,
            IEconomicNewsEventRepository<EconomicNewsEvent> economicNewsEventRepository)
            : base(container, nameof(DatabaseTaskManager))
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(marketDataRepository, nameof(marketDataRepository));
            Validate.NotNull(economicNewsEventRepository, nameof(economicNewsEventRepository));

            this.marketDataRepository = marketDataRepository;
            this.economicNewsEventRepository = economicNewsEventRepository;

            this.Receive<DataStatusRequest>(msg => this.OnMessage(msg, this.Sender));
            this.Receive<MarketDataDelivery>(msg => this.OnMessage(msg, this.Sender));
            this.Receive<MarketDataQueryRequest>(msg => this.OnMessage(msg, this.Sender));
        }

        private void OnMessage(DataStatusRequest message, IActorRef sender)
        {
            Debug.NotNull(message, nameof(message));

            this.LogMsgReceipt(message);

            var lastBarTimestampQuery = this.marketDataRepository.LastBarTimestamp(message.BarSpecification);

            sender.Tell(new DataStatusResponse(lastBarTimestampQuery, Guid.NewGuid(), this.Clock.TimeNow()));
        }

        private void OnMessage(MarketDataDelivery message, IActorRef sender)
        {
            Debug.NotNull(message, nameof(message));
            Debug.NotNull(sender, nameof(sender));

            this.LogMsgReceipt(message);

            var barSpec = message.MarketData.BarSpecification;
            var result = this.marketDataRepository.Add(message.MarketData);
            this.Logger.LogResult(result);

            var lastBarTimeQuery = this.marketDataRepository.LastBarTimestamp(barSpec);

#pragma warning disable IDE0034 // Simplify 'default' expression
            if (result.IsSuccess && lastBarTimeQuery.IsSuccess && lastBarTimeQuery.Value != default(ZonedDateTime))
#pragma warning restore IDE0034 // Simplify 'default' expression
            {
                this.Sender.Tell(new MarketDataPersisted(
                    barSpec,
                    lastBarTimeQuery.Value,
                    Guid.NewGuid(),
                    this.Clock.TimeNow()));
            }
        }

        private void OnMessage(MarketDataQueryRequest message, IActorRef sender)
        {
            Debug.NotNull(message, nameof(message));
            Debug.NotNull(sender, nameof(sender));

            this.LogMsgReceipt(message);

            var marketDataQuery = this.marketDataRepository.Find(
                message.BarSpecification,
                message.FromDateTime,
                message.ToDateTime);

            var marketData = marketDataQuery.IsSuccess
                           ? marketDataQuery.Value
                           : Option<MarketDataFrame>.None();

                sender.Tell(new MarketDataQueryResponse(
                    marketData,
                    marketDataQuery.IsSuccess,
                    marketDataQuery.Message,
                    Guid.NewGuid(),
                    this.Clock.TimeNow()));
        }
    }
}