//--------------------------------------------------------------------------------------------------
// <copyright file="CreateJob.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Scheduler.Commands
{
    using System;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Jobs;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using NodaTime;
    using Quartz;

    /// <summary>
    /// Represents a command to create a new job.
    /// </summary>
    [Immutable]
    public sealed class CreateJob : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateJob"/> class.
        /// </summary>
        /// <param name="jobSender">The sender of the job.</param>
        /// <param name="jobReceiver">The receiver for the job.</param>
        /// <param name="job">The job to schedule.</param>
        /// <param name="trigger">The job trigger.</param>
        /// <param name="identifier">The command identifier.</param>
        /// <param name="timestamp">The command timestamp.</param>
        public CreateJob(
            IEndpoint jobSender,
            IEndpoint jobReceiver,
            IScheduledJob job,
            ITrigger trigger,
            Guid identifier,
            ZonedDateTime timestamp)
            : base(identifier, timestamp)
        {
            Debug.NotNull(jobSender, nameof(jobSender));
            Debug.NotNull(jobReceiver, nameof(jobReceiver));
            Debug.NotNull(job, nameof(job));
            Debug.NotNull(trigger, nameof(trigger));
            Debug.NotDefault(identifier, nameof(identifier));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.JobSender = jobSender;
            this.JobReceiver = jobReceiver;
            this.Job = job;
            this.Trigger = trigger;
        }

        /// <summary>
        /// Gets the jobs sender.
        /// </summary>
        public IEndpoint JobSender { get; }

        /// <summary>
        /// Gets the jobs destination actor.
        /// </summary>
        public IEndpoint JobReceiver { get; }

        /// <summary>
        /// Gets the jobs message.
        /// </summary>
        public IScheduledJob Job { get; }

        /// <summary>
        /// Gets the jobs trigger.
        /// </summary>
        public ITrigger Trigger { get; }
    }
}
