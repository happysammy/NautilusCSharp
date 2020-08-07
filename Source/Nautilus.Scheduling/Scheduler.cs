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

using System.Collections.Specialized;
using Microsoft.Extensions.Logging;
using Nautilus.Common.Interfaces;
using Nautilus.Common.Messages.Commands;
using Nautilus.Common.Messaging;
using Nautilus.Scheduling.Messages;
using Quartz;
using Quartz.Impl;

namespace Nautilus.Scheduling
{
    /// <summary>
    /// Provides a system scheduling component with an internal quartz scheduler which processes Add
    /// and Remove messages.
    /// </summary>
    public sealed class Scheduler : MessageBusConnected
    {
        private readonly IScheduler quartzScheduler;

        /// <summary>
        /// Initializes a new instance of the <see cref="Scheduler"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        public Scheduler(IComponentryContainer container, IMessageBusAdapter messagingAdapter)
            : base(container,messagingAdapter)
        {
            var properties = new NameValueCollection
            {
                { "quartz.threadPool.threadCount", "2" },
            };

            var factory = new StdSchedulerFactory(properties);
            this.quartzScheduler = factory.GetScheduler().Result;

            this.RegisterHandler<CreateJob>(this.OnMessage);
            this.RegisterHandler<PauseJob>(this.OnMessage);
            this.RegisterHandler<ResumeJob>(this.OnMessage);
            this.RegisterHandler<RemoveJob>(this.OnMessage);
        }

        /// <inheritdoc />
        protected override void OnStart(Start message)
        {
            this.quartzScheduler.Start();
        }

        /// <inheritdoc />
        protected override void OnStop(Stop message)
        {
            this.quartzScheduler.Shutdown();
        }

        private void OnMessage(CreateJob message)
        {
            if (this.quartzScheduler.CheckExists(message.JobKey).Result)
            {
                this.Logger.LogError($"Job create failed (JobKey={message.JobKey}, Reason=DuplicateJobKey).");
                return;
            }

            var create = this.quartzScheduler.ScheduleJob(message.JobDetail, message.Trigger);
            if (create.IsCompletedSuccessfully)
            {
                this.Logger.LogInformation($"Job created (JobKey={message.JobKey}, TriggerKey={message.Trigger.Key}).");
            }
            else
            {
                this.Logger.LogError($"Job create failed (JobKey={message.JobKey}, Reason=Unknown).");
            }
        }

        private void OnMessage(PauseJob message)
        {
            if (!this.quartzScheduler.CheckExists(message.JobKey).Result)
            {
                this.Logger.LogError($"Job pause failed (JobKey={message.JobKey}, Reason=JobNotFound).");
                return;
            }

            var paused = this.quartzScheduler.PauseJob(message.JobKey);
            if (paused.IsCompletedSuccessfully)
            {
                this.Logger.LogInformation($"Job paused successfully (JobKey={message.JobKey}).");
            }
            else
            {
                this.Logger.LogError($"Job pause failed (JobKey={message.JobKey}, Reason=Unknown).");
            }
        }

        private void OnMessage(ResumeJob message)
        {
            if (!this.quartzScheduler.CheckExists(message.JobKey).Result)
            {
                this.Logger.LogError($"Job resume failed (JobKey={message.JobKey}, Reason=JobNotFound).");
                return;
            }

            var resume = this.quartzScheduler.ResumeJob(message.JobKey);
            if (resume.IsCompletedSuccessfully)
            {
                this.Logger.LogInformation($"Job resumed successfully (JobKey={message.JobKey}).");
            }
            else
            {
                this.Logger.LogError($"Job resume failed (JobKey={message.JobKey}, Reason=Unknown).");
            }
        }

        private void OnMessage(RemoveJob message)
        {
            if (message.IfJobExists)
            {
                if (!this.quartzScheduler.CheckExists(message.JobKey).Result)
                {
                    this.Logger.LogDebug($"Job not remove (JobKey={message.JobKey}, IfJobExists=True, Reason=JobNotFound).");
                    return;
                }

                this.quartzScheduler.DeleteJob(message.JobKey);
                return;
            }

            var deleted = this.quartzScheduler.DeleteJob(message.JobKey);
            if (deleted.IsCompletedSuccessfully)
            {
                this.Logger.LogInformation($"Job removed (JobKey={message.JobKey}).");
            }
            else
            {
                this.Logger.LogError($"Job remove failed (JobKey={message.JobKey}, Reason=Unknown).");
            }
        }
    }
}
