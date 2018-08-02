//--------------------------------------------------------------------------------------------------
// <copyright file="BarJob.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Messages.Jobs
{
    using System;
    using System.Collections.Generic;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Model;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Represents a bar job for the given symbol and bar specification (to close a bar).
    /// </summary>
    [Immutable]
    public sealed class BarJob : ValueObject<BarJob>, IEquatable<BarJob>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BarJob"/> class.
        /// </summary>
        /// <param name="barSpec">The jobs bar type..</param>
        public BarJob(BarSpecification barSpec)
        {
            Debug.NotNull(barSpec, nameof(barSpec));

            this.BarSpec = barSpec;
        }

        /// <summary>
        /// Gets the jobs symbol.
        /// </summary>
        public BarSpecification BarSpec { get; }

        /// <summary>
        /// Returns a string representation of this <see cref="BarJob"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString()
        {
            return $"{nameof(BarJob)}-{this.BarSpec}";
        }

        /// <summary>
        /// Returns a collection of objects to be included in equality checks.
        /// </summary>
        /// <returns>A collection of objects.</returns>
        protected override IEnumerable<object> GetMembersForEqualityCheck()
        {
            return new object[]
            {
                this.BarSpec,
            };
        }
    }
}
