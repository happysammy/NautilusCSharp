//--------------------------------------------------------------
// <copyright file="MarketDataRequest.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

using NautilusDB.Service.Responses;
using ServiceStack;

namespace NautilusDB.Service.Requests
{
    [Api("Market Data Request")]
    public class MarketDataRequest : IReturn<MarketDataResponse>
    {
        [ApiMember(Name = "Symbol", Description = "Ticker (no /)", IsRequired = true)]
        public string Symbol { get; private set; }

        [ApiMember(Name = "Exchange", Description = "Dukascopy", IsRequired = true)]
        public string Exchange { get; private set; }

        [ApiMember(Name = "Bar Quote Type", Description = "Bid, Ask, Mid", IsRequired = true)]
        public string BarQuoteType { get; private set; }

        [ApiMember(Name = "Bar Resolution", Description = "Second, Minute, Hour, Day", IsRequired = true)]
        public string BarResolution { get; private set; }

        [ApiMember(Name = "Ber Period", Description = "The period of the resolution (> 0)", IsRequired = true)]
        public int BerPeriod { get; private set; }

        [ApiMember(Name = "From DateTime", Description = "yyyy-MM-ddTHH:mm:ss.fff", IsRequired = true)]
        public string FromDateTime { get; private set; }

        [ApiMember(Name = "To DateTime", Description = "yyyy-MM-ddTHH:mm:ss.fff", IsRequired = true)]
        public string ToDateTime { get; private set; }
    }
}
