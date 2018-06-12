

namespace Nautilus.Scheduler
{
    using System.Threading.Tasks;
    using Akka.Actor;
    using Akka.Util.Internal;
    using Quartz;

    /// <summary>
    /// Job
    /// </summary>
    public class Job : IJob
    {
        private const string MessageKey = "message";
        private const string ActorKey = "actor";

        public Task Execute(IJobExecutionContext context)
        {
            var jdm = context.JobDetail.JobDataMap;
            if (jdm.ContainsKey(MessageKey) && jdm.ContainsKey(ActorKey))
            {
                var actor = jdm[ActorKey] as IActorRef;
                if (actor != null)
                {
                    actor.Tell(jdm[MessageKey]);
                }
            }

            return Task.CompletedTask;
        }

        public static JobBuilder CreateBuilderWithData(IActorRef actorRef, object message)
        {
            var jdm = new JobDataMap();
            jdm.AddAndReturn(MessageKey, message).Add(ActorKey, actorRef);
            return JobBuilder.Create<Job>().UsingJobData(jdm);
        }
    }
}
