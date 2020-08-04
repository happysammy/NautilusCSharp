// -------------------------------------------------------------------------------------------------
// <copyright file="TrimBarData.cs" company="Nautech Systems Pty Ltd">
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
using System.Collections.Generic;
using System.Linq;
using Nautilus.Common.Interfaces;
using Nautilus.Core.Annotations;
using Nautilus.Core.Correctness;
using Nautilus.Core.Message;
using Nautilus.DomainModel.Enums;
using Nautilus.DomainModel.ValueObjects;
using NodaTime;

namespace Nautilus.Data.Messages.Commands
{
    /// <summary>
    /// Represents a scheduled command to trim the bar data keys held in the database
    /// to be equal to the size of the rolling window.
    /// </summary>
    [Immutable]
    public sealed class TrimBarData : Command, IScheduledJob
    {
        private static readonly Type EventType = typeof(TrimBarData);

        /// <summary>
        /// Initializes a new instance of the <see cref="TrimBarData"/> class.
        /// </summary>
        /// <param name="barSpecifications">The bar specifications to trim.</param>
        /// <param name="rollingDays">The number of days in the rolling window.</param>
        /// <param name="commandId">The command identifier.</param>
        /// <param name="commandTimestamp">The command timestamp.</param>
        public TrimBarData(
            IEnumerable<BarSpecification> barSpecifications,
            int rollingDays,
            Guid commandId,
            ZonedDateTime commandTimestamp)
            : base(
                EventType,
                commandId,
                commandTimestamp)
        {
            Debug.PositiveInt32(rollingDays, nameof(rollingDays));

            this.RollingDays = rollingDays;
            this.BarStructures = barSpecifications.Select(b => b.BarStructure).Distinct();
        }
        /// <summary>
        /// Gets the commands rolling days to trim to.
        /// </summary>
        public int RollingDays { get; }

        /// <summary>
        /// Gets the commands bar structures to trim.
        /// </summary>
        public IEnumerable<BarStructure> BarStructures { get;  }
    }
}
