//--------------------------------------------------------------------------------------------------
// <copyright file="PauseJob.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Scheduler.Commands
{
    using System;
    using Akka.Actor;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using NodaTime;
    using Quartz;

    /// <summary>
    /// Represents a job command message to pause a job.
    /// </summary>
    [Immutable]
    public sealed class PauseJob : Command, IJobCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PauseJob"/> class.
        /// </summary>
        /// <param name="jobKey">The job key.</param>
        /// <param name="sender">The message sender.</param>
        /// <param name="identifier">The message identifier.</param>
        /// <param name="timestamp">The message timestamp.</param>
        public PauseJob(
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
        /// Gets the job to pause key.
        /// </summary>
        public JobKey JobKey { get; }

        /// <summary>
        /// Gets the pause job message sender.
        /// </summary>
        public IActorRef Sender { get; }
    }
}
