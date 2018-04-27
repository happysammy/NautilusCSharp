// -------------------------------------------------------------------------------------------------
// <copyright file="AverageVolume.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Indicators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NautechSystems.CSharp.Collections;
    using NautechSystems.CSharp.Extensions;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Indicators.Base;

    /// <summary>
    /// The average volume.
    /// </summary>
    public class AverageVolume : Indicator
    {
        /// <summary>
        /// The volumes.
        /// </summary>
        private readonly RollingList<int> volumes;

        /// <summary>
        /// The average volumes.
        /// </summary>
        private readonly List<int> averageVolumes;

        /// <summary>
        /// Initializes a new instance of the <see cref="AverageVolume"/> class.
        /// </summary>
        /// <param name="period">
        /// The period.
        /// </param>
        /// <param name="shift">
        /// The shift.
        /// </param>
        public AverageVolume(int period, int shift = 0) : base("Average Volume")
        {
            this.Initialized = false;
            this.Period = period;
            this.Shift = shift;

            this.volumes = new RollingList<int>(period);
            this.averageVolumes = new List<int>();

            Console.WriteLine(this.volumes.Count);
        }

        /// <summary>
        /// Gets the period.
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
        public int Count => this.averageVolumes.Count;

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="bar">
        /// The bar.
        /// </param>
        public void Update(Bar bar)
        {
            this.volumes.Add(bar.Volume.Value);
            this.averageVolumes.Add((int)Math.Round(this.volumes.Average()));

            if (!this.Initialized)
            {
                this.Initialized = true;
            }
        }

        /// <summary>
        /// Clears the cumulative list of average volumes.
        /// </summary>
        public void Reset()
        {
            this.volumes.Clear();
            this.averageVolumes.Clear();

            this.Initialized = false;
        }

        /// <summary>
        /// The get value.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetValue(int index) => this.averageVolumes.GetByReverseIndex(index);

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
    }
}
