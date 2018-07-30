//--------------------------------------------------------------------------------------------------
// <copyright file="JobRemoved.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Scheduler.Events
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Quartz;

    /// <summary>
    /// Represents an event where a job has been removed.
    /// </summary>
    [Immutable]
    public sealed class JobRemoved : JobEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JobRemoved"/> class.
        /// </summary>
        /// <param name="jobKey">The removed job key.</param>
        /// <param name="triggerKey">The removed trigger key.</param>
        /// <param name="job">The removed job.</param>
        public JobRemoved(
            JobKey jobKey,
            TriggerKey triggerKey,
            object job)
            : base(jobKey, triggerKey)
        {
            Debug.NotNull(job, nameof(job));
            Debug.NotNull(triggerKey, nameof(triggerKey));
            Debug.NotNull(job, nameof(job));

            this.Job = job;
        }

        /// <summary>
        /// Gets the removed job.
        /// </summary>
        public object Job { get; }

        /// <summary>
        /// Returns a string representation of the <see cref="JobRemoved"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString()
        {
            return $"{this.JobKey} with trigger {this.TriggerKey} has been removed.";
        }
    }
}
