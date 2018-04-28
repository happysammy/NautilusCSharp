//--------------------------------------------------------------
// <copyright file="TDLines.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.Indicators
{
    using System;
    using NautechSystems.CSharp.Extensions;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Indicators.Base;
    using NodaTime;

    /// <summary>
    /// The td lines.
    /// </summary>
    public class TDLines : Indicator
    {
        private readonly int decimals;
        private readonly TDPoints tdPoints;

        /// <summary>
        /// Initializes a new instance of the <see cref="TDLines"/> class.
        /// </summary>
        /// <param name="level">
        /// The level.
        /// </param>
        /// <param name="lookback">
        /// The lookback.
        /// </param>
        /// <param name="tickSize">
        /// The tick size.
        /// </param>
        public TDLines(int level, int lookback, decimal tickSize) : base("TD Lines")
        {
            this.Initialized = false;

            this.Level = level;
            this.Lookback = lookback;

            this.tdPoints = new TDPoints(level);

            this.decimals = tickSize.GetDecimalPlaces();
        }

        /// <summary>
        /// Gets the level.
        /// </summary>
        public int Level { get; }

        /// <summary>
        /// Gets the lookback.
        /// </summary>
        public int Lookback { get; }

        /// <summary>
        /// Gets the high start time.
        /// </summary>
        public ZonedDateTime? HighStartTime { get; private set; }

        /// <summary>
        /// Gets the high end time.
        /// </summary>
        public ZonedDateTime? HighEndTime { get; private set; }

        /// <summary>
        /// Gets the high start price.
        /// </summary>
        public Price HighStartPrice { get; private set; }

        /// <summary>
        /// Gets the high end price.
        /// </summary>
        public Price HighEndPrice { get; private set; }

        /// <summary>
        /// Gets the high start bar index.
        /// </summary>
        public int HighStartBarIndex { get; private set; }

        /// <summary>
        /// Gets the high end bar index.
        /// </summary>
        public int HighEndBarIndex { get; private set; }

        /// <summary>
        /// Gets the high slope.
        /// </summary>
        public double HighSlope { get; private set; }

        /// <summary>
        /// Gets the low start time.
        /// </summary>
        public ZonedDateTime? LowStartTime { get; private set; }

        /// <summary>
        /// Gets the low end time.
        /// </summary>
        public ZonedDateTime? LowEndTime { get; private set; }

        /// <summary>
        /// Gets the low start price.
        /// </summary>
        public decimal LowStartPrice { get; private set; }

        /// <summary>
        /// Gets the low end price.
        /// </summary>
        public decimal LowEndPrice { get; private set; }

        /// <summary>
        /// Gets the low start bar index.
        /// </summary>
        public int LowStartBarIndex { get; private set; }

        /// <summary>
        /// Gets the low end bar index.
        /// </summary>
        public int LowEndBarIndex { get; private set; }

        /// <summary>
        /// Gets the low slope.
        /// </summary>
        public double LowSlope { get; private set; }

        /// <summary>
        /// Gets the count of the bars held by the indicator.
        /// </summary>
        public int BarsCount => this.tdPoints.BarsCount;

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="bar">
        /// The bar.
        /// </param>
        public void Update(Bar bar)
        {
            this.tdPoints.Update(bar);

            if (!this.Initialized)
            {
                if (this.tdPoints.BarsCount < (this.Level * 2) + 1)
                {
                    return;
                }
            }

            this.GetTdLinesPoints();

            if (this.Initialized)
            {
                return;
            }

            if (this.tdPoints.HighCount > 1 || this.tdPoints.LowCount > 1)
            {
                this.Initialized = true;
            }
        }

        /// <summary>
        /// The get td line high price by bar.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        public decimal GetTdLineHighPriceByBar(int index)
        {
            var barsSpan = (this.BarsCount - 1 - index) - this.tdPoints.GetHighBarIndex(0) - 1; // TODO shouldn't need -1 to be correct??
            return this.HighStartPrice + Math.Round(barsSpan * (decimal)this.HighSlope, this.decimals);
        }

        /// <summary>
        /// The get td line low price by bar.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        public decimal GetTdLineLowPriceByBar(int index)
        {
            var barsSpan = (this.BarsCount - 1 - index) - this.tdPoints.GetLowBarIndex(0) - 1; // TODO shouldn't need -1 to be correct??
            return this.LowStartPrice + Math.Round(barsSpan * (decimal)this.LowSlope, this.decimals);
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return $"{this.Name}[Bars={this.BarsCount}] TdPointsHigh[" + this.tdPoints.HighCount + "]" +
                   Environment.NewLine +
                   $"{this.Name}[Bars={this.BarsCount}] TdPointsLow[" + this.tdPoints.LowCount + "]" +
                   Environment.NewLine +
                   $"HighStartTime={this.HighStartTime}, HighStartPrice={this.HighStartPrice}, HighEndTime={this.HighEndTime}, HighEndPrice={this.HighEndPrice}, HighSlope={this.HighSlope}" +
                   Environment.NewLine +
                   $"LowStartTime={this.LowStartTime}, LowStartPrice={this.LowStartPrice}, LowEndTime={this.LowEndTime}, LowEndPrice={this.LowEndPrice}, LowSlope={this.LowSlope}" +
                   Environment.NewLine +
                   $"HighBar[0]={this.GetTdLineHighPriceByBar(0)}" +
                   Environment.NewLine +
                   $"LowBar[0]={this.GetTdLineLowPriceByBar(0)}";
        }

        /// <summary>
        /// The get td lines points.
        /// </summary>
        private void GetTdLinesPoints()
        {
            for (int i = this.Lookback; i >= 1; i--)
            {
                if (this.tdPoints.HighCount > i && this.tdPoints.GetHighPrice(0) < this.tdPoints.GetHighPrice(i))
                {
                    this.HighStartTime = this.tdPoints.GetHighTime(0);
                    this.HighStartPrice = this.tdPoints.GetHighPrice(0);
                    this.HighStartBarIndex = this.tdPoints.GetHighBarIndex(0);

                    this.HighEndTime = this.tdPoints.GetHighTime(i);
                    this.HighEndPrice = this.tdPoints.GetHighPrice(i);
                    this.HighEndBarIndex = this.tdPoints.GetHighBarIndex(i);

                    var span = this.tdPoints.GetHighBarIndex(0) - this.tdPoints.GetHighBarIndex(i);
                    this.HighSlope = ((double)this.HighStartPrice.Value - (double)this.HighEndPrice.Value) / (double)span;
                }
            }

            for (int i = this.Lookback; i >= 1; i--)
            {
                if (this.tdPoints.LowCount > i && this.tdPoints.GetLowPrice(0) > this.tdPoints.GetLowPrice(i))
                {
                    this.LowStartTime = this.tdPoints.GetLowTime(0);
                    this.LowStartPrice = this.tdPoints.GetLowPrice(0).Value;
                    this.LowStartBarIndex = this.tdPoints.GetLowBarIndex(0);

                    this.LowEndTime = this.tdPoints.GetLowTime(i);
                    this.LowEndPrice = this.tdPoints.GetLowPrice(i).Value;
                    this.LowEndBarIndex = this.tdPoints.GetLowBarIndex(i);

                    var span = this.tdPoints.GetLowBarIndex(0) - this.tdPoints.GetLowBarIndex(i);
                    this.LowSlope = ((double)this.LowStartPrice - (double)this.LowEndPrice) / (double)span;
                }
            }
        }
    }
}