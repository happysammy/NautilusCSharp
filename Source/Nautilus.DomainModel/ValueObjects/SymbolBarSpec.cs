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
    /// Represents the concept of a <see cref="Symbol"/> and <see cref="BarSpecification"/>.
    /// </summary>
    public sealed class SymbolBarSpec
        : ValueObject<SymbolBarSpec>, IEquatable<SymbolBarSpec>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolBarSpec"/> class.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="barSpecification"></param>
        public SymbolBarSpec(
            Symbol symbol,
            BarSpecification barSpecification)
        {
            this.Symbol = symbol;
            this.BarSpecification = barSpecification;
        }

        /// <summary>
        /// Gets the symbol.
        /// </summary>
        public Symbol Symbol { get; }

        /// <summary>
        /// Gets the bar specification.
        /// </summary>
        public BarSpecification BarSpecification { get; }

        /// <summary>
        /// Returns a collection of objects to be included in equality checks.
        /// </summary>
        /// <returns>A collection of objects.</returns>
        protected override IEnumerable<object> GetMembersForEqualityCheck()
        {
            return new object[]
            {
                this.Symbol,
                this.BarSpecification,
            };
        }

        /// <summary>
        /// Returns a string representation of the <see cref="BarSpecification"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString()
        {
            return $"{this.Symbol}-{this.BarSpecification.Period}-{this.BarSpecification.Resolution}[{this.BarSpecification.QuoteType}]";
        }
    }
}
