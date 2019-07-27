//--------------------------------------------------------------------------------------------------
// <copyright file="SpreadAnalyzer.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Aggregation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Provides spread analysis for a markets <see cref="Tick"/>s.
    /// </summary>
    [PerformanceOptimized]
    public sealed class SpreadAnalyzer
    {
        private readonly List<decimal> thisBarsSpreads;
        private readonly List<(ZonedDateTime, decimal)> negativeSpreads;
        private readonly List<(ZonedDateTime, decimal)> averageSpreads;

        // Initialized on first tick.
        private bool isInitialized;
        private int decimalPrecision;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpreadAnalyzer"/> class.
        /// </summary>
        public SpreadAnalyzer()
        {
            this.thisBarsSpreads = new List<decimal>();
            this.negativeSpreads = new List<(ZonedDateTime, decimal)>();
            this.averageSpreads = new List<(ZonedDateTime, decimal)>();
            this.CurrentBid = Price.Create(decimal.One);  // TODO: Refactor this logic
            this.CurrentAsk = Price.Create(decimal.One);  // TODO: Refactor this logic
            this.MaxSpread = ValueTuple.Create(default(ZonedDateTime), decimal.MinValue);
            this.MinSpread = ValueTuple.Create(default(ZonedDateTime), decimal.MaxValue);
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
        public decimal CurrentSpread { get; private set; }

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
        public IReadOnlyList<(ZonedDateTime, decimal)> NegativeSpreads => this.negativeSpreads.ToList().AsReadOnly();

        /// <summary>
        /// Gets the spread analyzers negative spreads.
        /// </summary>
        public IReadOnlyList<(ZonedDateTime, decimal)> TotalAverageSpreads => this.averageSpreads.ToList().AsReadOnly();

        /// <summary>
        /// Updates the spread analyzer with the given tick.
        /// </summary>
        /// <param name="tick">The quote.</param>
        public void Update(Tick tick)
        {
            if (!this.isInitialized)
            {
                this.decimalPrecision = tick.Bid.DecimalPrecision;
                this.isInitialized = true;
            }

            this.CurrentBid = tick.Bid;
            this.CurrentAsk = tick.Ask;
            this.CurrentSpread = tick.Ask - tick.Bid;

            this.thisBarsSpreads.Add(this.CurrentSpread);

            if (this.CurrentSpread < decimal.Zero)
            {
                this.negativeSpreads.Add((tick.Timestamp, this.CurrentSpread));
            }

            if (this.CurrentSpread > this.MaxSpread.Spread)
            {
                this.MaxSpread = (tick.Timestamp, this.CurrentSpread);
            }

            if (this.CurrentSpread < this.MinSpread.Spread)
            {
                this.MinSpread = (tick.Timestamp, this.CurrentSpread);
            }

            if (this.averageSpreads.Count == 0)
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
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.AverageSpread = this.CalculateAverageSpread();
            this.averageSpreads.Add((timestamp, this.AverageSpread));
            this.thisBarsSpreads.Clear();
        }

        private decimal CalculateAverageSpread()
        {
            if (!this.isInitialized)
            {
                return decimal.Zero;
            }

            return Math.Round(
                this.thisBarsSpreads.Sum() / Math.Max(this.thisBarsSpreads.Count, 1),
                this.decimalPrecision);
        }
    }
}
