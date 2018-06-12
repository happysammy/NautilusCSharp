//--------------------------------------------------------------------------------------------------
// <copyright file="CreateJobFail.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Scheduler.Events
{
    using Quartz;

    public abstract class JobEvent : IJobEvent
    {
        protected JobEvent(JobKey jobKey, TriggerKey triggerKey)
        {
            JobKey = jobKey;
            TriggerKey = triggerKey;
        }

        /// <summary>
        ///     Job key
        /// </summary>
        public JobKey JobKey { get; private set; }

        /// <summary>
        ///     Trigger key
        /// </summary>
        public TriggerKey TriggerKey { get; private set; }
    }
}
