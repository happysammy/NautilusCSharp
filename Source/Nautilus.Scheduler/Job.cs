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
    using Nautilus.Core.Annotations;
    using Quartz;

    /// <summary>
    /// Represents a job.
    /// </summary>
    [Immutable]
    public sealed class Job : IJob
    {
        private const string MessageKey = "message";
        private const string ActorKey = "actor";

        /// <summary>
        /// Executes the job task.
        /// </summary>
        /// <param name="context">THe context.</param>
        /// <returns>The task completed token.</returns>
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

        /// <summary>
        /// Creates and returns a new job builder from the given parameters.
        /// </summary>
        /// <param name="actorRef">The actor address.</param>
        /// <param name="message">The message.</param>
        /// <returns>The job builder.</returns>
        public static JobBuilder CreateBuilderWithData(IActorRef actorRef, object message)
        {
            var jdm = new JobDataMap();
            jdm.AddAndReturn(MessageKey, message).Add(ActorKey, actorRef);

            return JobBuilder.Create<Job>().UsingJobData(jdm);
        }
    }
}
