//--------------------------------------------------------------
// <copyright file="FuzzyCandlesticks.cs" company="Nautech Systems Pty Ltd.">
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
    using Nautilus.Indicators.Enums;
    using Nautilus.Indicators.Objects;
    using Nautilus.Indicators.Extensions;

    /// <summary>
    /// The fuzzy candlesticks.
    /// </summary>
    public class FuzzyCandlesticks : Indicator
    {
        // TODO refactor

        /// <summary>
        /// The tick size.
        /// </summary>
        private readonly decimal tickSize;

        /// <summary>
        /// The length.
        /// </summary>
        private double length;

        /// <summary>
        /// The body percent.
        /// </summary>
        private double bodyPercent;

        /// <summary>
        /// The upper wick percent.
        /// </summary>
        private double upperWickPercent;

        /// <summary>
        /// The lower wick percent.
        /// </summary>
        private double lowerWickPercent;

        /// <summary>
        /// The mean length.
        /// </summary>
        private double meanLength;

        /// <summary>
        /// The mean body percent.
        /// </summary>
        private double meanBodyPercent;

        /// <summary>
        /// The mean upper wick percent.
        /// </summary>
        private double meanUpperWickPercent;

        /// <summary>
        /// The mean lower wick percent.
        /// </summary>
        private double meanLowerWickPercent;

        /// <summary>
        /// The sd length.
        /// </summary>
        private double sdLength;

        /// <summary>
        /// The sd body percent.
        /// </summary>
        private double sdBodyPercent;

        /// <summary>
        /// The sd upper wick percent.
        /// </summary>
        private double sdUpperWickPercent;

        /// <summary>
        /// The sd lower wick percent.
        /// </summary>
        private double sdLowerWickPercent;

        /// <summary>
        /// The bars.
        /// </summary>
        private readonly RollingList<Bar> bars;

        /// <summary>
        /// The fuzzy candles.
        /// </summary>
        private readonly List<FuzzyCandle> fuzzyCandles;

        /// <summary>
        /// Initializes a new instance of the <see cref="FuzzyCandlesticks"/> class.
        /// </summary>
        /// <param name="period">
        /// The period.
        /// </param>
        /// <param name="tickSize">
        /// The tick size.
        /// </param>
        public FuzzyCandlesticks(int period, decimal tickSize) : base("Fuzzy Candlesticks")
        {
            this.Initialized = false;

            this.tickSize = tickSize;
            this.Period = period;

            this.bars = new RollingList<Bar>(this.Period);
            this.fuzzyCandles = new List<FuzzyCandle>();
        }

        /// <summary>
        /// Gets the value of the calculation period for this indicator object.
        /// </summary>
        public int Period { get; }

        /// <summary>
        /// Returns the direction of the lase closed candle.
        /// </summary>
        public CandleDirection Direction => this.fuzzyCandles.Last().Direction;

        /// <summary>
        /// Returns the fuzzy size of the lase closed candle.
        /// </summary>
        public CandleSize Size => this.fuzzyCandles.Last().Size;

        /// <summary>
        /// Returns the fuzzy body of the lase closed candle.
        /// </summary>
        public CandleBody Body => this.fuzzyCandles.Last().Body;

        /// <summary>
        /// Returns the fuzzy upper wick of the lase closed candle.
        /// </summary>
        public CandleWick UpperWick => this.fuzzyCandles.Last().UpperWick;

        /// <summary>
        /// Returns the fuzzy lower wick of the lase closed candle.
        /// </summary>
        public CandleWick LowerWick => this.fuzzyCandles.Last().LowerWick;

        /// <summary>
        /// Returns the count of data elements held by the indicator object.
        /// </summary>
        public int Count => this.fuzzyCandles.Count;

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="currentBar">
        /// The current bar.
        /// </param>
        public void Update(Bar currentBar)
        {
            this.bars.Add(currentBar);

            this.UpdateCandleStats();

            var newFuzzyCandle = this.FuzzifyCandle();

            this.fuzzyCandles.Add(newFuzzyCandle);

            if (!this.Initialized)
            {
                this.Initialized = true;
            }
        }

        /// <summary>
        /// Clears the cumulative list of fuzzy candle elements.
        /// </summary>
        public void Reset()
        {
            this.fuzzyCandles.Clear();

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
            return $"{this.Name}[{this.Count}]: Direction={this.Direction}, Size={this.Size}, Body={this.Body}, UpperWick={this.UpperWick}, LowerWick={this.LowerWick}";
        }

        /// <summary>
        /// The get candle.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="FuzzyCandle"/>.
        /// </returns>
        public FuzzyCandle GetCandle(int index) => this.fuzzyCandles.GetByReverseIndex(index);

        /// <summary>
        /// The get direction.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="CandleDirection"/>.
        /// </returns>
        public CandleDirection GetDirection(int index) => this.fuzzyCandles.GetByReverseIndex(index).Direction;

        /// <summary>
        /// The get size.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="CandleSize"/>.
        /// </returns>
        public CandleSize GetSize(int index) => this.fuzzyCandles.GetByReverseIndex(index).Size;

        /// <summary>
        /// The get body.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="CandleBody"/>.
        /// </returns>
        public CandleBody GetBody(int index) => this.fuzzyCandles.GetByReverseIndex(index).Body;

        /// <summary>
        /// The get upper wick.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="CandleWick"/>.
        /// </returns>
        public CandleWick GetUpperWick(int index) => this.fuzzyCandles.GetByReverseIndex(index).UpperWick;

        /// <summary>
        /// The get lower wick.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="CandleWick"/>.
        /// </returns>
        public CandleWick GetLowerWick(int index) => this.fuzzyCandles.GetByReverseIndex(index).LowerWick;

        /// <summary>
        /// The update candle stats.
        /// </summary>
        private void UpdateCandleStats()
        {
            var rawLengths          = new double[this.Period];
            var lengths             = new double[this.Period];
            var bodyPercents        = new double[this.Period];
            var upperWickPercents   = new double[this.Period];
            var lowerWickPercents   = new double[this.Period];

            for (var i = 0; i < this.bars.Count; i++)
            {
                rawLengths[i] = (double)Math.Round(this.bars[i].High - this.bars[i].Low);
                lengths[i] = rawLengths[i] / (double)this.tickSize;
            }

            for (var i = 0; i < this.bars.Count; i++)
            {
                var bodyPercent = (double)Math.Abs(this.bars[i].Open - this.bars[i].Close) / rawLengths[i];
                bodyPercents[i] = Math.Round(bodyPercent, 2);
            }

            for (var i = 0; i < this.bars.Count; i++)
            {
                var upperWickPercent = (double)(this.bars[i].High - Math.Max(this.bars[i].Open.Value, this.bars[i].Close.Value)) / rawLengths[i];
                upperWickPercents[i] = Math.Round(upperWickPercent, 2);
            }

            for (var i = 0; i < this.bars.Count; i++)
            {
                var lowerWickPercent = (double)(Math.Min(this.bars[i].Open.Value, this.bars[i].Close.Value) - this.bars[i].Low) / rawLengths[i];
                lowerWickPercents[i] = Math.Round(lowerWickPercent, 2);
            }

            this.length                  = lengths[0];
            this.bodyPercent             = bodyPercents[0];
            this.upperWickPercent        = upperWickPercents[0];
            this.lowerWickPercent        = lowerWickPercents[0];

            this.meanLength = Math.Round(lengths.Average());
            this.meanBodyPercent = Math.Round(bodyPercents.Average(), 2);
            this.meanUpperWickPercent = Math.Round(upperWickPercents.Average(), 2);
            this.meanLowerWickPercent = Math.Round(lowerWickPercents.Average(), 2);

            this.sdLength = Math.Round(lengths.StandardDeviation());
            this.sdBodyPercent = Math.Round(bodyPercents.StandardDeviation(), 2);
            this.sdUpperWickPercent = Math.Round(upperWickPercents.StandardDeviation(), 2);
            this.sdLowerWickPercent = Math.Round(lowerWickPercents.StandardDeviation(), 2);
        }

        /// <summary>
        /// The fuzzify candle.
        /// </summary>
        /// <returns>
        /// The <see cref="FuzzyCandle"/>.
        /// </returns>
        private FuzzyCandle FuzzifyCandle()
        {
            var fuzzyDirection  = this.FuzzifyCandleDirection();
            var fuzzySize       = this.FuzzifyCandleSize();
            var fuzzyBody       = this.FuzzifyCandleBody();
            var fuzzyUpperWick  = this.FuzzifyUpperWick();
            var fuzzyLowerWick  = this.FuzzifyLowerWick();

            return new FuzzyCandle(fuzzyDirection, fuzzySize, fuzzyBody, fuzzyUpperWick, fuzzyLowerWick);
        }

        /// <summary>
        /// The fuzzify candle direction.
        /// </summary>
        /// <returns>
        /// The <see cref="CandleDirection"/>.
        /// </returns>
        private CandleDirection FuzzifyCandleDirection()
        {
            if (this.bars[0].Open == this.bars[0].Close)
            {
                return CandleDirection.None;
            }

            if (this.bars[0].Open < this.bars[0].Close)
            {
                return CandleDirection.Bull;
            }

            return this.bars[0].Open > this.bars[0].Close
                ? CandleDirection.Bear
                : CandleDirection.None;
        }

        /// <summary>
        /// The fuzzify candle size.
        /// </summary>
        /// <returns>
        /// The <see cref="CandleSize"/>.
        /// </returns>
        private CandleSize FuzzifyCandleSize()
        {
            if (this.length < this.meanLength - this.sdLength)
            {
                return CandleSize.VerySmall;
            }

            if (this.length >= this.meanLength - this.sdLength && this.length < this.meanLength + (this.sdLength * 0.5))
            {
                return CandleSize.Small;
            }

            if (this.length >= this.meanLength - (this.sdLength * 0.5) && this.length < this.meanLength + this.sdLength)
            {
                return CandleSize.Medium;
            }

            if (this.length >= this.meanLength + this.sdLength && this.length < this.meanLength + (this.sdLength * 2.0))
            {
                return CandleSize.Large;
            }

            if (this.length >= this.meanLength + (this.sdLength * 2.0) && this.length < this.meanLength + (this.sdLength * 3.0))
            {
                return CandleSize.VeryLarge;
            }

            return this.length >= this.meanLength + (this.sdLength * 3.0)
                ? CandleSize.ExtremelyLarge
                : CandleSize.None;
        }

        /// <summary>
        /// The fuzzify candle body.
        /// </summary>
        /// <returns>
        /// The <see cref="CandleBody"/>.
        /// </returns>
        private CandleBody FuzzifyCandleBody()
        {
            if (this.bodyPercent < this.meanBodyPercent - this.sdBodyPercent)
            {
                return CandleBody.None;
            }

            if (this.bodyPercent >= this.meanBodyPercent - this.sdBodyPercent && this.bodyPercent < this.meanBodyPercent - (this.sdBodyPercent * 0.5))
            {
                return CandleBody.Small;
            }

            if (this.bodyPercent >= this.meanBodyPercent - (this.sdBodyPercent * 0.5) && this.bodyPercent < this.meanBodyPercent + (this.sdBodyPercent * 0.5))
            {
                return CandleBody.Medium;
            }

            if (this.bodyPercent >= this.meanBodyPercent + (this.sdBodyPercent * 0.5) && this.bodyPercent < this.meanBodyPercent + this.sdBodyPercent)
            {
                return CandleBody.Large;
            }

            return this.bodyPercent >= this.meanBodyPercent + this.sdBodyPercent
                ? CandleBody.Trend
                : CandleBody.None;
        }

        /// <summary>
        /// The fuzzify upper wick.
        /// </summary>
        /// <returns>
        /// The <see cref="CandleWick"/>.
        /// </returns>
        private CandleWick FuzzifyUpperWick()
        {
            if (this.upperWickPercent <= 0)
            {
                return CandleWick.None;
            }

            if (this.upperWickPercent < this.meanUpperWickPercent - (this.sdUpperWickPercent * 0.5))
            {
                return CandleWick.Small;
            }

            if (this.upperWickPercent >= this.meanUpperWickPercent - (this.sdUpperWickPercent * 0.5) && this.upperWickPercent < this.meanUpperWickPercent + this.sdUpperWickPercent)
            {
                return CandleWick.Medium;
            }

            return this.upperWickPercent >= this.meanUpperWickPercent + this.sdUpperWickPercent ? CandleWick.Large : CandleWick.None;
        }

        /// <summary>
        /// The fuzzify lower wick.
        /// </summary>
        /// <returns>
        /// The <see cref="CandleWick"/>.
        /// </returns>
        private CandleWick FuzzifyLowerWick()
        {
            if (this.lowerWickPercent <= 0)
            {
                return CandleWick.None;
            }

            if (this.lowerWickPercent < this.meanLowerWickPercent - (this.sdLowerWickPercent * 0.5))
            {
                return CandleWick.Small;
            }

            if (this.lowerWickPercent >= this.meanLowerWickPercent - (this.sdLowerWickPercent * 0.5) && this.lowerWickPercent < this.meanLowerWickPercent + this.sdLowerWickPercent)
            {
                return CandleWick.Medium;
            }

            return this.lowerWickPercent >= this.meanLowerWickPercent + this.sdLowerWickPercent ? CandleWick.Large : CandleWick.None;
        }
    }
}