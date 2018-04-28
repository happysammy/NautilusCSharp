//--------------------------------------------------------------
// <copyright file="TDPoints.cs" company="Nautech Systems Pty Ltd.">
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
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Indicators.Base;
    using Nautilus.Indicators.Objects;
    using NodaTime;

    /// <summary>
    /// The td points.
    /// </summary>
    public class TDPoints : Indicator
    {
        /// <summary>
        /// The bars.
        /// </summary>
        private readonly RollingList<Bar> bars;

        /// <summary>
        /// The td points high.
        /// </summary>
        private readonly List<TDPoint> pointsHigh;

        /// <summary>
        /// The td points low.
        /// </summary>
        private readonly List<TDPoint> pointsLow;

        /// <summary>
        /// Initializes a new instance of the <see cref="TDPoints"/> class.
        /// </summary>
        /// <param name="level">
        /// The level.
        /// </param>
        public TDPoints(int level) : base("TD Points")
        {
            this.Initialized = false;

            this.Level = level;

            this.bars = new RollingList<Bar>((level * 2) + 1);
            this.pointsHigh = new List<TDPoint>();
            this.pointsLow = new List<TDPoint>();

            this.BarsCount = 0;
        }

        /// <summary>
        /// Gets the level.
        /// </summary>
        public int Level { get; }

        /// <summary>
        /// Returns the price of the current high TD Point.
        /// </summary>
        public Price HighPrice => this.pointsHigh.Last().Price;

        /// <summary>
        /// Returns the price of the current low TD Point.
        /// </summary>
        public Price LowPrice => this.pointsLow.Last().Price;

        /// <summary>
        /// Returns the time of the current high TD Point.
        /// </summary>
        public ZonedDateTime HighTime => this.pointsHigh.Last().Timestamp;

        /// <summary>
        /// Returns the time of the current low TD Point.
        /// </summary>
        public ZonedDateTime LowTime => this.pointsLow.Last().Timestamp;

        /// <summary>
        /// Returns the count of high TD Points held by the indicator.
        /// </summary>
        public int HighCount => this.pointsHigh.Count;

        /// <summary>
        /// Returns the count of low TD points held by the indicator.
        /// </summary>
        public int LowCount => this.pointsLow.Count;

        /// <summary>
        /// Gets the bar index of the last high TD Point.
        /// </summary>
        public int HighBarIndex { get; private set; }

        /// <summary>
        /// Gets the bar index of the last low TD Point.
        /// </summary>
        public int LowBarIndex { get; private set; }

        /// <summary>
        /// Gets the count of the bars held by the indicator.
        /// </summary>
        public int BarsCount { get; private set; }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="bar">
        /// The bar.
        /// </param>
        public void Update(Bar bar)
        {
            this.bars.Add(bar);
            this.BarsCount++;

            if (!this.Initialized)
            {
                if (this.BarsCount < (this.Level * 2) + 1)
                {
                    return;
                }
            }

            this.GetTdPoints();

            if (this.Initialized)
            {
                return;
            }

            if (this.HighCount > 0 || this.LowCount > 0)
            {
                this.Initialized = true;
            }
        }

        /// <summary>
        /// The reset.
        /// </summary>
        public void Reset()
        {
            this.bars.Clear();
            this.pointsHigh.Clear();
            this.pointsLow.Clear();

            this.BarsCount = 0;

            this.Initialized = false;
        }

        /// <summary>
        /// The get td point high.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="TDPoint"/>.
        /// </returns>
        public TDPoint GetTdPointHigh(int index) => this.pointsHigh.GetByReverseIndex(index);

        /// <summary>
        /// The get td point low.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="TDPoint"/>.
        /// </returns>
        public TDPoint GetTdPointLow(int index) => this.pointsLow.GetByReverseIndex(index);

        /// <summary>
        /// The get high price.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        public Price GetHighPrice(int index) => this.pointsHigh.GetByReverseIndex(index).Price;

        /// <summary>
        /// The get low price.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        public Price GetLowPrice(int index) => this.pointsLow.GetByReverseIndex(index).Price;

        /// <summary>
        /// The get high time.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="ZonedDateTime"/>.
        /// </returns>
        public ZonedDateTime? GetHighTime(int index) => this.pointsHigh.GetByReverseIndex(index).Timestamp;

        /// <summary>
        /// The get low time.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="ZonedDateTime"/>.
        /// </returns>
        public ZonedDateTime? GetLowTime(int index) => this.pointsLow.GetByReverseIndex(index).Timestamp;

        /// <summary>
        /// The get high bar index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetHighBarIndex(int index) => this.pointsHigh.GetByReverseIndex(index).BarIndex;

        /// <summary>
        /// The get low bar index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetLowBarIndex(int index) => this.pointsLow.GetByReverseIndex(index).BarIndex;

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return $"{this.Name}[Bars={this.BarsCount}] TdPointsHigh[" + this.HighCount + "]" + Environment.NewLine +
                   $"{this.Name}[Bars={this.BarsCount}] TdPointsLow[" + this.LowCount + "]";
        }

        /// <summary>
        /// The get td points.
        /// </summary>
        private void GetTdPoints()
        {
            if ((this.bars[this.Level].High > this.GetMaxHigh(this.Level + 1, this.Level)) && (this.bars[this.Level].High > this.GetMaxHigh(0, this.Level)))
            {
                this.HighBarIndex = this.BarsCount - 1 - this.Level;

                var newTdPointHigh = new TDPoint(this.bars[this.Level].High, this.HighBarIndex, this.bars[this.Level].Timestamp);
                this.pointsHigh.Add(newTdPointHigh);
            }

            if ((this.bars[this.Level].Low < this.GetMinLow(this.Level + 1, this.Level)) && (this.bars[this.Level].Low < this.GetMinLow(0, this.Level)))
            {
                this.LowBarIndex = this.BarsCount - 1 - this.Level;

                var newTdPointLow = new TDPoint(this.bars[this.Level].Low, this.LowBarIndex, this.bars[this.Level].Timestamp);
                this.pointsLow.Add(newTdPointLow);
            }
        }

        /// <summary>
        /// The get max high.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="range">
        /// The range.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        private Price GetMaxHigh(int index, int range)
        {
            return this.bars
               .Select(b => b.High)
               .Max(); // TODO: fix for range
        }

        /// <summary>
        /// The get min low.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="range">
        /// The range.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        private Price GetMinLow(int index, int range)
        {
            return this.bars
               .Select(b => b.Low)
               .Min(); // TODO: fix for range
        }
    }
}