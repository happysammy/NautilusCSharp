//--------------------------------------------------------------
// <copyright file="KeltnerChannel.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.Indicators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NautechSystems.CSharp.Extensions;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Indicators.Base;

    /// <summary>
    /// The keltner channel.
    /// </summary>
    public class KeltnerChannel : Indicator
    {
        private readonly decimal tickSize;
        private readonly int decimals;
        private readonly decimal multiplier;
        private readonly List<decimal> bandUpper;
        private readonly List<decimal> bandMiddle;
        private readonly List<decimal> bandLower;
        private readonly ExponentialMovingAverage ema;
        private readonly AverageTrueRange atr;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeltnerChannel"/> class.
        /// </summary>
        /// <param name="period">
        /// The period.
        /// </param>
        /// <param name="k">
        /// The K multiplier.
        /// </param>
        /// <param name="tickSize">
        /// The tick size.
        /// </param>
        public KeltnerChannel(int period, decimal k, decimal tickSize)
            : base(nameof(KeltnerChannel))
        {
            this.Initialized = false;

            this.Period = period;
            this.multiplier = k;

            this.bandUpper = new List<decimal>();
            this.bandMiddle = new List<decimal>();
            this.bandLower = new List<decimal>();

            this.ema = new ExponentialMovingAverage(this.Period, 0, tickSize);
            this.atr = new AverageTrueRange(this.Period, tickSize);

            this.tickSize = tickSize;
            this.decimals = Math.Max(tickSize.GetDecimalPlaces(), 2);
        }

        /// <summary>
        /// Gets the value of the calculation period for this indicator object.
        /// </summary>
        public int Period { get; }

        /// <summary>
        /// Gets the value of the upper band at last closed bar).
        /// </summary>
        public decimal Upper => this.bandUpper.Last();

        /// <summary>
        /// Gets the value of the middle band at last closed bar).
        /// </summary>
        public decimal Middle => this.bandMiddle.Last();

        /// <summary>
        /// Gets the value of the lower band at last closed bar).
        /// </summary>
        public decimal Lower => this.bandLower.Last();

        /// <summary>
        /// Gets the count of data elements held by the band lists.
        /// </summary>
        public int Count => this.bandMiddle.Count;

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="currentBar">
        /// The current bar.
        /// </param>
        public void Update(Bar currentBar)
        {
            this.ema.Update(currentBar);
            this.atr.Update(currentBar);

            var middleBand = this.ema.Value;
            var upperBand = this.CalculateUpperBand();
            var lowerBand = this.CalculateLowerBand();

            this.bandMiddle.Add(middleBand);
            this.bandUpper.Add(upperBand);
            this.bandLower.Add(lowerBand);

            if (!this.Initialized)
            {
                this.Initialized = true;
            }
        }

        /// <summary>
        /// Clears all cumulative lists of data.
        /// </summary>
        public void Reset()
        {
            this.bandUpper.Clear();
            this.bandMiddle.Clear();
            this.bandLower.Clear();
            this.ema.Reset();
            this.atr.Reset();

            this.Initialized = false;
        }

        /// <summary>
        /// The get upper band.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        public decimal GetUpperBand(int index) => Math.Round(this.bandUpper.GetByReverseIndex(index), this.decimals);

        /// <summary>
        /// The get middle band.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        public decimal GetMiddleBand(int index) => Math.Round(this.bandMiddle.GetByReverseIndex(index), this.decimals);

        /// <summary>
        /// The get lower band.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        public decimal GetLowerBand(int index) => Math.Round(this.bandLower.GetByReverseIndex(index), this.decimals);

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return $"{this.Name}[{this.Count}]: Upper={this.Upper}, Middle={this.Middle}, Lower={this.Lower}";
        }

        /// <summary>
        /// The calculate upper band.
        /// </summary>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        private decimal CalculateUpperBand()
        {
            return this.Initialized ? this.Middle + (this.atr.Value * this.multiplier) : 0m;
        }

        /// <summary>
        /// The calculate lower band.
        /// </summary>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        private decimal CalculateLowerBand()
        {
            return this.Initialized ? this.Middle - (this.atr.Value * this.multiplier) : 0m;
        }
    }
}