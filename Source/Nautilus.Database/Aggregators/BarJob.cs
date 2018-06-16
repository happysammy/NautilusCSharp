//--------------------------------------------------------------------------------------------------
// <copyright file="BarJob.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Aggregators
{
    using System;
    using System.Collections.Generic;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Represents a bar job for the given symbol and bar specification (to close a bar).
    /// </summary>
    [Immutable]
    public class BarJob : ValueObject<BarJob>, IEquatable<BarJob>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BarJob"/> class.
        /// </summary>
        /// <param name="symbol">The jobs symbol.</param>
        /// <param name="barSpecification">The jobs bar specification.</param>
        public BarJob(Symbol symbol, BarSpecification barSpecification)
        {
            Debug.NotNull(symbol, nameof(symbol));
            Debug.NotNull(barSpecification, nameof(barSpecification));

            Symbol = symbol;
            BarSpecification = barSpecification;
        }

        /// <summary>
        /// Gets the jobs symbol.
        /// </summary>
        public Symbol Symbol { get; }

        /// <summary>
        /// Gets the jobs bar speicifcation.
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
                this.BarSpecification
            };
        }

        /// <summary>
        /// Returns a string representation of this <see cref="BarJob"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString()
        {
            return $"{nameof(BarJob)}-{this.Symbol}-{this.BarSpecification}";
        }
    }
}
