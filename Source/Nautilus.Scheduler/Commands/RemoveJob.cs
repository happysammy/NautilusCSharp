//--------------------------------------------------------------------------------------------------
// <copyright file="RemoveJob.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Scheduler.Commands
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Quartz;

    /// <summary>
    /// The job command message to remove a cron scheduler.
    /// </summary>
    [Immutable]
    public sealed class RemoveJob : IJobCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveJob"/> class.
        /// </summary>
        /// <param name="jobKey"></param>
        /// <param name="triggerKey"></param>
        public RemoveJob(
            JobKey jobKey,
            TriggerKey triggerKey,
            object job)
        {
            Debug.NotNull(jobKey, nameof(jobKey));
            Debug.NotNull(triggerKey, nameof(triggerKey));
            Debug.NotNull(job, nameof(job));

            this.JobKey = jobKey;
            this.TriggerKey = triggerKey;
            this.Job = job;
        }

        /// <summary>
        /// Gets the jobs key.
        /// </summary>
        public JobKey JobKey { get; }

        /// <summary>
        /// Gets the jobs trigger key.
        /// </summary>
        public TriggerKey TriggerKey { get; }

        /// <summary>
        /// Gets the job message object.
        /// </summary>
        public object Job { get; }
    }
}
