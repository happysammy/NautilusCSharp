//--------------------------------------------------------------
// <copyright file="NewsEventService.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace NautilusDB.Service
{
    using System.Collections.Generic;
    using NautechSystems.CSharp.Validation;
    using NautilusDB.Service.Requests;
    using NautilusDB.Service.Responses;
    using ServiceStack;
    using System.Linq;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Extensions;
    using Nautilus.Database.Core.Interfaces;
    using Nautilus.Database.Core.Types;
    using Nautilus.DomainModel.Entities;

    /// <summary>
    /// The service which processes incoming <see cref="NewsEventRequest"/>(s).
    /// </summary>
    public class NewsEventService : Service
    {
        private readonly ILoggingAdapter logger;
        private readonly IEconomicEventRepository<EconomicEvent> economicEventRepository;

        public NewsEventService(
            ILoggingAdapter logger,
            IEconomicEventRepository<EconomicEvent> economicEventRepository)
        {
            Validate.NotNull(logger, nameof(logger));
            Validate.NotNull(economicEventRepository, nameof(economicEventRepository));

            this.logger = logger;
            this.economicEventRepository = economicEventRepository;
        }

        public object Get(NewsEventRequest request)
        {
            var newsEventsQuery = this.economicEventRepository.GetAll(
                newsEvent => request.Symbols.Contains(newsEvent.Currency)
                             && request.NewsImpacts.Contains(newsEvent.Impact)
                             && request.FromDateTime.Compare(newsEvent.Time) <= 0
                             && request.ToDateTime.Compare(newsEvent.Time) >= 0);

            this.logger.LogResult(newsEventsQuery);

            var newsEvents = new List<EconomicEvent>();

            if (newsEventsQuery.IsSuccess)
            {
                newsEvents = newsEventsQuery
                    .Value
                    .OrderBy(newsEvent => newsEvent.Time)
                    .ToList();
            }

            var newsEventsFrame = new EconomicNewsEventFrame(newsEvents);

            return newsEvents.IsEmpty()
                       ? new NewsEventResponse(false, "Could not find any news events", newsEventsFrame)
                       : new NewsEventResponse(true, "Success", newsEventsFrame);
        }
    }
}
