//--------------------------------------------------------------------------------------------------
// <copyright file="BarStretchStop.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Algorithms.EntryStop
{
    using System;
    using Nautilus.Core.Validation;
    using Nautilus.BlackBox.AlphaModel.Algorithm;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Indicators;

    /// <summary>
    /// The entry stop calculator.
    /// </summary>
    public sealed class BarStretchStop : AlgorithmBase, IEntryStopAlgorithm
    {
        private readonly AverageTrueRange averageTrueRange;
        private readonly TobyCrabelStretch tobyCrabelStretch;

        /// <summary>
        /// Initializes a new instance of the <see cref="BarStretchStop"/> class.
        /// </summary>
        /// <param name="tradeProfile">
        /// The trade Profile.
        /// </param>
        /// <param name="instrument">
        /// The instrument.
        /// </param>
        public BarStretchStop(TradeProfile tradeProfile, Instrument instrument)
            : base(new Label(nameof(BarStretchStop)), tradeProfile, instrument)
        {
           Validate.NotNull(tradeProfile, nameof(tradeProfile));
           Validate.NotNull(instrument, nameof(instrument));

            this.averageTrueRange = new AverageTrueRange(this.TradePeriod, this.TickSize);
            this.tobyCrabelStretch = new TobyCrabelStretch(this.TradePeriod);
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
        /// <returns>
        /// The <see cref="Price"/>.
        /// </returns>
        public Price CalculateBuy()
        {
            var buyStretchEntry =
                Math.Max(this.Open.Value, this.Close.Value)
                + this.CalculateBuffer()
                + this.AverageSpread
                + this.TickSize;

            var buyCandleExtreme = this.High + this.AverageSpread + this.TickSize;
            var buyEntryPrice = Math.Max(buyCandleExtreme, buyStretchEntry);

            Debug.DecimalNotOutOfRange(buyEntryPrice, nameof(buyEntryPrice), 0, int.MaxValue, RangeEndPoints.Exclusive);

            return Price.Create(Math.Round(buyEntryPrice, this.DecimalPlaces), this.TickSize);
        }

        /// <summary>
        /// The calculate sell.
        /// </summary>
        /// <returns>
        /// The <see cref="Price"/>.
        /// </returns>
        public Price CalculateSell()
        {
            var sellStretchEntry =
                Math.Min(this.Open.Value, this.Close.Value)
                - this.CalculateBuffer()
                - this.TickSize;

            var sellCandleExtreme = this.Low - this.TickSize;
            var sellEntryPrice = Math.Min(sellCandleExtreme, sellStretchEntry);

            Debug.DecimalNotOutOfRange(sellEntryPrice, nameof(sellEntryPrice), 0, int.MaxValue, RangeEndPoints.Exclusive);

            return Price.Create(Math.Round(sellEntryPrice, this.DecimalPlaces), this.TickSize);
        }

        private decimal CalculateBuffer()
        {
            var stretch = this.tobyCrabelStretch.SmallPercent(this.BarStore);
            var bufferBarRange = this.BarStore.GetBarRange(0);

            var buffer = stretch.PercentOf(Math.Max(bufferBarRange, this.averageTrueRange.Value));

            Debug.DecimalNotOutOfRange(buffer, nameof(buffer), 0, int.MaxValue);

            return buffer;
        }
    }
}