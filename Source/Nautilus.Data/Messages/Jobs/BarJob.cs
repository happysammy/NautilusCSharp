//--------------------------------------------------------------------------------------------------
// <copyright file="BarJob.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Messages.Jobs
{
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Annotations;
    using Nautilus.DomainModel.ValueObjects;
    using Quartz;

    /// <summary>
    /// Represents a bar job for the given symbol and bar specification (to close a bar).
    /// </summary>
    [Immutable]
    public sealed class BarJob : IScheduledJob
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BarJob"/> class.
        /// </summary>
        /// <param name="barSpec">The jobs bar type..</param>
        public BarJob(BarSpecification barSpec)
        {
            this.BarSpec = barSpec;
            this.Key = new JobKey(this.BarSpec.ToString(), "bar_aggregation");
        }

        /// <summary>
        /// Gets the jobs bar specification.
        /// </summary>
        public BarSpecification BarSpec { get; }

        /// <summary>
        /// Gets the jobs key.
        /// </summary>
        public JobKey Key { get; }

        /// <summary>
        /// Returns a string representation of this <see cref="BarJob"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString()
        {
            return $"{nameof(BarJob)}-{this.BarSpec}";
        }
    }
}
