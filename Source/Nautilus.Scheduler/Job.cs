//--------------------------------------------------------------------------------------------------
// <copyright file="Job.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

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
                if (jdm[ActorKey] is IActorRef actor)
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
