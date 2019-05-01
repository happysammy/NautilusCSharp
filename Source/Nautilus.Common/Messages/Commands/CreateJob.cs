//--------------------------------------------------------------------------------------------------
// <copyright file="CreateJob.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messages.Commands
{
    using System;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Scheduling;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
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
        /// <param name="jobReceiver">The receiver for the job.</param>
        /// <param name="job">The job to schedule.</param>
        /// <param name="jobKey">The job key.</param>
        /// <param name="trigger">The job trigger.</param>
        /// <param name="identifier">The command identifier.</param>
        /// <param name="timestamp">The command timestamp.</param>
        public CreateJob(
            IEndpoint jobReceiver,
            IScheduledJob job,
            JobKey jobKey,
            ITrigger trigger,
            Guid identifier,
            ZonedDateTime timestamp)
            : base(identifier, timestamp)
        {
            Debug.NotDefault(identifier, nameof(identifier));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.JobReceiver = jobReceiver;
            this.JobKey = jobKey;
            this.Trigger = trigger;
            this.JobDetail = Job.CreateBuilderWithData(
                    jobReceiver,
                    job)
                .WithIdentity(trigger.JobKey)
                .Build();
        }

        /// <summary>
        /// Gets the jobs receiver endpoint.
        /// </summary>
        public IEndpoint JobReceiver { get; }

        /// <summary>
        /// Gets the jobs key.
        /// </summary>
        public JobKey JobKey { get; }

        /// <summary>
        /// Gets the jobs trigger.
        /// </summary>
        public ITrigger Trigger { get; }

        /// <summary>
        /// Gets the jobs detail.
        /// </summary>
        public IJobDetail JobDetail { get; }
    }
}
