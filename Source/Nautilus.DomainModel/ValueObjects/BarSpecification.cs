//--------------------------------------------------------------------------------------------------
// <copyright file="BarSpecification.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
//--------------------------------------------------------------------------------------------------

using System;
using Nautilus.Core;
using Nautilus.Core.Annotations;
using Nautilus.Core.Correctness;
using Nautilus.Core.Extensions;
using Nautilus.DomainModel.Enums;
using NodaTime;

namespace Nautilus.DomainModel.ValueObjects
{
    /// <summary>
    /// Represents a bar specification being a quote type, resolution and period.
    /// </summary>
    [Immutable]
    public sealed class BarSpecification : IEquatable<object>, IEquatable<BarSpecification>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BarSpecification"/> class.
        /// </summary>
        /// <param name="period">The specification period.</param>
        /// <param name="barStructure">The specification resolution.</param>
        /// <param name="priceType">The specification quote type.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the period is not positive (> 0).</exception>
        public BarSpecification(
            int period,
            BarStructure barStructure,
            PriceType priceType)
        {
            Debug.PositiveInt32(period, nameof(period));

            this.Period = period;
            this.BarStructure = barStructure;
            this.PriceType = priceType;
            this.Duration = CalculateDuration(period, barStructure);
        }

        /// <summary>
        /// Gets the bars specifications period.
        /// </summary>
        public int Period { get;  }

        /// <summary>
        /// Gets the bars specifications resolution.
        /// </summary>
        public BarStructure BarStructure { get; }

        /// <summary>
        /// Gets the bar specifications quote type.
        /// </summary>
        public PriceType PriceType { get; }

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
        /// Returns a new <see cref="BarSpecification"/> from the given <see cref="string"/>.
        /// </summary>
        /// <param name="barSpecString">The bar specification string.</param>
        /// <returns>The created <see cref="BarSpecification"/>.</returns>
        public static BarSpecification FromString(string barSpecString)
        {
            Debug.NotEmptyOrWhiteSpace(barSpecString, nameof(barSpecString));

            var split = barSpecString.Split('-', 3);

            var period = Convert.ToInt32(split[0]);
            var resolution = split[1].ToUpper();
            var quoteType = split[2].ToUpper();

            return new BarSpecification(
                period,
                resolution.ToEnum<BarStructure>(),
                quoteType.ToEnum<PriceType>());
        }

        /// <summary>
        /// Returns a value indicating whether this object is equal to the given object.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns>The result of the equality check.</returns>
        public override bool Equals(object? obj) => obj is BarSpecification barSpec && this.Equals(barSpec);

        // Due to the convention that an IEquatable<T> argument can be null the compiler now emits
        // a warning unless Equals is marked with [AllowNull] or takes a nullable param. We don't
        // want to allow null here for the sake of silencing the warning and so temporarily using
        // #pragma warning disable CS8767 until a better refactoring is determined.
#pragma warning disable CS8767
        /// <summary>
        /// Returns a value indicating whether this <see cref="BarSpecification"/> is equal
        /// to the given <see cref="BarSpecification"/>.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public bool Equals(BarSpecification other)
        {
            return this.Period == other.Period &&
                   this.BarStructure == other.BarStructure &&
                   this.PriceType == other.PriceType;
        }

        /// <summary>
        /// Returns the hash code of the <see cref="BarSpecification"/>.
        /// </summary>
        /// <remarks>Non-readonly properties referenced in GetHashCode for serialization.</remarks>
        /// <returns>A <see cref="int"/>.</returns>
        public override int GetHashCode()
        {
            return Hash.GetCode(this.Period, this.BarStructure, this.PriceType);
        }

        /// <summary>
        /// Returns a string representation of the <see cref="BarSpecification"/>.
        /// </summary>
        /// <returns>A string.</returns>
        public override string ToString()
        {
            var period = this.Period.ToString();
            var structure = this.BarStructure.ToString().ToUpper();
            var priceType = this.PriceType.ToString().ToUpper();

            return $"{period}-{structure}-{priceType}";
        }

        private static Duration CalculateDuration(int barPeriod, BarStructure barStructure)
        {
            Debug.PositiveInt32(barPeriod, nameof(barPeriod));

            switch (barStructure)
            {
                case BarStructure.Tick:
                    return Duration.Zero;
                case BarStructure.Second:
                    return Duration.FromSeconds(barPeriod);
                case BarStructure.Minute:
                    return Duration.FromMinutes(barPeriod);
                case BarStructure.Hour:
                    return Duration.FromHours(barPeriod);
                case BarStructure.Day:
                    return Duration.FromDays(barPeriod);
                case BarStructure.TickImbalance:
                case BarStructure.Undefined:
                case BarStructure.Volume:
                case BarStructure.VolumeImbalance:
                case BarStructure.Dollar:
                case BarStructure.DollarImbalance:
                    goto default;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(barStructure, nameof(barStructure));
            }
        }
    }
}
