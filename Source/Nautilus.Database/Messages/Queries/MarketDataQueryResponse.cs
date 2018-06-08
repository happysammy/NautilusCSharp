// -------------------------------------------------------------------------------------------------
// <copyright file="MarketDataQueryResponse.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Messages.Queries
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.Data.Types;
    using NodaTime;

    [Immutable]
    public sealed class MarketDataQueryResponse : QueryResponseMessage
    {
        public MarketDataQueryResponse(
            Option<BarDataFrame> marketData,
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
        public Option<BarDataFrame> MarketData { get; }

        /// <summary>
        /// Gets a string representation of the <see cref="MarketDataQueryResponse"/> message.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{nameof(MarketDataQueryResponse)}-{this.Id}";
    }
}
