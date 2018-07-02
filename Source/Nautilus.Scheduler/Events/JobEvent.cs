//--------------------------------------------------------------------------------------------------
// <copyright file="JobEvent.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Scheduler.Events
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Quartz;

    /// <summary>
    /// The base class for all job events.
    /// </summary>
    [Immutable]
    public abstract class JobEvent : IJobEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JobEvent"/> class.
        /// </summary>
        /// <param name="jobKey">The event job key.</param>
        /// <param name="triggerKey">The event trigger key.</param>
        protected JobEvent(JobKey jobKey, TriggerKey triggerKey)
        {
            Debug.NotNull(jobKey, nameof(jobKey));
            Debug.NotNull(triggerKey, nameof(triggerKey));

            this.JobKey = jobKey;
            this.TriggerKey = triggerKey;
        }

        /// <summary>
        /// Gets the events job key.
        /// </summary>
        public JobKey JobKey { get; }

        /// <summary>
        /// Gets the events trigger key.
        /// </summary>
        public TriggerKey TriggerKey { get; }
    }
}
