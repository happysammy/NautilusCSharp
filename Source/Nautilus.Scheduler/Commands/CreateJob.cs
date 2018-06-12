

namespace Nautilus.Scheduler.Commands
{
    using Akka.Actor;
    using Quartz;

    /// <summary>
    ///     Message to add a trigger.
    /// </summary>
    public class CreateJob : IJobCommand
    {
        public CreateJob(IActorRef to, object message, ITrigger trigger)
        {
            To = to;
            Message = message;
            Trigger = trigger;
        }

        /// <summary>
        ///     The desination actor
        /// </summary>
        public IActorRef To { get; }

        /// <summary>
        ///     Message
        /// </summary>
        public object Message { get; }

        /// <summary>
        ///     Trigger
        /// </summary>
        public ITrigger Trigger { get; }
    }
}
