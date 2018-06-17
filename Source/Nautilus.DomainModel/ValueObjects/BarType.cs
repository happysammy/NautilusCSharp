//--------------------------------------------------------------------------------------------------
// <copyright file="SymbolBarSpec.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.ValueObjects
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the concept of a bar type representing a <see cref="Symbol"/> and
    /// <see cref="Specification"/>.
    /// </summary>
    public sealed class BarType : ValueObject<BarType>, IEquatable<BarType>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BarType"/> class.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="specification"></param>
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
        /// Returns a collection of objects to be included in equality checks.
        /// </summary>
        /// <returns>A collection of objects.</returns>
        protected override IEnumerable<object> GetMembersForEqualityCheck()
        {
            return new object[]
            {
                this.Symbol,
                this.Specification,
            };
        }

        /// <summary>
        /// Returns a string representation of the <see cref="Specification"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString()
        {
            return $"{this.Symbol}-{this.Specification.Period}-{this.Specification.Resolution}[{this.Specification.QuoteType}]";
        }
    }
}
