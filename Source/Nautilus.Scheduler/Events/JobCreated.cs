//--------------------------------------------------------------------------------------------------
// <copyright file="JobCreated.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Scheduler.Events
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using NodaTime;
    using Quartz;

    /// <summary>
    /// Represents an event where a job has been created.
    /// </summary>
    [Immutable]
    public sealed class JobCreated : JobEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JobCreated"/> class.
        /// </summary>
        /// <param name="jobKey">The created job key.</param>
        /// <param name="triggerKey">The created job trigger.</param>
        /// <param name="job">The created job.</param>
        /// <param name="identifier">The event identifier.</param>
        /// <param name="timestamp">The event timestamp.</param>
        public JobCreated(
            JobKey jobKey,
            TriggerKey triggerKey,
            object job,
            Guid identifier,
            ZonedDateTime timestamp)
        : base(jobKey, triggerKey, identifier, timestamp)
        {
            Debug.NotNull(job, nameof(job));
            Debug.NotNull(triggerKey, nameof(triggerKey));
            Debug.NotNull(job, nameof(job));

            this.Job = job;
        }

        /// <summary>
        /// Gets the job.
        /// </summary>
        public object Job { get; }

        /// <summary>
        /// Returns a string representation of the <see cref="JobCreated"/>.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
        {
            return $"{this.JobKey} with trigger {this.TriggerKey} has been created.";
        }
    }
}
