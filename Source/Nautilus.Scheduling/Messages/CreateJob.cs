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

using System;
using Nautilus.Core.Message;
using Nautilus.Core.Annotations;
using Nautilus.Core.Correctness;
using Nautilus.Core.Types;
using Nautilus.Messaging.Interfaces;
using NodaTime;
using Quartz;

namespace Nautilus.Scheduling.Messages
{
    /// <summary>
    /// Represents a command to create a new job.
    /// </summary>
    [Immutable]
    public sealed class CreateJob : Command
    {
        private static readonly Type CommandType = typeof(CreateJob);

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateJob"/> class.
        /// </summary>
        /// <param name="jobReceiver">The receiver for the job.</param>
        /// <param name="job">The job to schedule.</param>
        /// <param name="jobKey">The job key.</param>
        /// <param name="trigger">The job trigger.</param>
        /// <param name="identifier">The command identifier.</param>
        /// <param name="timestamp">The command timestamp.</param>
        public CreateJob(
            IEndpoint jobReceiver,
            Message job,
            JobKey jobKey,
            ITrigger trigger,
            Guid identifier,
            ZonedDateTime timestamp)
            : base(CommandType, identifier, timestamp)
        {
            Debug.NotDefault(identifier, nameof(identifier));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.JobReceiver = jobReceiver;
            this.JobKey = jobKey;
            this.Trigger = trigger;
            this.JobDetail = Job.CreateBuilderWithData(jobReceiver, job)
                .WithIdentity(jobKey)
                .Build();
        }

        /// <summary>
        /// Gets the jobs receiver endpoint.
        /// </summary>
        public IEndpoint JobReceiver { get; }

        /// <summary>
        /// Gets the key for the job to create.
        /// </summary>
        public JobKey JobKey { get; }

        /// <summary>
        /// Gets the jobs trigger.
        /// </summary>
        public ITrigger Trigger { get; }

        /// <summary>
        /// Gets the jobs detail.
        /// </summary>
        public IJobDetail JobDetail { get; }
    }
}
