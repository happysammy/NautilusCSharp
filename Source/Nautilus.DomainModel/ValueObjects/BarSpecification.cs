//--------------------------------------------------------------------------------------------------
// <copyright file="BarSpecification.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.ValueObjects
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Enums;
    using NodaTime;

    /// <summary>
    /// Represents a bar specification being a quote type, resolution and period.
    /// </summary>
    [Immutable]
    public struct BarSpecification : IEquatable<object>, IEquatable<BarSpecification>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BarSpecification"/> structure.
        /// </summary>
        /// <param name="period">The specification period.</param>
        /// <param name="resolution">The specification resolution.</param>
        /// <param name="quoteType">The specification quote type.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the period is not positive (> 0).</exception>
        public BarSpecification(
            int period,
            Resolution resolution,
            QuoteType quoteType)
        {
            Debug.PositiveInt32(period, nameof(period));

            this.Period = period;
            this.Resolution = resolution;
            this.QuoteType = quoteType;
            this.Duration = CalculateDuration(period, resolution);
        }

        /// <summary>
        /// Gets the bars specifications period.
        /// </summary>
        public int Period { get;  }

        /// <summary>
        /// Gets the bars specifications resolution.
        /// </summary>
        public Resolution Resolution { get; }

        /// <summary>
        /// Gets the bar specifications quote type.
        /// </summary>
        public QuoteType QuoteType { get; }

        /// <summary>
        /// Gets the bar time duration.
        /// </summary>
        public Duration Duration { get; }

        /// <summary>
        /// Returns a value indicating whether the <see cref="BarSpecification"/>s are equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator ==(BarSpecification left, BarSpecification right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns a value indicating whether the <see cref="BarSpecification"/>s are not equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator !=(BarSpecification left,  BarSpecification right) => !(left == right);

        /// <summary>
        /// Returns a value indicating whether this <see cref="BarSpecification"/> is equal
        /// to the given <see cref="object"/>.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public override bool Equals(object other) => other is BarSpecification barSpec && this.Equals(barSpec);

        /// <summary>
        /// Returns a value indicating whether this <see cref="BarSpecification"/> is equal
        /// to the given <see cref="BarSpecification"/>.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public bool Equals(BarSpecification other)
        {
            return this.Period == other.Period &&
                   this.Resolution == other.Resolution &&
                   this.QuoteType == other.QuoteType;
        }

        /// <summary>
        /// Returns the hash code of the <see cref="BarSpecification"/>.
        /// </summary>
        /// <remarks>Non-readonly properties referenced in GetHashCode for serialization.</remarks>
        /// <returns>A <see cref="int"/>.</returns>
        public override int GetHashCode()
        {
            return Hash.GetCode(this.Period, this.Resolution, this.QuoteType);
        }

        /// <summary>
        /// Returns a string representation of the <see cref="BarSpecification"/>.
        /// </summary>
        /// <returns>A string.</returns>
        public override string ToString() => $"{this.Period}-{this.Resolution}[{this.QuoteType}]";

        private static Duration CalculateDuration(int barPeriod, Resolution resolution)
        {
            Debug.PositiveInt32(barPeriod, nameof(barPeriod));

            switch (resolution)
            {
                case Resolution.TICK:
                    return Duration.Zero;
                case Resolution.SECOND:
                    return Duration.FromSeconds(barPeriod);
                case Resolution.MINUTE:
                    return Duration.FromMinutes(barPeriod);
                case Resolution.HOUR:
                    return Duration.FromHours(barPeriod);
                case Resolution.DAY:
                    return Duration.FromDays(barPeriod);
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(resolution, nameof(resolution));
            }
        }
    }
}
