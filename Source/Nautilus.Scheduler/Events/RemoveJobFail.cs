using System;
using Quartz;

namespace Nautilus.Scheduler.Events
{
    /// <summary>
    ///     Remove job fail
    /// </summary>
    public class RemoveJobFail : JobEvent
    {
        public RemoveJobFail(JobKey jobKey, TriggerKey triggerKey, Exception reason) : base(jobKey, triggerKey)
        {
            Reason = reason;
        }

        public Exception Reason { get; private set; }

        public override string ToString()
        {
            return string.Format("Remove job {0} with trigger {1} fail. With reason {2}", JobKey, TriggerKey, Reason);
        }
    }
}
