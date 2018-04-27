// -------------------------------------------------------------------------------------------------
// <copyright file="KeltnerChannelRatchetTrail.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Algorithms.TrailingStop
{
    using System;
    using NautechSystems.CSharp;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.AlphaModel.Algorithm;
    using Nautilus.BlackBox.AlphaModel.Signal;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Indicators;

    /// <summary>
    /// The <see cref="KeltnerChannel"/> ratchet trail.
    /// </summary>
    public sealed class KeltnerChannelRatchetTrail : TrailingStopAlgorithmBase, ITrailingStopAlgorithm
    {
        private readonly KeltnerChannel keltnerChannel;
        private readonly AverageTrueRange averageTrueRange;
        private readonly TobyCrabelStretch tobyCrabelStretch;
        private readonly int barsBackInsideKeltner;
        private readonly int barsBackTouchingKeltner;
        private readonly int barsBackOutsideKeltner;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeltnerChannelRatchetTrail"/> class.
        /// </summary>
        /// <param name="tradeProfile">
        /// The trade profile.
        /// </param>
        /// <param name="instrument">
        /// The instrument.
        /// </param>
        /// <param name="keltnerMultiple">
        /// The <see cref="KeltnerChannel"/> multiple.
        /// </param>
        /// <param name="barsBackInsideKeltner">
        /// The bars back inside <see cref="KeltnerChannel"/>.
        /// </param>
        /// <param name="barsBackTouchingKeltner">
        /// The bars back touching <see cref="KeltnerChannel"/>.
        /// </param>
        /// <param name="barsBackOutsideKeltner">
        /// The bars back outside <see cref="KeltnerChannel"/>.
        /// </param>
        /// <param name="forUnit">
        /// The for unit.
        /// </param>
        public KeltnerChannelRatchetTrail(
            TradeProfile tradeProfile,
            Instrument instrument,
            decimal keltnerMultiple,
            int barsBackInsideKeltner,
            int barsBackTouchingKeltner,
            int barsBackOutsideKeltner,
            int forUnit)
            : base(new Label(nameof(KeltnerChannelRatchetTrail)), tradeProfile, instrument, forUnit)
        {
            Validate.NotNull(tradeProfile, nameof(tradeProfile));
            Validate.NotNull(instrument, nameof(instrument));
            Validate.DecimalNotOutOfRange(keltnerMultiple, nameof(keltnerMultiple), decimal.Zero, decimal.MaxValue, RangeEndPoints.Exclusive);
            Validate.Int32NotOutOfRange(barsBackInsideKeltner, nameof(barsBackInsideKeltner), 0, int.MaxValue, RangeEndPoints.Exclusive);
            Validate.Int32NotOutOfRange(barsBackTouchingKeltner, nameof(barsBackTouchingKeltner), 0, int.MaxValue, RangeEndPoints.Exclusive);
            Validate.Int32NotOutOfRange(barsBackOutsideKeltner, nameof(barsBackOutsideKeltner), 0, int.MaxValue, RangeEndPoints.Exclusive);
            Validate.Int32NotOutOfRange(forUnit, nameof(forUnit), 0, int.MaxValue);

            this.averageTrueRange = new AverageTrueRange(this.TradePeriod, this.TickSize);
            this.keltnerChannel = new KeltnerChannel(this.TradePeriod, keltnerMultiple, this.TickSize);
            this.tobyCrabelStretch = new TobyCrabelStretch(this.TradePeriod);

            this.barsBackInsideKeltner = barsBackInsideKeltner;
            this.barsBackTouchingKeltner = barsBackTouchingKeltner;
            this.barsBackOutsideKeltner = barsBackOutsideKeltner;
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

            this.keltnerChannel.Update(bar);
            this.averageTrueRange.Update(bar);
        }

        /// <summary>
        /// The process signal long.
        /// </summary>
        /// <returns>
        /// The <see cref="TrailingStopResponse"/>.
        /// </returns>
        public Option<ITrailingStopResponse> CalculateLong()
        {
            var barsBack = this.barsBackInsideKeltner;

            if ((this.High + this.AverageSpread) >= this.keltnerChannel.Upper)
            {
                barsBack = this.barsBackTouchingKeltner;
            }

            if ((this.Low + this.AverageSpread) > this.keltnerChannel.Upper)
            {
                barsBack = this.barsBackOutsideKeltner;
            }

            var buffer = this.GetBuffer();

            var newStoploss = Math.Round(
                this.BarStore.GetMinLow(barsBack, 0)
                - buffer
                - this.TickSize,
                this.DecimalPlaces);

            return this.SignalResponseLong(true, Price.Create(newStoploss, this.TickSize));
        }

        /// <summary>
        /// The process signal short.
        /// </summary>
        /// <returns>
        /// An <see cref="ITrailingStopResponse"/>.
        /// </returns>
        public Option<ITrailingStopResponse> CalculateShort()
        {
            var barsBack = this.barsBackInsideKeltner;

            if (this.Low <= this.keltnerChannel.Lower)
            {
                barsBack = this.barsBackTouchingKeltner;
            }

            if (this.High < this.keltnerChannel.Lower)
            {
                barsBack = this.barsBackOutsideKeltner;
            }

            var buffer = this.GetBuffer();

            var newStoploss = Math.Round(
                this.BarStore.GetMaxHigh(barsBack, 0)
                + buffer
                + this.AverageSpread
                + this.TickSize,
                this.DecimalPlaces);

            return this.SignalResponseShort(true, Price.Create(newStoploss, this.TickSize));
        }

        private decimal GetBuffer()
        {
            return this.tobyCrabelStretch.LargePercent(this.BarStore).PercentOf(this.averageTrueRange.Value);
        }
    }
}