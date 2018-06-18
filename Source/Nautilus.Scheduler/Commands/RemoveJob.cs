//--------------------------------------------------------------------------------------------------
// <copyright file="RemoveJob.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Scheduler.Commands
{
    using System;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using NodaTime;
    using Quartz;

    /// <summary>
    /// The job command message to remove a cron scheduler.
    /// </summary>
    [Immutable]
    public sealed class RemoveJob : CommandMessage, IJobCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveJob"/> class.
        /// </summary>
        /// <param name="jobKey">The job key.</param>
        /// <param name="triggerKey">The job trigger key.</param>
        /// <param name="job">The job.</param>
        /// <param name="identifier">The message identifier.</param>
        /// <param name="timestamp">The message timestamp.</param>
        public RemoveJob(
            JobKey jobKey,
            TriggerKey triggerKey,
            object job,
            Guid identifier,
            ZonedDateTime timestamp)
            : base(identifier, timestamp)
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
