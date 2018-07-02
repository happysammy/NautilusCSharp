//--------------------------------------------------------------------------------------------------
// <copyright file="RemoveJobFail.cs" company="Nautech Systems Pty Ltd">
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
    using Quartz;

    /// <summary>
    /// Represents a job event where removing a job failed.
    /// </summary>
    [Immutable]
    public sealed class RemoveJobFail : JobEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveJobFail"/> class.
        /// </summary>
        /// <param name="jobKey">The job key.</param>
        /// <param name="triggerKey">The job trigger key.</param>
        /// <param name="reason">The job removal failure reason.</param>
        public RemoveJobFail(
            JobKey jobKey,
            TriggerKey triggerKey,
            Exception reason)
            : base(jobKey, triggerKey)
        {
            Debug.NotNull(jobKey, nameof(jobKey));
            Debug.NotNull(triggerKey, nameof(triggerKey));
            Debug.NotNull(reason, nameof(reason));

            this.Reason = reason;
        }

        /// <summary>
        /// Gets the exception reason why the job removal failed.
        /// </summary>
        public Exception Reason { get; }

        /// <summary>
        /// Returns a string representation of the <see cref="RemoveJobFail"/>.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
        {
            return $"Remove job {this.JobKey} with trigger {this.TriggerKey} failed. " +
                   $"With reason {this.Reason}";
        }
    }
}
