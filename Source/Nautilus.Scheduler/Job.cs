//--------------------------------------------------------------------------------------------------
// <copyright file="Job.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Scheduler
{
    using System.Threading.Tasks;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Annotations;
    using Nautilus.Messaging.Interfaces;
    using Quartz;

    /// <summary>
    /// Represents a job.
    /// </summary>
    [Immutable]
    public sealed class Job : IJob
    {
        private const string MessageKey = "message";
        private const string NautilusKey = "nautilus";

        /// <summary>
        /// Creates and returns a new job builder from the given parameters.
        /// </summary>
        /// <param name="receiver">The jobs receiver.</param>
        /// <param name="message">The job message.</param>
        /// <returns>The job builder.</returns>
        public static JobBuilder CreateBuilderWithData(IEndpoint receiver, IScheduledJob message)
        {
            var jdm = new JobDataMap
            {
                { MessageKey, message },
                { NautilusKey, receiver },
            };

            return JobBuilder.Create<Job>().UsingJobData(jdm);
        }

        /// <summary>
        /// Executes the job task.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The task completed token.</returns>
        public Task Execute(IJobExecutionContext context)
        {
            var jdm = context.JobDetail.JobDataMap;
            if (!jdm.ContainsKey(MessageKey) || !jdm.ContainsKey(NautilusKey))
            {
                return Task.CompletedTask;
            }

            if (jdm[NautilusKey] is IEndpoint receiver)
            {
                receiver.Send(jdm[MessageKey]);
            }

            return Task.CompletedTask;
        }
    }
}
