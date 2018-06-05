//--------------------------------------------------------------------------------------------------
// <copyright file="MarketDataFrame.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Core.Types
{
    using System.Linq;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using NodaTime;

    /// <summary>
    /// Contains financial market trade bar data.
    /// </summary>
    [Immutable]
    public sealed class MarketDataFrame
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarketDataFrame"/> class.
        /// </summary>
        /// <param name="symbolBarSpec">The symbol bar data.</param>
        /// <param name="barsData">The bars dictionary.</param>
        /// <exception cref="ValidationException">Throws if the bar specification is the default
        /// value, or if the bars collection is null or empty.</exception>
        public MarketDataFrame(SymbolBarSpec symbolBarSpec, BarData[] barsData)
        {
            Validate.NotNull(symbolBarSpec, nameof(symbolBarSpec));
            Validate.CollectionNotNullOrEmpty(barsData, nameof(barsData));

            this.SymbolBarSpec = symbolBarSpec;
            this.BarsData = barsData;
        }

        /// <summary>
        /// Gets the market data frames symbol.
        /// </summary>
        public SymbolBarSpec SymbolBarSpec { get; }

        /// <summary>
        /// Gets the market data frames bars.
        /// </summary>
        public BarData[] BarsData { get; }

        /// <summary>
        /// Gets the market data frames start time.
        /// </summary>
        public ZonedDateTime StartDateTime => this.BarsData.First().Timestamp;

        /// <summary>
        /// Gets the market data frames end time.
        /// </summary>
        public ZonedDateTime EndDateTime => this.BarsData.Last().Timestamp;
    }
}
