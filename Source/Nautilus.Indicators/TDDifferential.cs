//--------------------------------------------------------------------------------------------------
// <copyright file="TDDifferential.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Indicators
{
    using System;
    using Nautilus.Core.Collections;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Indicators.Base;

    /// <summary>
    /// The td differential.
    /// </summary>
    public class TDDifferential : Indicator
    {
        /// <summary>
        /// The tick size.
        /// </summary>
        private readonly decimal tickSize;

        /// <summary>
        /// The bars.
        /// </summary>
        private readonly RollingList<Bar> bars;

        /// <summary>
        /// The average volume.
        /// </summary>
        private readonly AverageVolume averageVolume;

        /// <summary>
        /// The average volume filter.
        /// </summary>
        private readonly bool averageVolumeFilter;

        /// <summary>
        /// Initializes a new instance of the <see cref="TDDifferential"/> class.
        /// </summary>
        /// <param name="averageVolumeFilter">
        /// The average volume filter.
        /// </param>
        /// <param name="averageVolumePeriod">
        /// The average volume period.
        /// </param>
        /// <param name="tickSize">
        /// The tick size.
        /// </param>
        public TDDifferential(bool averageVolumeFilter, int averageVolumePeriod, decimal tickSize) : base("TD Differential")
        {
            this.averageVolumeFilter = averageVolumeFilter;
            this.tickSize = tickSize;

            this.Initialized = false;

            this.bars = new RollingList<Bar>(5);

            this.averageVolume = new AverageVolume(averageVolumePeriod);
        }

        /// <summary>
        /// The buy pressure.
        /// </summary>
        public decimal BuyPressure => Math.Round((this.bars[0].Close - this.bars[0].Low) / this.tickSize) * this.bars[0].Volume.Value;

        /// <summary>
        /// The sell pressure.
        /// </summary>
        public decimal SellPressure => Math.Round((this.bars[0].High - this.bars[0].Close) / this.tickSize) * this.bars[0].Volume.Value;

        /// <summary>
        /// Gets a value indicating whether is differential buy.
        /// </summary>
        public bool IsDifferentialBuy { get; private set; }

        /// <summary>
        /// Gets a value indicating whether is differential sell.
        /// </summary>
        public bool IsDifferentialSell { get; private set; }

        /// <summary>
        /// Gets a value indicating whether is reverse differential buy.
        /// </summary>
        public bool IsReverseDifferentialBuy { get; private set; }

        /// <summary>
        /// Gets a value indicating whether is reverse differential sell.
        /// </summary>
        public bool IsReverseDifferentialSell { get; private set; }

        /// <summary>
        /// Gets a value indicating whether is anti differential buy.
        /// </summary>
        public bool IsAntiDifferentialBuy { get; private set; }

        /// <summary>
        /// Gets a value indicating whether is anti differential sell.
        /// </summary>
        public bool IsAntiDifferentialSell { get; private set; }

        /// <summary>
        /// Gets the bars count.
        /// </summary>
        public int BarsCount { get; private set; }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="currentBar">
        /// The current bar.
        /// </param>
        public void Update(Bar currentBar)
        {
            this.bars.Add(currentBar);
            this.BarsCount++;

            if (!this.Initialized)
            {
                if (this.bars.Count < 5)
                {
                    return;
                }

                this.Initialized = true;
            }

            CalculateSignals();
        }

        /// <summary>
        /// The calculate signals.
        /// </summary>
        private static void CalculateSignals()
        {
            //// TODO
            //this.IsDifferentialBuy = false;
            //this.IsDifferentialSell = false;
            //this.IsReverseDifferentialBuy = false;
            //this.IsReverseDifferentialSell = false;
            //this.IsAntiDifferentialBuy = false;
            //this.IsAntiDifferentialSell = false;

            //if (this.bars[2].Close > this.bars[1].Close && this.bars[1].Close > this.bars[0].Close && this.BuyPressure > this.SellPressure &&
            //    (!this.averageVolumeFilter || (this.averageVolumeFilter && this.AverageVolumeFilter())))
            //{
            //    this.IsDifferentialBuy = true;
            //}

            //if (this.bars[2].Close < this.bars[1].Close && this.bars[1].Close < this.bars[0].Close && this.BuyPressure < this.SellPressure &&
            //    (!this.averageVolumeFilter || (this.averageVolumeFilter && this.AverageVolumeFilter())))
            //{
            //    this.IsDifferentialSell = true;
            //}

            //if (this.bars[2].Close < this.bars[1].Close && this.bars[1].Close < this.bars[0].Close && this.BuyPressure > this.SellPressure &&
            //    (!this.averageVolumeFilter || (this.averageVolumeFilter && this.AverageVolumeFilter())))
            //{
            //    this.IsReverseDifferentialBuy = true;
            //}

            //if (this.bars[2].Close > this.bars[1].Close && this.bars[1].Close > this.bars[0].Close && this.BuyPressure < this.SellPressure &&
            //    (!this.averageVolumeFilter || (this.averageVolumeFilter && this.AverageVolumeFilter())))
            //{
            //    this.IsReverseDifferentialSell = true;
            //}

            //if (this.bars[4].Close > this.bars[3].Close && this.bars[3].Close > this.bars[2].Close && this.bars[2].Close < this.bars[1].Close && this.bars[1].Close > this.bars[0].Close && this.BuyPressure > this.SellPressure &&
            //    (!this.averageVolumeFilter || (this.averageVolumeFilter && this.AverageVolumeFilter())))
            //{
            //    this.IsAntiDifferentialBuy = true;
            //}

            //if (this.bars[4].Close < this.bars[3].Close && this.bars[3].Close < this.bars[2].Close && this.bars[2].Close > this.bars[1].Close && this.bars[1].Close < this.bars[0].Close && this.BuyPressure < this.SellPressure &&
            //    (!this.averageVolumeFilter || (this.averageVolumeFilter && this.AverageVolumeFilter())))
            //{
            //    this.IsAntiDifferentialSell = true;
            //}
        }

        /// <summary>
        /// The average volume filter.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool AverageVolumeFilter()
        {
            return this.bars[0].Volume.Value >= this.averageVolume.Value;
        }
    }
}