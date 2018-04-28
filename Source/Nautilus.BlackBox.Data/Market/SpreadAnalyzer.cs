//--------------------------------------------------------------
// <copyright file="SpreadAnalyzer.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.BlackBox.Data.Market
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using NautechSystems.CSharp.Extensions;
    using NautechSystems.CSharp.Validation;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The sealed <see cref="SpreadAnalyzer"/> class.
    /// </summary>
    public sealed class SpreadAnalyzer
    {
        private readonly decimal tickSize;
        private readonly int decimalPlaces;
        private readonly IList<decimal> thisBarsSpreads = new List<decimal>();
        private readonly IDictionary<ZonedDateTime, decimal> negativeSpreads = new Dictionary<ZonedDateTime, decimal>();
        private readonly IDictionary<ZonedDateTime, decimal> totalAverageSpreads = new Dictionary<ZonedDateTime, decimal>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SpreadAnalyzer"/> class.
        /// </summary>
        /// <param name="tickSize">The tick size.</param>
        /// <exception cref="ValidationException">Throws if the tick size is zero or negative.</exception>
        public SpreadAnalyzer(decimal tickSize)
        {
            Validate.DecimalNotOutOfRange(tickSize, nameof(tickSize), decimal.Zero, decimal.MaxValue, RangeEndPoints.Exclusive);

            this.tickSize = tickSize;
            this.decimalPlaces = tickSize.GetDecimalPlaces();

            this.AverageSpread = tickSize;
            this.MaxSpread = Tuple.Create(default(ZonedDateTime), decimal.MinValue);
            this.MinSpread = Tuple.Create(default(ZonedDateTime), decimal.MaxValue);
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
        public decimal CurrentSpread => Math.Round(this.CurrentAsk - this.CurrentBid, this.decimalPlaces);

        /// <summary>
        /// Gets the spread analyzers average spread.
        /// </summary>
        public decimal AverageSpread { get; private set; }

        /// <summary>
        /// Gets the spread analyzers maximum spread.
        /// </summary>
        public Tuple<ZonedDateTime, decimal> MaxSpread { get; private set; }

        /// <summary>
        /// Gets the spread analyzers minimum spread.
        /// </summary>
        public Tuple<ZonedDateTime, decimal> MinSpread { get; private set; }

        /// <summary>
        /// Gets the spread analyzers negative spreads.
        /// </summary>
        public IReadOnlyDictionary<ZonedDateTime, decimal> NegativeSpreads => this.negativeSpreads.ToImmutableDictionary();

        /// <summary>
        /// Gets the spread analyzers negative spreads.
        /// </summary>
        public IReadOnlyDictionary<ZonedDateTime, decimal> TotalAverageSpreads => this.totalAverageSpreads.ToImmutableDictionary();

        /// <summary>
        /// Updates the spread analyzer with the given quote.
        /// </summary>
        /// <param name="quote">The quote.</param>
        /// <exception cref="ValidationException">Throws if the quote is null.</exception>
        public void OnQuote(Tick quote)
        {
            Validate.NotNull(quote, nameof(quote));

            this.CurrentBid = quote.Bid;
            this.CurrentAsk = quote.Ask;

            this.thisBarsSpreads.Add(this.CurrentSpread);

            if (this.CurrentSpread > this.MaxSpread.Item2)
            {
                this.MaxSpread = Tuple.Create(quote.Timestamp, this.CurrentSpread);
            }

            if (this.CurrentSpread < this.MinSpread.Item2)
            {
                this.MinSpread = Tuple.Create(quote.Timestamp, this.CurrentSpread);
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
        /// <exception cref="ValidationException">Throws if the timestamp is the default value.</exception>
        public void OnBarUpdate(ZonedDateTime timestamp)
        {
            Validate.NotDefault(timestamp, nameof(timestamp));

            this.AverageSpread = this.CalculateAverageSpread();
            this.totalAverageSpreads.Add(timestamp, this.AverageSpread);
            this.thisBarsSpreads.Clear();
        }

        private decimal CalculateAverageSpread()
        {
            return Math.Max(Math.Round(this.thisBarsSpreads.Sum() / Math.Max(this.thisBarsSpreads.Count, 1), this.decimalPlaces), this.tickSize);
        }
    }
}
