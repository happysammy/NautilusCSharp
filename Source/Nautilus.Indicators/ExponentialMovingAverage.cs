// -------------------------------------------------------------------------------------------------
// <copyright file="ExponentialMovingAverage.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Indicators
{
    using System;
    using System.Collections.Generic;
    using NautechSystems.CSharp;
    using NautechSystems.CSharp.Extensions;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Indicators.Base;
    using NodaTime;

    /// <summary>
    /// The exponential moving average.
    /// </summary>
    public class ExponentialMovingAverage : Indicator
    {
        private readonly int decimals;
        private readonly List<decimal> ema;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExponentialMovingAverage"/> class.
        /// </summary>
        /// <param name="period">
        /// The period.
        /// </param>
        /// <param name="shift">
        /// The shift.
        /// </param>
        /// <param name="tickSize">
        /// The tick size.
        /// </param>
        public ExponentialMovingAverage(int period, int shift, decimal tickSize) : base("EMA")
        {
            this.Initialized = false;
            this.Period = period;
            this.Shift = shift;

            this.ema = new List<decimal>();

            this.decimals = Math.Max(tickSize.GetDecimalPlaces(), 2);
        }

        /// <summary>
        /// Gets the value of the calculation period for this indicator object.
        /// </summary>
        public int Period { get; }

        /// <summary>
        /// Gets the shift of the calculations for this indicator object.
        /// </summary>
        public int Shift { get; }

        /// <summary>
        /// Gets the exponential moving average at the last closed bar.
        /// </summary>
        public decimal Value => this.GetValue(0);

        /// <summary>
        /// Gets the count of data elements held by the indicator object.
        /// </summary>
        public int Count => this.ema.Count;

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="currentBar">
        /// The current bar.
        /// </param>
        public void Update(Bar currentBar)
        {
            if (!this.Initialized)
            {
                this.ema.Add(currentBar.Close.Value);
            }

            var currentEma = Math.Round((currentBar.Close.Value - this.Value) * SmoothingFactor(this.Period) + this.Value, this.decimals);

            this.ema.Add(currentEma);

            this.LastTime = currentBar.Timestamp;

            if (!this.Initialized)
            {
                this.Initialized = true;
            }
        }

        /// <summary>
        /// Clears the cumulative list of exponential moving averages.
        /// </summary>
        public void Reset()
        {
            this.ema.Clear();
            this.LastTime = Option<ZonedDateTime?>.None();
            this.Initialized = false;
        }

        /// <summary>
        /// The get value.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        public decimal GetValue(int index) => this.ema.GetByShiftedReverseIndex(index, this.Shift);

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return $"{this.Name}[{this.Count}]: {this.Value} at {this.LastTime:g}";
        }

        /// <summary>
        /// The smoothing factor.
        /// </summary>
        /// <param name="period">
        /// The period.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        private static decimal SmoothingFactor(int period)
        {
            return 2.0m / (period + 1.0m);
        }
    }
}
