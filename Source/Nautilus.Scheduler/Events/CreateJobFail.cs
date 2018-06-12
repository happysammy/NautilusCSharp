//--------------------------------------------------------------------------------------------------
// <copyright file="CreateJobFail.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Scheduler.Events
{
    using System;
    using Quartz;

    /// <summary>
    /// The create job fail job event message.
    /// </summary>
    public class CreateJobFail : JobEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateJobFail"/> class.
        /// </summary>
        /// <param name="jobKey"></param>
        /// <param name="triggerKey"></param>
        /// <param name="reason"></param>
        public CreateJobFail(
            JobKey jobKey,
            TriggerKey triggerKey,
            Exception reason)
            : base(jobKey, triggerKey)
        {
            Reason = reason;
        }

        /// <summary>
        ///     Fail reason
        /// </summary>
        public Exception Reason { get; private set; }

        public override string ToString()
        {
            return string.Format("Create job {0} with trigger {1} fail. With reason {2}", JobKey, TriggerKey, Reason);
        }
    }
}
