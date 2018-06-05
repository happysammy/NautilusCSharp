// -------------------------------------------------------------------------------------------------
// <copyright file="MarketDataQueryResponse.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using Nautilus.Core;
using Nautilus.Core.Annotations;
using Nautilus.Core.Validation;
using NodaTime;

namespace Nautilus.Database.Core.Messages.Queries
{
    using Nautilus.Database.Core.Types;

    [Immutable]
    public sealed class MarketDataQueryResponse : QueryResponseMessage
    {
        public MarketDataQueryResponse(
            Option<MarketDataFrame> marketData,
            bool isSuccess,
            string message,
            Guid identifier,
            ZonedDateTime timestamp)
            : base(isSuccess, message, identifier, timestamp)
        {
            Validate.NotNull(marketData, nameof(marketData));
            Validate.NotNull(message, nameof(message));
            Validate.NotDefault(identifier, nameof(identifier));
            Validate.NotDefault(timestamp, nameof(timestamp));

            this.MarketData = marketData;
        }

        /// <summary>
        /// Gets the response messages market data.
        /// </summary>
        public Option<MarketDataFrame> MarketData { get; }

        /// <summary>
        /// Gets a string representation of the <see cref="MarketDataQueryResponse"/> message.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{nameof(MarketDataQueryResponse)}-{this.Id}";
    }
}
