//--------------------------------------------------------------------------------------------------
// <copyright file="NewsEventService.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusDB.Service
{
    using System.Collections.Generic;
    using Nautilus.Core.Validation;
    using NautilusDB.Service.Requests;
    using NautilusDB.Service.Responses;
    using ServiceStack;
    using System.Linq;
    using Nautilus.Core.Extensions;
    using Nautilus.Common.Interfaces;
    using Nautilus.Database.Interfaces;
    using Nautilus.Database.Types;
    using Nautilus.DomainModel.Entities;

    /// <summary>
    /// The service which processes incoming <see cref="EconomicEventRequest"/>(s).
    /// </summary>
    public class EconomicEventService : Service
    {
        private readonly ILoggingAdapter logger;
        private readonly IRepository<EconomicEvent> economicEventRepository;

        public EconomicEventService(
            ILoggingAdapter logger,
            IRepository<EconomicEvent> economicEventRepository)
        {
            Validate.NotNull(logger, nameof(logger));
            Validate.NotNull(economicEventRepository, nameof(economicEventRepository));

            this.logger = logger;
            this.economicEventRepository = economicEventRepository;
        }

        public object Get(EconomicEventRequest request)
        {
            var newsEventsQuery = this.economicEventRepository.GetAll(
                newsEvent => request.Symbols.Contains(newsEvent.Currency)
                             && request.NewsImpacts.Contains(newsEvent.Impact)
                             && request.FromDateTime.Compare(newsEvent.Time) <= 0
                             && request.ToDateTime.Compare(newsEvent.Time) >= 0);

            var newsEvents = new List<EconomicEvent>();

            if (newsEventsQuery.IsSuccess)
            {
                newsEvents = newsEventsQuery
                    .Value
                    .OrderBy(newsEvent => newsEvent.Time)
                    .ToList();
            }

            var newsEventsFrame = new EconomicEventFrame(newsEvents);

            return newsEvents.IsEmpty()
                       ? new EconomicEventResponse(false, "Could not find any news events", newsEventsFrame)
                       : new EconomicEventResponse(true, "Success", newsEventsFrame);
        }
    }
}
