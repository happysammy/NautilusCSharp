//--------------------------------------------------------------------------------------------------
// <copyright file="VolatilityBasedStop.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Algorithms.StopLoss
{
    using System;
    using Nautilus.Core.Validation;
    using Nautilus.BlackBox.AlphaModel.Algorithm;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Indicators;

    /// <summary>
    /// The volatility based stop.
    /// </summary>
    public sealed class VolatilityBasedStop : StopLossAlgorithmBase, IStopLossAlgorithm
    {
        private readonly AverageTrueRange averageTrueRange;
        private readonly decimal atrMultiple;

        /// <summary>
        /// Initializes a new instance of the <see cref="VolatilityBasedStop"/> class.
        /// </summary>
        /// <param name="tradeProfile">
        /// The trade Profile.
        /// </param>
        /// <param name="instrument">
        /// The instrument.
        /// </param>
        /// <param name="atrMultiple">
        /// The average true range multiple.
        /// </param>
        public VolatilityBasedStop(
            TradeProfile tradeProfile,
            Instrument instrument,
            decimal atrMultiple)
            : base(new Label(nameof(VolatilityBasedStop)), tradeProfile, instrument)
        {
           Validate.NotNull(tradeProfile, nameof(tradeProfile));
           Validate.NotNull(instrument, nameof(instrument));
           Validate.DecimalNotOutOfRange(atrMultiple, nameof(atrMultiple), 0, int.MaxValue, RangeEndPoints.Exclusive);

            this.averageTrueRange = new AverageTrueRange(this.TradePeriod, this.TickSize);
            this.atrMultiple = atrMultiple;
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="bar">
        /// The bar.
        /// </param>
        public void Update(Bar bar)
        {
           Validate.NotNull(bar, nameof(bar));

            this.averageTrueRange.Update(bar);
        }

        /// <summary>
        /// The calculate buy.
        /// </summary>
        /// <param name="entryPrice">
        /// The entry price.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        public Price CalculateBuy(Price entryPrice)
        {
           Validate.NotNull(entryPrice, nameof(entryPrice));

            var stopLossPrice = Price.Create(
                Math.Round(
                entryPrice.Value
                - (this.averageTrueRange.Value * this.atrMultiple)
                - this.TickSize,
                this.DecimalPlaces),
                this.TickSize);

            return this.CalculateFinalStopLossBuy(entryPrice, stopLossPrice);
        }

        /// <summary>
        /// The calculate sell.
        /// </summary>
        /// <param name="entryPrice">
        /// The entry price.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        public Price CalculateSell(Price entryPrice)
        {
           Validate.NotNull(entryPrice, nameof(entryPrice));

            var stopLossPrice = Price.Create(
                Math.Round(
                entryPrice.Value
                + (this.averageTrueRange.Value * this.atrMultiple)
                + this.AverageSpread
                + this.TickSize,
                this.DecimalPlaces),
                this.TickSize);

            return this.CalculateFinalStopLossSell(entryPrice, stopLossPrice);
        }
    }
}