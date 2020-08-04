// -------------------------------------------------------------------------------------------------
// <copyright file="RedisConstants.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System.Threading.Tasks;
using Nautilus.Core.Annotations;
using Nautilus.Core.Types;
using Nautilus.Messaging.Interfaces;
using Quartz;

namespace Nautilus.Scheduling
{
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
        public static JobBuilder CreateBuilderWithData(IEndpoint receiver, Message message)
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
