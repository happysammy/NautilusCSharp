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
    using Nautilus.Common.Interfaces;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using NodaTime;
    using Quartz;

    /// <summary>
    /// Represents a command to remove a job.
    /// </summary>
    [Immutable]
    public sealed class RemoveJob : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveJob"/> class.
        /// </summary>
        /// <param name="jobKey">The job key to remove.</param>
        /// <param name="triggerKey">The job trigger key to remove.</param>
        /// <param name="sender">The command sender.</param>
        /// <param name="identifier">The command identifier.</param>
        /// <param name="timestamp">The command timestamp.</param>
        public RemoveJob(
            JobKey jobKey,
            TriggerKey triggerKey,
            IEndpoint sender,
            Guid identifier,
            ZonedDateTime timestamp)
            : base(identifier, timestamp)
        {
            Debug.NotNull(jobKey, nameof(jobKey));
            Debug.NotNull(triggerKey, nameof(triggerKey));
            Debug.NotNull(sender, nameof(sender));
            Debug.NotDefault(identifier, nameof(identifier));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.JobKey = jobKey;
            this.TriggerKey = triggerKey;
            this.Sender = sender;
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
        /// Gets the jobs message sender.
        /// </summary>
        public IEndpoint Sender { get; }
    }
}
