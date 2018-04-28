//--------------------------------------------------------------
// <copyright file="Momentum.cs" company="Nautech Systems Pty Ltd.">
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
    /// The momentum.
    /// </summary>
    public class Momentum : Indicator
    {
        private readonly int decimals;
        private readonly List<decimal> momentums;
        private readonly SimpleMovingAverage fastSma;
        private readonly SimpleMovingAverage slowSma;

        /// <summary>
        /// Initializes a new instance of the <see cref="Momentum"/> class.
        /// </summary>
        /// <param name="fast">
        /// The fast.
        /// </param>
        /// <param name="slow">
        /// The slow.
        /// </param>
        /// <param name="tickSize">
        /// The tick size.
        /// </param>
        public Momentum(int fast, int slow, decimal tickSize) : base(nameof(Momentum))
        {
            this.Initialized = false;

            this.Fast = fast;
            this.Slow = slow;

            this.momentums = new List<decimal>();

            this.fastSma = new SimpleMovingAverage(this.Fast, 0, tickSize);
            this.slowSma = new SimpleMovingAverage(this.Slow, 0, tickSize);

            this.decimals = Math.Max(tickSize.GetDecimalPlaces(), 2);
        }

        /// <summary>
        /// Gets the period for the fast simple moving average.
        /// </summary>
        public int Fast { get; }

        /// <summary>
        /// Gets the period for the slow simple moving average.
        /// </summary>
        public int Slow { get; }

        /// <summary>
        /// Gets the momentum at the last closed bar.
        /// </summary>
        public decimal Value => this.momentums.Last();

        /// <summary>
        /// Gets the count of data elements held by the indicator object.
        /// </summary>
        public int Count => this.momentums.Count;

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="currentBar">
        /// The current bar.
        /// </param>
        public void Update(Bar currentBar)
        {
            this.fastSma.Update(currentBar);
            this.slowSma.Update(currentBar);

            var newMomentum = Math.Round(this.fastSma.Value - this.slowSma.Value, this.decimals);

            this.momentums.Add(newMomentum);

            if (!this.Initialized)
            {
                this.Initialized = true;
            }
        }

        /// <summary>
        /// Clears the cumulative list of momentums.
        /// </summary>
        public void Reset()
        {
            this.momentums.Clear();

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
            return $"{this.Name}[{this.Count}]: {this.Value}";
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
        public decimal GetValue(int index) => this.momentums.GetByReverseIndex(index);
    }
}
