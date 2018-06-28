//--------------------------------------------------------------------------------------------------
// <copyright file="SpreadAnalyzer.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Aggregators
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Provides spread analysis for ticks.
    /// </summary>
    public sealed class SpreadAnalyzer
    {
        private readonly IList<decimal> thisBarsSpreads = new List<decimal>();
        private readonly IList<(ZonedDateTime, decimal)> negativeSpreads = new List<(ZonedDateTime, decimal)>();
        private readonly IList<(ZonedDateTime, decimal)> totalAverageSpreads = new List<(ZonedDateTime, decimal)>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SpreadAnalyzer"/> class.
        /// </summary>
        /// <param name="tickSize">The tick size.</param>
        /// <exception cref="ValidationException">Throws if the tick size is zero or negative.</exception>
        public SpreadAnalyzer(decimal tickSize)
        {
            Validate.DecimalNotOutOfRange(tickSize, nameof(tickSize), decimal.Zero, decimal.MaxValue, RangeEndPoints.Exclusive);

            this.AverageSpread = tickSize;
            this.MaxSpread = (default, decimal.MinValue);
            this.MinSpread = (default, decimal.MaxValue);
        }

        /// <summary>
        /// Gets the spread analyzers current bid.
        /// </summary>
        public Price CurrentBid { get; private set; }

        /// <summary>
        /// Gets the spread analyzers current ask.
        /// </summary>
        public Price CurrentAsk { get; private set; }

        /// <summary>
        /// Gets the spread analyzers current spread.
        /// </summary>
        public decimal CurrentSpread => this.CurrentAsk - this.CurrentBid;

        /// <summary>
        /// Gets the spread analyzers average spread.
        /// </summary>
        public decimal AverageSpread { get; private set; }

        /// <summary>
        /// Gets the spread analyzers maximum spread.
        /// </summary>
        public (ZonedDateTime Timestamp, decimal Spread) MaxSpread { get; private set; }

        /// <summary>
        /// Gets the spread analyzers minimum spread.
        /// </summary>
        public (ZonedDateTime Timestamp, decimal Spread) MinSpread { get; private set; }

        /// <summary>
        /// Gets the spread analyzers negative spreads.
        /// </summary>
        public IReadOnlyList<(ZonedDateTime, decimal)> NegativeSpreads => this.negativeSpreads.ToImmutableList();

        /// <summary>
        /// Gets the spread analyzers negative spreads.
        /// </summary>
        public IReadOnlyList<(ZonedDateTime, decimal)> TotalAverageSpreads => this.totalAverageSpreads.ToImmutableList();

        /// <summary>
        /// Updates the spread analyzer with the given tick.
        /// </summary>
        /// <param name="tick">The quote.</param>
        public void Update(Tick tick)
        {
            Debug.NotNull(tick, nameof(tick));

            this.CurrentBid = tick.Bid;
            this.CurrentAsk = tick.Ask;

            var spread = this.CurrentSpread;
            this.thisBarsSpreads.Add(spread);

            if (spread < decimal.Zero)
            {
                this.negativeSpreads.Add((tick.Timestamp, spread));
            }

            if (spread > this.MaxSpread.Spread)
            {
                this.MaxSpread = (tick.Timestamp, spread);
            }

            if (spread < this.MinSpread.Spread)
            {
                this.MinSpread = (tick.Timestamp, spread);
            }

            if (this.totalAverageSpreads.Count == 0)
            {
                this.AverageSpread = this.CalculateAverageSpread();
            }
        }

        /// <summary>
        /// Analyzes the spread data on the close of the bar with the given timestamp.
        /// </summary>
        /// <param name="timestamp">The timestamp.</param>
        public void OnBarUpdate(ZonedDateTime timestamp)
        {
            Validate.NotDefault(timestamp, nameof(timestamp));

            this.AverageSpread = Math.Max(this.CalculateAverageSpread(), decimal.Zero);
            this.totalAverageSpreads.Add((timestamp, this.AverageSpread));
            this.thisBarsSpreads.Clear();
        }

        private decimal CalculateAverageSpread()
        {
            if (this.CurrentBid is null)
            {
                return 0;
            }

            return Math.Round(this.thisBarsSpreads.Sum() / Math.Max(this.thisBarsSpreads.Count, 1),
                this.CurrentBid.Decimals);
        }
    }
}
