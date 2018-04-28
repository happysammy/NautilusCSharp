//--------------------------------------------------------------
// <copyright file="NewsEventService.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

using System.Collections.Generic;
using NautechSystems.CSharp.Validation;
using NautilusDB.Core.Types;
using NautilusDB.Service.Requests;
using NautilusDB.Service.Responses;
using ServiceStack;

namespace NautilusDB.Service
{
    using System.Linq;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Extensions;
    using Nautilus.Database.Core.Interfaces;
    using Nautilus.DomainModel.Entities;

    /// <summary>
    /// The service which processes incoming <see cref="NewsEventRequest"/>(s).
    /// </summary>
    public class NewsEventService : ServiceStack.Service
    {
        private readonly ILoggingAdapter logger;
        private readonly IEconomicNewsEventRepository<EconomicNewsEvent> economicNewsEventRepository;

        public NewsEventService(
            ILoggingAdapter logger,
            IEconomicNewsEventRepository<EconomicNewsEvent> economicNewsEventRepository)
        {
            Validate.NotNull(logger, nameof(logger));
            Validate.NotNull(economicNewsEventRepository, nameof(economicNewsEventRepository));

            this.logger = logger;
            this.economicNewsEventRepository = economicNewsEventRepository;
        }

        public object Get(NewsEventRequest request)
        {
            var newsEventsQuery = this.economicNewsEventRepository.GetAll(
                newsEvent => request.Symbols.Contains(newsEvent.Currency)
                             && request.NewsImpacts.Contains(newsEvent.Impact)
                             && request.FromDateTime.Compare(newsEvent.Time) <= 0
                             && request.ToDateTime.Compare(newsEvent.Time) >= 0);

            this.logger.LogResult(newsEventsQuery);

            var newsEvents = new List<EconomicNewsEvent>();

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
