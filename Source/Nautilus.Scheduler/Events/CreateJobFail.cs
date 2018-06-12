using System;
using Quartz;

namespace Nautilus.Scheduler.Events
{
    /// <summary>
    ///     Create job fail event
    /// </summary>
    public class CreateJobFail : JobEvent
    {
        public CreateJobFail(JobKey jobKey, TriggerKey triggerKey, Exception reason) : base(jobKey, triggerKey)
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
