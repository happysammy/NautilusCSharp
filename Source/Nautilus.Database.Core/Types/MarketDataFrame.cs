﻿//--------------------------------------------------------------------------------------------------
// <copyright file="MarketDataFrame.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Core.Types
{
    using Nautilus.DomainModel.ValueObjects;
    using System.Linq;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
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
        /// <param name="symbolBarData">The symbol bar data.</param>
        /// <param name="bars">The bars dictionary.</param>
        /// <exception cref="ValidationException">Throws if the bar specification is the default
        /// value, or if the bars collection is null or empty.</exception>
        public MarketDataFrame(SymbolBarData symbolBarData, Bar[] bars)
        {
            Validate.NotNull(symbolBarData, nameof(symbolBarData));
            Validate.CollectionNotNullOrEmpty(bars, nameof(bars));

            this.SymbolBarData = symbolBarData;
            this.Bars = bars;
        }

        /// <summary>
        /// Gets the market data frames symbol.
        /// </summary>
        public SymbolBarData SymbolBarData { get; }

        /// <summary>
        /// Gets the market data frames bars.
        /// </summary>
        public Bar[] Bars { get; }

        /// <summary>
        /// Gets the market data frames start time.
        /// </summary>
        public ZonedDateTime StartDateTime => this.Bars.First().Timestamp;

        /// <summary>
        /// Gets the market data frames end time.
        /// </summary>
        public ZonedDateTime EndDateTime => this.Bars.Last().Timestamp;
    }
}
