//--------------------------------------------------------------------------------------------------
// <copyright file="NewsEventRequest.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusDB.Service.Requests
{
    using System.Collections.Generic;
    using NautilusDB.Service.Responses;
    using NodaTime;
    using ServiceStack;
    using Nautilus.DomainModel.Enums;

    public class EconomicEventRequest : IReturn<EconomicEventResponse>
    {
        public IList<CurrencyCode> Symbols { get; private set; }

        public IList<NewsImpact> NewsImpacts { get; private set; }

        public ZonedDateTime FromDateTime { get; private set; }

        public ZonedDateTime ToDateTime { get; private set; }
    }
}
