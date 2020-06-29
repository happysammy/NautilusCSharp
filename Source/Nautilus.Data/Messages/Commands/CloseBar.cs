//--------------------------------------------------------------------------------------------------
// <copyright file="CloseBar.cs" company="Nautech Systems Pty Ltd">
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
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Messages.Commands
{
    using System;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Message;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Represents a command to close a <see cref="Bar"/> of the given <see cref="Specification"/>.
    /// </summary>
    [Immutable]
    public sealed class CloseBar : Command, IScheduledJob
    {
        private static readonly Type EventType = typeof(CloseBar);

        /// <summary>
        /// Initializes a new instance of the <see cref="CloseBar"/> class.
        /// </summary>
        /// <param name="specification">The bar specification.</param>
        /// <param name="scheduledTime">The scheduled job time.</param>
        /// <param name="commandId">The command identifier.</param>
        /// <param name="commandTimestamp">The command timestamp.</param>
        public CloseBar(
            BarSpecification specification,
            ZonedDateTime scheduledTime,
            Guid commandId,
            ZonedDateTime commandTimestamp)
            : base(
                EventType,
                commandId,
                commandTimestamp)
        {
            this.ScheduledTime = scheduledTime;
            this.Specification = specification;
        }

        /// <summary>
        /// Gets the commands bar specification to close.
        /// </summary>
        public BarSpecification Specification { get; }

        /// <inheritdoc />
        public ZonedDateTime ScheduledTime { get; }
    }
}
