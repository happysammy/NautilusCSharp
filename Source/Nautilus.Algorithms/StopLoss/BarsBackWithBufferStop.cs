// -------------------------------------------------------------------------------------------------
// <copyright file="BarsBackWithBufferStop.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Algorithms.StopLoss
{
    using System;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.AlphaModel.Algorithm;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Indicators;

    /// <summary>
    /// The bars back with buffer stop.
    /// </summary>
    public sealed class BarsBackWithBufferStop : StopLossAlgorithmBase, IStopLossAlgorithm
    {
        private readonly AverageTrueRange averageTrueRange;
        private readonly TobyCrabelStretch tobyCrabelStretch;
        private readonly int barsBack;
        private readonly decimal maxAtr;

        /// <summary>
        /// Initializes a new instance of the <see cref="BarsBackWithBufferStop"/> class.
        /// </summary>
        /// <param name="tradeProfile">
        /// The trade Profile.
        /// </param>
        /// <param name="instrument">
        /// The instrument.
        /// </param>
        /// <param name="barsBack">
        /// The bars back.
        /// </param>
        /// <param name="maxAtr">
        /// The max average true range.
        /// </param>
        public BarsBackWithBufferStop(
            TradeProfile tradeProfile,
            Instrument instrument,
            int barsBack,
            decimal maxAtr)
            : base(new Label(nameof(BarsBackWithBufferStop)), tradeProfile, instrument)
        {
           Validate.NotNull(tradeProfile, nameof(tradeProfile));
           Validate.NotNull(instrument, nameof(instrument));
           Validate.Int32NotOutOfRange(barsBack, nameof(barsBack), 0, int.MaxValue, RangeEndPoints.LowerExclusive);
           Validate.DecimalNotOutOfRange(maxAtr, nameof(maxAtr), 0, int.MaxValue);

            this.averageTrueRange = new AverageTrueRange(this.TradePeriod, this.TickSize);
            this.tobyCrabelStretch = new TobyCrabelStretch(this.TradePeriod);
            this.barsBack = barsBack;
            this.maxAtr = maxAtr;
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

            var stoplossMaxAtr = decimal.MinValue;

            if (this.maxAtr > 0)
            {
                stoplossMaxAtr = Math.Round(
                    entryPrice.Value
                    - (this.averageTrueRange.Value * this.maxAtr)
                    - this.TickSize,
                    this.DecimalPlaces);
            }

            var stretchBuffer = this.tobyCrabelStretch.LargePercent(this.BarStore);
            var largestBarRange = this.BarStore.GetLargestRange(this.barsBack, 0);

            var buffer = stretchBuffer.PercentOf(largestBarRange);

            var stoplossBarsBased =
                this.BarStore.GetMinLow(this.barsBack, 0)
                - buffer
                - this.TickSize;

            var stoplossPrice = Price.Create(
                Math.Round(Math.Max(stoplossMaxAtr, stoplossBarsBased), this.DecimalPlaces),
                this.TickSize);

            return this.CalculateFinalStopLossBuy(entryPrice, stoplossPrice);
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

            var stoplossMaxAtr = decimal.MaxValue;

            if (this.maxAtr > 0)
            {
                stoplossMaxAtr = Math.Round(
                    entryPrice.Value
                    + (this.averageTrueRange.Value * this.maxAtr)
                    + this.AverageSpread
                    + this.TickSize,
                    this.DecimalPlaces);
            }

            var stretchBuffer = this.tobyCrabelStretch.LargePercent(this.BarStore);
            var largestBarRange = this.BarStore.GetLargestRange(this.barsBack, 0);

            var buffer = stretchBuffer.PercentOf(largestBarRange);

            var stoplossBarsBased =
                this.BarStore.GetMaxHigh(this.barsBack, 0)
                + buffer
                + this.AverageSpread
                + this.TickSize;

            var stoplossPrice = Price.Create(
                Math.Round(Math.Min(stoplossMaxAtr, stoplossBarsBased), this.DecimalPlaces),
                this.TickSize);

            return this.CalculateFinalStopLossSell(entryPrice, stoplossPrice);
        }
    }
}