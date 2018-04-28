//--------------------------------------------------------------
// <copyright file="Swings.cs" company="Nautech Systems Pty Ltd.">
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
    using Nautilus.Indicators.Enums;
    using Nautilus.Indicators.Objects;
    using NodaTime;

    /// <summary>
    /// The swings.
    /// </summary>
    public class Swings : Indicator
    {
        private readonly RollingList<Bar> bars;
        private readonly List<SwingPoint> swingHighs = new List<SwingPoint>();
        private readonly List<SwingPoint> swingLows = new List<SwingPoint>();

        private Price maxHigh;
        private Price minLow;

        private bool isSwingHigh;
        private bool isSwingLow;

        /// <summary>
        /// Initializes a new instance of the <see cref="Swings"/> class.
        /// </summary>
        /// <param name="period">The period (> 0).</param>
        public Swings(int period)
            : base(nameof(Swings))
        {
            Validate.Int32NotOutOfRange(period, nameof(period), 0, int.MaxValue, RangeEndPoints.Exclusive);

            this.Period = period;
            this.Direction = SwingDirection.None;

            this.bars = new RollingList<Bar>(period);
        }

        /// <summary>
        /// Gets the value of the calculation period for this indicator object.
        /// </summary>
        public int Period { get; }

        /// <summary>
        /// Gets the direction of the current swing.
        /// </summary>
        public SwingDirection Direction { get; private set; }

        /// <summary>
        /// Gets the price of the current swing high.
        /// </summary>
        public Price HighPrice => this.swingHighs.Last().Price;

        /// <summary>
        /// Gets the price of the current swing low.
        /// </summary>
        public Price LowPrice => this.swingLows.Last().Price;

        /// <summary>
        /// Gets the time of the current swing high.
        /// </summary>
        public ZonedDateTime HighTime => this.swingHighs.Last().Timestamp;

        /// <summary>
        /// Gets the time of the current swing low.
        /// </summary>
        public ZonedDateTime LowTime => this.swingLows.Last().Timestamp;

        /// <summary>
        /// Gets the count of swing high points held by the indicator.
        /// </summary>
        public int HighCount => this.swingHighs.Count;

        /// <summary>
        /// Gets the count of swing low points held by the indicator.
        /// </summary>
        public int LowCount => this.swingLows.Count;

        /// <summary>
        /// Gets the bar index of the last swing high.
        /// </summary>
        public int HighBarIndex { get; private set; }

        /// <summary>
        /// Gets the bar index of the last swing low.
        /// </summary>
        public int LowBarIndex { get; private set; }

        /// <summary>
        /// Gets the count of the bars held by the indicator.
        /// </summary>
        public int BarsCount => this.bars.Count;

        /// <summary>
        /// Resets the swings indicator.
        /// </summary>
        public void Reset()
        {
            this.bars.Clear();
            this.swingHighs.Clear();
            this.swingLows.Clear();

            this.Direction = SwingDirection.None;
            this.Initialized = false;
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="currentBar">The current bar.</param>
        public void Update(Bar currentBar)
        {
            Validate.NotNull(currentBar, nameof(currentBar));

            this.bars.Add(currentBar);

            if (!this.Initialized)
            {
                if (this.bars.Count < this.Period)
                {
                    return;
                }

                if (this.HighCount > 0 || this.LowCount > 0)
                {
                    this.Initialized = true;
                }
            }

            this.UpdateMaxHigh();
            this.UpdateMinLow();
            this.UpdateIsSwingHigh();
            this.UpdateIsSwingLow();

            if (this.IsNoSwing())
            {
                return;
            }

            if (this.IsNewSwingHigh())
            {
                this.swingHighs[this.swingHighs.LastIndex()] = this.CreateSwingHigh();
            }

            if (this.IsSwingDirectionNowUp())
            {
                this.swingHighs.Add(this.CreateSwingHigh());
                this.Direction = SwingDirection.Up;
                this.HighBarIndex = this.bars.LastIndex();
            }

            if (this.IsNewSwingLow())
            {
                this.swingLows[this.swingLows.LastIndex()] = this.CreateSwingLow();
            }

            if (this.IsSwingDirectionNowDown())
            {
                this.swingLows.Add(this.CreateSwingLow());
                this.Direction = SwingDirection.Down;
                this.LowBarIndex = this.bars.LastIndex();
            }
        }

        /// <summary>
        /// The get swing high.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="SwingPoint"/>.
        /// </returns>
        public SwingPoint GetSwingHigh(int index) => this.swingHighs.GetByReverseIndex(index);

        /// <summary>
        /// The get swing low.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="SwingPoint"/>.
        /// </returns>
        public SwingPoint GetSwingLow(int index) => this.swingLows.GetByReverseIndex(index);

        /// <summary>
        /// The get high price.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        public Price GetHighPrice(int index) => this.swingHighs.GetByReverseIndex(index).Price;

        /// <summary>
        /// The get low price.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        public Price GetLowPrice(int index) => this.swingLows.GetByReverseIndex(index).Price;

        /// <summary>
        /// The get high time.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public ZonedDateTime? GetHighTime(int index) => this.swingHighs.GetByReverseIndex(index).Timestamp;

        /// <summary>
        /// The get low time.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public ZonedDateTime? GetLowTime(int index) => this.swingLows.GetByReverseIndex(index).Timestamp;

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return $"{this.Name}[Bars={this.BarsCount}] Direction={this.Direction}, [SwingHighs {this.HighCount}]: SwingHighTime={this.HighTime}, SwingHighPrice={this.HighPrice}" +
                Environment.NewLine +
                   $"{this.Name}[Bars={this.BarsCount}] Direction={this.Direction}, [SwingLows {this.LowCount}]: SwingLowTime={this.LowTime}, SwingLowPrice={this.LowPrice}";
        }

        private void UpdateMinLow()
        {
            this.minLow = this.bars.Select(bar => bar.Low).Min();
        }

        private void UpdateMaxHigh()
        {
            this.maxHigh = this.bars.Select(bar => bar.High).Max();
        }

        private void UpdateIsSwingLow()
        {
            this.isSwingLow = (this.bars[0].Low <= this.minLow) && (this.maxHigh >= this.bars[0].High);
        }

        private void UpdateIsSwingHigh()
        {
            this.isSwingHigh = (this.bars[0].High >= this.maxHigh) && (this.minLow <= this.bars[0].Low);
        }

        private bool IsNoSwing()
        {
            return !this.isSwingHigh && !this.isSwingLow;
        }

        private bool IsNewSwingHigh()
        {
            return this.isSwingHigh && this.Direction == SwingDirection.Up && this.maxHigh >= this.swingHighs.Last().Price;
        }

        private bool IsNewSwingLow()
        {
            return this.isSwingLow && this.Direction == SwingDirection.Down && this.minLow <= this.swingLows.Last().Price;
        }

        private bool IsSwingDirectionNowUp()
        {
            return this.isSwingHigh && (this.Direction == SwingDirection.Down || this.Direction == SwingDirection.None);
        }

        private bool IsSwingDirectionNowDown()
        {
            return this.isSwingLow && (this.Direction == SwingDirection.Up || this.Direction == SwingDirection.None);
        }

        private SwingPoint CreateSwingLow()
        {
            return new SwingPoint(SwingType.Low, this.minLow, this.bars.LastIndex(), this.bars[0].Timestamp);
        }

        private SwingPoint CreateSwingHigh()
        {
            return new SwingPoint(SwingType.High, this.maxHigh, this.bars.LastIndex(), this.bars[0].Timestamp);
        }
    }
}