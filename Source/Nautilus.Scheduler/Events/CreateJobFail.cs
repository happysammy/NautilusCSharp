//--------------------------------------------------------------------------------------------------
// <copyright file="CreateJobFail.cs" company="Nautech Systems Pty Ltd">
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
    /// Represents an event where the creation of a job failed.
    /// </summary>
    [Immutable]
    public sealed class CreateJobFail : JobEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateJobFail"/> class.
        /// </summary>
        /// <param name="jobKey">The job key.</param>
        /// <param name="triggerKey">The job trigger key.</param>
        /// <param name="reason">The creation failure reason.</param>
        /// <param name="identifier">The event identifier.</param>
        /// <param name="timestamp">The event timestamp.</param>
        public CreateJobFail(
            JobKey jobKey,
            TriggerKey triggerKey,
            Exception reason,
            Guid identifier,
            ZonedDateTime timestamp)
            : base(jobKey, triggerKey, identifier, timestamp)
        {
            Debug.NotNull(jobKey, nameof(jobKey));
            Debug.NotNull(triggerKey, nameof(triggerKey));
            Debug.NotNull(reason, nameof(reason));

            this.Reason = reason;
        }

        /// <summary>
        /// Gets the job creation failure reason.
        /// </summary>
        public Exception Reason { get; }

        /// <summary>
        /// Gets a string representation of the <see cref="CreateJobFail"/>.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
        {
            return $"Creation  of job {this.JobKey} with trigger {this.TriggerKey} failed. " +
                   $"With reason {this.Reason}";
        }
    }
}
