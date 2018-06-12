

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
