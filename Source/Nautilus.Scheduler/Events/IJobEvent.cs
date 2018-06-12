using Quartz;

namespace Nautilus.Scheduler.Events
{
    /// <summary>
    /// Base interface for job events
    /// </summary>
    internal interface IJobEvent
    {
        JobKey JobKey { get; }
        TriggerKey TriggerKey { get; }
    }
}