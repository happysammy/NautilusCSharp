//--------------------------------------------------------------------------------------------------
// <copyright file="BarType.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.ValueObjects
{
    using System;
    using Nautilus.DomainModel.ValueObjects.Base;

    /// <summary>
    /// Represents the concept of a bar type representing a <see cref="Symbol"/> and
    /// <see cref="BarSpecification"/>.
    /// </summary>
    public sealed class BarType : ValueObject<BarType>, IEquatable<BarType>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BarType"/> class.
        /// </summary>
        /// <param name="symbol">The bar type symbol.</param>
        /// <param name="specification">The bar type specification.</param>
        public BarType(
            Symbol symbol,
            BarSpecification specification)
        {
            this.Symbol = symbol;
            this.Specification = specification;
        }

        /// <summary>
        /// Gets the bar types symbol.
        /// </summary>
        public Symbol Symbol { get; }

        /// <summary>
        /// Gets the bar types specification.
        /// </summary>
        public BarSpecification Specification { get; }

        /// <summary>
        /// Returns a string representation of the <see cref="BarType"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString()
        {
            return $"{this.Symbol}-{this.Specification.Period}-{this.Specification.Resolution}[{this.Specification.QuoteType}]";
        }

        /// <summary>
        /// Returns a string representation of the <see cref="BarType"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public string ToChannel()
        {
            return $"{this.Symbol.ToString().ToLower()}" +
                   $"-{this.Specification.Period}" +
                   $"-{this.Specification.Resolution.ToString().ToLower()}" +
                   $"[{this.Specification.QuoteType.ToString().ToLower()}]";
        }

        /// <summary>
        /// Returns an array of objects to be included in equality checks.
        /// </summary>
        /// <returns>The array of equality members.</returns>
        protected override object[] GetEqualityArray()
        {
            return new object[]
            {
                this.Symbol,
                this.Specification,
            };
        }
    }
}
