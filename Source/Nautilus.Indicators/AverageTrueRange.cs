//--------------------------------------------------------------
// <copyright file="AverageTrueRange.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.Indicators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NautechSystems.CSharp.Collections;
    using NautechSystems.CSharp.Extensions;
    using NautechSystems.CSharp.Validation;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Indicators.Base;

    /// <summary>
    /// The average true range.
    /// </summary>
    public class AverageTrueRange : Indicator
    {
        private readonly int decimals;
        private readonly RollingList<decimal> trueRanges;
        private readonly List<decimal> averageTrueRanges;

        private Bar previousBar;

        /// <summary>
        /// Initializes a new instance of the <see cref="AverageTrueRange"/> class.
        /// </summary>
        /// <param name="period">
        /// The period.
        /// </param>
        /// <param name="tickSize">
        /// The tick size.
        /// </param>
        public AverageTrueRange(int period, decimal tickSize)
            : base(nameof(AverageTrueRange))
        {
            Validate.Int32NotOutOfRange(period, nameof(period), 0, int.MaxValue, RangeEndPoints.Exclusive);
            Validate.DecimalNotOutOfRange(tickSize, nameof(tickSize), decimal.Zero, decimal.MaxValue, RangeEndPoints.Exclusive);

            this.Period = period;

            this.trueRanges = new RollingList<decimal>(period);
            this.averageTrueRanges = new List<decimal>(period);

            this.decimals = Math.Max(tickSize.GetDecimalPlaces(), 2);
        }

        /// <summary>
        /// Gets the value of the calculation period for this indicator object.
        /// </summary>
        public int Period { get; }

        /// <summary>
        /// Returns the value of the geometric true range at the last closed bar.
        /// </summary>
        public decimal Value => this.GetValue(0);

        /// <summary>
        /// Returns the count of data elements held by the indicator object.
        /// </summary>
        public int Count => this.averageTrueRanges.Count;

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="currentBar">
        /// The current bar.
        /// </param>
        public void Update(Bar currentBar)
        {
            Validate.NotNull(currentBar, nameof(currentBar));

            if (!this.Initialized)
            {
                this.Intialize(currentBar);
            }

            var barRange = this.GetBarRange(currentBar);
            this.trueRanges.Add(barRange);

            this.averageTrueRanges.Add(Math.Round(this.trueRanges.Average(), this.decimals));

            this.previousBar = currentBar;
            this.LastTime = currentBar.Timestamp;
        }

        /// <summary>
        /// Clears the rolling list of true ranges and the list of geometric mean ranges.
        /// </summary>
        public void Reset()
        {
            this.trueRanges.Clear();
            this.averageTrueRanges.Clear();
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
        public decimal GetValue(int index) => this.averageTrueRanges.GetByReverseIndex(index);

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

        private void Intialize(Bar currentBar)
        {
            Validate.NotNull(currentBar, nameof(currentBar));

            this.previousBar = currentBar;

            this.Initialized = true;
        }

        private decimal GetBarRange(Bar currentBar)
        {
            Validate.NotNull(currentBar, nameof(currentBar));

            return Math.Max(this.previousBar.Close.Value, currentBar.High.Value) - Math.Min(currentBar.Low.Value, this.previousBar.Close.Value);
        }
    }
}
