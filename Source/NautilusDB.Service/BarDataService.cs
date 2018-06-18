//--------------------------------------------------------------------------------------------------
// <copyright file="MarketDataService.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusDB.Service
{
    using System;
    using Akka.Actor;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Validation;
    using NautilusDB.Service.Requests;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using ServiceStack;

    /// <summary>
    /// The service which processes incoming <see cref="BarDataRequest"/>(s).
    /// </summary>
    public class BarDataService : Service
    {
        private readonly IZonedClock clock;
        private readonly ILoggingAdapter logger;
        private readonly IActorRef databaseTaskManagerRef;

        public BarDataService(
            IZonedClock clock,
            ILoggingAdapter logger,
            IActorRef databaseTaskManagerRef)
        {
            Validate.NotNull(clock, nameof(clock));
            Validate.NotNull(logger, nameof(logger));
            Validate.NotNull(databaseTaskManagerRef, nameof(databaseTaskManagerRef));

            this.clock = clock;
            this.logger = logger;
            this.databaseTaskManagerRef = databaseTaskManagerRef;
        }

        public object Get(BarDataRequest request)
        {
//            Debug.NotNull(request, nameof(request));
//
//            var requestbarType = new barTypeification(
//                request.BarQuoteType.ToEnum<BarQuoteType>(),
//                request.BarResolution.ToEnum<BarResolution>(),
//                request.BerPeriod);
//
//            var queryMessage = new QueryRequest<SymbolbarType>(
//                new Symbol(request.Symbol, request.Exchange.ToEnum<Exchange>()),
//                requestbarType,
//                request.FromDateTime.ToZonedDateTimeFromIso(),
//                request.ToDateTime.ToZonedDateTimeFromIso(),
//                Guid.NewGuid(),
//                this.clock.TimeNow());
//
//            var barDataQuery = this.databaseTaskManagerRef.Ask<QueryResponse<BarDataFrame>>(queryMessage);
//
//            while (!barDataQuery.IsCompleted)
//            {
//                // Wait
//            }
//
//            return !barDataQuery.IsSuccess()
//                ? new BarDataResponse(true, marketData.Result.Message, marketData.Result.MarketData.Value)
//                : new BarDataResponse(false, marketData.Result.Message, null);
            return null;
        }
    }
}
