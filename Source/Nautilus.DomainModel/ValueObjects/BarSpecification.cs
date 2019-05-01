//--------------------------------------------------------------------------------------------------
// <copyright file="BarSpecification.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.ValueObjects
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects.Base;
    using NodaTime;

    /// <summary>
    /// Represents a bar specification being a quote type, resolution and period.
    /// </summary>
    [Immutable]
    public sealed class BarSpecification : ValueObject<BarSpecification>, IEquatable<BarSpecification>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BarSpecification"/> class.
        /// </summary>
        /// <param name="quoteType">The specification quote type.</param>
        /// <param name="resolution">The specification resolution.</param>
        /// <param name="period">The specification period.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the period is not position (> 0).</exception>
        public BarSpecification(
            QuoteType quoteType,
            Resolution resolution,
            int period)
        {
            Precondition.PositiveInt32(period, nameof(period));

            this.QuoteType = quoteType;
            this.Resolution = resolution;
            this.Period = period;
            this.TimePeriod = this.GetTimePeriod(period);
            this.Duration = this.TimePeriod.ToDuration();
        }

        /// <summary>
        /// Gets the bar specifications quote type.
        /// </summary>
        public QuoteType QuoteType { get; }

        /// <summary>
        /// Gets the bars specifications resolution.
        /// </summary>
        public Resolution Resolution { get; }

        /// <summary>
        /// Gets the bars specifications period.
        /// </summary>
        public int Period { get;  }

        /// <summary>
        /// Gets the bars specifications time period.
        /// </summary>
        public Period TimePeriod { get; }

        /// <summary>
        /// Gets the duration.
        /// </summary>
        public Duration Duration { get; }

        /// <summary>
        /// Gets a value indicating whether this bars time period is one day.
        /// </summary>
        public bool IsOneDayBar => this.Resolution == Resolution.Day && this.Period == 1;

        /// <summary>
        /// Returns the hash code of the <see cref="BarSpecification"/>.
        /// </summary>
        /// <remarks>Non-readonly properties referenced in GetHashCode for serialization.</remarks>
        /// <returns>A <see cref="int"/>.</returns>
        public override int GetHashCode()
        {
            return this.QuoteType.GetHashCode() +
                   this.Resolution.GetHashCode() +
                   this.Period.GetHashCode();
        }

        /// <summary>
        /// Returns a string representation of the <see cref="BarSpecification"/>.
        /// </summary>
        /// <returns>A string.</returns>
        public override string ToString() => $"{this.Period}-{this.Resolution}[{this.QuoteType}]";

        /// <summary>
        /// Returns an array of objects to be included in equality checks.
        /// </summary>
        /// <returns>The array of equality members.</returns>
        protected override object[] GetEqualityArray()
        {
            return new object[]
                       {
                           this.QuoteType,
                           this.Resolution,
                           this.Period,
                       };
        }

        private Period GetTimePeriod(int barPeriod)
        {
            Debug.PositiveInt32(barPeriod, nameof(barPeriod));

            switch (this.Resolution)
            {
                case Resolution.Tick:
                    return NodaTime.Period.Zero;

                case Resolution.Second:
                    return NodaTime.Period.FromSeconds(barPeriod);

                case Resolution.Minute:
                    return NodaTime.Period.FromMinutes(barPeriod);

                case Resolution.Hour:
                    return NodaTime.Period.FromHours(barPeriod);

                case Resolution.Day:
                    return NodaTime.Period.FromDays(barPeriod);

                default: throw new InvalidOperationException("The bar resolution was not recognised.");
            }
        }
    }
}
