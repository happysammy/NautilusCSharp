//--------------------------------------------------------------------------------------------------
// <copyright file="ResumeJob.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Scheduler.Commands
{
    using System;
    using Akka.Actor;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using NodaTime;
    using Quartz;

    /// <summary>
    /// Represents a job command message to resume a job.
    /// </summary>
    [Immutable]
    public sealed class ResumeJob : CommandMessage, IJobCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResumeJob"/> class.
        /// </summary>
        /// <param name="jobKey">The job key.</param>
        /// <param name="sender">The message sender.</param>
        /// <param name="identifier">The message identifier.</param>
        /// <param name="timestamp">The message timestamp.</param>
        public ResumeJob(
            JobKey jobKey,
            IActorRef sender,
            Guid identifier,
            ZonedDateTime timestamp)
            : base(identifier, timestamp)
        {
            Debug.NotNull(jobKey, nameof(jobKey));

            this.JobKey = jobKey;
            this.Sender = sender;
        }

        /// <summary>
        /// Gets the job to resume key.
        /// </summary>
        public JobKey JobKey { get; }

        /// <summary>
        /// Gets the resume job message sender.
        /// </summary>
        public IActorRef Sender { get; }
    }
}
