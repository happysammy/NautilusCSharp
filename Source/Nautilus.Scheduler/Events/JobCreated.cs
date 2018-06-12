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

    /// <summary>
    ///     Job created event
    /// </summary>
    public class JobCreated : JobEvent
    {
        public JobCreated(JobKey jobKey, TriggerKey triggerKey) : base(jobKey, triggerKey)
        {
        }


        public override string ToString()
        {
            return string.Format("{0} with trigger {1} has been created.", JobKey, TriggerKey);
        }
    }

    /// <summary>
    ///     Job removed event
    /// </summary>
    public class JobRemoved : JobEvent
    {
        public JobRemoved(JobKey jobKey, TriggerKey triggerKey) : base(jobKey, triggerKey)
        {
        }


        public override string ToString()
        {
            return string.Format("{0} with trigger {1} has been removed.", JobKey, TriggerKey);
        }
    }
}
