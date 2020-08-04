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

using Nautilus.Core.Message;
using System;
using Nautilus.Core.Annotations;
using Nautilus.Core.Correctness;
using NodaTime;
using Quartz;

namespace Nautilus.Scheduling.Messages
{
    /// <summary>
    /// Represents a command to remove a job.
    /// </summary>
    [Immutable]
    public sealed class RemoveJob : Command
    {
        private static readonly Type CommandType = typeof(RemoveJob);

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveJob"/> class.
        /// </summary>
        /// <param name="jobKey">The job key to remove.</param>
        /// <param name="identifier">The command identifier.</param>
        /// <param name="timestamp">The command timestamp.</param>
        public RemoveJob(
            JobKey jobKey,
            Guid identifier,
            ZonedDateTime timestamp)
            : base(CommandType, identifier, timestamp)
        {
            Debug.NotDefault(identifier, nameof(identifier));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.JobKey = jobKey;
        }

        /// <summary>
        /// Gets the jobs key.
        /// </summary>
        public JobKey JobKey { get; }
    }
}
