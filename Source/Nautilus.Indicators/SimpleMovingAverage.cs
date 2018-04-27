// -------------------------------------------------------------------------------------------------
// <copyright file="SimpleMovingAverage.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Indicators
{
    using System;
    using System.Collections.Generic;
    using NautechSystems.CSharp.Collections;
    using NautechSystems.CSharp.Extensions;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Indicators.Base;

    /// <summary>
    /// The simple moving average.
    /// </summary>
    public class SimpleMovingAverage : Indicator
    {
        private readonly int decimals;
        private readonly RollingList<decimal> priceValues;
        private readonly List<decimal> sma = new List<decimal>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleMovingAverage"/> class.
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
        public SimpleMovingAverage(int period, int shift, decimal tickSize) : base("SMA")
        {
            this.Initialized = false;
            this.Period = period;
            this.Shift = shift;

            this.priceValues = new RollingList<decimal>(this.Period);

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
        /// Gets the simple moving average at the last closed bar.
        /// </summary>
        public decimal Value => this.sma.GetByReverseIndex(0);

        /// <summary>
        /// Gets the count of data elements held by the indicator object.
        /// </summary>
        public int Count => this.sma.Count;

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="currentBar">
        /// The current bar.
        /// </param>
        public void Update(Bar currentBar)
        {
            this.priceValues.Add(currentBar.Close.Value);

            if (!this.Initialized)
            {
                this.sma.Add(currentBar.Close.Value);
            }

            var currentSma = this.CalculateAverage(); // TODO change algorithm to more efficient version.

            this.sma.Add(currentSma);

            this.LastTime = currentBar.Timestamp;

            if (!this.Initialized)
            {
                this.Initialized = true;
            }
        }

        /// <summary>
        /// Clears the list of cumulative simple moving averages.
        /// </summary>
        public void Reset()
        {
            this.sma.Clear();

            this.Initialized = false;
        }

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
        /// The calculate average.
        /// </summary>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        private decimal CalculateAverage()
        {
            if (this.priceValues.Count <= this.Shift)
            {
                return 0;
            }

            decimal sum = 0;

            var divisor = Math.Min(this.priceValues.Count - this.Shift, this.Period);

            for (int i = 0; i < divisor; i++)
            {
                sum += this.priceValues[i];
            }

            return Math.Round(sum / divisor, this.decimals);
        }
    }
}