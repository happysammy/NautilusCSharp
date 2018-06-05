//--------------------------------------------------------------------------------------------------
// <copyright file="AlgorithmBase.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.AlphaModel.Algorithm
{
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Validation;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The abstract <see cref="AlgorithmBase"/> class. The base class for all algorithms.
    /// </summary>
    public abstract class AlgorithmBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlgorithmBase"/> class.
        /// </summary>
        /// <param name="algorithmLabel">The algorithm label.</param>
        /// <param name="tradeProfile">The algorithm trade profile.</param>
        /// <param name="instrument">The algorithm instrument.</param>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        protected AlgorithmBase(
            Label algorithmLabel,
            TradeProfile tradeProfile,
            Instrument instrument)
        {
            Validate.NotNull(algorithmLabel, nameof(algorithmLabel));
            Validate.NotNull(tradeProfile, nameof(tradeProfile));
            Validate.NotNull(instrument, nameof(instrument));

            this.AlgorithmLabel = algorithmLabel;
            this.TradeProfile = tradeProfile;
            this.Instrument = instrument;
            this.TickSize = instrument.TickSize;
            this.DecimalPlaces = this.TickSize.GetDecimalPlaces();
        }

        /// <summary>
        /// Gets a value indicating whether the algorithm is initialized (wired up to data).
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Gets the algorithms label.
        /// </summary>
        protected Label AlgorithmLabel { get; }

        /// <summary>
        /// Gets the algorithms trade period.
        /// </summary>
        protected TradeProfile TradeProfile { get; }

        /// <summary>
        /// Gets the algorithms instrument.
        /// </summary>
        protected Instrument Instrument { get; }

        /// <summary>
        /// Gets the algorithms bar store.
        /// </summary>
        protected IBarStore BarStore { get; private set; }

        /// <summary>
        /// Gets the algorithms market data provider.
        /// </summary>
        protected IMarketDataProvider MarketDataProvider { get; private set; }

        /// <summary>
        /// Returns the algorithms trade period.
        /// </summary>
        protected int TradePeriod => this.TradeProfile.TradePeriod;

        /// <summary>
        /// Gets the algorithms tick size.
        /// </summary>
        protected decimal TickSize { get; }

        /// <summary>
        /// Gets the algorithms price decimal places.
        /// </summary>
        protected int DecimalPlaces { get; }

        /// <summary>
        /// Returns the last open price.
        /// </summary>
        protected Price Open => this.BarStore.Open;

        /// <summary>
        /// Returns the last high price.
        /// </summary>
        protected Price High => this.BarStore.High;

        /// <summary>
        /// Returns the last low price.
        /// </summary>
        protected Price Low => this.BarStore.Low;

        /// <summary>
        /// Returns the last close price.
        /// </summary>
        protected Price Close => this.BarStore.Close;

        /// <summary>
        /// Returns the average spread.
        /// </summary>
        protected decimal AverageSpread => this.MarketDataProvider.AverageSpread;

        /// <summary>
        /// Returns the best entry stop for buy orders.
        /// </summary>
        protected Price BestEntryStopBuy => this.CalculateBestEntryStopBuy();

        /// <summary>
        /// Returns the best entry stop sell sell orders.
        /// </summary>
        protected Price BestEntryStopSell => this.CalculateBestEntryStopSell();

        /// <summary>
        /// Returns the best stop price for long positions.
        /// </summary>
        protected Price BestStopLong => this.CalculateBestEntryStopSell();

        /// <summary>
        /// Returns the best stop price for short positions.
        /// </summary>
        protected Price BestStopShort => this.CalculateBestEntryStopBuy();

        /// <summary>
        /// Initializes the algorithm with the given <see cref="IBarStore"/> and
        /// <see cref="IMarketDataProvider"/>.
        /// </summary>
        /// <param name="barStore">The bar store.</param>
        /// <param name="marketDataProvider">The market data provider.</param>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public void Initialize(
            IBarStore barStore,
            IMarketDataProvider marketDataProvider)
        {
            Validate.NotNull(barStore, nameof(barStore));
            Validate.NotNull(marketDataProvider, nameof(marketDataProvider));

            this.BarStore = barStore;
            this.MarketDataProvider = marketDataProvider;
            this.IsInitialized = true;
        }

        /// <summary>
        /// Returns the open <see cref="Price"/> at the given bar index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>A <see cref="Price"/>.</returns>
        protected Price GetOpen(int index) => this.BarStore.GetOpen(index);

        /// <summary>
        /// Returns the high <see cref="Price"/> at the given bar index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>A <see cref="Price"/>.</returns>
        protected Price GetHigh(int index) => this.BarStore.GetHigh(index);

        /// <summary>
        /// Returns the low <see cref="Price"/> at the given bar index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>A <see cref="Price"/>.</returns>
        protected Price GetLow(int index) => this.BarStore.GetLow(index);

        /// <summary>
        /// Returns the close <see cref="Price"/> at the given bar index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>A <see cref="Price"/>.</returns>
        protected Price GetClose(int index) => this.BarStore.GetClose(index);

        /// <summary>
        /// Returns the volume at the given bar index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>A <see cref="int"/>.</returns>
        protected Quantity GetVolume(int index) => this.BarStore.GetVolume(index);

        /// <summary>
        /// Returns the highest high <see cref="Price"/> of the given bar period (with shift).
        /// </summary>
        /// <param name="period">The period.</param>
        /// <param name="shift">The shift.</param>
        /// <returns>A <see cref="Price"/>.</returns>
        protected Price HighestHigh(int period, int shift = 0) => this.BarStore.GetMaxHigh(period, shift);

        /// <summary>
        /// Returns the lowest low <see cref="Price "/> of the given bar period (with shift).
        /// </summary>
        /// <param name="period">The period.</param>
        /// <param name="shift">The shift.</param>
        /// <returns>A <see cref="Price"/>.</returns>
        protected Price LowestLow(int period, int shift = 0) => this.BarStore.GetMinLow(period, shift);

        private Price CalculateBestEntryStopBuy()
        {
            var bestEntryStopBuy =
                this.MarketDataProvider.LastQuote.Ask
                + (this.Instrument.MinStopDistanceEntry * this.TickSize)
                + this.TickSize;

            Debug.DecimalNotOutOfRange(
                bestEntryStopBuy,
                nameof(bestEntryStopBuy),
                decimal.Zero,
                decimal.MaxValue,
                RangeEndPoints.Exclusive);

            return Price.Create(bestEntryStopBuy, this.TickSize);
        }

        private Price CalculateBestEntryStopSell()
        {
            var bestEntryStopSell =
                this.MarketDataProvider.LastQuote.Bid
                - (this.Instrument.MinStopDistanceEntry * this.TickSize)
                - this.TickSize;

            Debug.DecimalNotOutOfRange(
                bestEntryStopSell,
                nameof(bestEntryStopSell),
                decimal.Zero,
                decimal.MaxValue,
                RangeEndPoints.Exclusive);

            return Price.Create(bestEntryStopSell, this.TickSize);
        }
    }
}