//--------------------------------------------------------------
// <copyright file="MarketDataProvider.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.BlackBox.AlphaModel.Strategy
{
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The sealed <see cref="MarketDataProvider"/> class. Provides the latest quote and average
    /// spread to algorithms.
    /// </summary>
    public sealed class MarketDataProvider : IMarketDataProvider
    {
        private readonly Symbol symbol;

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketDataProvider"/> class.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <exception cref="ValidationException">Throws if the symbol is null.</exception>
        public MarketDataProvider(Symbol symbol)
        {
            Validate.NotNull(symbol, nameof(symbol));

            this.symbol = symbol;
        }

        /// <summary>
        /// Gets the last quote.
        /// </summary>
        public Tick LastQuote { get; private set; }

        /// <summary>
        /// Gets the average spread.
        /// </summary>
        public decimal AverageSpread { get; private set; }

        /// <summary>
        /// Updates the market data provider with the given quote and average spread.
        /// </summary>
        /// <param name="lastQuote">The last quote.</param>
        /// <param name="averageSpread">The average spread.</param>
        /// <exception cref="ValidationException">Throws if the last quote is null, or if the average
        /// spread is negative, or if the quote symbol does not match.</exception>
        public void Update(Tick lastQuote, decimal averageSpread)
        {
            Validate.NotNull(lastQuote, nameof(lastQuote));
            Validate.NotNull(averageSpread, nameof(averageSpread));
            Validate.EqualTo(this.symbol, nameof(lastQuote.Symbol), lastQuote.Symbol);

            this.LastQuote = lastQuote;
            this.AverageSpread = averageSpread;
        }
    }
}