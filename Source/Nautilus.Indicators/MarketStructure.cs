//--------------------------------------------------------------
// <copyright file="MarketStructure.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.Indicators
{
    using System;
    using System.Collections.Generic;
    using NautechSystems.CSharp;
    using NautechSystems.CSharp.Extensions;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Indicators.Base;
    using Nautilus.Indicators.Enums;
    using Nautilus.Indicators.Objects;
    using NodaTime;

    /// <summary>
    /// The market structure.
    /// </summary>
    public class MarketStructure : Indicator
    {
        private readonly int decimals;
        private readonly List<Bar> bars;
        private readonly List<MarketDataPoint> swingHighDataPoints;
        private readonly List<MarketDataPoint> swingLowDataPoints;
        private readonly Swings swings;
        private readonly KeltnerChannel keltnerChannel;
        private readonly Momentum momentum;

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketStructure"/> class.
        /// </summary>
        /// <param name="period">
        /// The period.
        /// </param>
        /// <param name="keltnerMultiplier">
        /// The k multiplier.
        /// </param>
        /// <param name="fastSma">
        /// The fast sma.
        /// </param>
        /// <param name="slowSma">
        /// The slow sma.
        /// </param>
        /// <param name="tickSize">
        /// The tick size.
        /// </param>
        public MarketStructure(
            int period,
            decimal keltnerMultiplier,
            int fastSma,
            int slowSma,
            decimal tickSize)
            : base(nameof(MarketStructure))
        {
            this.Initialized = false;
            this.Period = period;

            this.bars = new List<Bar>();

            this.swingHighDataPoints = new List<MarketDataPoint>();
            this.swingLowDataPoints = new List<MarketDataPoint>();

            this.swings = new Swings(period);
            this.keltnerChannel = new KeltnerChannel(this.Period, keltnerMultiplier, tickSize);
            this.momentum = new Momentum(fastSma, slowSma, tickSize);

            this.decimals = Math.Max(tickSize.GetDecimalPlaces(), 2);
        }

        /// <summary>
        /// Gets the value of the calculation period for this indicator object.
        /// </summary>
        public int Period { get; }

        /// <summary>
        /// Gets the direction of the current swing.
        /// </summary>
        public SwingDirection SwingDirection => this.swings.Direction;

        /// <summary>
        /// Gets the count of data elements held by the indicator object.
        /// </summary>
        public int SwingHighCount => this.swingHighDataPoints.Count;

        /// <summary>
        /// Gets the count of data elements held by the indicator object.
        /// </summary>
        public int SwingLowCount => this.swingLowDataPoints.Count;

        /// <summary>
        /// Gets the price of the last swing high.
        /// </summary>
        public Price SwingHighPrice => this.GetSwingHighPrice(0);

        /// <summary>
        /// Gets the price of the last swing low.
        /// </summary>
        public Price SwingLowPrice => this.GetSwingLowPrice(0);

        /// <summary>
        /// Gets the time of the last swing high.
        /// </summary>
        public Option<ZonedDateTime?> SwingHighTime => this.GetSwingHighTime(0);

        /// <summary>
        /// Gets the time of the last swing high.
        /// </summary>
        public Option<ZonedDateTime?> SwingLowTime => this.GetSwingLowTime(0);

        /// <summary>
        /// Gets the length of the last swing high leg.
        /// </summary>
        public decimal SwingHighLength => this.GetSwingHighLength(0);

        /// <summary>
        /// Gets the length of the last swing low leg.
        /// </summary>
        public decimal SwingLowLength => this.GetSwingLowLength(0);

        /// <summary>
        /// Gets the volume at the last swing high.
        /// </summary>
        public int SwingHighVolume => this.GetSwingHighVolume(0);

        /// <summary>
        /// Gets the volume at the last swing low.
        /// </summary>
        public int SwingLowVolume => this.GetSwingLowVolume(0);

        /// <summary>
        /// Gets the keltner position of the last swing high.
        /// </summary>
        public decimal SwingHighKeltnerPos => this.GetSwingHighKeltnerPos(0);

        /// <summary>
        /// Gets the keltner position of the last swing low.
        /// </summary>
        public decimal SwingLowKeltnerPos => this.GetSwingLowKeltnerPos(0);

        /// <summary>
        /// Gets the momentum at the last swing high.
        /// </summary>
        public decimal SwingHighMomentum => this.GetSwingHighMomentum(0);

        /// <summary>
        /// Gets the momentum at the last swing low.
        /// </summary>
        public decimal SwingLowMomentum => this.GetSwingLowMomentum(0);

        /// <summary>
        /// Gets the bar index of the last swing high.
        /// </summary>
        public int HighBarIndex => this.swings.HighBarIndex;

        /// <summary>
        /// Gets the bar index of the last swing low.
        /// </summary>
        public int LowBarIndex => this.swings.LowBarIndex;

        /// <summary>
        /// Gets the count of bar objects held by the indicator.
        /// </summary>
        public int BarsCount => this.bars.Count;

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="currentBar">
        /// The current bar.
        /// </param>
        public void Update(Bar currentBar)
        {
            this.bars.Add(currentBar);

            this.swings.Update(currentBar);
            this.keltnerChannel.Update(currentBar);
            this.momentum.Update(currentBar);

            if (this.swings.Direction == SwingDirection.Up && this.swings.HighCount == this.swingHighDataPoints.Count)
            {
                var newMarketDataPoint = this.CreateMarketDataPoint(SwingType.High);
                this.swingHighDataPoints[this.GetBySwingHighIndex(0)] = newMarketDataPoint;
            }

            if (this.swings.Direction == SwingDirection.Down && this.swings.LowCount == this.swingLowDataPoints.Count)
            {
                var newMarketDataPoint = this.CreateMarketDataPoint(SwingType.Low);
                this.swingLowDataPoints[this.GetBySwingLowIndex(0)] = newMarketDataPoint;
            }

            if (this.swings.HighCount > this.swingHighDataPoints.Count)
            {
                var newMarketDataPoint = this.CreateMarketDataPoint(SwingType.High);
                this.swingHighDataPoints.Add(newMarketDataPoint);
            }

            if (this.swings.LowCount > this.swingLowDataPoints.Count)
            {
                var newMarketDataPoint = this.CreateMarketDataPoint(SwingType.Low);
                this.swingLowDataPoints.Add(newMarketDataPoint);
            }

            if (this.Initialized)
            {
                return;
            }

            if (this.SwingHighCount > 0 && this.SwingLowCount > 0)
            {
                this.Initialized = true;
            }
        }

        /// <summary>
        /// Clears the cumulative list of market data points, and resets all indicators.
        /// </summary>
        public void Reset()
        {
            this.swingHighDataPoints.Clear();
            this.swingLowDataPoints.Clear();
            this.bars.Clear();
            this.swings.Reset();
            this.keltnerChannel.Reset();
            this.momentum.Reset();

            this.Initialized = false;
        }

        /// <summary>
        /// The get swing high data point.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="MarketDataPoint"/>.
        /// </returns>
        public MarketDataPoint GetSwingHighDataPoint(int index)
        {
            return index < this.swingHighDataPoints.Count
                ? this.swingHighDataPoints[this.GetBySwingHighIndex(index)]
                : new MarketDataPoint();
        }

        /// <summary>
        /// The get swing low data point.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="MarketDataPoint"/>.
        /// </returns>
        public MarketDataPoint GetSwingLowDataPoint(int index)
        {
            return index < this.swingLowDataPoints.Count
                ? this.swingLowDataPoints[this.GetBySwingLowIndex(index)]
                : new MarketDataPoint();
        }

        /// <summary>
        /// The get swing high price.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        public Price GetSwingHighPrice(int index)
        {
            return index < this.swingHighDataPoints.Count
                ? this.swingHighDataPoints[this.GetBySwingHighIndex(index)].Price
                : Price.Zero();
        }

        /// <summary>
        /// The get swing low price.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        public Price GetSwingLowPrice(int index)
        {
            return index < this.swingLowDataPoints.Count
                ? this.swingLowDataPoints[this.GetBySwingLowIndex(index)].Price
                : Price.Zero();
        }

        /// <summary>
        /// The get swing high time.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public Option<ZonedDateTime?> GetSwingHighTime(int index)
        {
            return index < this.swingHighDataPoints.Count
                ? this.swingHighDataPoints[this.GetBySwingHighIndex(index)].Timestamp
                : Option<ZonedDateTime?>.None();
        }

        /// <summary>
        /// The get swing low time.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public Option<ZonedDateTime?> GetSwingLowTime(int index)
        {
            return index < this.swingLowDataPoints.Count
                ? this.swingLowDataPoints[this.GetBySwingLowIndex(index)].Timestamp
                : Option<ZonedDateTime?>.None();
        }

        /// <summary>
        /// The get swing high length.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        public decimal GetSwingHighLength(int index)
        {
            return index < this.swingHighDataPoints.Count ? Math.Round(this.swingHighDataPoints[this.GetBySwingHighIndex(index)].SwingLength, this.decimals) : 0m;
        }

        /// <summary>
        /// The get swing low length.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        public decimal GetSwingLowLength(int index)
        {
            return index < this.swingLowDataPoints.Count ? Math.Round(this.swingLowDataPoints[this.GetBySwingLowIndex(index)].SwingLength, this.decimals) : 0m;
        }

        /// <summary>
        /// The get swing high volume.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetSwingHighVolume(int index)
        {
            return (int) (index < this.swingHighDataPoints.Count ? this.swingHighDataPoints[this.GetBySwingHighIndex(index)].Volume : 0);
        }

        /// <summary>
        /// The get swing low volume.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetSwingLowVolume(int index)
        {
            return (int) ( index < this.swingLowDataPoints.Count ? this.swingLowDataPoints[this.GetBySwingLowIndex(index)].Volume : 0);
        }

        /// <summary>
        /// The get swing high keltner pos.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        public decimal GetSwingHighKeltnerPos(int index)
        {
            return index < this.swingHighDataPoints.Count ? this.swingHighDataPoints[this.GetBySwingHighIndex(index)].KeltnerPosition : 0m;
        }

        /// <summary>
        /// The get swing low keltner pos.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        public decimal GetSwingLowKeltnerPos(int index)
        {
            return index < this.swingLowDataPoints.Count ? this.swingLowDataPoints[this.GetBySwingLowIndex(index)].KeltnerPosition : 0m;
        }

        /// <summary>
        /// The get swing high momentum.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        public decimal GetSwingHighMomentum(int index)
        {
            return index < this.swingHighDataPoints.Count ? this.swingHighDataPoints[this.GetBySwingHighIndex(index)].Momentum : 0m;
        }

        /// <summary>
        /// The get swing low momentum.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        public decimal GetSwingLowMomentum(int index)
        {
            return index < this.swingLowDataPoints.Count ? this.swingLowDataPoints[this.GetBySwingLowIndex(index)].Momentum : 0m;
        }

        /// <summary>
        /// The get highest momentum at swing.
        /// </summary>
        /// <param name="lookback">
        /// The lookback.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        public decimal GetHighestMomentumAtSwing(int lookback)
        {
            var highestMomentum = decimal.MinValue;

            for (int i = 0; i < Math.Min(this.swingHighDataPoints.Count, lookback); i++)
            {
                if (this.swingHighDataPoints[this.GetBySwingHighIndex(i)].Momentum > highestMomentum)
                {
                    highestMomentum = this.swingHighDataPoints[this.GetBySwingHighIndex(i)].Momentum;
                }
            }

            return highestMomentum;
        }

        /// <summary>
        /// The get lowest momentum at swing.
        /// </summary>
        /// <param name="lookback">
        /// The lookback.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        public decimal GetLowestMomentumAtSwing(int lookback)
        {
            var lowestMomentum = decimal.MaxValue;

            for (int i = 0; i < Math.Min(this.swingLowDataPoints.Count, lookback); i++)
            {
                if (this.swingLowDataPoints[this.GetBySwingLowIndex(i)].Momentum < lowestMomentum)
                {
                    lowestMomentum = this.swingLowDataPoints[this.GetBySwingLowIndex(i)].Momentum;
                }
            }

            return lowestMomentum;
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return $"{this.Name}[SwingHigh {this.SwingHighCount}]: SwingHighPrice={this.SwingHighPrice}, SwingHighTime={this.SwingHighTime}, SwingHighLength{this.SwingHighLength}, SwingHighVolume={this.SwingHighVolume}, SwingHighKeltner={this.SwingHighKeltnerPos}, SwingHighMomentum={this.SwingHighMomentum}" +
                   Environment.NewLine +
                   $"{this.Name}[SwingLow {this.SwingLowCount}]: SwingLowPrice={this.SwingLowPrice}, SwingLowTime={this.SwingLowTime}, SwingLowLength{this.SwingLowLength}, SwingLowVolume={this.SwingLowVolume}, SwingLowKeltner={this.SwingLowKeltnerPos}, SwingLowMomentum={this.SwingLowMomentum}";
        }

        /// <summary>
        /// The create market data point.
        /// </summary>
        /// <param name="swingType">
        /// The swing type.
        /// </param>
        /// <returns>
        /// The <see cref="MarketDataPoint"/>.
        /// </returns>
        private MarketDataPoint CreateMarketDataPoint(SwingType swingType)
        {
            if (swingType == SwingType.High)
            {
                var barIndex = this.swings.HighBarIndex;
                return new MarketDataPoint(
                    this.swings.HighPrice,
                    this.CalculateVolume(barIndex),
                    SwingType.High,
                    this.CalculateSwingLength(),
                    this.CalculateKeltnerPosition(SwingType.High, barIndex),
                    this.CalculateMomentum(barIndex),
                    this.swings.HighTime);
            }

            if (swingType == SwingType.Low)
            {
                var barIndex = this.swings.LowBarIndex;
                return new MarketDataPoint(
                    this.swings.LowPrice,
                    this.CalculateVolume(barIndex),
                    SwingType.Low,
                    this.CalculateSwingLength(),
                    this.CalculateKeltnerPosition(SwingType.Low, barIndex),
                    this.CalculateMomentum(barIndex),
                    this.swings.LowTime);
            }

            return new MarketDataPoint();
        }

        /// <summary>
        /// The calculate swing length.
        /// </summary>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        private decimal CalculateSwingLength()
        {
            return this.Initialized ? Math.Abs(this.swings.HighPrice - this.swings.LowPrice) : 0m;
        }

        /// <summary>
        /// The calculate volume.
        /// </summary>
        /// <param name="barIndex">
        /// The bar index.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int CalculateVolume(int barIndex)
        {
            // TODO unit test required
            var volumeAtSwing = 0;

            // left of swing
            if (this.bars.Count > barIndex + 1)
            {
                volumeAtSwing += this.bars[barIndex + 1].Volume;
            }

            // at swing
            if (this.bars.Count > barIndex)
            {
                volumeAtSwing += this.bars[barIndex].Volume;
            }

            // right of swing
            if (this.bars.Count > barIndex && barIndex > 0)
            {
                volumeAtSwing += this.bars[barIndex - 1].Volume;
            }

            return volumeAtSwing;
        }

        /// <summary>
        /// The calculate keltner position.
        /// </summary>
        /// <param name="swingType">
        /// The swing type.
        /// </param>
        /// <param name="barsIndex">
        /// The bars index.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        private decimal CalculateKeltnerPosition(SwingType swingType, int barsIndex)
        {
            if (swingType == SwingType.High)
            {
                var keltnerWidth = this.keltnerChannel.GetUpperBand(this.bars.Count - barsIndex) - this.keltnerChannel.GetMiddleBand(this.bars.Count - barsIndex);

                return Math.Round((this.bars[barsIndex].High - this.keltnerChannel.GetMiddleBand(this.bars.Count - barsIndex)) / keltnerWidth, 3); // TODO not working
            }

            if (swingType == SwingType.Low)
            {
                var keltnerWidth = this.keltnerChannel.GetMiddleBand(this.bars.Count - barsIndex) - this.keltnerChannel.GetLowerBand(this.bars.Count - barsIndex);

                return Math.Round((this.bars[barsIndex].Low  - this.keltnerChannel.GetMiddleBand(this.bars.Count - barsIndex)) / keltnerWidth, 3); // TODO not working
            }

            return 0m;
        }

        /// <summary>
        /// The calculate momentum.
        /// </summary>
        /// <param name="barIndex">
        /// The bar index.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        private decimal CalculateMomentum(int barIndex)
        {
            return this.momentum.GetValue(this.bars.Count - 1 - barIndex);
        }

        /// <summary>
        /// The get by swing high index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int GetBySwingHighIndex(int index)
        {
            return this.SwingHighCount - 1 - index;
        }

        /// <summary>
        /// The get by swing low index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int GetBySwingLowIndex(int index)
        {
            return this.SwingLowCount - 1 - index;
        }
    }
}
