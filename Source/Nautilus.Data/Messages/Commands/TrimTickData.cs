// -------------------------------------------------------------------------------------------------
// <copyright file="TrimTickData.cs" company="Nautech Systems Pty Ltd">
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
using Nautilus.Common.Interfaces;
using Nautilus.Core.Annotations;
using Nautilus.Core.Correctness;
using Nautilus.Core.Message;
using NodaTime;

namespace Nautilus.Data.Messages.Commands
{
    /// <summary>
    /// Represents a scheduled command to trim the tick data with timestamps prior to the trim from date time.
    /// </summary>
    [Immutable]
    public sealed class TrimTickData : Command, IScheduledJob
    {
        private static readonly Type EventType = typeof(TrimTickData);

        /// <summary>
        /// Initializes a new instance of the <see cref="TrimTickData"/> class.
        /// </summary>
        /// <param name="rollingDays">The date time the tick data should be trimmed from.</param>
        /// <param name="scheduledTime">The command scheduled time.</param>
        /// <param name="commandId">The command identifier.</param>
        /// <param name="commandTimestamp">The command timestamp.</param>
        public TrimTickData(
            int rollingDays,
            ZonedDateTime scheduledTime,
            Guid commandId,
            ZonedDateTime commandTimestamp)
            : base(
                EventType,
                commandId,
                commandTimestamp)
        {
            Condition.PositiveInt32(rollingDays, nameof(rollingDays));

            this.RollingDays = rollingDays;
            this.ScheduledTime = scheduledTime;
        }

        /// <inheritdoc />
        public ZonedDateTime ScheduledTime { get; }

        /// <summary>
        /// Gets the commands rolling days to trim to.
        /// </summary>
        public int RollingDays { get; }
    }
}
