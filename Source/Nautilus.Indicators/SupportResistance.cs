//--------------------------------------------------------------
// <copyright file="SupportResistance.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.Indicators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Indicators.Base;
    using Nautilus.Indicators.Enums;

    /// <summary>
    /// The support resistance.
    /// </summary>
    public class SupportResistance : Indicator
    {
        private readonly List<Price> resistanceLevels;
        private readonly List<Price> supportLevels;
        private readonly Swings swingsWeek;
        private readonly Swings swingsMonth;
        private readonly Swings swingsQuarter;
        private readonly Swings swingsBiAnnual;
        private readonly Swings swingsAnnual;
        private readonly List<Bar> bars;
        private Price lastClose;
        private List<Price> levels;

        /// <summary>
        /// Initializes a new instance of the <see cref="SupportResistance"/> class.
        /// </summary>
        public SupportResistance() :
            base(nameof(SupportResistance))
        {
            this.DailyBarsCount = 0;
            this.IntradayBarsCount = 0;

            this.bars = new List<Bar>();
            this.levels = new List<Price>();
            this.resistanceLevels = new List<Price>();
            this.supportLevels = new List<Price>();

            this.swingsWeek = new Swings(5);
            this.swingsMonth = new Swings(20);
            this.swingsQuarter = new Swings(60);
            this.swingsBiAnnual = new Swings(120);
            this.swingsAnnual = new Swings(252);
        }

        /// <summary>
        /// Gets the price of the next resistance level.
        /// </summary>
        public Price NextResistance => this.resistanceLevels[0];

        /// <summary>
        /// Gets the price of the next support level.
        /// </summary>
        public Price NextSupport => this.supportLevels[0];

        /// <summary>
        /// Gets the count of price levels held by the resistance levels list.
        /// </summary>
        public int ResistanceCount => this.resistanceLevels.Count;

        /// <summary>
        /// Gets the count of price levels held by the support levels list.
        /// </summary>
        public int SupportCount => this.supportLevels.Count;

        /// <summary>
        /// Gets the count of daily bars held by the indicator.
        /// </summary>
        public int DailyBarsCount { get; private set; }

        /// <summary>
        /// Gets the count of intraday bars processed by the indicator.
        /// </summary>
        public int IntradayBarsCount { get; private set; }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="currentDailyBar">
        /// The current daily bar.
        /// </param>
        public void Update(Bar currentDailyBar)
        {
            this.bars.Add(currentDailyBar);
            this.DailyBarsCount++;

            this.swingsWeek.Update(currentDailyBar);
            this.swingsMonth.Update(currentDailyBar);
            this.swingsQuarter.Update(currentDailyBar);
            this.swingsBiAnnual.Update(currentDailyBar);
            this.swingsAnnual.Update(currentDailyBar);

            if (!this.Initialized)
            {
                if (this.swingsWeek.HighCount > 0
                    && this.swingsWeek.LowCount > 0
                    && this.swingsMonth.HighCount > 0
                    && this.swingsMonth.LowCount > 0
                    && this.swingsQuarter.HighCount > 0
                    && this.swingsQuarter.LowCount > 0
                    && this.swingsBiAnnual.HighCount > 0
                    && this.swingsBiAnnual.LowCount > 0
                    && this.swingsAnnual.HighCount > 0
                    && this.swingsAnnual.LowCount > 0)
                {
                    this.Initialized = true;
                }

                return;
            }

            this.GetLevels();
            this.SetResistanceLevels();
            this.SetSupportLevels();
        }

        /// <summary>
        /// The update close.
        /// </summary>
        /// <param name="bar">
        /// The bar.
        /// </param>
        public void UpdateClose(Bar bar)
        {
            this.lastClose = bar.Close;
            this.IntradayBarsCount++;

            this.SetResistanceLevels();
            this.SetSupportLevels();
        }

        /// <summary>
        /// Resets all indicator data lists.
        /// </summary>
        public void Reset()
        {
            this.lastClose = Price.Zero();
            this.bars.Clear();
            this.DailyBarsCount = 0;
            this.IntradayBarsCount = 0;

            this.levels.Clear();
            this.resistanceLevels.Clear();
            this.supportLevels.Clear();

            this.swingsWeek.Reset();
            this.swingsMonth.Reset();
            this.swingsQuarter.Reset();
            this.swingsBiAnnual.Reset();
            this.swingsAnnual.Reset();

            this.Initialized = false;
        }

        /// <summary>
        /// The get resistance levels list.
        /// </summary>
        /// <returns>
        /// The list.
        /// </returns>
        public List<Price> GetResistanceLevelsList()
        {
            return this.resistanceLevels;
        }

        /// <summary>
        /// The get support levels list.
        /// </summary>
        /// <returns>
        /// The list.
        /// </returns>
        public List<Price> GetSupportLevelsList()
        {
            return this.supportLevels;
        }

        /// <summary>
        /// The get resistance level.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        public Price GetResistanceLevel(int index)
        {
            return index < this.resistanceLevels.Count ? this.resistanceLevels[index] : Price.Zero();
        }

        /// <summary>
        /// The get support level.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        public Price GetSupportLevel(int index)
        {
            return index < this.supportLevels.Count ? this.supportLevels[index] : Price.Zero();
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            // TODO
            return $"{this.Name}[Resistance {this.resistanceLevels.Count}]: NextResistance={this.NextResistance}"
                   + Environment.NewLine
                   + $"{this.Name}[Support {this.supportLevels.Count}]: NextSupport={this.NextSupport}"
                   + Environment.NewLine
                   + $"Levels= {this.levels.Count} ";
        }

        /// <summary>
        /// The get levels.
        /// </summary>
        private void GetLevels()
        {
            this.levels.Clear();

            this.levels.Add(this.bars[this.bars.Count - 2].High);
            this.levels.Add(this.bars[this.bars.Count - 3].High);
            this.levels.Add(this.bars[this.bars.Count - 2].Low);
            this.levels.Add(this.bars[this.bars.Count - 3].Low);

            if (this.swingsWeek.Direction == SwingDirection.Up)
            {
                this.levels.Add(this.swingsWeek.GetHighPrice(1));
                this.levels.Add(this.swingsWeek.GetLowPrice(0));
            }

            if (this.swingsWeek.Direction == SwingDirection.Down)
            {
                this.levels.Add(this.swingsWeek.GetHighPrice(0));
                this.levels.Add(this.swingsWeek.GetLowPrice(1));
            }

            if (this.swingsMonth.Direction == SwingDirection.Up)
            {
                this.levels.Add(this.swingsMonth.GetHighPrice(1));
                this.levels.Add(this.swingsMonth.GetLowPrice(0));
            }

            if (this.swingsMonth.Direction == SwingDirection.Down)
            {
                this.levels.Add(this.swingsMonth.GetHighPrice(0));
                this.levels.Add(this.swingsMonth.GetLowPrice(1));
            }

            if (this.swingsQuarter.Direction == SwingDirection.Up)
            {
                this.levels.Add(this.swingsQuarter.GetHighPrice(1));
                this.levels.Add(this.swingsQuarter.GetLowPrice(0));
            }

            if (this.swingsQuarter.Direction == SwingDirection.Down)
            {
                this.levels.Add(this.swingsQuarter.GetHighPrice(0));
                this.levels.Add(this.swingsQuarter.GetLowPrice(1));
            }

            if (this.swingsBiAnnual.Direction == SwingDirection.Up)
            {
                this.levels.Add(this.swingsBiAnnual.GetHighPrice(1));
                this.levels.Add(this.swingsBiAnnual.GetLowPrice(0));
            }

            if (this.swingsBiAnnual.Direction == SwingDirection.Down)
            {
                this.levels.Add(this.swingsBiAnnual.GetHighPrice(0));
                this.levels.Add(this.swingsBiAnnual.GetLowPrice(1));
            }

            if (this.swingsAnnual.Direction == SwingDirection.Up)
            {
                this.levels.Add(this.swingsAnnual.GetHighPrice(1));
                this.levels.Add(this.swingsAnnual.GetLowPrice(0));
            }

            if (this.swingsAnnual.Direction == SwingDirection.Down)
            {
                this.levels.Add(this.swingsAnnual.GetHighPrice(0));
                this.levels.Add(this.swingsAnnual.GetLowPrice(1));
            }

            this.levels = this.levels.Distinct().ToList();
            this.levels.Sort();
        }

        /// <summary>
        /// The set resistance levels.
        /// </summary>
        private void SetResistanceLevels()
        {
            this.resistanceLevels.Clear();

            foreach (var level in this.levels)
            {
                if (level >= this.lastClose)
                {
                    this.resistanceLevels.Add(level);
                }
            }

            this.resistanceLevels.Sort();
        }

        /// <summary>
        /// The set support levels.
        /// </summary>
        private void SetSupportLevels()
        {
            this.supportLevels.Clear();

            foreach (var level in this.levels)
            {
                if (level <= this.lastClose)
                {
                    this.supportLevels.Add(level);
                }
            }

            this.supportLevels.Reverse();
        }
    }
}