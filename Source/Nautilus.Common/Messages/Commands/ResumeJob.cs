//--------------------------------------------------------------------------------------------------
// <copyright file="ResumeJob.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messages.Commands
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using NodaTime;
    using Quartz;

    /// <summary>
    /// Represents a command to resume a job.
    /// </summary>
    [Immutable]
    public sealed class ResumeJob : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResumeJob"/> class.
        /// </summary>
        /// <param name="jobKey">The job key to resume.</param>
        /// <param name="identifier">The command identifier.</param>
        /// <param name="timestamp">The command timestamp.</param>
        public ResumeJob(
            JobKey jobKey,
            Guid identifier,
            ZonedDateTime timestamp)
            : base(identifier, timestamp)
        {
            Debug.NotDefault(identifier, nameof(identifier));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.JobKey = jobKey;
        }

        /// <summary>
        /// Gets the job to resume key.
        /// </summary>
        public JobKey JobKey { get; }
    }
}
